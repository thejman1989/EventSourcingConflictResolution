using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApplication.Events;

namespace TodoApplication.Interface.Scenarios
{
    [Serializable()]
    public class OfflineReplayStep : ReplayStep
    {
        public IEvent onlineEvent { get; set; }
        public IEvent offlineEvent { get; set; }

        public OfflineReplayStep(IEvent onlineEvent, IEvent offlineEvent)
        {
            this.onlineEvent = onlineEvent;
            this.offlineEvent = offlineEvent;
        }
    }
}
