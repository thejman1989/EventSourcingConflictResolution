using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Aggregate
{
    public class ListAggregate : AggregateBase
    {
        public string name { get; private set; }
        public Guid id { get; private set; }
        public List<TodoItemAggregate> todoItems;

        
        //Create an empty aggregate so it can be reloaded from the database later. 
        public ListAggregate() : this(Guid.Empty) { }

        public ListAggregate(Guid id)
        {
            this.id = id;
            todoItems = new List<TodoItemAggregate>();

            Register<TodoItemCreated>(e => 
            {
                //If the list already contains the same item
                if (todoItems.Any(item => item.id == e.aggregateId))
                {                    
                    //Will never happen exept when same event gets handled twice... or when 2 random guids are the same. 
                }
                else
                {
                    // Make sure index is unique. 
                    freeIndexIfNeeded(e.index);
                    TodoItemAggregate newItem = new TodoItemAggregate(Guid.Empty);
                    newItem.RaiseEvent(e);
                    todoItems.Add(newItem);                           
                }
            });

            Register<TodoItemDeleted>(e => 
                {
                    if(!(todoItems.Any(item => item.id == e.aggregateId)))
                    {
                        // Item that needs to be deleted is not there. 
                        Debug.WriteLine("Attempted to remove an item that was not there.");
                    }
                    else
                    {
                        todoItems.Remove(todoItems.First(item => item.id == e.aggregateId));
                    }
                    
                });
            Register<TodoItemDescriptionChanged>(e => redirectToTodoItem(e));
            Register<TodoItemNameChanged>(e => redirectToTodoItem(e));
            Register<TodoItemPriorityChanged>(e => redirectToTodoItem(e));
            Register<TodoItemPriorityDecreased>(e => redirectToTodoItem(e));
            Register<TodoItemPriorityIncreased>(e => redirectToTodoItem(e));
            Register<TodoItemIndexChanged>(e => 
            {
                freeIndexIfNeeded(e.newIndex);
                redirectToTodoItem(e); 
            });
            Register<TodoItemsDeleted>(e => removeMultiple(e));

            Register<ListCreated>(e => 
            {
                if (id.Equals(Guid.Empty))
                {
                    this.id = e.aggregateId;
                    this.name = e.name;
                }
            });
           
            Register<ListNameChanged>(e =>
            {
                this.name = e.newName;
            });
        }

        /// <summary>
        ///  This function frees the index if that is needed. This is done when an item is inserted at an occupied index so every other item shifts right.
        /// </summary>
        /// <param name="requestedIndex"></param>
        public void freeIndexIfNeeded(int requestedIndex)
        {
            List<int> takenIndices = getTakenIndices();
            if (takenIndices.Contains(requestedIndex))
            {
                //If the index is taken, shift consecutive event indexes. 
                foreach (TodoItemAggregate todoItem in todoItems)
                {
                    if (todoItem.index >= requestedIndex)
                    {
                        todoItem.index += 1;
                    }
                }               
            }
        }

        public void redirectToTodoItem(IEvent e)
        {
            TodoItemAggregate todoItem = todoItems.Find(item => item.id == e.aggregateId);
            if (todoItem == null)
            {
                // This is caused when the merger has allowed a change action to be appended after a delete action. 
                // This is done because they do locally commute, but they cannot be executed after each other.                
                Debug.WriteLine("Error, tried to append event to item that does not exist.");
                return;
            }
            todoItem.RaiseEvent(e);
        }

        public void removeMultiple(TodoItemsDeleted @event)
        {
            foreach (Guid guid in @event.aggregateIds)
            {
                this.RaiseEvent(new TodoItemDeleted(guid));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ListAggregate))
            {
                return false;
            }

            ListAggregate toCheck = (ListAggregate)obj;
            if (!(toCheck.id.Equals(this.id)))
                return false;
            if (!(toCheck.name.Equals(this.name)))
                return false;
            for (int i = 0; i < toCheck.todoItems.Count; i++)
            {
                if (!(toCheck.todoItems.ElementAt(i).Equals(this.todoItems.ElementAt(i))))
                    return false; 
            }
            return true; 
        }

        public override bool Equals(IAggregate other)
        {
            if (other.GetType() != typeof(ListAggregate))
            {
                return false;
            }

            ListAggregate toCheck = (ListAggregate)other;
            if (!(toCheck.id.Equals(this.id)))
                return false;
            if (!(toCheck.name.Equals(this.name)))
                return false;
            if(!(toCheck.todoItems.Count.Equals(this.todoItems.Count)))
                return false;
            for (int i = 0; i < toCheck.todoItems.Count; i++)
            {
                if (!(toCheck.todoItems.ElementAt(i).Equals(this.todoItems.ElementAt(i))))
                    return false;
            }
            return true; 
        }

        public List<int> getTakenIndices()
        {
            List<int> takenIndices = new List<int>();
            foreach (TodoItemAggregate item in todoItems)
            {
                takenIndices.Add(item.index);
            }
            return takenIndices;
        }
    }
}
