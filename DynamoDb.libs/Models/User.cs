using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDb.libs.Models
{
    public class User
    {
        public string Email { get; set; }
        public string SubscriptionPlan { get; set; }
        public string LastPaid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public string LastAccessed { get; set; }
    }
}
