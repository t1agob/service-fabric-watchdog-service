using Microsoft.ServiceFabric.WatchdogService.Interfaces;
using Microsoft.ServiceFabric.WatchdogService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Fabric.Health;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.ServiceFabric.WatchdogService
{
    public class LogicAppErrorAlert : IWatchdogWebhook
    {        
        public string WebhookUrl { get; set; }
        public TimeSpan AlertTimeout { get; set; }
        public Dictionary<string, HealthDataAlertState> ClusterAlertStatus { get; set; }
        public Dictionary<string, HealthDataAlertState> NodeAlertStatus { get; set; }
        public Dictionary<string, HealthDataAlertState> ApplicationAlertStatus { get; set; }

        public LogicAppErrorAlert(string url, TimeSpan alertTimeout)
        {
            if (string.IsNullOrWhiteSpace(url) || alertTimeout == TimeSpan.Zero)
            {
                throw new ArgumentException("Argument is empty");
            }

            // Initialize State Dictionaries
            ClusterAlertStatus = new Dictionary<string, HealthDataAlertState>();
            NodeAlertStatus = new Dictionary<string, HealthDataAlertState>();
            ApplicationAlertStatus = new Dictionary<string, HealthDataAlertState>();

            // set webhook
            this.WebhookUrl = url;

            // set alert timeout
            this.AlertTimeout = alertTimeout;
        }

        public async Task ReportClusterError(string cluster, HealthState healthState, CancellationToken cancellationToken)
        {
            // check if alert is to be skipped
            var currentTimestamp = DateTime.UtcNow;
            if (ClusterAlertStatus.ContainsKey(cluster))
            {
                if(ClusterAlertStatus[cluster].State == healthState.ToString())
                {
                    return;
                }

                if(ClusterAlertStatus[cluster].LastAlertTimestamp + AlertTimeout < currentTimestamp)
                {
                    return;
                }
            }

            if(healthState == HealthState.Error || 
                healthState == HealthState.Warning)
            {
                var clusterHealth = new HealthData()
                {
                    Cluster = cluster,
                    HealthState = healthState.ToString(),
                    Application = "null", // logic apps cannot compare with null
                    NodeName = "null" // logic apps cannot compare with null
                };

                await PostMessage(clusterHealth);

                // Add to Cluster Dictionary
                var data = new HealthDataAlertState()
                {
                    State = healthState.ToString(),
                    LastAlertTimestamp = currentTimestamp
                };

                if (ClusterAlertStatus.ContainsKey(cluster))
                {
                    ClusterAlertStatus[cluster] = data;
                }
                else
                {
                    ClusterAlertStatus.Add(cluster, data);
                }
            }
        }

        public async Task ReportNodeError(string cluster, string node, HealthState healthState, CancellationToken cancellationToken)
        {
            // check if alert is to be skipped
            var currentTimestamp = DateTime.UtcNow;
            if (NodeAlertStatus.ContainsKey(node))
            {
                if (NodeAlertStatus[node].State == healthState.ToString())
                {
                    return;
                }

                if (NodeAlertStatus[node].LastAlertTimestamp + AlertTimeout < currentTimestamp)
                {
                    return;
                }
            }

            if (healthState == HealthState.Error ||
                healthState == HealthState.Warning)
            {
                var nodeHealth = new HealthData()
                {
                    Cluster = cluster,
                    HealthState = healthState.ToString(),
                    Application = "null", // logic apps cannot compare to null
                    NodeName = node
                };

                await PostMessage(nodeHealth);

                // Add to Node Dictionary
                var data = new HealthDataAlertState()
                {
                    State = healthState.ToString(),
                    LastAlertTimestamp = currentTimestamp
                };

                if (NodeAlertStatus.ContainsKey(node))
                {
                    NodeAlertStatus[node] = data;
                }
                else
                {
                    NodeAlertStatus.Add(node, data);
                }
            }
        }

        public async Task ReportApplicationError(string cluster, string node, string application, HealthState healthState, CancellationToken cancellationToken)
        {
            // check if alert is to be skipped
            var currentTimestamp = DateTime.UtcNow;
            if (ApplicationAlertStatus.ContainsKey(application))
            {
                if (ApplicationAlertStatus[application].State == healthState.ToString())
                {
                    return;
                }

                if (ApplicationAlertStatus[application].LastAlertTimestamp + AlertTimeout < currentTimestamp)
                {
                    return;
                }
            }

            if (healthState == HealthState.Error ||
                healthState == HealthState.Warning)
            {
                var applicationHealth = new HealthData()
                {
                    Cluster = cluster,
                    Application = application,
                    HealthState = healthState.ToString(),
                    NodeName = node
                };

                await PostMessage(applicationHealth);

                // Add to Application Dictionary
                var data = new HealthDataAlertState()
                {
                    State = healthState.ToString(),
                    LastAlertTimestamp = currentTimestamp
                };

                if (ApplicationAlertStatus.ContainsKey(application))
                {
                    ApplicationAlertStatus[application] = data;
                }
                else
                {
                    ApplicationAlertStatus.Add(application, data);
                }
            }            
        }

        public async Task PostMessage(HealthData healthData)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(healthData);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(WebhookUrl, stringContent);

                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("An error occurred while trying to reach the webhook.");
                }
            }
        }
    }
}
