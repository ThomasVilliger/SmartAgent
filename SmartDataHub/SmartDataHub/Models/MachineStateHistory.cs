using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDataHub.Models
{
    public class MachineStateHistory
    {
        
        public enum State { Stopped, Running }

        [Required]
        public int Id { get; set; }

        [Required]
        public int SmartAgentHistoryId { get; set; }

        [Required]
        [Display(Name = "Machine Id")]
        public int MachineId { get; set; }
        public virtual Machine Machine { get; set; }

        [Required]
        public State MachineState { get; set; }
        [Required]
        [Display(Name = "Start")]
        public DateTime StartDateTime { get; set; }
        [Required]
        [Display(Name = "End")]
        public DateTime EndDateTime { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        [Display(Name = "Daily Cycle Counter")]
        public int DailyCycleCounter { get; set; }
        [Required]
        [Display(Name = "State Cycle Counter")]
        public int CyclesInThisPeriod { get; set; }



    }
}
