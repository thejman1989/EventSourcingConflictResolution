using EventStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Aggregate;
using TodoApplication.Events;

namespace TodoApplication.Prototypes
{
    // TODO this is not the correct implementation. 
    // The idea is that everything is merged together and then check if the syntax is correct. 
    // Now the two arrays of events are checked to see if they can be put together according to the rules of the syntax. 
    // Is this the same ? Does this give the same results? 
    public class SyntacticMerger : MergingMethod
    {

        List<IEvent> serverSideEvents;
        public List<IEvent> clientSideEvents { get; set; }

        List<IEvent> allPreviousEvents = new List<IEvent>();

        public SyntacticMerger(ICollection<EventMessage> serverSideEvents, List<IEvent> clientSideEvents, ICollection<EventMessage> allPreviousEvents)
            : base(serverSideEvents, clientSideEvents)
        {
            this.serverSideEvents = messageToEventList(serverSideEvents);
            this.serverSideEvents = removeNullEvents(this.serverSideEvents);

            this.clientSideEvents = clientSideEvents;
            this.clientSideEvents = removeNullEvents(this.clientSideEvents);

            this.allPreviousEvents = messageToEventList(allPreviousEvents);
            this.allPreviousEvents = removeNullEvents(this.allPreviousEvents);
        }
        

        /// <summary>
        /// Gets all conflicts by analysing the events and checking if the syntax is still in order when the events would be merged. 
        /// </summary>
        /// <returns>The conflicts that arise.</returns>
        override
        public List<Conflict> getConflicts()
        {
            List<IEvent> returnList = new List<IEvent>();
            List<Conflict> conflictList = new List<Conflict>();

            for(int i = 0; i < serverSideEvents.Count; i++)
            {                    
                IEvent leftSideEvent = serverSideEvents.ElementAt(i);
                if (leftSideEvent == null)
                    break;

                for (int y = 0; y < clientSideEvents.Count; y++)
                {
                    IEvent rightSideEvent = clientSideEvents.ElementAt(y);
                    if (rightSideEvent == null)
                        break;

                   
                    // Check unique index conflicts.
                    if (leftSideEvent.GetType() == typeof(TodoItemIndexChanged) && rightSideEvent.GetType() == typeof(TodoItemIndexChanged))
                    {
                        if (leftSideEvent.Equals(rightSideEvent))
                        {
                            // Here the rightside event should be removed. Events removed are also removed from original array. 
                            clientSideEvents.Remove(rightSideEvent);
                        }
                        
                        else if (((TodoItemIndexChanged)leftSideEvent).newIndex == ((TodoItemIndexChanged)rightSideEvent).newIndex)
                        {
                            // If changed to the same index, conflict. 
                            List<IEvent> conflictingEvents = new List<IEvent>();
                            conflictingEvents.Add(leftSideEvent);
                            conflictingEvents.Add(rightSideEvent);
                            conflictList.Add(new Conflict(conflictingEvents));
                        }                        
                    }
                    else if (leftSideEvent.GetType() == typeof(TodoItemIndexChanged))
                    {
                        // Index changed to an index at which a todoitem has been created? Conflict
                        if (rightSideEvent.GetType() == typeof(TodoItemCreated))
                        {
                            if (((TodoItemCreated)rightSideEvent).index == ((TodoItemIndexChanged)leftSideEvent).newIndex)
                            {
                                List<IEvent> conflictingEvents = new List<IEvent>();
                                conflictingEvents.Add(leftSideEvent);
                                conflictingEvents.Add(rightSideEvent);
                                conflictList.Add(new Conflict(conflictingEvents));
                            }
                        }
                    }
                    else if (rightSideEvent.GetType() == typeof(TodoItemIndexChanged))
                    {
                        // Same as the above. 
                        if (leftSideEvent.GetType() == typeof(TodoItemCreated))
                        {
                            if (((TodoItemCreated)leftSideEvent).index == ((TodoItemIndexChanged)rightSideEvent).newIndex)
                            {
                                List<IEvent> conflictingEvents = new List<IEvent>();
                                conflictingEvents.Add(leftSideEvent);
                                conflictingEvents.Add(rightSideEvent);
                                conflictList.Add(new Conflict(conflictingEvents));
                            }
                        }
                    }

                    // Check same event conflicts. 
                    else if (leftSideEvent.GetType() == rightSideEvent.GetType())
                    {
                        Debug.WriteLine("Same Type!");
                        if (leftSideEvent.aggregateId == rightSideEvent.aggregateId)
                        {
                            // TODO remove events that are the same. Do for all methods. Important for number of events reached at the end.
                            // Now they are both appended. 
                            // If type and aggregate are the same, check if it is alltogether the same event, then there is no conflict. 
                            if (leftSideEvent.Equals(rightSideEvent))
                            {
                                // Priority in/decrement can both be appended, all others can be removed. 
                                if (leftSideEvent.GetType() != typeof(TodoItemPriorityDecreased) && leftSideEvent.GetType() != typeof(TodoItemPriorityIncreased))
                                    clientSideEvents.Remove(rightSideEvent);
                            }
                            // Else add a conflict to the conflict list. 
                            else
                            {
                                Debug.WriteLine("Same ID, CONFLICTTTTTTTTTTTTTTT!");
                                List<IEvent> conflictingEvents = new List<IEvent>();
                                conflictingEvents.Add(leftSideEvent);
                                conflictingEvents.Add(rightSideEvent);                                
                                conflictList.Add(new Conflict(conflictingEvents));
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Not on same aggregate.");
                        }
                    }

                    // If the left side event deleted the todo item, there can be no change in that todo item in the right side events. 
                    else if (leftSideEvent.GetType() == typeof(TodoItemDeleted))
                    {
                        if (leftSideEvent.aggregateId == rightSideEvent.aggregateId)
                        {
                            Debug.WriteLine("Edit on deleted item!");
                            List<IEvent> conflictingEvents = new List<IEvent>();
                            conflictingEvents.Add(leftSideEvent);
                            conflictingEvents.Add(rightSideEvent);
                            conflictList.Add(new Conflict(conflictingEvents));
                        }
                    }

                    else if (leftSideEvent.GetType() == typeof(TodoItemsDeleted))
                    {
                        foreach (Guid LeftSideId in ((TodoItemsDeleted)leftSideEvent).aggregateIds)
                        {
                            if (LeftSideId == rightSideEvent.aggregateId)
                            {
                                Debug.WriteLine("Edit on multi-deleted item!");
                                List<IEvent> conflictingEvents = new List<IEvent>();
                                conflictingEvents.Add(leftSideEvent);
                                conflictingEvents.Add(rightSideEvent);
                                conflictList.Add(new Conflict(conflictingEvents));
                            }
                        }
                    }
                }
            }
            return conflictList;
        }        
    }
}
