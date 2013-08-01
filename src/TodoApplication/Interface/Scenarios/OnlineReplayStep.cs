using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Interface.Scenarios
{
    [Serializable()]
    public class OnlineReplayStep : ReplayStep
    {
        public IEvent onlineEvent {get; set;}

        public OnlineReplayStep(IEvent onlineEvent)
        {
            this.onlineEvent = onlineEvent;
        }

    }
}
