using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Repositories
{
    public class EventSubmissionRepository : GRepo<EventSubmission>, IEventSubmissionRepository
    {
        private readonly DatabaseContext _context;
        public EventSubmissionRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }
        public async Task ApproveEventSubmission(int submissionId)
        {
            var submission = await _context.EventSubmissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.IsApproved = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectEventSubmission(int submissionId)
        {
            var submission = await _context.EventSubmissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.IsApproved = false;
                await _context.SaveChangesAsync();
            }
        }
    }

}
