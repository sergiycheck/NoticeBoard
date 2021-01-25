using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoticeBoard.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string Message { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
