﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CA2_Talents_Webapp.Models;
using CA2_Talents_Webapp.Services;
using CA2_Talents_Webapp.SQLDatabase;
using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CA2_Talents_Webapp.Controllers
{
    [Produces("application/json")]
    public class TalentController : ControllerBase
    {
        // SQL Database configuration
        private readonly IConfiguration configuration;
        private IHostingEnvironment _env;
        private readonly IS3Service _service;

        public TalentController(IConfiguration configuration, IHostingEnvironment env, IS3Service service)
        {
            this.configuration = configuration;
            _env = env;
            _service = service;
        }

        [HttpGet]
        public JsonResult GetAllTalents()
        {
            TalentDb talentDb = new TalentDb(configuration);
            List<Talent> talents = new List<Talent>();
            talents = talentDb.getAllTalents();
            List<object> recordList = new List<object>();
            foreach (var talent in talents)
            {
                recordList.Add(new
                {
                    talentId = talent.TalentId,
                    talentName = talent.TalentName,
                    talentTitle = talent.TalentTitle,
                    talentDesc = talent.TalentDesc,
                    imageURL = talent.ImageURL,
                    createdDate = talent.CreatedDate,
                    createdBy = talent.CreatedBy,
                    updatedDate = talent.UpdatedDate,
                    updatedBy = talent.UpdatedBy
                });
            }

            return new JsonResult(recordList);
        }


        [HttpPost]
        public async Task<string> CreateTalent(Talent talent, IFormFile talentImgFile)
        {
            string imageName = "";
            //Save to images
            if (talentImgFile == null)
            {
                //Do nothing
            }
            else
            {
                try
                {
                    imageName = talentImgFile.FileName;
                    var dir = _env.WebRootPath;
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
                    if (response.Medical.ToString().Equals("Likely") || response.Medical.ToString().Equals("VeryLikely"))
                    {
                        System.IO.File.Delete(path);
                        return "NSFW";
                    }
                    if (response.Spoof.ToString().Equals("Likely") || response.Spoof.ToString().Equals("VeryLikely"))
                    {
                        System.IO.File.Delete(path);
                        return "NSFW";
                    }

                    await _service.UploadFileAsync(path);
                    System.IO.File.Delete(path);
                } catch(Exception ex)
                {
                    return ex.Message;
                }              
            }

            TalentDb talentDb = new TalentDb(configuration);
            talent.ImageURL = imageName;
            talentDb.addTalent(talent);
            return "Successfully added";
        }


        [HttpPost]
        public async Task<string> UpdateTalent(Talent talent, IFormFile talentImgFile)
        {
            string imageName = talent.ImageURL;
            //Save to images
            if (talentImgFile == null)
            {
                //Do nothing
            }
            else
            {
                try
                {
                    imageName = talentImgFile.FileName;
                    var dir = _env.WebRootPath;
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
                    if (response.Medical.ToString().Equals("Likely") || response.Medical.ToString().Equals("VeryLikely"))
                    {
                        System.IO.File.Delete(path);
                        return "NSFW";
                    }
                    if (response.Spoof.ToString().Equals("Likely") || response.Spoof.ToString().Equals("VeryLikely"))
                    {
                        System.IO.File.Delete(path);
                        return "NSFW";
                    }

                    await _service.UploadFileAsync(path);
                    System.IO.File.Delete(path);

                } catch(Exception ex)
                {
                    return ex.Message;
                }
            }
            TalentDb talentDb = new TalentDb(configuration);
            talent.ImageURL = imageName;
            talentDb.editTalent(talent);
            return "Successfully updated";
        }

        [HttpDelete]
        public string DeleteTalent(int talentId)
        {
            try
            {
                TalentDb talentDb = new TalentDb(configuration);
                talentDb.deleteTalent(talentId);
                string msg = "deleteSuccess";

                Console.WriteLine("--------------Success delete!!-----------");
                return msg;

            }
            catch (Exception ex)
            {
                Console.WriteLine("------------ERROR DELETINg--------------"+ex);
                return "error";
            }
        }
    }
}