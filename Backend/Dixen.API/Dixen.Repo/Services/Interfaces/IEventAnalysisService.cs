using Dixen.Repo.DTOs.Analysis;
using Dixen.Repo.DTOs.EventAttendance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IEventAnalysisService
    {
        Task<List<EventSummaryDto>> AnalyzeEventsAsync();
        AnalyticsResultDto CalculateAnalytics(List<EventSummaryDto> data);
       
    }
}
