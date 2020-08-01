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
    public class GetUser : IGetUser
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public GetUser(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<DynamoTableItems> GetUsers(string email)
        {
            var queryRequest = RequestBuilder(email);

            var result = await ScanAsync(queryRequest);

            return new DynamoTableItems
            {
                Users = result.Items.Select(Map).ToList()
            };
        }
        
        private User Map(Dictionary<string, AttributeValue> result)
        {
            return new User
            {
                Email = result["Email"].S,
                SubscriptionPlan = result["SubscriptionPlan"].S,
                LastPaid = result["LastPaid"].S,
                UserName = result["UserName"].S,
                Password = result["Password"].S,
                PhoneNo = result["PhoneNo"].S,
                LastAccessed = result["LastAccessed"].S
            };
        }

        private async Task<ScanResponse> ScanAsync(ScanRequest request)
        {
            var response = await _dynamoDbClient.ScanAsync(request);

            return response;
        }

        private ScanRequest RequestBuilder(string email)
        {
            if (email == null)
            {
                return new ScanRequest
                {
                    TableName = "User"
                };
            }

            return new ScanRequest
            {
                TableName = "User",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_Email", new AttributeValue { S = email }}
                },
                FilterExpression = "Email = :v_Email",
                ProjectionExpression = "Email, SubscriptionPlan, LastPaid, UserName, Password, PhoneNo, LastAccessed"
            };
        }
    }
}
