using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class HcScen_1 : Scenario
    {
        public HcScen_1()
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

            addOnlineReplayStep(new OnlineReplayStep(new ListCreated(listGuid, "Groceries")));
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem1, "apples", "Buy a big sack of apples", 1, 1)));
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem2, "ice", "strawberry ice cream. ", 2, 2)));
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem3, "potatoes", "slices", 3, 3)));
            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("All the Groceries")));

            //This is where a user goes offline. So at version 5. 
            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem1, "Big apples"), 
                                                    new TodoItemNameChanged(todoItem1, "Apples")));
            
            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem1, 4),
                                                    new TodoItemIndexChanged(todoItem2, 4)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("All the groceries")));
                   
            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemCreated(todoItem4, "Garbage bags", "Those big red ones", 1, 5),
                                                    new TodoItemCreated(todoItem5, "Red Garbage bags", "", 1, 5)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    null,
                                                    new TodoItemPriorityIncreased(todoItem4)));


            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem5),
                                                    new TodoItemPriorityIncreased(todoItem5)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem4, "Red Garbage bags"),
                                                    new TodoItemDescriptionChanged(todoItem1,"Elstar apples 10x")));

            List<Guid> toDelete = new List<Guid>();
            toDelete.Add(todoItem1);
            toDelete.Add(todoItem2);
            toDelete.Add(todoItem3);
            toDelete.Add(todoItem4);
            toDelete.Add(todoItem5);

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemsDeleted(toDelete)));


            addOfflineReplayStep(new OfflineReplayStep(
                                                    new ListNameChanged("Pharmacyitems"),
                                                    new ListNameChanged("Pharmaceuticals")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemCreated(todoItem6, "Pills", "Black and Yellow", 1, 1),
                                                    new TodoItemCreated(todoItem7, "Anti Nausia", "Syndicate", 1, 2)));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemIndexChanged(todoItem7,3)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem7,2),
                                                    new TodoItemIndexChanged(todoItem6,2)));


            addOfflineReplayStep(new OfflineReplayStep(
                                                    null,
                                                    new TodoItemIndexChanged(todoItem7, 1)));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem8, "Anti Diarrhea", "Standard brand", 1, 3)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem8, "Diahrea medicine"),
                                                    new TodoItemNameChanged(todoItem8, "BIG BOX anti diahrea")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem8),
                                                    new TodoItemDescriptionChanged(todoItem8, "The cheap stuff")));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemDeleted(todoItem6)));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem9,"Coughing sirup","",2,2)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDescriptionChanged(todoItem9, "Buy a lot"),
                                                    new TodoItemDescriptionChanged(todoItem9, "The cheap stuff")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem9, 2),
                                                    new TodoItemIndexChanged(todoItem9, 3)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityDecreased(todoItem9),
                                                    new TodoItemPriorityIncreased(todoItem9)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem9),
                                                    new TodoItemPriorityIncreased(todoItem9)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Done")));


        }
    }
}
