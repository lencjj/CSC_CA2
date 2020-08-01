using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DynamoDb.libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDb.libs.DynamoDb
{
    public class UpdateUser : IUpdateUser
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IGetUser _getUser;
        private static readonly string tableName = "User";

        public UpdateUser(IAmazonDynamoDB dynamoDbClient, IGetUser getUser)
        {
            _dynamoDbClient = dynamoDbClient;
            _getUser = getUser;
        }

        public async Task<User> UpdateUserLastAccessed(string email, string newAccessed)
        {
            var response = await _getUser.GetUsers(email);

            var previousAccessed = response.Users.Select(p => p.LastAccessed).FirstOrDefault();

            var request = RequestBuilder(email, previousAccessed, newAccessed);

            var result = await UpdateItemAsync(request);

            return new User
            {
                Email = result.Attributes["Email"].S,
                SubscriptionPlan = result.Attributes["SubscriptionPlan"].S,
                LastPaid = result.Attributes["LastPaid"].S,
                UserName = result.Attributes["UserName"].S,
                Password = result.Attributes["Password"].S,
                PhoneNo = result.Attributes["PhoneNo"].S,
                LastAccessed = result.Attributes["LastAccessed"].S
            };

        }
        
        private UpdateItemRequest RequestBuilder(string email, string previousAccessed, string newAccessed)
        {
            var request = new UpdateItemRequest
            {
                Key = new Dictionary<string, AttributeValue>()
                {
                    {"Email", new AttributeValue{ S = email}}
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#LastAccessed", "LastAccessed"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {
                        ":newAccessed", new AttributeValue
                        {
                            S = newAccessed
                        }
                    },
                    {
                        ":lastAccessed", new AttributeValue
                        {
                            S = previousAccessed
                        }
                    }
                },
                UpdateExpression = "SET #LastAccessed = :newAccessed", // new access
                ConditionExpression = "#LastAccessed = :lastAccessed", // old access
                TableName = tableName,
                ReturnValues = "ALL_NEW"
            };

            return request;

        }

        private async Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request)
        {
            var response = await _dynamoDbClient.UpdateItemAsync(request);

            return response;
        }


    }
}
