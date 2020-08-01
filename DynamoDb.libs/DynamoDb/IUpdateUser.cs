using DynamoDb.libs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDb.libs.DynamoDb
{
    public interface IUpdateUser
    {
        Task<User> UpdateUserLastAccessed(string email, string newAccessed);
    }
}
