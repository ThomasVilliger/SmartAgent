using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SmartDataHub.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Timers;
using Microsoft.EntityFrameworkCore;

namespace SmartDataHub
{
    public static class DataAccess
    { 
        private static readonly HttpClient client = new HttpClient();
        public static DbContextOptions<SmartDataHubStorageContext> DbContextOptions;

        public static List<Machine> GetMachines(int smartAgentId)
        {
            SmartDataHubStorageContext dbContext = new SmartDataHubStorageContext(DbContextOptions);
            return dbContext.Machine.Where(m => m.SmartAgentId == smartAgentId  && m.Active == true).ToList();
        }


        public static List<InputMonitoring> GetInputMonitorings(int smartAgentId)
        {
            SmartDataHubStorageContext dbContext = new SmartDataHubStorageContext(DbContextOptions);
            return dbContext.InputMonitoring.Where(m => m.SmartAgentId == smartAgentId).ToList();
        }


        public static async Task GetNewHistoryDataFromSmartAgent (int smartAgentId)
        {       
           SmartDataHubStorageContext dbContext = new SmartDataHubStorageContext(DbContextOptions);

           var smartAgent  = dbContext.SmartAgent.FirstOrDefault(s => s.SmartAgentId == smartAgentId);
           string ip = smartAgent.IpAddress;
           string url = String.Format(@"http://{0}:8800/api/getMachineStateHistoryData/{1}", ip, smartAgent.LastSmartAgentHistoryId);

            var response = await client.GetAsync(url);
            var responseMessage = await response.Content.ReadAsStringAsync(); 
            var success = response.IsSuccessStatusCode;

            if (success)
            {
                var historyData = new List<MachineStateHistory>();  
                historyData = JsonConvert.DeserializeObject<List<MachineStateHistory>>(responseMessage);
                historyData.OrderBy(h => h.SmartAgentHistoryId);

                foreach (MachineStateHistory m in historyData)
                {
                    m.Duration = m.EndDateTime - m.StartDateTime;
                }

                dbContext.MachineStateHistory.AddRange(historyData);       
                smartAgent.LastSmartAgentHistoryId = historyData.Last().SmartAgentHistoryId;
                dbContext.SaveChanges();
            }
        }
    }
}
