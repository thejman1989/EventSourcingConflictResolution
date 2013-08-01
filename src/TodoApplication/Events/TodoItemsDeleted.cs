using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemsDeleted : IEvent
    {
        public Guid aggregateId { get; set; }
        public List<Guid> aggregateIds { get; set; }

        public TodoItemsDeleted(List<Guid> AggregateIds)
        {
            this.aggregateIds = AggregateIds;
        }

        public bool Equals(IEvent toCompare)
        {            
            if (toCompare.GetType() == typeof(TodoItemsDeleted))
            {
                TodoItemsDeleted toCompareEvent = (TodoItemsDeleted)toCompare;
                if (toCompareEvent.aggregateIds.Count == this.aggregateIds.Count) 
                {
                    for (int i = 0; i < this.aggregateIds.Count; i++)
                    {
                        if (!(this.aggregateIds.ElementAt(i).Equals(toCompareEvent.aggregateIds.ElementAt(i))))
                            return false; 
                    }
                    if (toCompareEvent.aggregateIds == this.aggregateIds)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
