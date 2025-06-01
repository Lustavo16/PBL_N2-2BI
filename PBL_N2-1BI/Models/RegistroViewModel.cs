using Newtonsoft.Json;
using System;

namespace PBL_N2_1BI.Models
{
    public class RegistroViewModel :PadraoViewModel
    {
        [JsonProperty("dataRegistro")]
        public DateTime DataRegistro { get; set; }

        [JsonProperty("valorTemperatura")]
        public double ValorTemperatura { get; set; }
    }
}
