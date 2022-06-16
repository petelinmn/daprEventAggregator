using Common;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using EventAggregator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkerManager.Actors.Contract;

namespace Dashboard
{
    public partial class EventSequenceEditor : UserControl
    {
        class ArgDict
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
        }

        public enum EditorState
        {
            Stop,
            Idle,
            Run
        }

        public event EventHandler? OnStop;

        public event EventHandler? OnChange;

        public string? Filename { get; set; } = null;

        public Guid Guid { get; set; } = Guid.Empty;

        public bool Saved { get; set; } = true;

        public string Value
        {
            get => commandEditor.Text;
            set => commandEditor.Text = value;
        }

        private DaprClient Client { get; }

        public EventSequenceEditor()
        {
            InitializeComponent();

            commandEditor.Dock = DockStyle.Fill;

            Client = new DaprClientBuilder().Build();
        }

        public EditorState State { get; set; } = EditorState.Stop;
        private int currentLineIndex = -1;

        public void Run()
        {
            State = EditorState.Run;

            listBox1.Visible = true;
            currentLineIndex = 0;
            commandEditor.Visible = false;
            listBox1.Dock = DockStyle.Fill;

            convertSourceToList();

            Process();
        }

        public void Stop()
        {
            State = EditorState.Stop;

            listBox1.Visible = false;
            commandEditor.Visible = true;

            OnStop?.Invoke(this, null);
        }

        public async Task StepOver()
        {
            if (currentLineIndex == -1)
            {
                currentLineIndex = 0;
                convertSourceToList();

                listBox1.Visible = true;
                commandEditor.Visible = false;
                listBox1.Dock = DockStyle.Fill;
            }

            var command = listBox1.Items[currentLineIndex].ToString() ?? "";
            await ExecuteCommand(command);

            currentLineIndex++;

            if (currentLineIndex >= listBox1.Items.Count)
            {
                listBox1.Visible = false;
                commandEditor.Visible = true;
                listBox1.Dock = DockStyle.None;
                currentLineIndex = -1;
            }
        }

        private async Task ExecuteCommand(string command)
        {
            await CommandProcessing(command);

            await Task.Delay(100);
        }

        private async void Process()
        {
            Guid = Guid.NewGuid();
            int itemsCount = listBox1.Items.Count;
            var items = new List<string>();
            foreach (var item in listBox1.Items)
            {
                var strValue = item.ToString();
                if (!string.IsNullOrWhiteSpace(strValue))
                    items.Add(strValue);
            }

            await Task.Run(async () => {
                for (currentLineIndex = 0; currentLineIndex < items.Count; Interlocked.Increment(ref currentLineIndex))
                {
                    if (State == EditorState.Stop) break;

                    var command = items[currentLineIndex].Trim();

                    await ExecuteCommand(command);
                }
            });

            Stop();
        }

        private string[] Split(string source)
        {
            var result = new List<string>();
            var currentWord = "";
            var startObject = false;
            int nestLevel = 0;
            foreach (var c in source)
            {
                if (c == '}')
                {
                    currentWord += c;
                    nestLevel--;
                    if (nestLevel == 0)
                        startObject = false;

                    continue;
                }

                if (c == '{')
                {
                    startObject = true;
                    currentWord += c;
                    nestLevel++;
                    continue;
                }

                if (c == ' ' && !startObject)
                {
                    result.Add(currentWord);
                    currentWord = "";
                    continue;
                }


                currentWord += c;
            }

            result.Add(currentWord);

            return result.ToArray();
        }

