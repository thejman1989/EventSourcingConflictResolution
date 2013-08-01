using EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Prototypes
{
    public abstract class MergingMethod
    {
        public MergingMethod(ICollection<EventMessage> leftSideEvents, List<IEvent> rightSideEvents) { }
        public abstract List<Conflict> getConflicts();
        protected int amountEventsDeleted = 0;

        protected List<IEvent> messageToEventList(ICollection<EventMessage> messages)
        {
            List<IEvent> returnList = new List<IEvent>();

            foreach (EventMessage message in messages)
            {
                returnList.Add((IEvent)message.Body);
            }

            return returnList;
        }

        protected List<IEvent> removeNullEvents(List<IEvent> toRemoveFrom)
        {            
            toRemoveFrom.RemoveAll(e => e == null);
            return toRemoveFrom;
        }

        public int getAmountEventsDeleted()
        {
            return amountEventsDeleted;
        }
    }
}
