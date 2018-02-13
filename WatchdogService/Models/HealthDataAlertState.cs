using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.WatchdogService.Models
{
    public class HealthDataAlertState
    {
        public string State { get; set; }
        public DateTime LastAlertTimestamp { get; set; } 
    }
}
