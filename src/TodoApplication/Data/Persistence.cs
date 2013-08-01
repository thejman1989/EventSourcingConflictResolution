using EventStore;
using EventStore.Dispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using TodoApplication.Events;
using TodoApplication.Prototypes;

namespace TodoApplication.Data
{
    public class Persistence
    {
        public static IStoreEvents store;
        private static readonly Guid StreamId = Guid.Parse("9030a52e-7da4-4a15-bf7f-6ed28764c6bf");
        public bool hasConnection { get; private set; }
        private List<IEvent> bufferedEvents = new List<IEvent>();
        private int latestVersion = -1;
        private int totalConflicts = 0;
        private int amountEventsDeleted = 0;
        public List<Conflict> allConflicts = new List<Conflict>();

        public Persistence()
        {
            using (var scope = new TransactionScope())
            {
                store = WireupEventStore();
            }
            this.setConnection(true);
        }

        public ICollection<EventMessage> getAllEvents()
        {
            using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
            {
                latestVersion = stream.StreamRevision;
                return stream.CommittedEvents;                
            }
        }

        public int getLatestStreamRevision()
        {
            using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
            {
                return stream.StreamRevision;
            }
        }

        /// <summary>
        /// Simulates the conenction of the persistence, If there is no connection, the events will be stored in a list. 
        /// On regaining connection, the events are appended to the eventstore. 
        /// </summary>
        /// <param name="hasConnection">Boolean stating if there is a connection or not. </param>
        public void setConnection(bool hasConnection)
        {
            this.hasConnection = hasConnection;
            // When the connection returns, append all events to the eventstore. 
            if (hasConnection)
            {
                // If there have been new events in the time that the app was offline. 
                if (bufferedEvents.Count > 0)
                {
                    if (latestVersion == getLatestStreamRevision())
                    {
                        //No need for anything to happen because the versions are the same.
                        //MessageBox.Show("Same version, no conflict");
                    }
                    else
                    {
                        // This is where conflicts get handled. 
                        //MessageBox.Show("Version of app = " + latestVersion + " Version of store = " + getLatestStreamRevision());

                        using (var storeStream = store.OpenStream(StreamId, latestVersion + 1, int.MaxValue))
                        {
                            // This contains all events in the store that conflict with the current buffered events. 
                            ICollection<EventMessage> storeSideConflictingEvents = storeStream.CommittedEvents;
                           
                            // Select merging strategy. 
                            // 
                            List<Conflict> conflicts;

                            using (var allPreviousStream = store.OpenStream(StreamId, int.MinValue, latestVersion))
                            {
                                OperationBasedMerger merger = new OperationBasedMerger(storeSideConflictingEvents, bufferedEvents, allPreviousStream.CommittedEvents);
                                //OperationalTransformationMerger merger = new OperationalTransformationMerger(storeSideConflictingEvents, bufferedEvents);
                                //SyntacticMerger merger = new SyntacticMerger(storeSideConflictingEvents, bufferedEvents, allPreviousStream.CommittedEvents);
                                conflicts = merger.getConflicts();
                                bufferedEvents = merger.clientSideEvents;
                            }
                            
                            foreach (Conflict conflict in conflicts)
                            {
                                Debug.WriteLine(conflict.ToString());
                                foreach(IEvent @event in conflict.conflictingEvents)
                                {
                                    if (bufferedEvents.Any(e => e.Equals(@event)))
                                    { 
                                        bufferedEvents.RemoveAt(bufferedEvents.FindIndex(e => (e.Equals(@event))));
                                        amountEventsDeleted++;
                                    }                                      
                                }
                            }
                            totalConflicts += conflicts.Count;
                            // BufferedEvents contains the events that are done "locally" and conflict with the storeSideEvents.  
                            allConflicts.AddRange(conflicts);
                        }                        
                    }
                }
                foreach (object @event in this.bufferedEvents)
                {
                    PersistEvent(@event);
                }
            }
            // If the connection is lost, create a new buffer for the events that are stored in the meantime and store the latest version. 
            else
            {
                bufferedEvents = new List<IEvent>();
                latestVersion = getLatestStreamRevision();
            }
        }

        /// <summary>
        /// Persists the event in the datastore and returns the revision it now has. 
        /// </summary>
        /// <param name="event">The event to store. </param>
        /// <returns>An int stating the revision to which the event was written. </returns>
        public void PersistEvent(object @event)
        {
            if (this.hasConnection)
            {
                using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
                {
                    stream.Add(new EventMessage { Body = @event });
                    stream.CommitChanges(Guid.NewGuid());
                    latestVersion = getLatestStreamRevision();
                }
            }
            else
            {
                bufferedEvents.Add((IEvent)@event);
            }
        }

        /// <summary>
        /// Persists multiple events to the database. This can be sped up a lot but all events would be placed in the same commit which is not usefull in our situation. 
        /// </summary>
        /// <param name="events"></param>
        public void PersistEvents(List<IEvent> events)
        {
            using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
            {   
                foreach(object @event in events)
                {
                    stream.Add(new EventMessage { Body = @event });
                    stream.CommitChanges(Guid.NewGuid());
                }                
            }
        }

        private static IStoreEvents WireupEventStore()
        {
            return Wireup.Init()
               //.LogToOutputWindow()
               .UsingSqlPersistence("MYSQL")
                   .EnlistInAmbientTransaction() // two-phase commit
                   .InitializeStorageEngine()
                   .UsingJsonSerialization()
                       .Compress()
                //.EncryptWith(EncryptionKey)
                //.HookIntoPipelineUsing(new[] { new AuthorizationPipelineHook() })
               .UsingAsynchronousDispatchScheduler()
               // Enable this to call dispatchCommit method on every commit. 
                   //.DispatchTo(new DelegateMessageDispatcher(DispatchCommit))
               .Build();
        }

        // This is a method that can handle commit errors. 
        private static void DispatchCommit(Commit commit)
        {
            // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
            // MassTransit, WCF, or some other communications infrastructure.
            // This can be a class as well--just implement IDispatchCommits.
            try
            {
                foreach (var @event in commit.Events)
                    Console.WriteLine("Messages from commit have been dispatched: ");// + ((SomeDomainEvent)@event.Body).Value);
            }
            catch (Exception)
            {
                Console.WriteLine("If for some reason we are unable to dispatch, we'd just handle it here.");
            }
        }

        public string getDebugInfo()
        {
            StringBuilder returnString = new StringBuilder();
            using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
            {                
                foreach (Commit comm in store.Advanced.GetFrom(DateTime.MinValue))
                {
                    returnString.AppendLine("Stream id            :" + comm.StreamId);
                    returnString.AppendLine("Stream rev           :" + comm.StreamRevision);
                    returnString.AppendLine("Commit id            :" + comm.CommitId);
                    returnString.AppendLine("Commit Sequence      :" + comm.CommitSequence);
                    returnString.AppendLine("Commit Stamp         :" + comm.CommitStamp);
                    returnString.AppendLine("Stream Type          :" + comm.ToString());
                    returnString.AppendLine("Event                :" + comm.Events[0].Body);
                }
            }
            return returnString.ToString();
        }
    }
}
