using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Prototypes
{
    public class Conflict
    {
        public List<IEvent> conflictingEvents { get; private set; }

        public Conflict(List<IEvent> conflictingEvents)
        {
            this.conflictingEvents = conflictingEvents;
        }

        override
        public string ToString()
        {
            StringBuilder returnString = new StringBuilder();
            foreach (IEvent @event in conflictingEvents)
            {
                returnString.Append("Conflicting event: ");
                returnString.Append(@event.GetType());
                returnString.Append(Environment.NewLine);
            }
            return returnString.ToString(); 
        }
        
    }
}
