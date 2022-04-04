using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignModule.Models
{
    public class ResponseDomain
    {
        [JsonProperty("Result")]
        public string Result { get; set; }
    }
}