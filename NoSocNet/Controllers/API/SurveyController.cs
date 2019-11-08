using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoSocNet.Core.Models;
using NoSocNet.Core.Services;

namespace NoSocNet.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly SurveyService surveyService;
        public SurveyController(
            SurveyService surveyService
            )
        {
            this.surveyService = surveyService;
        }
        // GET: api/Survey
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SurveyDto>>> Get()
        {
            return await this.surveyService.List(null, 0, 100);
        }


        public class SurveyInput
        {
            [Required]
            [RegularExpression("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}")]
            public string ChatRoomId { get; set; }

            [Required]
            [Range(1, Int32.MaxValue)]
            public int SurveyId { get; set; }
        };


        //[HttpPost]
        //public async Task<ActionResult> Send([FromBody] SurveyInput input)
        //{
        //    surveyService.send();
        //}


        // GET: api/Survey/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Survey
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Survey/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
