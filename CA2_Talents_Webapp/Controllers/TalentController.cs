using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CA2_Talents_Webapp.Models;
using CA2_Talents_Webapp.SQLDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CA2_Talents_Webapp.Controllers
{
    public class TalentController : ControllerBase
    {
        // SQL Database configuration
        private readonly IConfiguration configuration;
        public TalentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // --- All the talent methods ---
        [HttpPost]
        public IActionResult CreateTalent(Talent talent)
        {
            TalentDb talentDb = new TalentDb(configuration);
            talent.UpdatedBy = talent.CreatedBy;
            talent.ImageURL = "imageUrl.jpg";
            talentDb.addTalent(talent);
            Console.WriteLine("Successfully added");
            return Ok();
        }


    }
}