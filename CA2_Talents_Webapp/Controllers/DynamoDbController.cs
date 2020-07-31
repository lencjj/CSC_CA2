using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamoDb.libs.DynamoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CA2_Talents_Webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamoDbController : ControllerBase
    {
        // DyanamoDb
        private readonly ICreateTable _createTable;

        // Constructor
        public DynamoDbController(ICreateTable createTable)
        {
            _createTable = createTable;
        }

        // DynamoDb methods -------------------------------------------------------------------------------------- 
        public IActionResult CreateDynamoUserTable() // table creation
        {
            _createTable.CreateDynamoDbUserTable();
            return Ok();
        }

    }
}