        public async Task CommandProcessing(string command)
        {
            var commandName = command.Split(' ').FirstOrDefault();
            var parts = Split(command);
            switch (commandName?.ToLower())
            {
                case "delay":
                    if (parts.Length < 2)
                        throw new ArgumentException("Delay command requires duration argument");
                    var durationInMilliseconds = Convert.ToInt32(parts[1]);
                    await Task.Delay(durationInMilliseconds);
                    break;
                case "publish":
                    if (parts.Length < 2)
                        throw new ArgumentException("Event name is required");
                    var eventName = parts[1];
                    await Client.PublishEventAsync("pubsub", eventName, new EventRequest()
                    {
                        Name = eventName,
                        DateTime = DateTime.Now,
                        Arg = parts.Length >= 3 ? parts[2] : "{}",
                        ContextId = Guid,
                        Id = Guid.NewGuid()
                    });
                    await Task.Delay(200);
                    break;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            convertSourceToList();
            Saved = false;
            OnChange?.Invoke(this, null);
        }

        private void convertSourceToList()
        {
            var lines = commandEditor.Text.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line));
            listBox1.Items.Clear();
            foreach (var line in lines)
            {
                listBox1.Items.Add(line);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentLineIndex >= listBox1.Items.Count)
            {
                Stop();
                return;
            }

