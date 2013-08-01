using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class HcScen_2 : Scenario
    {
        public HcScen_2()
            : base()
        {
            this.addTestEvents();
        }

        private void addTestEvents()
        {
            Guid listGuid = Guid.NewGuid();
            Guid todoItem1 = Guid.NewGuid();
            Guid todoItem2 = Guid.NewGuid();
            Guid todoItem3 = Guid.NewGuid();
            Guid todoItem4 = Guid.NewGuid();
            Guid todoItem5 = Guid.NewGuid();
            Guid todoItem6 = Guid.NewGuid();
            Guid todoItem7 = Guid.NewGuid();
            Guid todoItem8 = Guid.NewGuid();
            Guid todoItem9 = Guid.NewGuid();
            Guid todoItem10 = Guid.NewGuid();
           
            
            addOnlineReplayStep(new OnlineReplayStep(new ListCreated(listGuid, "Beach party")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemCreated(todoItem1,"Get the tent set up","Main tent",1,1),
                                                    new TodoItemCreated(todoItem2,"Get the music ready","Hardrock", 1, 1)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Beach event")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemCreated(todoItem3, "Get drinks ready", "Beer and sprite", 1, 2),
                                                   new TodoItemDeleted(todoItem2)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem1, 3),
                                                   new TodoItemCreated(todoItem4, "Fill the pool","The big one first.",2,3)));
            
            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem3,1),
                                                   new TodoItemNameChanged(todoItem4,"Fill the BIG pool")));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemDeleted(todoItem1)));


            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemCreated(todoItem5,"Buy some bandaids","only if there is time left",1,5),
                                                   new TodoItemCreated(todoItem6, "Mow the lawn", "", 2, 6)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                  new TodoItemNameChanged(todoItem3,"DRINKS!"),
                                                  new TodoItemNameChanged(todoItem3, "SOMEONE do the drinks")));
           
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemDescriptionChanged(todoItem6,"What lawn???")));

            
            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemDeleted(todoItem6),
                                                   new TodoItemDeleted(todoItem6)));
            
            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem3,1),
                                                   new TodoItemIndexChanged(todoItem3,2)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Beach event - One hour left")));


            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem4,1),
                                                   new TodoItemIndexChanged(todoItem5,1)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem5,3),
                                                   new TodoItemIndexChanged(todoItem4,1)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemDeleted(todoItem3),
                                                   new TodoItemDeleted(todoItem3)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Get ready, about to start.")));


            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemDescriptionChanged(todoItem5, "Important!"),
                                                   new TodoItemCreated(todoItem7,"clean the room","",3,3)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem5,1),
                                                   new TodoItemDescriptionChanged(todoItem7,"the vip room")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   null,
                                                   new TodoItemIndexChanged(todoItem7,1)));


            addOnlineReplayStep(new OnlineReplayStep(new TodoItemDeleted(todoItem5)));
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemDeleted(todoItem7)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemCreated(todoItem8,"Handle the entrance","Disturbance",1 ,1),
                                                   new TodoItemCreated(todoItem9,"Fix sound system","Main stage",1 ,1)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Event has started.")));


            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemCreated(todoItem10, "Go to the", "", 2, 2),
                                                   new TodoItemNameChanged(todoItem8, "Need a hand here")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                   new TodoItemIndexChanged(todoItem5, 1),
                                                   new TodoItemIndexChanged(todoItem8, 2)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Event is done.")));


        }
    }
}
