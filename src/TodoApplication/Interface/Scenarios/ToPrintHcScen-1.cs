using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class ToPrintHcScen_1 : Scenario
    {
        public ToPrintHcScen_1()
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

SingleUserStep:     new ListCreated(listId, "Groceries")
SingleUserStep:     new TodoItemCreated(todoItem1, "apples", "Buy a big sack of apples", 1, 1)
SingleUserStep:     new TodoItemCreated(todoItem2, "ice", "strawberry ice cream. ", 2, 2)
SingleUserStep:     new TodoItemCreated(todoItem3, "potatoes", "slices", 3, 3)
SingleUserStep:     new ListNameChanged("All the Groceries")

DualUserStep: 
                    new TodoItemNameChanged(todoItem1, "Big apples") 
                    new TodoItemNameChanged(todoItem1, "Apples")
            
DualUserStep: 
                    new TodoItemIndexChanged(todoItem1, 4)
                    new TodoItemIndexChanged(todoItem2, 4)

SingleUserStep:     new ListNameChanged("All the groceries")
                   
DualUserStep: 
                    new TodoItemCreated(todoItem4, "Garbage bags", "Those big red ones", 1, 5)
                    new TodoItemCreated(todoItem5, "Red Garbage bags", "", 1, 5)

DualUserStep: 
                    (nothing committed)
                    new TodoItemPriorityIncreased(todoItem4)


DualUserStep: 
                    new TodoItemDeleted(todoItem5)
                    new TodoItemPriorityIncreased(todoItem5)

DualUserStep: 
                    new TodoItemNameChanged(todoItem4, "Red Garbage bags")
                    new TodoItemDescriptionChanged(todoItem1,"Elstar apples 10x")

SingleUserStep:     new TodoItemsDeleted(todoItem1, todoItem2, todoItem3, todoItem4, todoItem5)


DualUserStep: 
                    new ListNameChanged("Pharmacyitems")
                    new ListNameChanged("Pharmaceuticals")

DualUserStep: 
                    new TodoItemCreated(todoItem6, "Pills", "Black and Yellow", 1, 1)
                    new TodoItemCreated(todoItem7, "Anti Nausia", "Syndicate", 1, 2)

SingleUserStep:     new TodoItemIndexChanged(todoItem7,3)

DualUserStep: 
                    new TodoItemIndexChanged(todoItem7,2)
                    new TodoItemIndexChanged(todoItem6,2)

DualUserStep: 
                    (nothing committed)
                    new TodoItemIndexChanged(todoItem7, 1)

SingleUserStep:     new TodoItemCreated(todoItem8, "Anti stomach ache", "Standard brand", 1, 3)

DualUserStep: 
                    new TodoItemNameChanged(todoItem8, "Stomach ache medicine")
                    new TodoItemNameChanged(todoItem8, "BIG BOX of anti stomach ache")

DualUserStep: 
                    new TodoItemDeleted(todoItem8)
                    new TodoItemDescriptionChanged(todoItem8, "The cheap stuff")

SingleUserStep:     new TodoItemDeleted(todoItem6)

SingleUserStep:     new TodoItemCreated(todoItem9,"Coughing sirup","",2,2)

DualUserStep: 
                    new TodoItemDescriptionChanged(todoItem9, "Buy a lot")
                    new TodoItemDescriptionChanged(todoItem9, "The cheap stuff")

DualUserStep: 
                    new TodoItemIndexChanged(todoItem9, 2)
                    new TodoItemIndexChanged(todoItem9, 3)

DualUserStep: 
                    new TodoItemPriorityDecreased(todoItem9)
                    new TodoItemPriorityIncreased(todoItem9)

DualUserStep: 
                    new TodoItemDeleted(todoItem9)
                    new TodoItemPriorityIncreased(todoItem9)

SingleUserStep:     new ListNameChanged("Done")


        }
    }
}
