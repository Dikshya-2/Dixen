using Dixen.Repo.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IJWTService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
    }
}
