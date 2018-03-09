using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class Machine
    {
        [Required] // is also used as the machineId!!!
        public virtual int MachineId { get; set; }

        [Display(Name = "SmartAgent")]
        public virtual int SmartAgentId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public virtual string MachineName { get; set; }
        //[Required]
        //public virtual int MachineId { get; set; }
        [Required]
        [Display(Name = "Input Pin")]
        public virtual int CycleInputPin { get; set; }

        [Required]
        [Display(Name = "State Timeout")]
        public virtual int MachineStateTimeOut { get; set; }

        [Required]
        [Display(Name = "Publish Intervall")]
        public virtual int PublishingIntervall { get; set; }
      
        public virtual SmartAgent SmartAgent { get; set; }
        [Required]
        public virtual bool Active { get; set; }
    }
}