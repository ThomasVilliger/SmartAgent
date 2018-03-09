using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartDataHub.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace SmartDataHub
{
    // this class handels the access to the database
    public class DataAccess
    {
        private static readonly HttpClient _client = new HttpClient();
        private static SmartDataHubStorageContext _dbContext;

        public static void Initialize(DbContextOptions<SmartDataHubStorageContext> dbContextOptions)
        {
            _dbContext = new SmartDataHubStorageContext(dbContextOptions);
        }

        internal static List<MachineStateHistory> GetMachineStateHistoryData(int machineId, DateTime fromDate, DateTime toDate)
        {
            return _dbContext.MachineStateHistory.Where(h => h.MachineId == machineId && (h.StartDateTime >= fromDate && h.EndDateTime <= toDate)).ToList();
        }

        public static List<Machine> GetMachines(int smartAgentId)
        {
            return _dbContext.Machine.Where(m => m.SmartAgentId == smartAgentId && m.Active == true).ToList();
        }


        public static List<InputMonitoring> GetInputMonitorings(int smartAgentId)
        {
            return _dbContext.InputMonitoring.Where(m => m.SmartAgentId == smartAgentId).ToList();
        }

        // gets all the not imported machine state history data from the SmartAgent and stores it on the database
        public static async Task GetNewHistoryDataFromSmartAgent(int smartAgentId)
        {
            var smartAgent = _dbContext.SmartAgent.FirstOrDefault(s => s.SmartAgentId == smartAgentId);
            string ip = smartAgent.IpAddress;
            string url = String.Format(@"http://{0}:8800/api/getMachineStateHistoryData/{1}", ip, smartAgent.LastSmartAgentHistoryId);

            var response = await _client.GetAsync(url);
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

                _dbContext.MachineStateHistory.AddRange(historyData);
                smartAgent.LastSmartAgentHistoryId = historyData.Last().SmartAgentHistoryId;
                _dbContext.SaveChanges();
            }
        }

        public static List<SmartAgent> GetSmartAgents()
        {
            return _dbContext.SmartAgent.OrderBy(s => s.Priority).ToList();
        }
    }
}
