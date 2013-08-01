using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class ToPrintHcScen_2 : Scenario
    {
        public ToPrintHcScen_2()
            : base()
        {
            this.addTestEvents();
        }

        private void addTestEvents()
        {
            Guid listId = Guid.NewGuid();
            Guid todo1 = Guid.NewGuid();
            Guid todo2 = Guid.NewGuid();
            Guid todo3 = Guid.NewGuid();
            Guid todo4 = Guid.NewGuid();
            Guid todo5 = Guid.NewGuid();
            Guid todo6 = Guid.NewGuid();
            Guid todo7 = Guid.NewGuid();
            Guid todo8 = Guid.NewGuid();
            Guid todo9 = Guid.NewGuid();
            Guid todo10 = Guid.NewGuid();
           
            
SingleUserStep:     new ListCreated(listId, "Beach party")

DualUserStep: 
                    new TodoItemCreated(todo1,"Get the tent set up","Main tent",1,1)
                    new TodoItemCreated(todo2,"Get the music ready","Hardrock", 1, 1)

SingleUserStep:     new ListNameChanged("Beach event")

DualUserStep: 
                    new TodoItemCreated(todo3, "Get drinks ready", "Beer and sprite", 1, 2)
                    new TodoItemDeleted(todo2)

DualUserStep: 
                    new TodoItemIndexChanged(todo1, 3)
                    new TodoItemCreated(todo4, "Fill the pool","The big one first.",2,3)
            
DualUserStep: 
                    new TodoItemIndexChanged(todo3,1)
                    new TodoItemNameChanged(todo4,"Fill the BIG pool")

SingleUserStep:     new TodoItemDeleted(todo1)


DualUserStep: 
                    new TodoItemCreated(todo5,"Buy some bandaids","only if there is time left",1,5)
                    new TodoItemCreated(todo6, "Mow the lawn", "", 2, 6)

DualUserStep: 
                    new TodoItemNameChanged(todo3,"DRINKS!")
                    new TodoItemNameChanged(todo3, "SOMEONE do the drinks")
           
SingleUserStep:     new TodoItemDescriptionChanged(todo6,"What lawn???")

            
DualUserStep: 
                    new TodoItemDeleted(todo6)
                    new TodoItemDeleted(todo6)
            
DualUserStep: 
                    new TodoItemIndexChanged(todo3,1)
                    new TodoItemIndexChanged(todo3,2)

SingleUserStep:     new ListNameChanged("Beach event - One hour left")


DualUserStep: 
                    new TodoItemIndexChanged(todo4,1)
                    new TodoItemIndexChanged(todo5,1)

DualUserStep: 
                    new TodoItemIndexChanged(todo5,3)
                    new TodoItemIndexChanged(todo4,1)

DualUserStep: 
                    new TodoItemDeleted(todo3)
                    new TodoItemDeleted(todo3)

SingleUserStep:     new ListNameChanged("Get ready, about to start.")


DualUserStep: 
                    new TodoItemDescriptionChanged(todo5, "Important!")
                    new TodoItemCreated(todo7,"clean the room","",3,3)

DualUserStep: 
                    new TodoItemIndexChanged(todo5,1)
                    new TodoItemDescriptionChanged(todo7,"the vip room")

DualUserStep: 
                    (nothing committed)
                    new TodoItemIndexChanged(todo7,1)


SingleUserStep:     new TodoItemDeleted(todo5)
SingleUserStep:     new TodoItemDeleted(todo7)

DualUserStep: 
                    new TodoItemCreated(todo8,"Handle the entrance","Disturbance",1 ,1)
                    new TodoItemCreated(todo9,"Fix sound system","Main stage",1 ,1)

SingleUserStep:     new ListNameChanged("Event has started.")


DualUserStep: 
                    new TodoItemCreated(todo10, "Go to the", "", 2, 2)
                    new TodoItemNameChanged(todo8, "Need a hand here")

DualUserStep: 
                    new TodoItemIndexChanged(todo5, 1)
                    new TodoItemIndexChanged(todo8, 2)

SingleUserStep:     new ListNameChanged("Event is done.")


        }
    }
}
