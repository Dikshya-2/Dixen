using Dixen.Repo.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Repositories.Interfaces
{
    public interface IEventSubmissionRepository : IGRepo<EventSubmission>
    {
        Task ApproveEventSubmission(int submissionId);
        Task RejectEventSubmission(int submissionId);
    }

}
