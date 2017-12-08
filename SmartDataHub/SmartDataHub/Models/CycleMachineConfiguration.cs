using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class CycleMachineConfiguration
    {
        [Required]
        public virtual int CycleMachineConfigurationId { get; set; }

        public virtual int SmartAgentId { get; set; }
        [Required]
        public virtual string MachineName { get; set; }
        [Required]
        public virtual int MachineId { get; set; }
        [Required]
        public virtual int CycleInputPin { get; set; }
        [Required]
        public virtual int MachineStateTimeOut { get; set; }
        [Required]
        public virtual int PublishingIntervall { get; set; }
      
        public virtual SmartAgent SmartAgent { get; set; }
        [Required]
        public virtual bool Active { get; set; }
    }
}