using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class InputMonitoring
    {
        [Required]
        public virtual int InputMonitoringId { get; set; }
        [Required]
        [Display(Name = "SmartAgent")]
        public virtual int SmartAgentId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public virtual string MonitoringName { get; set; }

        [Required]
        [Display(Name = "Input Pin")]
        public virtual int InputPin { get; set; }

        [Required]
        [Display(Name = "Output Pin")]
        public virtual int OutputPin { get; set; }
       
        public virtual SmartAgent SmartAgent { get; set; }
        [Required]
        public virtual bool Active { get; set; }
    }
}