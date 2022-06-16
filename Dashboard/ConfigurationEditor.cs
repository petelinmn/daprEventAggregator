using Common;
using Configuration.Actors.Contract;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
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

namespace Dashboard
{
    public partial class ConfigurationEditor : Form
    {
        public ConfigurationEditor()
        {
            InitializeComponent();
        }

        IConfigurationActor? _configActor { get; set; } = null;
        IPersonActor? _personActor { get; set; } = null;
        Person CurrentPerson { get; set; } = new Person();
        private async void ConfigurationEditor_Load(object sender, EventArgs e)
        {
            _configActor = ActorProxy.Create<IConfigurationActor>(
                new ActorId("ConfigurationActor_Load"), "ConfigurationActor");

            _personActor = ActorProxy.Create<IPersonActor>(
                new ActorId("PersonActor_Load"), "PersonActor");
            
            bloodPressureLabel.Text = $"Blood pressure: {trackBarBPLow.Value}-{trackBarBPHigh.Value}";
            labelPulseValue.Text = $"Pulse: {trackBarPulse.Value}";

            //await UpdatePersonList();
        }

        private async void configurationEditor2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void StereotypesTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBarPulse_Scroll(object sender, EventArgs e)
        {
            labelPulseValue.Text = $"Pulse: {trackBarPulse.Value}";
        }

        private void trackBarBPLow_Scroll(object sender, EventArgs e)
        {
            bloodPressureLabel.Text = $"Blood pressure: {trackBarBPLow.Value}-{trackBarBPHigh.Value}";
        }

        private void labelPulseValue_Click(object sender, EventArgs e)
        {

        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            /*await UpdatePersonList();

            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken cancellationToken = source.Token;
            using var client = new DaprClientBuilder().Build();
            
            var response = await new HttpClient().GetAsync("http://localhost:5000/event");
            var responseContent = await response.Content.ReadAsStringAsync();

            var eventList = new List<Event>();

            eventList = JsonConvert.DeserializeObject<List<Event>>(responseContent);

            try
            {
                //var result = client.CreateInvokeMethodRequest(HttpMethod.Get, "eventAggregator2", "event", cancellationToken);
                //eventList = await client.InvokeMethodAsync<List<Event>>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //var eventList = await _configActor.GetEvents();
            eventList.Reverse();

            var newData = string.Join(Environment.NewLine, eventList);

            if (newData != richTextBox1.Text)
                richTextBox1.Text = newData;

            if (CurrentPerson != null)
            {
                var person = await _personActor.GetPerson(CurrentPerson.Name);
                if (person == null)
                    return;

                CurrentPerson = person;
                
                var actions = CurrentPerson.AppliedActions.ToList();
                actions.Reverse();
                listBox3.Items.Clear();
                foreach (var action in actions)
                {
                    listBox3.Items.Add(action.Value);
                }
            }*/
        }

        private async Task UpdatePersonList()
        {
            var persons = await _personActor.GetPersons();
            if (!(persons?.Count > 0))
            {
                return;
            }

            if (listBox2.Items.Count != persons.Count)
            {
                listBox2.Items.Clear();
                foreach (var person in persons)
                {
                    listBox2.Items.Add(person);
                }
            }

            for (var i = 0; i < listBox2.Items.Count; i++)
            {
                var cur = (Person)listBox2.Items[i];
                var person = persons.First(p => p.Name == cur.Name);
                if (person.CardiovascularSystem.Measurements.Count
                    != cur.CardiovascularSystem.Measurements.Count)
                {
                    listBox2.Items[i] = person;
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //CurrentPerson.CardiovascularSystem.Measurements.Add(new Measure {
            //    DateTime = DateTime.Now,
            //    Type = MeasureType
            //    BloodPressure = new BloodPressure
            //    {
            //         DiastolicValue = trackBarBPLow.Value,
            //         SystolicValue = trackBarBPHigh.Value,
            //    },
            //    Pulse = trackBarPulse.Value
            //});

            //UpdateCardioList();

            //await Save();
        }

        void UpdateCardioList()
        {
            listBox1.Items.Clear();
            for (var i = CurrentPerson.CardiovascularSystem.Measurements.Count - 1; i >= 0; i--)
            {
                listBox1.Items.Add(CurrentPerson.CardiovascularSystem.Measurements.ElementAt(i));
            }
            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Save();
        }

        private async Task Save()
        {
            if (string.IsNullOrEmpty(newPersonNameTextBox.Text))
                return;

            await _personActor.Save(CurrentPerson, true);

            await Task.Delay(500);

            var indexOfPerson = -1;
            for (var i = 0; i < listBox2.Items.Count; i++)
            {
                var personInList = (Person)listBox2.Items[i];
                if (personInList.Name == CurrentPerson.Name)
                {
                    indexOfPerson = i;
                    listBox2.Items[i] = CurrentPerson;
                    listBox2.SelectedIndex = i;
                    break;
                }
            }

            if (indexOfPerson == -1)
            {
                var persons = await _personActor.GetPersons();
                listBox2.Items.Clear();
                foreach (var p in persons)
                {
                    listBox2.Items.Add(p);
                }
            }

            await UpdatePersonList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CurrentPerson = new Person();
            newPersonNameTextBox.Text = "";
            newPersonNameTextBox.Enabled = true;
            listBox1.Items.Clear();
        }

        private void newPersonNameTextBox_TextChanged(object sender, EventArgs e)
        {
            CurrentPerson.Name = newPersonNameTextBox.Text;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CurrentPerson.Birthday = dateTimePicker1.Value;
        }

        private async void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null)
                return;

            newPersonNameTextBox.Enabled = false;
            CurrentPerson = listBox2.SelectedItem as Person;
            if (CurrentPerson == null)
                MessageBox.Show("CurrentPerson is null");
            await UpdatePersonField(CurrentPerson.Name);
        }

        async Task UpdatePersonField(string Name)
        {
            CurrentPerson = await _personActor.GetPerson(Name);

            newPersonNameTextBox.Text = CurrentPerson.Name;
            dateTimePicker1.Value = CurrentPerson.Birthday;
            textBox1.Text = $"{string.Join(",", CurrentPerson.AppliedStereoTypes)}";
            UpdateCardioList();
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            CurrentPerson.CardiovascularSystem.Measurements.Add(new Measure
            {
                DateTime = DateTime.Now,
                Type = MeasureType.Pressure,
                Value = $"{trackBarBPLow.Value}-{trackBarBPHigh.Value}"
            });

            UpdateCardioList();

            await Save();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            CurrentPerson.CardiovascularSystem.Measurements.Add(new Measure
            {
                DateTime = DateTime.Now,
                Type = MeasureType.Pulse,
                Value = trackBarPulse.Value.ToString()
            });

            UpdateCardioList();

            await Save();
        }
    }
}
