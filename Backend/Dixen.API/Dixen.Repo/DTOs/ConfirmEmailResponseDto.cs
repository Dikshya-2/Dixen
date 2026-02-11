using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.DTOs
{
    public class ConfirmEmailResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string QrCodeImage { get; set; } = string.Empty;
        public string ManualKey { get; set; } = string.Empty;
    }

}
