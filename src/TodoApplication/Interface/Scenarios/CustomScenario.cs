using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;
using TodoApplication.Interface.Scenarios;

namespace TodoApplication.Scenarios
{
    public class CustomScenario : Scenario
    {
        public CustomScenario() : base()
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
            base.addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemNameChanged(firstTodo, "Big apples"), 
                                                    new TodoItemNameChanged(firstTodo, "Apples")));
            
            base.addOfflineReplayStep(new OfflineReplayStep(
                                                    new TodoItemIndexChanged(firstTodo,4),
                                                    new TodoItemIndexChanged(secondTodo,4)));
                                                    //new TodoItemCreated(fourthTodo, "Cleaning", "Soap soap soap", 4,4)));
       
            
            base.addOfflineReplayStep(new OfflineReplayStep(
                                                    null,//new TodoItemCreated(fourthTodo, "Cleaning", "Soap soap soap", 4, 4),
                                                    null));

            base.addOnlineReplayStep(new OnlineReplayStep(new ListNameChanged("Conflicts created")));
        }
    }
}
