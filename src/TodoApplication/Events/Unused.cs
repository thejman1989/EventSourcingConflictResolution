using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    class Unused : IEvent
    {
        public Guid aggregateId { get; set; }

        public Unused()
        {
        
        }
        
        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType().Equals(typeof(Unused)))
                return true;            
            else
                return false; 
        }
    }
}
