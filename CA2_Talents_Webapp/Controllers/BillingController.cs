using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using CA2_Talents_Webapp.Models;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Google.Cloud.BigQuery.V2;
using System.Configuration;
using System.Globalization;
using DynamoDb.libs.DynamoDb;

namespace CA2_Talents_Webapp.Controllers
{
    public class BillingController : ControllerBase
    {
        const string poolId = " ap-southeast-1_NF7svFGXu";
        const string appClientId = "37ggb1jujk4spjdaragjqb4nuv";
        static Amazon.RegionEndpoint region = Amazon.RegionEndpoint.APSoutheast1;

        // DyanamoDb
        private readonly IAddUser _addUser;
        private readonly IGetUser _getUser;
        private readonly IUpdateUser _updateUser;

        // Constructor
        public BillingController(IAddUser addUser, IGetUser getUser, IUpdateUser updateUser)
        {
            _addUser = addUser;
            _getUser = getUser;
            _updateUser = updateUser;
        }

        // DynamoDb methods -------------------------------------------------------------------------------------- 
        public IActionResult AddUser([FromQuery]
            string email, 
            string subplan, 
            string lastpaid,
            string username,
            string password, 
            string phoneno,
            string lastaccessed)
        {
            _addUser.AddNewEntry(email, subplan, lastpaid, username, password, phoneno, lastaccessed);
            return Ok();
        }
        
        public async Task<IActionResult> GetUser([FromQuery] string email)
        {
            var response = await _getUser.GetUsers(email);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserLastAccessed(string email, string lastaccessed)
        {
            var response = await _updateUser.UpdateUserLastAccessed(email, lastaccessed);
            return Ok(response);
        }

        // DynamoDb methods --------------------------------------------------------------------------------------

        [HttpPost]
        public IActionResult CreateSubscription(ChargeDTO chargeDto)
        {
            try
            {
                // Change this file accordingly
                string credential_path = @"C:\Users\Brian Chong\Desktop\SP Year 3\CSC\Assignment 2\My Map Projects-2ff7e55dc99d.json";
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
                StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";
                int planNo = chargeDto.Plan;
                String planName = "";
                DateTime dateTimeNow = DateTime.Now;

                //Create customer
                var customerOptions = new CustomerCreateOptions
                {
                    Description = chargeDto.CardName,
                    Source = chargeDto.StripeToken,
                    Email = chargeDto.Email,
                    Metadata = new Dictionary<string, string>()
                    {
                        { "Phone Number", chargeDto.Phone }
                    }
                };

                var customerService = new CustomerService();
                Customer customer = customerService.Create(customerOptions);


                //Create subscription
                string plan = "";
                //Standard plan
                if (planNo == 1)
                {
                    plan = "price_1H9njJHhYK7K9XttfEulEs63";
                    planName = "Standard plan";
                }
                //Premium plan
                else if (planNo == 2)
                {
                    plan = "price_1H9nnZHhYK7K9XttJdGEg31G";
                    planName = "Premium plan";
                }
                var options = new SubscriptionCreateOptions
                {
                    Customer = customer.Id,
                    Items = new List<SubscriptionItemOptions>
                {
                new SubscriptionItemOptions
                    {
                        Price = plan,
                    },
                },
                };
                var service = new SubscriptionService();
                Subscription subscription = service.Create(options);

                
                SignUpUser(chargeDto.Email, chargeDto.Password, customer.Id).Wait();
                InsertBQData(chargeDto.Email, planNo);

                return Redirect("/Home/Index?Msg=Success");
            }
            catch (Exception ex)
            {
                return Redirect("/Home/Index?Msg=" + ex.Message);
            }
        }

        private static async Task SignUpUser(string email, string password, string customerId)
        {

            IAmazonCognitoIdentityProvider provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), region);
            SignUpRequest signUpRequest = new SignUpRequest()
            {
                ClientId = appClientId,
                Username = email,
                Password = password
            };

            List<AttributeType> attributes = new List<AttributeType>()
            {
                new AttributeType(){Name = "email", Value = email},
                new AttributeType(){Name = "custom:stripeId", Value = customerId}
            };
            signUpRequest.UserAttributes = attributes;

            try
            {
                SignUpResponse result = await provider.SignUpAsync(signUpRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void InsertBQData(string email, int plan)
        {
            try
            {
                string projectId = "turnkey-guild-265901";
                DateTime now = DateTime.Now;
                string planName = "";
                Console.Clear();
                Console.WriteLine("email: " + email);
                Console.WriteLine("plan: " + plan);

                BigQueryClient client = BigQueryClient.Create(projectId);

                // Create the dataset if it doesn't exist.
                BigQueryDataset dataset = client.GetOrCreateDataset("mydata");

                // Create the table if it doesn't exist.
                BigQueryTable table = dataset.GetOrCreateTable("customer", new TableSchemaBuilder
                {
                    { "email", BigQueryDbType.String },
                    { "subscriptionStarted", BigQueryDbType.Timestamp },
                    { "planType", BigQueryDbType.String } //BigQueryDbType.Int64
                }.Build());

                if (plan == 1)
                {
                    planName = "Standard user";
                }
                else
                {
                    planName = "Premium user";
                }

                //Insert data into table
                table.InsertRow(new BigQueryInsertRow
                {
                    { "email", email },
                    { "subscriptionStarted", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                    { "planType", planName }
                });
                Console.WriteLine("Inserted: " + email + "successfully");
            }// End of try
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }// End of insertBQData

        [HttpPost]
        public ActionResult CreateSession()
        {
            StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";

            var options = new SessionCreateOptions
            {
                Customer = "cus_HWIVyLfT0yhlOg",
                ReturnUrl = "https://localhost:44356/Home/Billing",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return Redirect(session.Url);
        }

        [HttpPost]
        public ActionResult pauseSubscription()
        {
            StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";

            var options = new SubscriptionUpdateOptions
            {
                PauseCollection = new SubscriptionPauseCollectionOptions
                {
                    Behavior = "void",
                },
            };
            var service = new SubscriptionService();
            service.Update("sub_HXNOGgEcJT2vvM", options);
            return Redirect("/Home/Billing");
        }

        [HttpPost]
        public ActionResult continueSubscription()
        {
            StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";

            var options = new SubscriptionUpdateOptions();
            options.AddExtraParam("pause_collection", "");
            var service = new SubscriptionService();
            service.Update("sub_HXNOGgEcJT2vvM", options);
            return Redirect("/Home/Billing");
        }
    }
}