using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using TodoApplication.Events;
using TodoApplication.Aggregate;

namespace TodoApplication.Interface
{
    /// <summary>
    /// Class to generate random events. Events are generated according to certain rules so that no faulty sets can be created.    
    /// </summary>
    public class Generator
    {
        private readonly int nameLength = 1; 
        private readonly int descriptionLenght = 3;        
        private readonly Random differentSeedRandom = new Random();
        private readonly int maxsimultaneousDelete = 3;

        private int nextPriority = 0;
        private List<String> randomStringList; 
        //private Persistence persistence = new Persistence();
        //private List<Guid> availableTodoItems = new List<Guid>();
        private bool offline = false; 

        private List<IEvent> generatedEvents;

        private List<IEvent> allPreviousEvents;

        public Generator()
        {
            buildRandomStringList();
        }

        /// <summary>
        /// If the state is given, then an offline generator is created. 
        /// </summary>
        /// <param name="currentState"> The state of the list.</param>
        public Generator(List<IEvent> allPreviousEvents)
        {
            buildRandomStringList();
            this.offline = true;
            this.allPreviousEvents = allPreviousEvents;
        }

        /// <summary>
        /// Generates a list of random events. 
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public List<IEvent> generateRandomEvents(int amount)
        {
            generatedEvents = new List<IEvent>();
            // If the event generator is offline then there is no need to init the list. 
            if (amount > 0 && !offline)
            {
                generatedEvents.Add(new ListCreated(Guid.NewGuid(), "Random List"));
            }
            while(generatedEvents.Count < amount)
            {
                generatedEvents.AddRange(getRandomEvent());
            }

            return generatedEvents;
        }

        /// <summary>
        /// Creates a random event. Needs to be updated every time new events get created!
        /// </summary>
        /// <returns>An event. </returns>
        private List<IEvent> getRandomEvent()
        {
            //returns less then specified number. 
            int whichEvent = differentSeedRandom.Next(10);
            Debug.WriteLine("Case: " + whichEvent); 
            switch (whichEvent)
            {
                    // List name changed
                case 0:
                    ListNameChanged listNameChanged = new ListNameChanged(getRandomString(nameLength));
                    return toList(listNameChanged); 
                    // Todo item created
                case 1:
                    return toList(getRandomTodoItemCreated());
                    // Todo item description changed. 
                case 2:
                    // Can only add events to items that exist. 
                    if(getAvailableTodoItems().Count == 0)
                    {
                        break;
                    }
                    TodoItemDescriptionChanged todoItemDescriptionChanged = new TodoItemDescriptionChanged(getRandomTodoItemGuid(), getRandomString(descriptionLenght));
                    return toList(todoItemDescriptionChanged);
                    // Todo item name changed. 
                case 3:
                    // Can only add events to items that exist. 
                    if (getAvailableTodoItems().Count == 0)
                    {
                        break;
                    }
                    TodoItemNameChanged todoItemNameChanged = new TodoItemNameChanged(getRandomTodoItemGuid(), getRandomString(nameLength));
                    return toList(todoItemNameChanged);
                case 4:
                    // Need at least two items to change Priority. It's more of a swap priority. 
                    if (getAvailableTodoItems().Count < 2)
                    {
                        break;
                    }
                    return getRandomTodoItemPriorityChanged(); 
                    // Todo item deleted. 
                case 5:
                    if (getAvailableTodoItems().Count <= 1)
                    {
                        break;
                    }
                    return toList(getRandomTodoItemDeleted());
                    // Todo item Priority increased. 
                case 6:
                    if (getAvailableTodoItems().Count == 0)
                    {
                        break;
                    }
                    return toList(new TodoItemPriorityIncreased(getRandomTodoItemGuid()));
                    // Todo item Priority decreased. 
                case 7:
                    if (getAvailableTodoItems().Count == 0)
                    {
                        break;
                    }
                    TodoItemAggregate toDecrease = getAvailableTodoItems().ElementAt(
                                        differentSeedRandom.Next(getAvailableTodoItems().Count));

                    if (toDecrease.priority < 2)
                        break;
                    return toList(new TodoItemPriorityDecreased(toDecrease.id));
                    // Multiple delete of todo items. 
                case 8:
                    if (getAvailableTodoItems().Count <=  3)
                        break; 
                    // Build a list of items to remove. Random amount. 
                    List<Guid> removedItems = new List<Guid>();
                    for (int i = 0; i < differentSeedRandom.Next(2, maxsimultaneousDelete + 1); i++)
                    {
                        removedItems.Add(getAvailableTodoItems().ElementAt(
                                        differentSeedRandom.Next(getAvailableTodoItems().Count)).id);
                    }
                    // Make sure there are no duplicates.
                    TodoItemsDeleted deletedItems = new TodoItemsDeleted(removedItems.Distinct().ToList());
                    return toList(deletedItems);
                case 9:
                    if (getAvailableTodoItems().Count == 0)
                        break;
                    return toList(getRandomTodoItemIndexChanged());


            }
            return getRandomEvent();
        }

        private IEvent getRandomTodoItemIndexChanged()
        {
            
            Guid itemToChange = getAvailableTodoItems().ElementAt(differentSeedRandom.Next(getAvailableTodoItems().Count)).id;
            

            TodoItemIndexChanged newEvent = new TodoItemIndexChanged(itemToChange, getFirstAvailableIndex());
            return newEvent;
        }

