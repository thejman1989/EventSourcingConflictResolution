using EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TodoApplication.Data;
using TodoApplication.Events;
using TodoApplication.Exceptions;

namespace TodoApplication.Aggregate
{
    public class ListState
    {
        private Persistence persistence;         
        public ListAggregate currentList;

        /// <summary>
        /// Gives a listState that is not hooked to persistence, this allows you to build state from a list of events, else the application will crash. 
        /// </summary>
        public ListState() : this(null) { }

        public ListState(Persistence persistence)
        {
            this.persistence = persistence;
            currentList = new ListAggregate();            
        }

        /// <summary>
        /// Load the event onto the state and persist it in the eventstore. 
        /// </summary>
        /// <param name="event">The event to load and persist. </param>
        public void LoadAndPersist(IEvent @event)
        {
            Load(@event);
            persistence.PersistEvent(@event);    
        }

        /// <summary>
        /// Load the event onto the state without persisting it. This is used for when the state is loaded from the eventstore. 
        /// </summary>
        /// <param name="event">The event to load. </param>
        public void Load(object @event)
        {
            if(@event != null)
                currentList.RaiseEvent(@event);
        }

        public void LoadList(List<IEvent> events)
        {
            foreach (IEvent @event in events)
            {
                this.Load(@event);
            }
        }


        public void LoadList(ICollection<EventMessage> events)
        {
            foreach (EventMessage message in events)
            {
                this.Load(message.Body);
            }
        }

        /// <summary>
        /// Loads all commits from the eventstore and creates the liststate. 
        /// </summary>
        public void loadFromPersistence()
        {
            ICollection<EventMessage> allEvents = this.persistence.getAllEvents();
            foreach (EventMessage eventMessage in allEvents)
            {
                //Only take the first event in the commit. This is because we only do one event per commit. 
                //This is done to simplify the programming as the specific multi commit stream is not neccesary. 
                Load(eventMessage.Body);
            }
        }

        private int oneByOneIterator = 0; 
        
        public void loadFromPersistenceOneByOne()
        {
            ICollection<EventMessage> oneByOneList  = this.persistence.getAllEvents().ToList();
            if (oneByOneIterator >= oneByOneList.Count())
            {
                MessageBox.Show("No more events to play");
            }
            else
            {
                Load(oneByOneList.ElementAt(oneByOneIterator).Body);
                oneByOneIterator++;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(ListState))
            {
                return false;
            }

            ListState toCheck = (ListState)obj;
            if (!toCheck.currentList.Equals(this.currentList))
            {
                return false; 
            }
            return true; 
        }


        public List<int> getTakenIndices()
        {
            return this.currentList.getTakenIndices();
        }
    }
}