            if (currentLineIndex >= 0 && currentLineIndex != listBox1.SelectedIndex)
            {
                listBox1.SelectedIndex = currentLineIndex;
                Text = currentLineIndex.ToString();
            }
        }

        List<Event>? EventList = new List<Event>();
        List<WorkerInfo> WorkerList = new List<WorkerInfo>();
        List<Event> PreviousEvents = new List<Event>();
        List<WorkerInfo> PreviousWorkerInfos = new List<WorkerInfo>();
        private async void timer2_Tick(object sender, EventArgs e)
        {
            var response = await new HttpClient().GetAsync($"http://localhost:5000/event/{Guid}");
            var responseContent = await response.Content.ReadAsStringAsync();

            EventList = JsonConvert.DeserializeObject<List<Event>>(responseContent);

            response = await new HttpClient().GetAsync($"http://localhost:5001/event/{Guid}");
            responseContent = await response.Content.ReadAsStringAsync();

            var eventList2 = JsonConvert.DeserializeObject<List<Event>>(responseContent);
            EventList?.AddRange(eventList2 ?? new List<Event>());

            var workerManagerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId($"WorkerManagerActor_Dashboard"), "WorkerManagerActor");

            WorkerList = await workerManagerActor.GetWorkersByContext(Guid);

            var shouldUpdateFlowControl = false;
            if (EventList.Count != PreviousEvents.Count || WorkerList.Count != PreviousWorkerInfos.Count)
                shouldUpdateFlowControl = true;

            if (EventList.Any(e => !PreviousEvents.Any(e2 => e.Id == e2.Id)))
                shouldUpdateFlowControl = true;

            if (WorkerList.Any(e => !PreviousWorkerInfos.Any(e2 => e.Id == e2.Id)
                || (PreviousWorkerInfos.Any(e2 => e.Id == e2.Id && e.Data?.Result != e2.Data?.Result))))
                shouldUpdateFlowControl = true;

            if (shouldUpdateFlowControl)
            {
                edysonFlowControl1.SetData(EventList, WorkerList);

                listBox2.Items.Clear();
                if (EventList != null)
                    foreach (var ev in EventList)
                        listBox2.Items.Add(ev);

                foreach (var worker in WorkerList)
                    listBox2.Items.Add(worker);

                if (listBox2.Items.Count > 0 && listBox2.Items.Count > _selectedIndex)
                    listBox2.SelectedIndex = _selectedIndex;
            }

            PreviousEvents = EventList;
            PreviousWorkerInfos = WorkerList;
        }

        int _selectedIndex = 0;
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedIndex == listBox2.SelectedIndex) return;
            _selectedIndex = listBox2.SelectedIndex;

            var @event = listBox2.SelectedItem as Event;
            if (@event != null)
                SelectEventProcessing(@event);

            var worker = listBox2.SelectedItem as WorkerInfo;
            if (worker != null)
                SelectWorkerProcessing(worker);
        }

        private void SelectWorkerProcessing(WorkerInfo worker)
        {
            edysonFlowControl1.SelectObject(worker.Id);
        }

        private void SelectEventProcessing(Event? @event)
        {
            edysonFlowControl1.SelectObject(@event.Id);

            argumentsTextBox.Text =
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject(@event.Arg),
                Formatting.Indented).Replace(@"\", "");

            var obj = GetData(@event.Arg, new Type[]
                {
                    new ArgDict().GetType(),
                    new List<ArgDict>().GetType()
                });

            var argDict = obj as ArgDict;
            var listOfArgDict = obj as List<ArgDict>;

            Dictionary<string, List<Tuple<float, float>>> dataForChart =
                new Dictionary<string, List<Tuple<float, float>>>();

            if (argDict != null && argDict.Name != null)
            {
                if (float.TryParse(argDict.Value, out float val))
                {
                    dataForChart[argDict.Name] = new List<Tuple<float, float>>() { new Tuple<float, float>(1, val) };
                }
                else
                {
                    var items = GetPointFromString(argDict.Value);
                    dataForChart[argDict.Name + '1'] = new List<Tuple<float, float>>() { new Tuple<float, float>(1, items[0]) };
                    dataForChart[argDict.Name + '2'] = new List<Tuple<float, float>>() { new Tuple<float, float>(1, items[1]) };
                }
            }

            if (listOfArgDict != null)
            {
                foreach (var item in listOfArgDict)
                {
                    if (float.TryParse(item.Value, out float val) && item.Name != null)
                    {
                        if (dataForChart.ContainsKey(item.Name))
                        {
                            var list = dataForChart[item.Name];
                            list.Add(new Tuple<float, float>(list.Count, val));
                        }
                        else
                            dataForChart[item.Name] = new List<Tuple<float, float>>() { new Tuple<float, float>(0, val) };
                    }
                    else
                    {
                        var items = GetPointFromString(item.Value);
                        for (var i = 0; i < items.Length; i++)
                        {
                            var lineName = item.Name + i;
                            var val2 = items[i];

                            if (dataForChart.ContainsKey(lineName))
                            {
                                var list = dataForChart[lineName];
                                list.Add(new Tuple<float, float>(list.Count + 1, val2));
                            }
                            else
                                dataForChart[lineName] = new List<Tuple<float, float>>() { new Tuple<float, float>(1, val2) };
                        }
                    }

                }
            }

            simpleChart1.SetData(dataForChart);
        }

        int[] GetPointFromString(string str)
        {
            var splitted = str.Trim().Split('-');

            var result = new List<int>();
            for (var i = 0; i < splitted.Length; i++)
            {
                var splittedItem = splitted[i];
                if (int.TryParse(splittedItem, out var num))
                    result.Add(num);
                else
                    result.Add(0);
            }

            return result.ToArray();
        }

        object? GetData(string arg, Type[] types, int nextIndex = 0)
        {
            try
            {
                if (nextIndex >= types.Length)
                    return null;

                return JsonConvert.DeserializeObject(arg, types[nextIndex]);
            }
            catch { }

            return GetData(arg, types, ++nextIndex);
        }

        private void simpleChart1_VisibleChanged(object sender, EventArgs e)
        {
            simpleChart1.Draw();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            simpleChart1.Draw();
        }

        private void edysonFlowControl1_OnSelectObject(object sender, OnSelectObjectEventArgs e)
        {
            var i = 0;
            var str = $"selected:{e.SelectedGuid.ToString()}{Environment.NewLine}";
            foreach (var item in listBox2.Items)
            {
                var entity = item as BaseEntity;
                str += $"entity != null: {entity != null}, Id: {entity.Id}{Environment.NewLine}";
                if (entity != null && entity.Id == e.SelectedGuid)
                {
                    listBox2.SelectedIndex = i;
                    return;
                }

                i++;
            }
            //MessageBox.Show(str);
        }
    }
}
