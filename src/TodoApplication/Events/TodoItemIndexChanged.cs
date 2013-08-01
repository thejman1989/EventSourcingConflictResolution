using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemIndexChanged : IEvent
    {
        public Guid aggregateId { get; set; }
        public int newIndex { get; protected set; }

        public TodoItemIndexChanged(Guid aggregateId, int newIndex)
        {
            this.aggregateId = aggregateId;
            this.newIndex = newIndex;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemIndexChanged))
            {
                TodoItemIndexChanged toCompareEvent = (TodoItemIndexChanged)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.newIndex == this.newIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

