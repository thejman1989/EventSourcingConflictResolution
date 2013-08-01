using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class ListNameChanged : IEvent
    {
        public Guid aggregateId { get; set; }
        public string newName { get; private set; }

        public ListNameChanged(string newName)
        {
            this.newName = newName;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(ListNameChanged))
            {
                ListNameChanged toCompareEvent = (ListNameChanged)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                    if (toCompareEvent.newName == this.newName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
