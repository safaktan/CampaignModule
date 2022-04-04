using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignModule.Models
{
    public class Orders
    {

        [JsonProperty("product_Code")]
        public string ProductCode { get; set; }
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
    }
}