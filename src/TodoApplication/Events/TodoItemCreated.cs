using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemCreated : IEvent
    {
        public Guid aggregateId { get; set; }
        public string name { get; protected set; }
        public string description { get; protected set; }
        public int priority { get; protected set; }
        public int index { get; protected set; }

        public TodoItemCreated(Guid AggregateId, string name, string description, int priority, int index)
        {
            this.aggregateId = AggregateId;
            this.name = name;
            this.description = description;
            this.priority = priority;
            this.index = index;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemCreated))
            {
                TodoItemCreated toCompareEvent = (TodoItemCreated)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.name == this.name)
                    {
                        if (toCompareEvent.description == this.description)
                        {
                            if (toCompareEvent.priority == this.priority)
                            {
                                if(toCompareEvent.index == this.index)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
