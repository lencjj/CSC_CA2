using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Talents_Webapp.Models
{
    public class ChargeDTO
    {
        public string CardName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Plan { get; set; }
        public string StripeToken { get; set; }
    }
}
