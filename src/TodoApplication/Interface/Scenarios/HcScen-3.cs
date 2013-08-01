using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Scenarios;

namespace TodoApplication.Interface.Scenarios
{
    public class HcScen_3 : Scenario
    {
        public HcScen_3()
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
            
            
            addOnlineReplayStep(new OnlineReplayStep(new ListCreated(listGuid, "TestList")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemCreated(todoItem1, "One", "One", 1, 1),
                                                    new TodoItemCreated(todoItem2, "Two", "Two", 2, 2)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("TestList2")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemCreated(todoItem3, "Three", "Three", 3, 3),
                                                    new TodoItemCreated(todoItem4, "Four", "Four", 4, 4)));
            
            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem1,"One One"),
                                                    new TodoItemNameChanged(todoItem1,"One Two")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem2, 1),
                                                    new TodoItemIndexChanged(todoItem2, 1)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityChanged(todoItem2, 3),
                                                    new TodoItemPriorityChanged(todoItem2, 4)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityIncreased(todoItem2),
                                                    new TodoItemPriorityIncreased(todoItem2)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityDecreased(todoItem2),
                                                    new TodoItemPriorityDecreased(todoItem2)));

            addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("TestList3")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem1),
                                                    new TodoItemDeleted(todoItem1)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem2,5),
                                                    new TodoItemIndexChanged(todoItem2,5)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(todoItem3, 6),
                                                    new TodoItemIndexChanged(todoItem3, 7)));                                                  

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem4, "NewFour"),
                                                    new TodoItemNameChanged(todoItem4, "OldFour")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem4),
                                                    new TodoItemDescriptionChanged(todoItem4, "NewDescription")));

            List<Guid> toDelete = new List<Guid>();
            toDelete.Add(todoItem1);

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(todoItem1),
                                                    new TodoItemsDeleted(toDelete)));

            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem8, "Eight", "Eight", 1, 1)));
            addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(todoItem9, "Nine", "Nine", 2, 2)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(todoItem8, "One"),
                                                    new TodoItemDescriptionChanged(todoItem8, "First")));  

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDescriptionChanged(todoItem8, "One"),
                                                    new TodoItemNameChanged(todoItem8, "First")));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityChanged(todoItem9, 3),
                                                    new TodoItemPriorityIncreased(todoItem9)));

            addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemPriorityDecreased(todoItem9),
                                                    new TodoItemPriorityIncreased(todoItem9)));

        }
    }
}
