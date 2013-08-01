using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication
{
    public class ListCreated : IEvent
    {
        public Guid aggregateId { get; set; }
        public string name { get; private set; } 
        
        public ListCreated(Guid id, string name)
        {
            this.aggregateId = id;
            this.name = name; 
        }

        /// <summary>
        /// Returns literal Equals over all attributes. 
        /// </summary>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(ListCreated))
            {               
                ListCreated toCompareEvent = (ListCreated)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.name == this.name)
                    {
                        return true;
                    }
                }
            }
            return false; 
        }
    }
}
