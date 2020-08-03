using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CA2_Talents_Webapp.Models;
using CA2_Talents_Webapp.SQLDatabase;
using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CA2_Talents_Webapp.Controllers
{
    public class TalentController : ControllerBase
    {
        // SQL Database configuration
        private readonly IConfiguration configuration;
        private IHostingEnvironment _env;
        public TalentController(IConfiguration configuration, IHostingEnvironment env)
        {
            this.configuration = configuration;
            _env = env;
        }

        // --- All the talent methods ---
        [HttpPost]
        public string CreateTalent(Talent talent, IFormFile talentImgFile)
        {
            //Save to images
            Console.WriteLine(_env.ContentRootPath);
            var dir = _env.ContentRootPath + "/Images";
            Console.WriteLine(dir);
            string path = Path.Combine(dir, talentImgFile.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                talentImgFile.CopyTo(fileStream);
            }

            // Instantiates a client
            var client = ImageAnnotatorClient.Create();
            // Load the image file into memory
            var image = Image.FromFile(path);
            // Check for NSFW images
            var response = client.DetectSafeSearch(image);
            if (response.Adult.ToString().Equals("Likely") || response.Adult.ToString().Equals("VeryLikely"))
            {

                System.IO.File.Delete(path);
                return "NSFW";
            }
            if (response.Racy.ToString().Equals("Likely") || response.Racy.ToString().Equals("VeryLikely"))
            {
                System.IO.File.Delete(path);
                return "NSFW";
            }
            if (response.Violence.ToString().Equals("Likely") || response.Violence.ToString().Equals("VeryLikely"))
            {
                System.IO.File.Delete(path);
                return "NSFW";
            }
            if (response.Medical.ToString().Equals("L") || response.Medical.ToString().Equals("VeryLikely"))
            {
                System.IO.File.Delete(path);
                return "NSFW";
            }
            if (response.Spoof.ToString().Equals("LIKELY") || response.Spoof.ToString().Equals("VERY LIKELY"))
            {
                System.IO.File.Delete(path);
                return "NSFW";
            }

            TalentDb talentDb = new TalentDb(configuration);
            talent.ImageURL = talentImgFile.FileName;
            talentDb.addTalent(talent);
            return "Successfully added";
        }


    }
}