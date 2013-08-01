using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Exceptions
{
    public class AggregateNotFoundExeption : Exception
    {
        public AggregateNotFoundExeption(Guid aggregateNotFound) 
            : base(String.Format("The aggregate for the event with id: {0} could not be found.", aggregateNotFound.ToString()))
        {
        }
    }
}
