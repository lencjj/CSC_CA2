using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Talents_Webapp.Models
{
    public class Talent
    {
        public int TalentId { get; set; }
        public string TalentName { get; set; }
        public string TalentTitle { get; set; }
        public string TalentDesc { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
