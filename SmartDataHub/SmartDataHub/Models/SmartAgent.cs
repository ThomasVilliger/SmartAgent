using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartDataHub.Models
{
    public class SmartAgent
    {
        [Required]
        public virtual int SmartAgentId { get; set; }
        [Required]
        public virtual string Name { get; set; }

        [RegularExpression(@"^(?:[0-9]{1,3}.){3}[0-9]{1,3}$")]
        [Required]
        public virtual string IpAddress { get; set; }
    }
}