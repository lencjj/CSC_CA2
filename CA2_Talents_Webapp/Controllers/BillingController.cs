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

namespace CA2_Talents_Webapp.Controllers
{
    public class BillingController : ControllerBase
    {
        const string poolId = " ap-southeast-1_NF7svFGXu";
        const string appClientId = "37ggb1jujk4spjdaragjqb4nuv";
        static Amazon.RegionEndpoint region = Amazon.RegionEndpoint.APSoutheast1;

        [HttpPost]
        public IActionResult CreateSubscription(ChargeDTO chargeDto)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";
                int planNo = chargeDto.Plan;

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
                }
                //Premium plan
                else if (planNo == 2)
                {
                    plan = "price_1H9nnZHhYK7K9XttJdGEg31G";
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

                SignUpUser(chargeDto.Email, "Password123", customer.Id).Wait();

                return Redirect("/Home/Index?Msg=Success");
            } catch(Exception ex)
            {
                return Redirect("/Home/Index?Msg=" +ex.Message);
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