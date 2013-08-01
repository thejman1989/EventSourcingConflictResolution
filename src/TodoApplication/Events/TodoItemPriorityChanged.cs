using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemPriorityChanged : IEvent
    {
        public Guid aggregateId { get; set; }
        public int newPriority { get; set; }

        public TodoItemPriorityChanged(Guid aggregateId, int newPriority)
        {
            this.aggregateId = aggregateId;
            this.newPriority = newPriority;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemPriorityChanged))
            {
                TodoItemPriorityChanged toCompareEvent = (TodoItemPriorityChanged)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.newPriority == this.newPriority)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
