using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemDescriptionChanged : IEvent
    {
        public Guid aggregateId { get; set; }
        public string newDescription { get; protected set; }

        public TodoItemDescriptionChanged(Guid aggregateId, string newDescription)
        {
            this.aggregateId = aggregateId;
            this.newDescription = newDescription;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemDescriptionChanged))
            {
                TodoItemDescriptionChanged toCompareEvent = (TodoItemDescriptionChanged)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.newDescription == this.newDescription)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
