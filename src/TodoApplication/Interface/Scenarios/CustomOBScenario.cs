using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Scenarios;
using TodoApplication.Events;

namespace TodoApplication.Interface.Scenarios
{
    class CustomOBScenario : Scenario
    {
        public CustomOBScenario()
            : base()
        {
            this.addTestEvents();
        }

        private void addTestEvents()
        {
            Guid listGuid = Guid.NewGuid();
            Guid firstTodo = Guid.NewGuid();
            Guid secondTodo = Guid.NewGuid();
            Guid thirdTodo = Guid.NewGuid();
            Guid fourthTodo = Guid.NewGuid();
            base.addOnlineReplayStep(new OnlineReplayStep(new ListCreated(listGuid, "Groceries")));

            base.addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(firstTodo, "apples", "Buy a big sack of apples", 1,1)));
            base.addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(secondTodo, "ice", "strawberry ice cream. ", 2,2)));
            base.addOnlineReplayStep(new OnlineReplayStep(new TodoItemCreated(thirdTodo, "potatoes", "slices", 3,3)));

            base.addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("All the Groceries")));

            //This is where a user goes offline. So at version 5. 
            //base.addOfflineReplayStep(new OfflineReplayStep(
            //                                        new TodoItemNameChanged(firstTodo, "Big apples"), 
            //                                        new TodoItemNameChanged(firstTodo, "Apples")));

            base.addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(firstTodo, "Big Applesack"),
                                                    new TodoItemNameChanged(firstTodo, "Apples")));
            
            base.addOfflineReplayStep(new OfflineReplayStep(
                                                     new TodoItemNameChanged(firstTodo, "Small Applesack"),
                                                    new TodoItemCreated(fourthTodo, "Cleaning", "Soap soap soap", 4,4)));
       
            
            base.addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemDeleted(firstTodo),
                                                    new ListNameChanged("All the important Groceries")));

            base.addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Conflicts created")));
        }    
    }
}
