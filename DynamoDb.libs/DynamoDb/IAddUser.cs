using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDb.libs.DynamoDb
{
    public interface IAddUser
    {
        Task AddNewEntry(string email, string subPlan, string lastPaid, string username, string password, string phoneNo, string lastAccessed);
    }
}
