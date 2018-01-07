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
            //DataAccess.DbContextOptions = options;
        }

        public DbSet<SmartDataHub.Models.Machine> Machine { get; set; }

        public DbSet<SmartDataHub.Models.SmartAgent> SmartAgent { get; set; }

        public DbSet<SmartDataHub.Models.InputMonitoring> InputMonitoring { get; set; }

        public DbSet<SmartDataHub.Models.MachineStateHistory> MachineStateHistory { get; set; }
    }
}
