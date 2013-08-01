using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TodoApplication.Data;
using TodoApplication.Events;
using TodoApplication.Interface.Scenarios;
using EventStore.Serialization;
using System.IO;

namespace TodoApplication.Scenarios
{

    [Serializable]
    /// <summary>
    /// Scenario datatype. A scenario is a simulation of a series of events that provide merging conflicts, 
    /// it also contains data that indicates when a event provider goes offline. 
    /// When an event provider is offline, the events are stored locally. These events have to be appended to the rest of the events when
    /// the provider comes online again. The TodoApplication then checks if the last known version of the offline provider matches that of the
    /// current last commit. If not, the events have to be merged together. 
    /// 
    /// </summary>
    /// 
    public class Scenario : ISerializable 
    {
        public List<ReplayStep> replaySteps { get; protected set; }
        public int Count { get; set; }

        public Scenario()
        {
            this.replaySteps = new List<ReplayStep>();
            this.Count = 0;
        }

        public void addOnlineReplayStep(ReplayStep step)
        {
            this.replaySteps.Add(step);
            Count++;
        }

        public void addOfflineReplayStep(ReplayStep step)
        {
            this.replaySteps.Add(step);
            Count++;
        }

        public void playEvents(Persistence onlinePersistence, Persistence occasionalyConnectedPersistence)
        {
            // Set for each step if it is online, and use it later as if it WAS online.
            bool wasOnline = true;
            foreach (ReplayStep step in replaySteps)
            {
                // If online,  persist the event in the online normal store. 
                if (step.GetType() == typeof(OnlineReplayStep))
                {
                    // If the last step was not online, it means that the store has come online and has to be set this way. 
                    if (!wasOnline)
                    {
                        occasionalyConnectedPersistence.setConnection(true);
                        wasOnline = true;
                    }
                    onlinePersistence.PersistEvent(((OnlineReplayStep)step).onlineEvent);
                }               
                else if (step.GetType() == typeof(OfflineReplayStep))
                {
                    if (wasOnline)
                    {
                        occasionalyConnectedPersistence.setConnection(false);
                        wasOnline = false; 
                    }
                    onlinePersistence.PersistEvent(((OfflineReplayStep)step).onlineEvent);
                    occasionalyConnectedPersistence.PersistEvent(((OfflineReplayStep)step).offlineEvent);                    
                }
            }
        }

        //Deserialization constructor.
        public Scenario(SerializationInfo info, StreamingContext ctxt) 
        {
            replaySteps = new List<ReplayStep>();
            //Get the values from info and assign them to the appropriate properties
            Count = (int)info.GetValue("Count", typeof(int));
            JsonSerializer sd = new JsonSerializer();           
            
            for (int i = 0; i < Count; i++)
            {
                 
                Object a = info.GetValue("replayStep" + i, typeof(String));
                String serializedStep = (String)info.GetValue("replayStep" + i, typeof(String));
               
                
                // int pos = (int)_MemoryStream.Position;
                using (Stream s = GenerateStreamFromString(serializedStep))
                {
                    Object o = sd.Deserialize<Object>(s);
                    replaySteps.Add((ReplayStep)o);  
                }                              
            }
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            //You can use any custom name for your name-value pair. But make sure you
            // read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
            // then you should read the same with "EmployeeId"
            info.AddValue("Count", Count);
            JsonSerializer sd = new JsonSerializer();
            
            for (int i = 0; i < Count; i++)
            {
                using (var ms = new MemoryStream())
                {
                    FileStream fileWriteStream = new FileStream(@"c:\\TEMP\temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);                    
                    //info.AddValue("ReplayStep" + i, replaySteps.ElementAt(i));
                    sd.Serialize(fileWriteStream, replaySteps.ElementAt(i));

                    FileStream fileReadStream = new FileStream(@"c:\\TEMP\temp.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    StreamReader reader = new StreamReader(fileReadStream);
                    info.AddValue("replayStep" + i, reader.ReadToEnd());
                    reader.Close();
                }
            }
        }
        
        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
