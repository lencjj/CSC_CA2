using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using CA2_Talents_Webapp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CA2_Talents_Webapp.Controllers
{

    public class UserpoolController : ControllerBase
    {
        const string poolId = " ap-southeast-1_NF7svFGXu";
        const string appClientId = "37ggb1jujk4spjdaragjqb4nuv";
        static Amazon.RegionEndpoint region = Amazon.RegionEndpoint.APSoutheast1;

        string standardPlan = "price_1H9njJHhYK7K9XttfEulEs63";
        string premiumPlan = "price_1H9nnZHhYK7K9XttJdGEg31G";

        [HttpPost]
        public async Task<IActionResult> SignInUser(LoginCreds loginCreds)
        {
            IAmazonCognitoIdentityProvider provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), region);
            CognitoUserPool userPool = new CognitoUserPool(poolId, appClientId, provider);
            CognitoUser user = new CognitoUser(loginCreds.Email, appClientId, userPool, provider);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = loginCreds.Password
            };

            AuthFlowResponse authResponse = null;
            try
            {
                //Authenticate user and retrieve stripe Id
                authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

                GetUserRequest getUserRequest = new GetUserRequest();
                getUserRequest.AccessToken = authResponse.AuthenticationResult.AccessToken;

                GetUserResponse getUser = await provider.GetUserAsync(getUserRequest);
                string email = getUser.UserAttributes.Where(a => a.Name == "email").First().Value;
                string stripeId = getUser.UserAttributes.Where(a => a.Name == "custom:stripeId").First().Value;

                //Retrieve plan type from stripe
                string userType = "Not registered as a Life Time Talents user";
                StripeConfiguration.ApiKey = "sk_test_51GxEfiHhYK7K9XttqUpv12yjajZLs01TY95VhvzVfPEb5Ed8GaF3GFUV2iuhFZGkBgHoNib4iHBDlpALqWPplth6008EdMnnaw";
                var service = new CustomerService();
                Customer customer = service.Get(stripeId);
                var subscriptions = customer.Subscriptions; 
                for(int i = 0; i < subscriptions.Count(); i++) 
                {
                    if (subscriptions.ElementAt(i).Plan.Id.Equals(standardPlan))
                    {
                        userType = "You are registered as a standard user";
                    }
                    else if (subscriptions.ElementAt(i).Plan.Id.Equals(premiumPlan))
                    {
                        userType = "You are registered as a premium user";
                    }

                }
                Console.WriteLine(userType);

                return Redirect("/Home/Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login failed: " + ex.Message);
                return Redirect("/Home/Login");
            }
        }
    }
}