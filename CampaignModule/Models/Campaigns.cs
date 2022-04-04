using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignModule.Models
{
    public class Campaigns
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("product_Code")]
        public string ProductCode { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("price_manipulation_limit")]
        public string PriceManipulationLimit { get; set; }
        [JsonProperty("target_sales_count")]
        public string TargetSalesCount { get; set; }
    }
}