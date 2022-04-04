using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampaignModule.Models
{
    public class Products
    {
        [JsonProperty("product_Code")]
        public string ProductCode{get;set;}

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("stock")]

        public string Stock { get; set; }
    }
}