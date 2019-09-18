using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Http;  extension must be added to project
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace AlgoTest
{
    class Program
    {
        private static ServiceProvider serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider();
        static void Main(string[] args)
        {
            //openAndClosePrices("1-January-2000", "22-February-2000", "Monday").Wait();
            Console.WriteLine(largestMatrix(new List<List<int>>()
            {
                new List<int>(){1,1,1,1,1},
                new List<int>(){1,1,1,0,0},
                new List<int>(){1,1,1,0,0},
                new List<int>(){1,1,1,0,0},
                new List<int>(){1,1,1,1,1}
                /*new List<int>(){1,1,0,1,0},
                new List<int>(){0,1,1,1,0},
                new List<int>(){1,1,1,1,0},
                new List<int>(){0,1,1,1,1}*/
            }));
        }

        public static int largestMatrix(List<List<int>> arr)
        {
            int[,] arr1 = new int[arr.Count, arr[0].Count];

            int result = 0;

            for (int i = 0; i < arr.Count; i++)
            {
                for (int j = 0; j < arr[i].Count; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        arr1[i, j] = arr[i][j];
                    }
                    else if(arr[i][j] > 0)
                    {
                        arr1[i, j] = 1 + Math.Min(Math.Min(arr1[i, j - 1], arr1[i - 1, j - 1]), arr1[i - 1, j]);
                    }

                    if (arr1[i, j] > result)
                        result = arr1[i, j];
                }
            }
            
            return result;
        }


        static async Task openAndClosePrices(string firstDate, string lastDate, string weekDay)
        {
            Dictionary<string, int> monthPair = 
                new Dictionary<string, int>()
                {
                    ["January"] = 01,
                    ["February"] = 02,
                    ["March"] = 03,
                    ["April"] = 04,
                    ["May"] = 05,
                    ["June"] = 06,
                    ["July"] = 07,
                    ["August"] = 08,
                    ["September"] = 09,
                    ["October"] = 10,
                    ["November"] = 11,
                    ["December"] = 12
                };
            string[] d1 = firstDate.Split("-");
            string[] d2 = lastDate.Split("-");
            DateTime date1 = new DateTime(int.Parse(d1[2]), monthPair[d1[1]], int.Parse(d1[0]));
            DateTime date2 = new DateTime(int.Parse(d2[2]), monthPair[d2[1]], int.Parse(d2[0]));

            // using (var client = new WebClient());
            var _clientFactory = serviceProvider.GetService<IHttpClientFactory>();

            for(int i =1; i<= 5; i++)
            {
                using (var client = _clientFactory.CreateClient())
                {
                    //client.BaseAddress = "https://jsonmock.hackerrank.com/";

                    client.BaseAddress = new Uri("https://jsonmock.hackerrank.com/");
                    string path = $"api/stocks/?page={i}";

                    try
                    {
                        //string response = client.DownloadString(path);
                        string response = await client.GetStringAsync(path);
                        //var content = await response.Content.ReadAsStringAsync();
                        JObject jObject = JObject.Parse(response);
                        JArray jArray = (JArray)jObject["data"];

                        /*the first Where LINQ operation tries to reduce the number of records to check by
                        filtering out the records that don't have the same year as the dates specified 
                        in the function's arguments*/

                        var records = jArray.Select(x => new DataRecord
                        {
                            Date = (string)x["date"],
                            Open = (double)x["open"],
                            High = (double)x["high"],
                            Low = (double)x["low"],
                            Close = (double)x["close"]
                        }).Where(x => int.Parse(x.Date.Split("-")[2]) >= int.Parse(d1[2]) && int.Parse(x.Date.Split("-")[2]) <= int.Parse(d2[2]))
                        .Where(x => new DateTime(int.Parse(x.Date.Split("-")[2]), monthPair[x.Date.Split("-")[1]], int.Parse(x.Date.Split("-")[0])) >= date1 && new DateTime(int.Parse(x.Date.Split("-")[2]), monthPair[x.Date.Split("-")[1]], int.Parse(x.Date.Split("-")[0])) <= date2 && new DateTime(int.Parse(x.Date.Split("-")[2]), monthPair[x.Date.Split("-")[1]], int.Parse(x.Date.Split("-")[0])).ToLongDateString().StartsWith(weekDay))
                        .Select(x => new {
                            x.Date,
                            x.Open,
                            x.Close
                        });

                        foreach (var record in records)
                        {
                            Console.WriteLine($"{record.Date} {record.Open} {record.Close}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception occured: {ex.Message},, stack trace: {ex.StackTrace}");
                    }
                }
            }
        }
    }
}
