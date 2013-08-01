using EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Prototypes
{
    public class OperationalTransformationMerger : MergingMethod
    {
        private List<IEvent> serverSideEvents;
        public List<IEvent> clientSideEvents { get; set; }

        public OperationalTransformationMerger(ICollection<EventMessage> serverSideEvents, List<IEvent> clientSideEvents)
            : base(serverSideEvents, clientSideEvents)
        {
            this.serverSideEvents = messageToEventList(serverSideEvents);
            this.serverSideEvents = removeNullEvents(this.serverSideEvents);
            this.clientSideEvents = clientSideEvents;
            this.clientSideEvents = removeNullEvents(this.clientSideEvents);
        }

        public override List<Conflict> getConflicts()
        {
            List<Conflict> conflicts = new List<Conflict>();

            foreach (IEvent serverEvent in serverSideEvents)
            {
                if (serverEvent == null)
                    break;

                for (int i = 0; i < clientSideEvents.Count; i++)
                {
                    IEvent clientEvent = clientSideEvents.ElementAt(i);

                    if (clientEvent == null)
                        break;

                    IEvent newEvent = null;

                    if(clientEvent.GetType().Equals(typeof(ListCreated)))
                        newEvent = transformEvent((ListCreated)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(ListNameChanged)))
                        newEvent = transformEvent((ListNameChanged)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemCreated)))
                        newEvent = transformEvent((TodoItemCreated)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemDeleted)))
                        newEvent = transformEvent((TodoItemDeleted)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemDescriptionChanged)))
                        newEvent = transformEvent((TodoItemDescriptionChanged)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemNameChanged)))
                        newEvent = transformEvent((TodoItemNameChanged)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemPriorityChanged)))
                        newEvent = transformEvent((TodoItemPriorityChanged)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemPriorityDecreased)))
                        newEvent = transformEvent((TodoItemPriorityDecreased)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemPriorityIncreased)))
                        newEvent = transformEvent((TodoItemPriorityIncreased)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemsDeleted)))
                        newEvent = transformEvent((TodoItemsDeleted)clientEvent, serverEvent);
                    else if (clientEvent.GetType().Equals(typeof(TodoItemIndexChanged)))
                        newEvent = transformEvent((TodoItemIndexChanged)clientEvent, serverEvent);
                    
                    if (newEvent == null)
                    {
                        //If it could not be transformed, a conflict arises. 
                        List<IEvent> conflictList = new List<IEvent>();                        
                        conflictList.Add(serverEvent);
                        conflictList.Add(clientEvent);
                        conflicts.Add(new Conflict(conflictList));
                    }
                    else if (newEvent.GetType() == typeof(Unused))
                    {
                        clientSideEvents.Remove(clientEvent);
                    }
                    else
                    {
                        clientSideEvents[i]= newEvent;
                    }
                }
            }
            return conflicts; 
        }

        public IEvent transformEvent(ListCreated toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(ListCreated)))
            {
                if (toTransform.name == ((ListCreated)transformAgainst).name)
                {
                    return new Unused(); 
                }
                else
                {
                    //Cannot simultaniously create lists. 
                    return null;                    
                }
            }
            return toTransform;
        }

        public IEvent transformEvent(ListNameChanged toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(ListNameChanged)))
            {
                if (toTransform.newName == ((ListNameChanged)transformAgainst).newName)
                    return new Unused();
                //Cannot simultaniously change name. 
                return null;
            }
            return toTransform;
        }
       
        public IEvent transformEvent(TodoItemCreated toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
            {
                TodoItemCreated transFormAgainstCreated = (TodoItemCreated)transformAgainst;
                if (transFormAgainstCreated.aggregateId == toTransform.aggregateId)
                {
                    if (transFormAgainstCreated.Equals(toTransform))
                    {
                        //Both events are the same so remove one. 
                        return new Unused();
                    }
                    //Else cannot create same id item so conflict. 
                    return null;
                }                
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                //Create after delete is no problem. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemIndexChanged)))
            {
                TodoItemIndexChanged transFormAgainstIndex = (TodoItemIndexChanged)transformAgainst;
                if (transFormAgainstIndex.newIndex == toTransform.index)
                {
                    // Create new transform at index +1. This is because the client side has lower prio than server side events. 
                    toTransform = new TodoItemCreated(toTransform.aggregateId, toTransform.name, toTransform.description, toTransform.priority, toTransform.index + 1);
                }
            }
           
            return toTransform;
        }
       
        public IEvent transformEvent(TodoItemDeleted toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
            {
                //Delete an item that has been created server side, impossible through UI, how to know id of item to delete?  
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused();                
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemIndexChanged)))
            {
                // No problem
            }
            return toTransform;
        }

        public IEvent transformEvent(TodoItemDescriptionChanged toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
            {
                //Change a description on an item that has been created server side without this side knowing, impossible. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused(); 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDescriptionChanged)))
            {
                // If it is the exact same, remove the event. 
                if (toTransform.Equals((TodoItemDescriptionChanged)transformAgainst))
                    return new Unused();
                    // If it is On the same aggregate but not the same, conflict. 
                else if (toTransform.aggregateId == ((TodoItemDescriptionChanged)transformAgainst).aggregateId)
                    return null;
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }
            return toTransform;
        }

        public IEvent transformEvent(TodoItemNameChanged toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
            {
                //Change a name on an item that has been created server side without this side knowing, impossible. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused(); 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemNameChanged)))
            {
                // If it is the exact same, remove the event. 
                if (toTransform.Equals((TodoItemNameChanged)transformAgainst))
                    return new Unused();
                // If it is On the same aggregate but not the same, conflict. 
                else if (toTransform.aggregateId == ((TodoItemNameChanged)transformAgainst).aggregateId)
                    return null;
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }
            return toTransform;
        }

        public IEvent transformEvent(TodoItemPriorityChanged toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused(); 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityChanged)))
            {
                if (((TodoItemPriorityChanged)transformAgainst).aggregateId == toTransform.aggregateId)
                {
                    //Same change? Do nothing.
                    if (((TodoItemPriorityChanged)transformAgainst).Equals(toTransform))
                        return new Unused();
                    //Else conflict
                    return null;
                }                
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityDecreased)))
            {
                //Can just append. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityIncreased)))
            {
                //Can also just append. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }
            return toTransform;
        }

        public IEvent transformEvent(TodoItemPriorityDecreased toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused(); 
            }           
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityChanged)))
            {
                //Can just append. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityDecreased)))
            {
                //Can just append       
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityIncreased)))
            {
                //Can also just append. 
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }

            return toTransform;
        }

        public IEvent transformEvent(TodoItemPriorityIncreased toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                if (toTransform.aggregateId == ((TodoItemDeleted)transformAgainst).aggregateId)
                    //Delete todo item when other side has also deleted it. No problem, remove this event.
                    return new Unused(); 
            }            
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityChanged)))
            {
                //Can just append       
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityDecreased)))
            {
                //Can just append       
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemPriorityIncreased)))
            {
                //Can just append       
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                //If the item deleted is deleted by multiple on the server, remove this event 
                if (((TodoItemsDeleted)transformAgainst).aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }

            return toTransform;
        }
        
        public IEvent transformEvent(TodoItemsDeleted toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                //If this multi delete contains the delete on the other side, remove it from the deletes. 
                if (toTransform.aggregateIds.Contains(((TodoItemDeleted)transformAgainst).aggregateId))
                {
                    List<Guid> newAggregateList = toTransform.aggregateIds;
                    newAggregateList.Remove(((TodoItemDeleted)transformAgainst).aggregateId);
                    TodoItemsDeleted transformedEvent = new TodoItemsDeleted(newAggregateList);
                    return transformedEvent;
                }
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                // Figure out overlap and remove that. 
                TodoItemsDeleted transformAgainstEvent = (TodoItemsDeleted)transformAgainst;
                List<Guid> newAggregateList = toTransform.aggregateIds;
                foreach (Guid toRemoveGuid in transformAgainstEvent.aggregateIds)
                {
                    if (newAggregateList.Contains(toRemoveGuid))
                        newAggregateList.Remove(toRemoveGuid);
                }
                // If the list is now empty, remove the event. 
                if (newAggregateList.Count == 0)
                    return new Unused();
                // Else return a new event with the new list. 
                return new TodoItemsDeleted(newAggregateList);
            }
            return toTransform;
        }

        public IEvent transformEvent(TodoItemIndexChanged toTransform, IEvent transformAgainst)
        {
            if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
            {
                // Check if index is same. 
                TodoItemCreated transformAgainstCreated = (TodoItemCreated)transformAgainst;
                if (transformAgainstCreated.index == toTransform.newIndex)
                {
                    return new TodoItemIndexChanged(toTransform.aggregateId, toTransform.newIndex + 1);
                }
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
            {
                //If it's removed then remove this event as well. 
                if (toTransform.aggregateId == transformAgainst.aggregateId)
                    return new Unused();
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
            {
                TodoItemsDeleted transformAgainstDeleted= (TodoItemsDeleted) transformAgainst;
                if (transformAgainstDeleted.aggregateIds.Contains(toTransform.aggregateId))
                    return new Unused();
            }
            else if (transformAgainst.GetType().Equals(typeof(TodoItemIndexChanged)))
            {
                TodoItemIndexChanged transformAgainstIndex = (TodoItemIndexChanged)transformAgainst;
                if (transformAgainstIndex.newIndex == toTransform.newIndex)
                    return new TodoItemIndexChanged(toTransform.aggregateId, toTransform.newIndex +1);
            }
            return toTransform;
        }
    }
}

//Basic transformations.
//if (transformAgainst.GetType().Equals(typeof(ListCreated)))
//            {
//                return null;
//            }
//            else if (transformAgainst.GetType().Equals(typeof(ListNameChanged)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemCreated)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemDeleted)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemDescriptionChanged)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemNameChanged)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemOrderChanged)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemOrderDecreased)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemOrderIncreased)))
//            {

//            }
//            else if (transformAgainst.GetType().Equals(typeof(TodoItemsDeleted)))
//            {

//            }
//            return toTransform;