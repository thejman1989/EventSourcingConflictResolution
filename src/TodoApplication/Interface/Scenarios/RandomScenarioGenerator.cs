using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TodoApplication.Events;
using TodoApplication.Scenarios;
using EventStore.Serialization;
using TodoApplication.Aggregate;

namespace TodoApplication.Interface.Scenarios
{
    public class RandomScenarioGenerator
    {
        Generator eventGenerator;
        private readonly Random differentSeedRandom;
        private bool previousWasOffline = true;
        
        private List<IEvent> generatedEvents;
        int totalEvents = 0;
        int currentEvent = 0;
        int scenariolength = 0;
        int totalConsequentOfflineEvents = 0;

        //Settings
        int switchAfterOnline = 3;
        int switchAfterOffline = 10;
        int maxConsequentOfflineEvents = 16;
        int maxConsequentOnlineEvents = 16;

        public RandomScenarioGenerator()
        {
            eventGenerator = new Generator();
            differentSeedRandom = new Random();
        }

        public Scenario generateRandomScenario(int length)
        {
            // Need to times the length of the replayevents because offline steps contain 2 events. 
            generatedEvents = eventGenerator.generateRandomEvents(length * 2);

            Scenario generatedScenario = new Scenario();

            while (totalEvents < (length - 1))
            {
                addRandomReplaySteps(generatedScenario);
            }

            // Add a final online event to make the scenario commit its last offline events. 
            generatedScenario.addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Final List")));
            return generatedScenario;
        }

        ///// <summary>
        ///// Function to give a random change to return true; 
        ///// </summary>
        ///// <param name="probability">Probability to give true = 1 / probability.</param>
        ///// <returns></returns>
        //public bool rollDice(int probability)
        //{
        //    int goAgain = differentSeedRandom.Next(probability);
        //    return goAgain == probability - 1;
        //}

        public void addRandomReplaySteps(Scenario currentScenario)
        {
            if (previousWasOffline)
            {
                addOnlineSteps(differentSeedRandom.Next(1, maxConsequentOnlineEvents), currentScenario);
                previousWasOffline = false; 
            }
            else
            {         
                addOfflineSteps(differentSeedRandom.Next(maxConsequentOfflineEvents), currentScenario);
                previousWasOffline = true;
            }
        }

        private void addOnlineSteps(int amount, Scenario currentScenario)
        {
            for (int i = 0; i < amount; i++)
            {
                IEvent toUse = generatedEvents.ElementAt(currentEvent);
                currentEvent++;
                currentScenario.addOnlineReplayStep(new OnlineReplayStep(toUse));
                scenariolength++;
                totalEvents++;
            }
        }

        private void addOfflineSteps(int amount, Scenario currentScenario)
        {
            // Offline events are generated seperately, as to not affect the online event list, to keep it random.             
            Generator offlineGenerator = new Generator(generatedEvents.GetRange(0, currentEvent));

            List<IEvent> offlineEvents = offlineGenerator.generateRandomEvents(amount);

            List<IEvent> onlineEvents = generatedEvents.GetRange(currentEvent, amount);
            currentEvent += amount;

            for (int i = 0; i < amount; i++)
            {
                OfflineReplayStep offlineStep = new OfflineReplayStep(onlineEvents.ElementAt(i), offlineEvents.ElementAt(i));
                currentScenario.addOfflineReplayStep(offlineStep);
                scenariolength++;
                totalEvents += 2;
            }
        }

        //public void addRandomReplaySteps(Scenario currentScenario)
        //{
            //// If the previous was offline, try to add more offline events. 
            //// If previous was offline, 3 times more chance the next is also offline. 
            //if (totalConsequentOfflineEvents >= maxConsequentOfflineEvents)
            //{
            //    addOnlineReplayStep(currentScenario);                
            //}

            
            //if (previousWasOffline)
            //{

            //    if (rollDice(switchAfterOffline))
            //        addOnlineReplayStep(currentScenario);
            //    else
            //        addOfflineReplayStep(currentScenario);

            //    //int goAgain = differentSeedRandom.Next(6);
            //    //switch (goAgain)
            //    //{
            //    //    case 0:
            //    //    case 1:
            //    //    case 2:
            //    //    case 3:
            //    //    case 4:
            //    //        addOfflineReplayStep(currentScenario);
            //    //        break;
            //    //    case 5:
            //    //        addOnlineReplayStep(currentScenario);                        
            //    //        break;

