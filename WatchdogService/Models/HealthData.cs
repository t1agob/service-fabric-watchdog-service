using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.WatchdogService.Models
{
    [JsonObject]
    [Serializable]
    public class HealthData
    {
        public string Cluster { get; set; }
        public string NodeName { get; set; }
        public string Application { get; set; }
        public string HealthState { get; set; }
    }
}
