using System;

namespace PBL_N2_1BI.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string erro, string requestId = null)
        {
            Erro = erro;
            RequestId = requestId;
        }

        public ErrorViewModel()
        {
        }

        public string Erro { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