            //    //}
            //}
            //else
            //{
            //    if (rollDice(switchAfterOnline))
            //        addOfflineReplayStep(currentScenario);
            //    else
            //        addOnlineReplayStep(currentScenario);
            //    //int type = differentSeedRandom.Next(4);
            //    //switch (type)
            //    //{
            //    //    // Three times higher probability that it will be an online event. 
            //    //    case 0:
            //    //    case 1:
            //    //    case 2:
            //    //        addOnlineReplayStep(currentScenario);
            //    //        break;
            //    //    case 3:
            //    //        addOfflineReplayStep(currentScenario);
            //    //        break;
            //    //}
            //}
        //}

        //public void addOnlineReplayStep(Scenario currentScenario)
        //{
        //    IEvent onlineEventCase0 = generatedEvents.ElementAt(currentEvent);
        //    currentEvent++;
        //    ReplayStep newReplayStep = new OnlineReplayStep(onlineEventCase0);
        //    currentScenario.addOnlineReplayStep(newReplayStep);
        //    previousWasOffline = false;
        //    scenariolength++;
        //    totalConsequentOfflineEvents = 0;
        //}

        //public void addOfflineReplayStep(Scenario currentScenario)
        //{


        //    IEvent onlineEventCase1 = null;
        //    IEvent offlineEvent = null;

        //    // Create one of four different offline step types, both users commit, one of them commits or both nothing. 
        //    int type = differentSeedRandom.Next(4);
        //    switch (type)
        //    {
        //            // Both users supply events. 
        //        case 0:
        //            onlineEventCase1 = generatedEvents.ElementAt(currentEvent);
        //            currentEvent++;
        //            offlineEvent = generatedEvents.ElementAt(currentEvent);
        //            currentEvent++;
        //            scenariolength += 2;
        //            break;
        //            // Just offlineEvent.
        //        case 1:
        //            onlineEventCase1 = null;
        //            offlineEvent = generatedEvents.ElementAt(currentEvent);
        //            currentEvent++;
        //            scenariolength++;
        //            break;
        //            // Just online event. 
        //        case 2:
        //            onlineEventCase1 = generatedEvents.ElementAt(currentEvent);
        //            currentEvent++;
        //            offlineEvent = null;
        //            scenariolength++;
        //            break;
        //            // No events. 
        //        case 3:
        //            onlineEventCase1 = null;
        //            offlineEvent = null;
        //            break;

        //    }


        //    ReplayStep newReplayStep = new OfflineReplayStep(onlineEventCase1, offlineEvent);
        //    currentScenario.addOfflineReplayStep(newReplayStep);
        //    previousWasOffline = true;
        //    totalConsequentOfflineEvents++;
        //}


        /// <summary>
        /// Function to save object to external file
        /// </summary>
        /// <param name="_Object">object to save</param>
        /// <param name="_FileName">File name to save object</param>
        /// <returns>Return true if object save successfully, if not return false</returns>
        public bool ObjectToFile(object _Object, string _FileName)
        {
            try
            {
                // create new memory stream
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream();

                // create new BinaryFormatter
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                // Serializes an object, or graph of connected objects, to the given stream.
                _BinaryFormatter.Serialize(_MemoryStream, _Object);

                // convert stream to byte array
                byte[] _ByteArray = _MemoryStream.ToArray();

                // Open file for writing
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray.ToArray(), 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                // cleanup
                _MemoryStream.Close();
                _MemoryStream.Dispose();
                _MemoryStream = null;
                _ByteArray = null;

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // Error occured, return null
            return false;
        }

        /// <summary>
        /// Function to save object to external file
        /// </summary>
        /// <param name="_Object">object to save</param>
        /// <param name="_FileName">File name to save object</param>
        /// <returns>Return true if object save successfully, if not return false</returns>
        public object FileToObject(string _FileName)
        {
            try
            {          
                FileStream fsSource = new FileStream(_FileName,
            FileMode.Open, FileAccess.Read);

                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(bytes);

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
                            = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                object desObject = _BinaryFormatter.Deserialize(_MemoryStream);

                fsSource.Close();
                _MemoryStream.Close();
                _MemoryStream.Dispose();
                _MemoryStream = null;

                return desObject;
            }
            catch (Exception _Exception)
            {
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }            
            return null;
        }

    }
}
