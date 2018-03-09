using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDataHub
{
    // signalR Hub for the report site
    public class ReportHub : Hub
    {
        public async void ExecuteReport(int machineId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                fromDate = fromDate + TimeSpan.FromHours(1);
                toDate = toDate + TimeSpan.FromHours(1);
                await ReportResponse(machineId, fromDate, toDate);
            }

            catch (Exception ex)
            {
                await Clients.Client(Context.ConnectionId).InvokeAsync("ReportHeaderResponse", false, ex.Message);
            }
        }

        private async Task ReportResponse(int machineId, DateTime fromDate, DateTime toDate)
        {
            var historyData = DataAccess.GetMachineStateHistoryData(machineId, fromDate, toDate);

            if (historyData.Any())
            {
                string header = String.Format("report execution time: {0} ", DateTime.Now.ToString());

                await Clients.All.InvokeAsync("ReportHeaderResponse", true, header);
                await Clients.All.InvokeAsync("ReportDataResponse", historyData);
            }

            else
            {
                string header = String.Format("no data for machine {0} {1}", machineId, DateTime.Now.ToString());
                await Clients.Client(Context.ConnectionId).InvokeAsync("ReportHeaderResponse", true, header);
            }
        }
    }
}


