using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface ITwoFAService
    {
        string GenerateQrCodeUri(string email, string unformattedKey);
        string GenerateQrCodeImage(string email, string secretKey);
    }

}
