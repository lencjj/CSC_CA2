using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDb.libs.DynamoDb
{
    public class AddUser : IAddUser
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public AddUser(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }


        public async Task AddNewEntry(string email, string subPlan, string lastPaid, string username, string password, string phoneNo, string lastAccessed)
        {
            var queryRequest = RequestBuilder(email, subPlan, lastPaid, username, password, phoneNo, lastAccessed);

            await PutItemAsync(queryRequest);
        }

        private PutItemRequest RequestBuilder(string email, string subPlan, string lastPaid, string username, string password, string phoneNo, string lastAccessed)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                {"Email", new AttributeValue {S = email}},
                {"SubscriptionPlan", new AttributeValue {S = subPlan}},
                {"LastPaid", new AttributeValue {S = lastPaid}},
                {"UserName", new AttributeValue {S = username}},
                {"Password", new AttributeValue {S = password}},
                {"PhoneNo", new AttributeValue {S = phoneNo}},
                {"LastAccessed", new AttributeValue {S = lastAccessed}}
            };

            return new PutItemRequest
            {
                TableName = "User",
                Item = item
            };
        }
        private async Task PutItemAsync(PutItemRequest request)
        {
            await _dynamoDbClient.PutItemAsync(request);
        }

    }
}
