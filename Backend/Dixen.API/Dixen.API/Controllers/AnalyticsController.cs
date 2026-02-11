using Dixen.Repo.Services;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/reporting")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IEventAnalysisService _service;
        public AnalyticsController(IEventAnalysisService analysisService)
        {
            _service = analysisService;
        }

        [HttpGet("events-summary")]
        public async Task<IActionResult> GetEventsSummary()
        {
            try
            {
                var data = await _service.AnalyzeEventsAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "REAL ERROR",
                    detail: ex.Message,  
                    statusCode: 500
                );
            }
        }
    }
}
