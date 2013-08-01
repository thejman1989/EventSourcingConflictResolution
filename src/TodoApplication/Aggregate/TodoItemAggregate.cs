using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Aggregate
{
    public class TodoItemAggregate : AggregateBase
    {
        public string name { get; protected set; }
        public string description { get; protected set; }
        public int priority { get; protected set; }
        public int index { get; set; }

        public TodoItemAggregate(Guid aggregateId) : this()
        {            
            RaiseEvent(new TodoItemCreated(aggregateId,"UNINIT","UNINIT",-1, -1) );
        }

        private TodoItemAggregate()
        {
            //Register all possible events on this aggregate. 
            Register<TodoItemCreated>(e => 
                {
                    this.id = e.aggregateId;
                    this.name = e.name;
                    this.description = e.description;
                    this.priority = e.priority;
                    this.index = e.index;
                }
            );         
            Register<TodoItemNameChanged>(e => { this.name = e.newName; AppliedEventCount++; });
            Register<TodoItemPriorityChanged>(e => { this.priority = e.newPriority; AppliedEventCount++; });
            Register<TodoItemDescriptionChanged>(e => { this.description = e.newDescription; AppliedEventCount++; });
            Register<TodoItemIndexChanged>(e => { this.index = e.newIndex; AppliedEventCount++; });
            Register<TodoItemPriorityDecreased>(e => { this.priority--; });
            Register<TodoItemPriorityIncreased>(e => { this.priority++; });
        }

        public int AppliedEventCount { get; private set; }

        public override bool Equals(object obj)
        {
            //if this gets called implement right function.
            if (obj.GetType() != typeof(TodoItemAggregate))
                return false;

            TodoItemAggregate toCheck = (TodoItemAggregate)obj;

            if (!(toCheck.description.Equals(this.description)))
                return false;
            if (!(toCheck.id.Equals(this.id)))
                return false;
            if (!(toCheck.name.Equals(this.name)))
                return false;
            if (!(toCheck.priority.Equals(this.priority)))
                return false;
            if (!toCheck.index.Equals(this.index))
                return false; 

            return true; 
        }

        public override bool Equals(IAggregate other)
        {
            if (other.GetType() != typeof(TodoItemAggregate))
                return false;

            TodoItemAggregate toCheck = (TodoItemAggregate)other;

            if (!(toCheck.description.Equals(this.description)))
                return false;
            if (!(toCheck.id.Equals(this.id)))
                return false;
            if (!(toCheck.name.Equals(this.name)))
                return false;
            if (!(toCheck.priority.Equals(this.priority)))
                return false;
            if (!toCheck.index.Equals(this.index))
                return false; 

            return true; 
        }
    }
}
