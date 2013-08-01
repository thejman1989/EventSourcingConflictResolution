using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemNameChanged : IEvent
    {
        public Guid aggregateId { get; set; }
        public string newName { get; protected set; }
       
        public TodoItemNameChanged(Guid aggregateId, string newName)
        {
            this.aggregateId = aggregateId;
            this.newName = newName;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemNameChanged))
            {
                TodoItemNameChanged toCompareEvent = (TodoItemNameChanged)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.newName == this.newName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
