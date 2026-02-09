using Dixen.Repo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dixen.API.Controllers
{
    [Route("api/reporting")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly EventAnalysisService _service;

        public AnalyticsController(EventAnalysisService analysisService)
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
