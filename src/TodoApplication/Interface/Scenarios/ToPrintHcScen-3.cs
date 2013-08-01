using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class ToPrintHcScen_3 : Scenario
    {
        public ToPrintHcScen_3()
            : base()
        {
            this.addTestEvents();
        }


        private void addTestEvents()
        {
            Guid listId = Guid.NewGuid();
            Guid todoItem1 = Guid.NewGuid();
            Guid todoItem2 = Guid.NewGuid();
            Guid todoItem3 = Guid.NewGuid();
            Guid todoItem4 = Guid.NewGuid();
            Guid todoItem5 = Guid.NewGuid();
            Guid todoItem6 = Guid.NewGuid();
            Guid todoItem7 = Guid.NewGuid();
            Guid todoItem8 = Guid.NewGuid();
            Guid todoItem9 = Guid.NewGuid();
            
            
SingleUserStep:     new ListCreated(listId, "TestList")

DualUserStep: 
                    new TodoItemCreated(todoItem1, "One", "One", 1, 1)
                    new TodoItemCreated(todoItem2, "Two", "Two", 2, 2)

SingleUserStep:     new ListNameChanged("TestList2")

DualUserStep: 
                    new TodoItemCreated(todoItem3, "Three", "Three", 3, 3)
                    new TodoItemCreated(todoItem4, "Four", "Four", 4, 4)
            
DualUserStep: 
                    new TodoItemNameChanged(todoItem1,"One One")
                    new TodoItemNameChanged(todoItem1,"One Two")

DualUserStep: 
                    new TodoItemIndexChanged(todoItem2, 1)
                    new TodoItemIndexChanged(todoItem2, 1)

DualUserStep: 
                    new TodoItemPriorityChanged(todoItem2, 3)
                    new TodoItemPriorityChanged(todoItem2, 4)

DualUserStep: 
                    new TodoItemPriorityIncreased(todoItem2)
                    new TodoItemPriorityIncreased(todoItem2)

DualUserStep: 
                    new TodoItemPriorityDecreased(todoItem2)
                    new TodoItemPriorityDecreased(todoItem2)

SingleUserStep:     new ListNameChanged("TestList3")

DualUserStep: 
                    new TodoItemDeleted(todoItem1)
                    new TodoItemDeleted(todoItem1)

DualUserStep: 
                    new TodoItemIndexChanged(todoItem2,5)
                    new TodoItemIndexChanged(todoItem2,5)

DualUserStep: 
                    new TodoItemIndexChanged(todoItem3, 6)
                    new TodoItemIndexChanged(todoItem3, 7)                                                  

DualUserStep: 
                    new TodoItemNameChanged(todoItem4, "NewFour")
                    new TodoItemNameChanged(todoItem4, "OldFour")

DualUserStep: 
                    new TodoItemDeleted(todoItem4)
                    new TodoItemDescriptionChanged(todoItem4, "NewDescription")

DualUserStep: 
                    new TodoItemDeleted(todoItem1)
                    new TodoItemsDeleted(todoItem1)

SingleUserStep:     new TodoItemCreated(todoItem8, "Eight", "Eight", 1, 1)
SingleUserStep:     new TodoItemCreated(todoItem9, "Nine", "Nine", 2, 2)

DualUserStep: 
                    new TodoItemNameChanged(todoItem8, "One")
                    new TodoItemDescriptionChanged(todoItem8, "First")  

DualUserStep: 
                    new TodoItemDescriptionChanged(todoItem8, "One")
                    new TodoItemNameChanged(todoItem8, "First")

DualUserStep: 
                    new TodoItemPriorityChanged(todoItem9, 3)
                    new TodoItemPriorityIncreased(todoItem9)

DualUserStep: 
                    new TodoItemPriorityDecreased(todoItem9)
                    new TodoItemPriorityIncreased(todoItem9)

        }
    }
}
