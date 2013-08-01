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
    class OperationBasedMerger : MergingMethod
    {
        List<IEvent> serverSideEvents;
        public List<IEvent> clientSideEvents {get; set;}
        List<IEvent> allPreviousEvents;

        public OperationBasedMerger(ICollection<EventMessage> serverSideEvents, List<IEvent> clientSideEvents, ICollection<EventMessage> allPreviousEvents)
            : base(serverSideEvents, clientSideEvents)
        {
            this.serverSideEvents = messageToEventList(serverSideEvents);
            this.serverSideEvents = removeNullEvents(this.serverSideEvents);
           
            this.clientSideEvents = clientSideEvents;
            this.clientSideEvents = removeNullEvents(this.clientSideEvents);
            
            this.allPreviousEvents = messageToEventList(allPreviousEvents);
        }

        public override List<Conflict> getConflicts()
        {
            removeRedundantEvents();

            List<Conflict> conflicts = new List<Conflict>();
            for(int i = 0; i< serverSideEvents.Count; i++)            
            {
                IEvent leftSideEvent = serverSideEvents.ElementAt(i);
                for(int y = 0; y < clientSideEvents.Count; y++)
                {
                    IEvent rightSideEvent = clientSideEvents.ElementAt(y);
                    // Both before is a conflict. 
                    if (before(leftSideEvent, rightSideEvent) && before(rightSideEvent, leftSideEvent))
                    {
                        Debug.WriteLine("Both before! :");
                        // If they are the same then the right side event can be removed.
                        if (leftSideEvent.Equals(rightSideEvent))
                        {
                            clientSideEvents.Remove(rightSideEvent);
                        }
                        // If they commute locally they can be reorderd to remove the problem. 
                        else if (commuteLocally(leftSideEvent, rightSideEvent, allPreviousEvents))
                        {
                            // Should reorder them to make sure there is really no problem. Reordering is impossible in event sourcing env. 
                            // Now just appends in wrong order. TODO IMPORTANT
                            Debug.WriteLine("   Commute locally, no problem.");
                        }
                        else
                        {
                            Debug.WriteLine("   No local commute, conflict.");
                            List<IEvent> conflictList = new List<IEvent>();
                            conflictList.Add(leftSideEvent);
                            conflictList.Add(rightSideEvent);
                            conflicts.Add(new Conflict(conflictList));    
                        }                                            
                    }
                }
            }           
            return conflicts;
        }

        /// <summary>
        /// Check if a certain event has to be performed before another event. 
        /// </summary>
        /// <param name="firstEvent"> The first event.</param>
        /// <param name="secondEvent">The second event.</param>
        /// <returns>If the first event has to be before the second event. </returns>
        /// 
        private bool before(IEvent firstEvent, IEvent secondEvent)
        {
            return !commuteGlobally(firstEvent,secondEvent);
        }

        /// <summary>
        /// Checks if the events commute globally, this means testing it to some basic rules, like no edits on the same aggregateId. 
        /// This corresponds a lot with the Syntax merging strategy. 
        /// </summary>
        /// <returns></returns>
        private bool commuteGlobally(IEvent leftSideEvent, IEvent rightSideEvent)
        {
            // Lippe says disjoint read and write sets. This is no same events on same id Stole this from syntactic merger. 
            // If one of the events is null then there is no conflict and they thus commute locally.     
            if (leftSideEvent == null || rightSideEvent == null)
                return true;

            // If the events are of the same type... 
            if (leftSideEvent.GetType() == rightSideEvent.GetType())
            {
                if (leftSideEvent.aggregateId == rightSideEvent.aggregateId)
                {
                    // Return false, let top method catcht it and remove one if they are the same...
                    return false;
                    //// If type and aggregate are the same, check if it is alltogether the same event, then there is no conflict. 
                    //// Have to figure out how to remove one of the two events..... 
                    //if (leftSideEvent.Equals(rightSideEvent))
                    //{
                    //    // REMOVE ONE EVENT.
                    //    return true;
                    //}
                    //else
                    //{                      
                    //    return false; 
                    //}
                }              
            }

            // If the leftSideEvent deletes the item beign edited by the right side then they do not commute. 
            if (leftSideEvent.GetType() == typeof(TodoItemDeleted))
            {
                if(leftSideEvent.aggregateId == rightSideEvent.aggregateId)
                    return false;
            }
            // Same as normal delete but on multiple.
            if (leftSideEvent.GetType() == typeof(TodoItemsDeleted))
            {
                foreach (Guid id in ((TodoItemsDeleted)leftSideEvent).aggregateIds)
                {
                    if (rightSideEvent.aggregateId == id)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// Checks if two arrays of events(Transformations) commute locally.
        /// </summary>
        /// <returns> Boolean saying they commute locally. </returns>
        private bool commuteLocally(List<IEvent> leftEvents, List<IEvent> rightEvents, List<IEvent> previousEvents)
        {
            ListState currentState = new ListState();
            ListState firstCheckState = new ListState();
            ListState secondCheckState = new ListState();

            // Load all current events to the states.
            foreach (IEvent @event in previousEvents)
            {
                currentState.Load(@event);
                firstCheckState.Load(@event);
                secondCheckState.Load(@event);
            }

            // Load all left side events on first check. 
            foreach (IEvent message in leftEvents)
            {
                //firstCheckState.Load(message.Body);
                loadIfPossible(firstCheckState, message);
            }

            //Load all right side events on first check. 
            foreach (IEvent @event in rightEvents)
            {
                loadIfPossible(firstCheckState, @event);
                //firstCheckState.Load(@event);
            }

            // Do it the other way around for the second check. 
            foreach (IEvent @event in rightEvents)
            {
                //secondCheckState.Load(@event);
                loadIfPossible(secondCheckState, @event);
            }
            foreach (IEvent message in leftEvents)
            {
                //secondCheckState.Load(message.Body);
                loadIfPossible(secondCheckState, message);
            }

            // Now check if the two states are the same. 
            return firstCheckState.Equals(secondCheckState);
        }


        /// <summary>
        /// Checks if two of events(Transformations) commute locally.
        /// </summary>
        /// <returns> Boolean saying they commute locally. </returns>
        private bool commuteLocally(IEvent leftSideEvent, IEvent rightSideEvent, List<IEvent> previousEvents)
        {           
            ListState currentState = new ListState();
            ListState checkState = new ListState();

            // Load all current events to the states.
            foreach (IEvent @event in previousEvents)
            {
                currentState.Load(@event);
                checkState.Load(@event);
            }

            // Load one way
            currentState.Load(leftSideEvent);
            currentState.Load(rightSideEvent);

            // Load other way around.
            checkState.Load(rightSideEvent);
            checkState.Load(leftSideEvent);

            // Now check if the two states are the same. 
            return currentState.Equals(checkState);
        }

        private bool loadIfPossible(ListState currentState, EventMessage @event)
        {
            return loadIfPossible(currentState, @event.Body);
        }

        private bool loadIfPossible(ListState currentState, object @event)
        {
            IEvent currentEvent = (IEvent) @event;
            if (currentState.currentList.id == currentEvent.aggregateId)
            {
                currentState.Load(currentEvent);
                return true;
            }
            if (currentState.currentList.todoItems.Any(e => e.id == currentEvent.aggregateId))
            {
                currentState.Load(currentEvent);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Function to remove redundant events, all events that can be removed without changing final state. 
        /// </summary>
        private void removeRedundantEvents()
        {
           serverSideEvents = removeRedundantEvents(serverSideEvents);
           clientSideEvents= removeRedundantEvents(clientSideEvents);
        }

        private List<IEvent> removeRedundantEvents(List<IEvent> events)
        {
            List<IEvent> returnList = new List<IEvent>();
            
            ListState finalState = new ListState();
            
            //Load the normal final state of all the events. 
            finalState.LoadList(allPreviousEvents);
            finalState.LoadList(events);            
           
            foreach (IEvent @event in events)
            {
                if (@event == null)
                    break;
                returnList.Add(@event);
                
                ListState checkState = new ListState();
                checkState.LoadList(allPreviousEvents);
                // Load a stat without the current event. 
                IEnumerable<IEvent> without = events.Where(e => !(e.Equals(@event)));
                List<IEvent> withoutList = without.ToList();
                checkState.LoadList(without.ToList());

                // Check if it is the same as the final state. 
                if (checkState.Equals(finalState))
                {
                    Debug.WriteLine("Redundant operation found!!!");
                    returnList.Remove(@event);
                }
            }

            return returnList;
        }

    }
}
