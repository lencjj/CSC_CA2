using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CA2_Talents_Webapp.Models;
using Microsoft.Extensions.Configuration;
using CA2_Talents_Webapp.SQLDatabase;

namespace CA2_Talents_Webapp.Controllers
{
    public class HomeController : Controller
    {

        // SQL Database configuration (need to be REMOVED)
        private readonly IConfiguration configuration;
        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        // Test INSERT
        public IActionResult Page2()
        {
           // TalentDb talentDb = new TalentDb(configuration);

           //  Talent talent = new Talent();

           // talent.TalentId = 3;
           // talent.TalentName = "John Ta";
           // talent.TalentTitle = "Being John Ta";
           // talent.TalentDesc = "NAaaa";
           // talent.ImageURL = "Uwwwwwwwwqqqwqqqqq";
           // //talent.CreatedDate = DateTime.Now;
           //// talent.CreatedBy = "Jing Hui heh";
           // //talent.UpdatedDate = DateTime.Now;
           // talent.UpdatedBy = "Jing Hui";

            // ADD
            // string msg = talentDb.addTalent(talent);

            // GET
            // Talent talent = talentDb.getTalentById(2);

            // GET ALL
            // List<Talent> talents = talentDb.getAllTalents();

            // UPDATE
            // string msg = talentDb.editTalent(talent);

            // DELETE
            // string msg = talentDb.deleteTalent(8);


            return View();
        }

        // -----------------------------------------------------

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult GuestUser()
        {
            return View();
        }


        public IActionResult StandardUser(string name)
        {
            if (name.Length>0)
                TempData["name"] = name;
            else
                TempData["name"] = "no name";
            TempData.Keep();
            return View();
        }

        public IActionResult Main(string name, string plan, string stripeId)
        {
            if (name == null || plan == null)
            {
                TempData["name"] = "";
                TempData["plan"] = "";
                TempData["stripeId"] = "";
            }
            else if (name.Length > 0 && plan.Length > 0) {
                TempData["name"] = name;
                TempData["plan"] = plan;
                TempData["stripeId"] = stripeId;
            }
            else
            {
                TempData["name"] = "";
                TempData["plan"] = "";
                TempData["stripeId"] = "";
            }              
            TempData.Keep();
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Discussion()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
