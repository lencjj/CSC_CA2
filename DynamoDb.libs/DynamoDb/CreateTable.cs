//very good document about sort keys all https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.CoreComponents.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDb.libs.DynamoDb
{
    public class CreateTable : ICreateTable
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private static readonly string tableName = "User";

        public CreateTable(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public void CreateDynamoDbUserTable()
        {
            try
            {
                CreateUserTable();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void CreateUserTable()
        {
            Console.WriteLine("Creating Table...");

            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Email",
                        AttributeType = "S"
                    }//,
                    //    new AttributeDefinition
                    //{
                    //    AttributeName = "SubscriptionPlan",
                    //    AttributeType = "S"
                    //}
            },
                KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Email",
                    KeyType = "HASH" // Partition Key
                }//,
                //new KeySchemaElement
                //{
                //    AttributeName = "SubscriptionPlan",
                //    KeyType = "Range" // Sort Key
                //}
            },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                },
                TableName = tableName
            };

            var response = _dynamoDbClient.CreateTableAsync(request);

            WaitUntilTableReady(tableName);
        }

        private void WaitUntilTableReady(string tableName)
        {
            string status = null;

            do
            {
                Thread.Sleep(5000);
                try
                {
                    var res = _dynamoDbClient.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    status = res.Result.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {

                }

            } while (status != "ACTIVE");
            {
                Console.WriteLine("Table Created Successfully");
            }
        }
    }

}
