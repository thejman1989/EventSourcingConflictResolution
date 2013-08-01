using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemPriorityDecreased : IEvent
    {
        public Guid aggregateId { get; set; }

        public TodoItemPriorityDecreased(Guid aggregateId)
        {
            this.aggregateId = aggregateId;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemPriorityDecreased))
            {
                TodoItemPriorityDecreased toCompareEvent = (TodoItemPriorityDecreased)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    return true;                   
                }
            }
            return false;
        }

    }
}
