using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Talents_Webapp.Services
{
    public class S3Service : IS3Service
    {
        public async Task UploadFileAsync(string path)
        {
            try
            {
                var credentials = new BasicAWSCredentials("Insert Key Here");
                var config = new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1
                };



                var client = new AmazonS3Client(credentials, config);
                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.UploadAsync(path, "storetalentimages");


            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                }
                else
                {
                    Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }
}
