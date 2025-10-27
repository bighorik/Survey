using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Survey.Domain;
using Survey.Persistence;

namespace Survey.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly SurveyRepository _repository;
        public SurveyController(SurveyRepository repository)
        {
            _repository = repository;
        }

        [EnableCors("AllowAll")]
        [HttpGet]
        public Task<SurveyStats> Get()
        {
            return _repository.GetSurveyStats();
        }

        [EnableCors("AllowAll")]
        [HttpPost]
        public Task Get([FromBody] SurveyResult insertation)
        {
            return _repository.InsertSurvey(insertation);
        }
    }
}
