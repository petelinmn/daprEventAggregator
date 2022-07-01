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

            //await Task.Delay(100);
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
                    await Task.Delay(120);
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
        List<Stereotype>? StereotypeList = new List<Stereotype>();
        List<WorkerInfo> WorkerList = new List<WorkerInfo>();
        List<Event> PreviousEvents = new List<Event>();
        List<Stereotype>? PrevioudStereotypeList = new List<Stereotype>();
        List<WorkerInfo> PreviousWorkerInfos = new List<WorkerInfo>();
        private async void timer2_Tick(object sender, EventArgs e)
        {
            try
            {


                var response = await new HttpClient().GetAsync($"http://localhost:5003/event/{Guid}");
                var responseContent = await response.Content.ReadAsStringAsync();

                EventList = JsonConvert.DeserializeObject<List<Event>>(responseContent);

                response = await new HttpClient().GetAsync($"http://localhost:5002/stereotype/{Guid}");
                responseContent = await response.Content.ReadAsStringAsync();

                StereotypeList = JsonConvert.DeserializeObject<List<Stereotype>>(responseContent);

                var workerManagerActor = ActorProxy.Create<IWorkerManagerActor>(
                    new ActorId($"WorkerManagerActor_Dashboard"), "WorkerManagerActor");

                WorkerList = await workerManagerActor.GetWorkersByContext(Guid);

                var shouldUpdateFlowControl = false;
                if (EventList?.Count != PreviousEvents.Count
                    || StereotypeList?.Count != PrevioudStereotypeList?.Count
                    || WorkerList.Count != PreviousWorkerInfos.Count)
                    shouldUpdateFlowControl = true;

                if (EventList?.Any(e => !PreviousEvents.Any(e2 => e.Id == e2.Id)) == true)
                    shouldUpdateFlowControl = true;

                if (StereotypeList?.Any(e => !PrevioudStereotypeList?.Any(e2 => e.Id == e2.Id) == true) == true)
                    shouldUpdateFlowControl = true;

                if (WorkerList.Any(e => !PreviousWorkerInfos.Any(e2 => e.Id == e2.Id)
                    || (PreviousWorkerInfos.Any(e2 => e.Id == e2.Id && e.Data?.Result != e2.Data?.Result))))
                    shouldUpdateFlowControl = true;

                if (shouldUpdateFlowControl)
                {
                    edysonFlowControl1.SetData(EventList, StereotypeList, WorkerList);
                }

                PreviousEvents = EventList;
                PrevioudStereotypeList = StereotypeList;
                PreviousWorkerInfos = WorkerList;
            }
            catch
            {

            }
        }

        private void SelectWorkerProcessing(WorkerInfo worker)
        {

        }

        private void SelectEventProcessing(Event? @event)
        {
            argumentsTextBox.Text =
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject(@event.Arg),
                Formatting.Indented).Replace(@"\", "");

            var obj = ArgDict.GetData(@event.Arg, new Type[]
                {
                    new Dictionary<string, string>().GetType(),
                    new List<Dictionary<string, string>>().GetType(),
                    new List<List<Dictionary<string, string>>>().GetType(),
                });

            var argDict = obj as Dictionary<string, string>;
            var listOfArgDict = obj as List<Dictionary<string, string>>;
            var listOfListOfArgDict = obj as List<List<Dictionary<string, string>>>;

            while (tabControl1.TabCount > 1)
                tabControl1.TabPages.RemoveAt(tabControl1.TabCount - 1);

            if (argDict != null && argDict.ContainsKey("Name") && argDict.ContainsKey("Value") && float.TryParse(argDict["Value"], out float val))
            {
                var dataForChart = new Dictionary<string, List<PointF>>();
                dataForChart[argDict["Value"]] = new List<PointF>() { new PointF(1, val) };

                tabControl1.TabPages.Add(dataForChart.FirstOrDefault().Key ?? "Chart");
                var tabPage = tabControl1.TabPages[tabControl1.TabCount - 1];
                var simpleChart = new SimpleChart();
                tabPage.Controls.Add(simpleChart);
                tabPage.Controls.Add(new TextBox());
                simpleChart.Dock = DockStyle.Fill;
                tabControl1.SelectedIndexChanged += (o, e) => simpleChart.Draw();

                simpleChart.SetData(dataForChart);
            }

            if (listOfArgDict != null)
            {
                var charts = ArgDict.FlattenToDictPoints2(listOfArgDict);
                var data = charts.ToDictionary(i => i.Key, i => i.Value.Where(i => i.Key == "Value").FirstOrDefault().Value.Select((i, index) => new PointF(index, i.Value)).ToList());

                tabControl1.TabPages.Add(data.FirstOrDefault().Key ?? "Chart");
                var tabPage = tabControl1.TabPages[tabControl1.TabCount - 1];
                var simpleChart = new SimpleChart();
                tabPage.Controls.Add(simpleChart);
                tabPage.Controls.Add(new TextBox());
                simpleChart.Dock = DockStyle.Fill;
                tabControl1.SelectedIndexChanged += (o, e) => simpleChart.Draw();

                simpleChart.SetData(data);
            }

            if (listOfListOfArgDict != null)
            {
                var charts = ArgDict.FlattenToDictPoints2(listOfListOfArgDict);

                foreach (var chart in charts)
                {
                    tabControl1.TabPages.Add(chart.Key);
                    var tabPage = tabControl1.TabPages[tabControl1.TabCount - 1];
                    var simpleChart = new SimpleChart();
                    tabPage.Controls.Add(simpleChart);
                    tabPage.Controls.Add(new TextBox());
                    simpleChart.Dock = DockStyle.Fill;
                    tabControl1.SelectedIndexChanged += (o, e) => simpleChart.Draw();

                    var stereotype = @event as Stereotype;
                    List<PointF>? upperBound = null;
                    List<PointF>? lowerBound = null;
                    if (stereotype != null)
                    {
                        upperBound = stereotype.UpperBounds?.ContainsKey(chart.Key) == true
                            ? stereotype.UpperBounds[chart.Key] : null;
                        lowerBound = stereotype.LowerBounds?.ContainsKey(chart.Key) == true
                            ? stereotype.LowerBounds[chart.Key] : null;
                    }

                    var data0 = charts.Where(x => x.Key == chart.Key).ToDictionary(x => x.Key,
                        x => x.Value.Where(i => i.Key == "Value").FirstOrDefault().Value.Select((i, index) => new PointF(index, i.HasValue ? i.Value : 0f)).ToList()
                    );

                    simpleChart.SetData(data0, upperBound, lowerBound,
                        stereotype?.ConfirmedProperties?.Any(p => p == chart.Key) == true);
                }
            }
        }

        private void simpleChart1_VisibleChanged(object sender, EventArgs e)
        {
            simpleChart1.Draw();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            simpleChart1.Draw();
        }

        private async void edysonFlowControl1_OnSelectObject(object sender, OnSelectObjectEventArgs e)
        {
            tabControl1.Visible = false;

            if (edysonFlowControl1?.SelectedEvent != null)
                SelectEventProcessing(edysonFlowControl1.SelectedEvent);

            if (edysonFlowControl1?.SelectedStereotype != null)
                SelectEventProcessing(edysonFlowControl1.SelectedStereotype);

            if (edysonFlowControl1?.SelectedWorker != null)
                SelectWorkerProcessing(edysonFlowControl1.SelectedWorker);


            if (edysonFlowControl1?.SelectedWorker == null)
            {
                await Task.Delay(200);

                if (tabControl1.TabCount >= 2)
                    tabControl1.SelectedIndex = 1;

                tabControl1.Visible = true;
            }
        }
    }
}
