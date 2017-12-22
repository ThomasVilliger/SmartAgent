using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartDataHub.Models;
using SmartDataHub;

namespace SmartDataHub.Models
{
    public class SmartDataHubStorageContext : DbContext
    {
        public SmartDataHubStorageContext (DbContextOptions<SmartDataHubStorageContext> options)
            : base(options)
        {
            SmartDataSignalRclient._cycleMachineConfigurationContext = CycleMachineConfiguration;
        }

        public DbSet<SmartDataHub.Models.CycleMachineConfiguration> CycleMachineConfiguration { get; set; }

        public DbSet<SmartDataHub.Models.SmartAgent> SmartAgent { get; set; }

        public DbSet<SmartDataHub.Models.InputMonitoringConfiguration> InputMonitoringConfiguration { get; set; }
    }
}
