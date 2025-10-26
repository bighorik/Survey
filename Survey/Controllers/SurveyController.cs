using Microsoft.AspNetCore.Mvc;
using Survey.Domain;
using Survey.Persistence;

namespace Survey.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyController(SurveyRepository repository) : ControllerBase
    {
        [HttpGet]
        public Task<SurveyStats> Get()
        {
            return repository.GetSurveyStats();
        }

        [HttpPost]
        public Task Get([FromBody] SurveyResult insertation)
        {
            return repository.InsertSurvey(insertation);
        }
    }
}
