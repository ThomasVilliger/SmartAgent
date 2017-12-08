using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class InputMonitoringConfiguration
    {
        [Required]
        public virtual int InputMonitoringConfigurationId { get; set; }
        [Required]
        public virtual int SmartAgentId { get; set; }
        [Required]
        public virtual string MonitoringName { get; set; }
        [Required]
        public virtual int InputPin { get; set; }
        [Required]
        public virtual int OutputPin { get; set; }
       
        public virtual SmartAgent SmartAgent { get; set; }
        [Required]
        public virtual bool Active { get; set; }
    }
}