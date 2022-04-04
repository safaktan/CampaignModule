using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CampaignModule.Models;

namespace CampaignModule.Controllers
{
    public class HomeController : Controller
    {
        //Dictionary<string, GenericDomain<Campaigns>> denemList = new Dictionary<string, GenericDomain<Campaigns>>();
        List<Products> productList = new List<Products>();
        List<Campaigns> campaignList = new List<Campaigns>();
        List<Orders> orderList = new List<Orders>();
        public ActionResult Index()
        {
            return View();
        }

        public void WriteTheOutputFile(string content)
        {
            string outputFilePath = Server.MapPath("/Files/Output.txt");
            if (!String.IsNullOrEmpty(content))
                System.IO.File.AppendAllText(outputFilePath, content + Environment.NewLine);
            else
                System.IO.File.WriteAllText(outputFilePath, content);
        }


        public async Task<JsonResult> GetCampaignInformation()
        {
            try
            {
                string scenarioFilePath = Server.MapPath("/Files/Scenario.txt");
                var fileStream = new FileStream(scenarioFilePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string campaignStatus = "Active";
                    string campaignHours = "00:00";

                    List<int> totalSalesQuantityList = new List<int>();
                    List<int> priceList = new List<int>();

                    int totalIncreasedTime = 0;

                    productList.Clear();
                    orderList.Clear();
                    campaignList.Clear();
                    WriteTheOutputFile(String.Empty);

                    string line;
                    bool loopEnd = false;
                    while ((line = streamReader.ReadLine()) != null && loopEnd == false)
                    {
                        var splittedLine = line.Split(' ');
                        if (splittedLine.Length > 0)
                        {
                            if (splittedLine[0].ToLower().Equals("create_product"))
                            {
                                var result = CheckFunctionParameter(splittedLine);
                                if (result)
                                {
                                    var productInfo = GetProductInfoFromProductList(productList, splittedLine[1]);

                                    if (productInfo.Count == 0 || productInfo["Existsing"] == 0)
                                    {
                                        Products products = new Products();
                                        products.ProductCode = splittedLine[1];
                                        products.Price = splittedLine[2];
                                        products.Stock = splittedLine[3];

                                        productList.Add(products);
                                        WriteTheOutputFile("Product created; code " + products.ProductCode + ", price " + products.Price + ", stock " + products.Stock);
                                    }
                                    else
                                    {
                                        loopEnd = true;
                                        WriteTheOutputFile("Product " + splittedLine[1] + " already exists.");
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("create_product function parameter is missing or much");
                                }
                            }
                            else if (splittedLine[0].ToLower().Equals("get_product_info"))
                            {
                                var result = CheckFunctionParameter(splittedLine);

                                if (result)
                                {

                                    var productInfo = GetProductInfoFromProductList(productList, splittedLine[1]);

                                    if (productInfo.Count != 0 && productInfo["Existsing"] == 1)
                                    {
                                        string productInfoResult = "";
                                        for (int i = 0; i < productList.Count; i++)
                                        {
                                            if (productList[i].ProductCode.ToLower().Equals(splittedLine[1].ToLower()))
                                            {
                                                productInfoResult = "Product " + productList[i].ProductCode + " info; price " + productList[i].Price + ", stock " + productList[i].Stock;
                                                WriteTheOutputFile(productInfoResult);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        loopEnd = true;
                                        WriteTheOutputFile("Product " + splittedLine[1] + " does not exists");
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("get_product_info function parameter is missing or much");
                                }

                            }
                            else if (splittedLine[0].ToLower().Equals("create_order"))
                            {
                                var result = CheckFunctionParameter(splittedLine);
                                if (result)
                                {
                                    var productInfo = GetProductInfoFromProductList(productList, splittedLine[1]);

                                    if (productInfo.Count != 0 && productInfo["Existsing"] == 1)
                                    {
                                        Orders orders = new Orders();
                                        orders.ProductCode = splittedLine[1];
                                        orders.Quantity = splittedLine[2];

                                        priceList.Add(productInfo["Price"] * Convert.ToInt32(orders.Quantity));

                                        totalSalesQuantityList.Add(Convert.ToInt32(orders.Quantity));

                                        UpdateProductStock(productList, orders);


                                        orderList.Add(orders);

                                        WriteTheOutputFile("Order created; code " + orders.ProductCode + ", quantity " + orders.Quantity);
                                    }
                                    else
                                    {
                                        loopEnd = true;
                                        WriteTheOutputFile("Product " + splittedLine[1] + " does not exists");
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("create_order function parameter is missing or much");
                                }
                            }
                            else if (splittedLine[0].ToLower().Equals("create_campaign"))
                            {
                                var result = CheckFunctionParameter(splittedLine);
                                if (result)
                                {

                                    var productInfo = GetProductInfoFromProductList(productList, splittedLine[2]);

                                    if (productInfo.Count != 0 && productInfo["Existsing"] == 1)
                                    {
                                        Campaigns campaigns = new Campaigns();
                                        campaigns.Name = splittedLine[1];
                                        campaigns.ProductCode = splittedLine[2];
                                        campaigns.Duration = splittedLine[3];
                                        campaigns.PriceManipulationLimit = splittedLine[4];
                                        campaigns.TargetSalesCount = splittedLine[5];

                                        campaignList.Add(campaigns);

                                        WriteTheOutputFile("Campaign created; name " + campaigns.Name + ", product " + campaigns.ProductCode + ", duration " + campaigns.Duration + ", limit " + campaigns.PriceManipulationLimit + ", target sales count " + campaigns.TargetSalesCount);
                                    }
                                    else
                                    {
                                        loopEnd = true;
                                        WriteTheOutputFile("Product " + splittedLine[2] + " does not exists");
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("create_campaign function parameter is missing or much");
                                }
                            }
                            else if (splittedLine[0].ToLower().Equals("get_campaign_info"))
                            {
                                var result = CheckFunctionParameter(splittedLine);

                                if (result)
                                {
                                    var campaignExist = IsCampaignExist(campaignList, splittedLine[1]);
                                    if (campaignExist)
                                    {
                                        string campaignInfo = "";
                                        for (int i = 0; i < campaignList.Count; i++)
                                        {
                                            if (campaignList[i].Name.ToLower().Equals(splittedLine[1].ToLower()))
                                            {
                                                if (Convert.ToInt32(campaignList[i].Duration) <= totalIncreasedTime)
                                                    campaignStatus = "Ended";

                                                var totalPrice = GetTotalPrice(priceList);


                                                var totalSales = GetTotalSales(totalSalesQuantityList);

                                                var avgPrice = 0.0;
                                                if(totalSales != 0)
                                                    avgPrice = totalPrice / totalSales;



                                                campaignInfo = "Campaign " + campaignList[i].Name + " info; Status " + campaignStatus + ", Target Sales " + campaignList[i].TargetSalesCount + ", Total Sales " + totalSales + ", Turnover " + (totalSales * avgPrice) + ", Average Item Price " + avgPrice;
                                                WriteTheOutputFile(campaignInfo);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        loopEnd = true;
                                        WriteTheOutputFile("Campaign " + splittedLine[1] + " does not exists");
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("get_campaign_info function parameter is missing or much");
                                }
                            }
                            else if (splittedLine[0].ToLower().Equals("increase_time"))
                            {
                                var result = CheckFunctionParameter(splittedLine);

                                if (result)
                                {
                                    string campaignHoursInfo = "";
                                    campaignHours = SetCampaignTime(campaignHours, Convert.ToInt32(splittedLine[1]));
                                    totalIncreasedTime += Convert.ToInt32(splittedLine[1]);
                                    campaignHoursInfo = "This is " + campaignHours;
                                    WriteTheOutputFile(campaignHoursInfo);
                                    // ürün fiyat güncelle

                                    if (productList.Count > 0)
                                    {
                                        var percentageOfPrice = (Convert.ToDouble(productList[0].Price) * 2.0) / 100.0;

                                        int cellimnNumber = Convert.ToInt32(Math.Ceiling(percentageOfPrice));
                                        var res = Convert.ToInt32(productList[0].Price) - cellimnNumber;

                                        productList[0].Price = res.ToString();
                                    }
                                }
                                else
                                {
                                    loopEnd = true;
                                    WriteTheOutputFile("increase_time function parameter is missing or much");

                                }
                            }
                            else
                            {
                                loopEnd = true;
                                WriteTheOutputFile("Invalid scenario command: " + splittedLine[0].ToLower());
                            }
                        }
                        else
                        {
                            loopEnd = true;
                            WriteTheOutputFile("Scenario files can not be blank");
                        }
                    }
                }


                var resultList = CreateOutput();


                return Json(new { data = resultList, errorCode = "", errorMessage = "" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<ResponseDomain> CreateOutput()
        {

            try
            {
                List<ResponseDomain> outputList = new List<ResponseDomain>();
                string outputFilePath = Server.MapPath("/Files/Output.txt");
                var fileStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    bool loopEnd = false;
                    while ((line = streamReader.ReadLine()) != null && loopEnd == false)
                    {
                        ResponseDomain responseDomain = new ResponseDomain();
                        responseDomain.Result = line;
                        outputList.Add(responseDomain);
                    }
                }
                return outputList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return null;
        }

        public void UpdateProductStock(List<Products> list, Orders orders)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].ProductCode.ToLower().Equals(orders.ProductCode.ToLower()))
                {
                    var res = 0;

                    if (Convert.ToInt32(list[i].Stock) - Convert.ToInt32(orders.Quantity) >= 0)
                        res = Convert.ToInt32(list[i].Stock) - Convert.ToInt32(orders.Quantity);

                    list[i].Stock = res.ToString();
                }
            }
        }

        public bool IsCampaignExist(List<Campaigns> list, string name)
        {
            bool result = false;

            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Name.ToLower().Equals(name.ToLower()))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public Dictionary<string,int> GetProductInfoFromProductList(List<Products> list, string productCode)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            int priceResult = 0;
            int productExisting = 0;
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].ProductCode.ToLower().Equals(productCode.ToLower()))
                {
                    productExisting = 1;
                    result.Add("Existsing", productExisting);

                    priceResult = Convert.ToInt32(list[i].Price);
                    result.Add("Price", priceResult);

                    break;
                }
                else
                {
                    result.Add("Existsing", productExisting);
                }
            }
            return result;
        }
        public string SetCampaignTime(string hours, int increasedValue)
        {
            string result = "";
            var splittedHours = hours.Split(':');

            var hoursValue = Convert.ToInt32(splittedHours[0]) + increasedValue;

            if (hoursValue < 10)
                result = "0" + hoursValue + ":00";
            else
                result = hoursValue + ":00";


            return result;
        }

        public bool CheckFunctionParameter(string[] functionName)
        {
            if (functionName[0].ToLower().Equals("create_product"))
            {
                if (functionName.Length - 1 == 3)
                    return true;
            }
            else if (functionName[0].ToLower().Equals("get_product_info"))
            {
                if (functionName.Length - 1 == 1)
                    return true;
            }
            else if (functionName[0].ToLower().Equals("create_order"))
            {
                if (functionName.Length - 1 == 2)
                    return true;
            }
            else if (functionName[0].ToLower().Equals("create_campaign"))
            {
                if (functionName.Length - 1 == 5)
                    return true;
            }
            else if (functionName[0].ToLower().Equals("get_campaign_info"))
            {
                if (functionName.Length - 1 == 1)
                    return true;
            }
            else if (functionName[0].ToLower().Equals("increase_time"))
            {
                if (functionName.Length - 1 == 1)
                    return true;
            }

            return false;
        }

        public int GetTotalSales(List<int> list)
        {
            int result = 0;
            for(int i = 0; i < list.Count; i++)
            {
                result += list[i];
            }
            return result;
        }

        public int GetTotalPrice(List<int> list)
        {
            int result = 0;
            for(int i = 0; i < list.Count; i++)
            {
                result += list[i];
            }
            return result;
        }

        public static Task<JsonResult> WriteTheScenarioFile()
        {

            //read File
            //public string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/Scenario.txt");

            var fileStream = new FileStream(Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory + "/Files/Scenario.txt"), FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    // process the line
                }
            }

            //string static deneme = _filePath;

            //Call the realed function

            //write the result file 

            //read the result file for viewing // convert json

            //return the result json


            return null;





        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}