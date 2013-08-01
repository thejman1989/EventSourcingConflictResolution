using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Interface.Scenarios;
using TodoApplication.Scenarios;

namespace TodoApplication.Data
{
    public class ScenarioAnalyser
    {
        public Scenario currentScenario;
        public int baseConflictAmount = 0;
        public float avarageConflictLength = 0;
        public int longestoffline = int.MinValue;
        public int shortestoffline = int.MaxValue;
        public int amountOfEvents = 0;
        public int amountOfflineEvents = 0;
        public int amountConflictSessions = 0;

        public ScenarioAnalyser(string scenarioLocation)
        {
            RandomScenarioGenerator randGen = new RandomScenarioGenerator();
            currentScenario = (Scenario)randGen.FileToObject(scenarioLocation);
            analyseScenario();
        }

        public ScenarioAnalyser(Scenario scenario)
        {
            currentScenario = scenario;
            analyseScenario();
        }

        private void analyseScenario()
        {
            bool prevOffline = false;
            List<IEvent> offlineEvents = new List<IEvent>();
            List<IEvent> onlineEvents = new List<IEvent>();

            foreach (ReplayStep step in currentScenario.replaySteps)
            {
                if(step.GetType() == (typeof(OnlineReplayStep)))
                {
                    //Goes online after being oflline. 
                    if(prevOffline)
                    {
                        baseConflictAmount += offlineEvents.Count;
                        if (offlineEvents.Count < shortestoffline)
                            shortestoffline = offlineEvents.Count;
                        if (offlineEvents.Count > longestoffline)
                            longestoffline = offlineEvents.Count;

                        amountConflictSessions++;

                        offlineEvents = new List<IEvent>();
                        onlineEvents = new List<IEvent>();
                    }
                    prevOffline = false;
                    amountOfEvents++;
                }

                if (step.GetType() == (typeof(OfflineReplayStep)))
                {
                    //Gather events. 
                    OfflineReplayStep offlineStep = (OfflineReplayStep)step;
                    if (offlineStep.onlineEvent != null)
                    {
                        onlineEvents.Add(offlineStep.onlineEvent);
                        amountOfEvents++;
                    }
                    if (offlineStep.offlineEvent != null)
                    {
                        offlineEvents.Add(offlineStep.offlineEvent);
                        amountOfEvents++;
                        amountOfflineEvents++;
                    }                       
                    prevOffline = true;
                }
            }
            avarageConflictLength = (float)baseConflictAmount / (float)amountConflictSessions;
        }
    }
}
