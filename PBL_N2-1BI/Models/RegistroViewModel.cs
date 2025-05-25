using Newtonsoft.Json;
using System;

namespace PBL_N2_1BI.Models
{
    public class RegistroViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("dataRegistro")]
        public DateTime DataRegistro { get; set; }

        [JsonProperty("valorUmidade")]
        public int ValorUmidade { get; set; }

        [JsonProperty("valorTemperatura")]
        public double ValorTemperatura { get; set; }

        [JsonProperty("valorLuminosidade")]
        public int ValorLuminosidade { get; set; }
    }
}
