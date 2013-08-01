using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventStore;
using EventStore.Dispatcher;
using System.Transactions;
using TodoApplication.Events;
using TodoApplication.Aggregate;
using TodoApplication.Interface;
using TodoApplication.Data;
using TodoApplication.Scenarios;
using TodoApplication.Interface.Scenarios;
using System.Diagnostics;
using SQLDeleteCommits;

namespace TodoApplication
{
    public partial class TodoForm : Form
    {
        private Persistence persistence;
        private ListPanel currentListPanel;
        private readonly bool DebugMode = true;
        private ListState listState;
        
        public TodoForm()
        {
            this.persistence = new Persistence();            
            this.listState = new ListState(persistence);
            InitializeComponent();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (persistence.hasConnection)
            {
                persistence.setConnection(false);
                connectionButton.Text = "Set Online"; 
            }
            else
            {
                persistence.setConnection(true);
                connectionButton.Text = "Set Offline";
            }
        }


        private void loadScenarioButton_Click(object sender, EventArgs e)
        {
            new SQLDeleter();
            HcScen_1 testScen = new HcScen_1();

            ScenarioAnalyser analyser = new ScenarioAnalyser(testScen);

            Persistence occConnectedPers = new Persistence();
            testScen.playEvents(this.persistence, occConnectedPers);

            //BackgroundWorker newWorker = new BackgroundWorker();
            //newWorker.WorkerSupportsCancellation = false;
            //newWorker.WorkerReportsProgress = true;
            //newWorker.DoWork += getAllScenarioTestResults;
            //newWorker.ProgressChanged += updateScenarioTestResults;
            //newWorker.RunWorkerAsync();
        }

        private void updateScenarioTestResults(object sender, ProgressChangedEventArgs e)
        {
            eventText.Text += e.UserState as String;
        }

        private void getAllScenarioTestResults(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            List<String> results = new List<String>();
            for (int i = 1; i < 10; i++)
            {
                new SQLDeleter();
                Persistence offLinePersistence = new Persistence();
                this.persistence = new Persistence();

                //string loc = "C:\\TEMP\\set1.txt";
                string loc = "C:\\TEMP\\RScen" + i + ".txt";
                RandomScenarioGenerator randGen = new RandomScenarioGenerator();
                // Scenario generatedScenario = randGen.generateRandomScenario(2000);
                //randGen.ObjectToFile(generatedScenario, loc);
                Scenario temp = (Scenario)randGen.FileToObject(loc);
                //generatedScenario.playEvents(this.persistence, tempPersistence);         
                ScenarioAnalyser analyser = new ScenarioAnalyser(loc);
                //Scenario temp = new CustomScenario();

                //CustomOBScenario obScen = new CustomOBScenario();
                temp.playEvents(this.persistence, offLinePersistence);

                results.Add("Testset: " + i +"\n"
                                + "Commited : " + this.persistence.getLatestStreamRevision() + " \n"
                                + "BaseConflicts : " + analyser.baseConflictAmount + " \n"
                                + "FOund conflicts " + offLinePersistence.allConflicts.Count + " \n"
                                + "Percentage solved" + (1 - (float)offLinePersistence.allConflicts.Count / (float)analyser.baseConflictAmount) + " \n"
                                + "" + "" + " \n");

                worker.ReportProgress(0, results[i-1]);
                                
            }
        }


        private void loadStateButton_Click(object sender, EventArgs e)
        {

            if (DebugMode)
            {
                addText(persistence.getDebugInfo());
            }
            listState = new ListState(persistence);
            listState.loadFromPersistence();

            refreshTodoListPanel();            
        }


        private void reloadStateButton_Click(object sender, EventArgs e)
        {
            refreshTodoListPanel();
        }

        private void playOneByOneButton_Click(object sender, EventArgs e)
        {
            listState.loadFromPersistenceOneByOne();
            refreshTodoListPanel();
        }

        private void refreshTodoListPanel()
        {
            if (currentListPanel != null)
            {
                this.Controls.Remove(currentListPanel);
            }
            currentListPanel = new ListPanel();
            currentListPanel.SetBounds(690, 8, 395, 680);
            currentListPanel.setListState(this.listState);
            this.Controls.Add(currentListPanel);
        }

        private void addText(string text)
        {
            eventText.Text += text + Environment.NewLine;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            setButtonsState(true);
        }

        private void addEventButton_Click(object sender, EventArgs e)
        {
            AddTodoItem addTodoItemForm = new AddTodoItem(listState);
            addTodoItemForm.FormClosed += addTodoItemForm_FormClosed;
            addTodoItemForm.Show();
        }

        void addTodoItemForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AddTodoItem closedForm = (AddTodoItem)sender;
            TodoItemCreated createdEvent = closedForm.createdItem;
            if (persistence.getLatestStreamRevision() == 0)
            {
                listState.LoadAndPersist(new ListCreated(Guid.NewGuid(), "New List" ));
            }
            listState.LoadAndPersist(createdEvent);
            refreshTodoListPanel();
        }

        private void loadRandomEvents_Click(object sender, EventArgs e)
        {
            GeneratorForm genForm = new GeneratorForm();
            genForm.FormClosed += genForm_FormClosed;
            genForm.Show();
        }

        private List<IEvent> randomEventList;

        void genForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            setButtonsState(false);

            GeneratorForm genForm = (GeneratorForm)sender;
            Generator eventGenerator = new Generator();
            //randomEventList = eventGenerator.generateEvents(genForm.amount);
            randomEventList = eventGenerator.generateRandomEvents(genForm.amount);
            BackgroundWorker newWorker = new BackgroundWorker();

            newWorker.WorkerSupportsCancellation = false;
            newWorker.WorkerReportsProgress = false;
            newWorker.DoWork += bw_DoWork;
            newWorker.RunWorkerCompleted += bw_RunWorkerCompleted;
            newWorker.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // This is time consuming. Shouldn't eat the interface. 
            System.Threading.Thread.Sleep(500);
            persistence.PersistEvents(randomEventList);
        }

        private void setButtonsState(bool Enabled)
        {
            connectionButton.Enabled = Enabled;
            loadStateButton.Enabled = Enabled;
            reloadStateButton.Enabled = Enabled;
            playOneByOneButton.Enabled = Enabled;
            loadRandomEvents.Enabled = Enabled;
            addEventButton.Enabled = Enabled;
        }

        //TODO remove test function.
        // TODO put everything on github
        private void testGenerator()
        {
            Generator gen = new Generator();
            List<IEvent> gent = gen.generateRandomEvents(1000);

            int ListChanged = 0;
            int TodoCreated = 0;
            int PrioChanged = 0;
            int PrioDec = 0;
            int TodoDeleted = 0;
            int TodosDeleted = 0;
            int IndexChanged = 0;
            foreach (IEvent @event in gent)
            {
                if (@event.GetType().Equals(typeof(ListNameChanged)))
                    ListChanged++;
                else if (@event.GetType().Equals(typeof(TodoItemCreated)))
                    TodoCreated++;
                else if (@event.GetType().Equals(typeof(TodoItemPriorityChanged)))
                    PrioChanged++;
                else if (@event.GetType().Equals(typeof(TodoItemDeleted)))
                    TodoDeleted++;
                else if (@event.GetType().Equals(typeof(TodoItemsDeleted)))
                    TodosDeleted++;
                else if (@event.GetType().Equals(typeof(TodoItemIndexChanged)))
                    IndexChanged++;
                else if (@event.GetType().Equals(typeof(TodoItemPriorityDecreased)))
                    PrioDec++;
            }           
        }

    }
}