        private int getFirstAvailableIndex()
        {
            List<int> takenIndices = getTakenIndices();
            if (takenIndices.Count == 0)
                return 1;
            int availableIndex = -1;

            for (int i = 0; i < takenIndices.Count; i++)
            {
                //If the index is not ascending so 1-2-3-4 then get the missing one to be the new index.
                if ((i + 1) != takenIndices.ElementAt(i))
                {
                    availableIndex = i + 1;
                }
            }

            if (availableIndex == -1)
                availableIndex = takenIndices.Max() + 1;

            return availableIndex;
        }

        /// <summary>
        /// Gets a random delete event. 
        /// </summary>
        /// <returns>Delete event.</returns>
        private IEvent getRandomTodoItemDeleted()
        {            
            // First load all the made events to a state so their priority can be read. 
            return new TodoItemDeleted(getAvailableTodoItems().ElementAt(
                                        differentSeedRandom.Next(getAvailableTodoItems().Count)).id);

        }

        /// <summary>
        /// Priorit----y should be ascending so this returns the next available priority. 
        /// </summary>
        /// <returns>The next priority. </returns>
        private int getNextPriority()
        {
            nextPriority++;
            return nextPriority;
        }

        /// <summary>
        /// Creates a list of events that changes the priority. 
        /// There are two becuase the priority is changed against another event. 
        /// </summary>
        /// <param name="createdList">The list of current created items. </param>
        /// <returns>A List of two events that change each others priority. </returns>
        private List<IEvent> getRandomTodoItemPriorityChanged()
        {
            List<IEvent> returnList = new List<IEvent>();
           
            List<TodoItemAggregate> usedPrioritys = getAvailableTodoItems();        

            // Get the two items that need to be changed with each other. 
            int leftChangee = differentSeedRandom.Next(usedPrioritys.Count);
            TodoItemAggregate leftSide = usedPrioritys.ElementAt(leftChangee);
            // Remove this from list so it does not change with itself. 
            usedPrioritys.RemoveAt(leftChangee);

            int rightChangee = differentSeedRandom.Next(usedPrioritys.Count);
            TodoItemAggregate rightSide = usedPrioritys.ElementAt(rightChangee);
            
            // Added the priority change for the one that recieves the change. 
            TodoItemPriorityChanged todoItemPriorityChanged = new TodoItemPriorityChanged(leftSide.id, rightSide.priority);
            returnList.Add(todoItemPriorityChanged);

            // Need to add the change for its counterpart. 
            TodoItemPriorityChanged counterTodoItemPriorityChanged = new TodoItemPriorityChanged(rightSide.id, leftSide.priority);
            returnList.Add(counterTodoItemPriorityChanged);

            return returnList; 
        }

        private TodoItemCreated getRandomTodoItemCreated()
        {
            Guid newGuid = Guid.NewGuid();
            TodoItemCreated todoItemCreated = new TodoItemCreated(newGuid, getRandomString(nameLength), getRandomString(descriptionLenght), getNextPriority(), getFirstAvailableIndex());
            return todoItemCreated;
        }

        private Guid getRandomTodoItemGuid()
        {
            List<TodoItemAggregate> availableTodoItems = getAvailableTodoItems();
            return availableTodoItems.ElementAt(differentSeedRandom.Next(availableTodoItems.Count)).id;
        }

        /// <summary>
        /// Creates random string of length 
        /// </summary>
        /// <param name="length">The lenght of the string. </param>
        private string getRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(randomStringList.ElementAt(differentSeedRandom.Next(randomStringList.Count)));
                builder.Append(" ");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Builds a random string list from a file.
        /// </summary>
        private void buildRandomStringList()
        {
            randomStringList = new List<String>();
            StreamReader streamReader = new StreamReader("../../Resources/woorden2.txt");
            while (!streamReader.EndOfStream)
            {
                randomStringList.Add(streamReader.ReadLine());
            }            
            streamReader.Close();
        }

        /// <summary>
        /// TUrns an event into a list. 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        private List<IEvent> toList(IEvent @event)
        {
            List<IEvent> returnList = new List<IEvent>();
            returnList.Add(@event);
            return returnList;
        }

        /// <summary>
        /// Calculates the state off of the currently generated events. 
        /// </summary>
        /// <returns>Current state.</returns>
        private ListState calculateState()
        {
            if (offline)
                return calculateOfflineListState();
            else
                return calculateOnlineListState();

        }

        private ListState calculateOfflineListState()
        {
            ListState newState = new ListState();
            
            newState.LoadList(allPreviousEvents);
            newState.LoadList(generatedEvents);
            return newState;
        }

        private ListState calculateOnlineListState()
        {
            ListState currState = new ListState();
            foreach (IEvent @event in generatedEvents)
            {
                currState.Load(@event);
            }
            return currState;
        }


        private List<int> getTakenIndices()
        {
            return calculateState().getTakenIndices();
        }

        /// <summary>
        /// Gets all currently available todoItems by calculating the current state of all generated events. 
        /// </summary>
        /// <returns>List of todoItems. </returns>
        private List<TodoItemAggregate> getAvailableTodoItems()
        {
            ListState currState = calculateState();
            return currState.currentList.todoItems;
        }


    }
}
