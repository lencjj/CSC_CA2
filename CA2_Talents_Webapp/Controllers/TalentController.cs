using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CA2_Talents_Webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TalentController : ControllerBase
    {
        // SQL Database configuration
        private readonly IConfiguration configuration;
        public TalentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // --- All the talent methods ---


    }
}