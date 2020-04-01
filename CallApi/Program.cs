using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CallApi
{
    class Program
    {
        static int tableWidth = 100;

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string login = "supremecomponents";
            string apikey = "07b23129ead7328ca4f14a9c08fa89f333e30d08042a5ec4d211e7b66851825d";
            string[,] parts = new string[5, 2] {
                { "24AA256-I/MS", "MICROCHIP" },
                { "LT1805CSPBF", "Arrows" },
                { "MAX32,32CAE+T", "MAXIM" },
                { "MIC5319-3.3YD5-.TR", "" },
                { "SSL1523P/N2112", "NXP"}
            };

            UriBuilder builder = new UriBuilder("http://api.arrow.com/itemservice/v3/en/search/list");
            builder.Query = "?req={\"request\":{\"login\":\"" + login + "\",\"apikey\":\"" + apikey + "\",\"remoteIp\":\"::1\",\"useExact\":true,\"parts\":[{\"partNum\":\"24AA256-I/MS\",\"mfr\":\"MICROCHIP\"},{\"partNum\":\"LT1805CSPBF\",\"mfr\":\"Arrows\"},{\"partNum\":\"MAX32,32CAE+T\",\"mfr\":\"MAXIM\"},{\"partNum\":\"MIC5319-3.3YD5-.TR\"},{\"partNum\":\"SSL1523P/N2112\",\"mfr\":\"NXP\"}]}}";

            //Create a query
            HttpClient client = new HttpClient();
            var result = client.GetAsync(builder.Uri).Result;

            using (StreamReader sr = new StreamReader(result.Content.ReadAsStreamAsync().Result))
            {
                string jsonPretty = JsonPrettify(sr.ReadToEnd());
                var details = JObject.Parse(jsonPretty);
                //Console.WriteLine(string.Concat("Parts Requested: ",
                //    details["itemserviceresult"]["data"]));
                string jsonData = JsonPrettify(details["itemserviceresult"]["data"].ToString());
                // var datas = JObject.Parse(jsonData);
                var datas = JArray.Parse(jsonData);
                foreach (JObject data in datas)
                {
                    string partsRequested = data.GetValue("partsRequested").ToString();
                    string partsFound = data.GetValue("partsFound").ToString();
                    string partsError = data.GetValue("partsError").ToString();
                    Console.WriteLine(string.Concat("Parts Requested: ", partsRequested));
                    Console.WriteLine(string.Concat("Parts Found: ", partsFound));
                    Console.WriteLine(string.Concat("Parts Error: ", partsError));
                    PrintLine();
                    Console.WriteLine("SEARCH PARTS RESULT");
                    PrintLine();
                    PrintRow("Part No", "Manufacture", "Description");
                    
                    string resultList = JsonPrettify(data.GetValue("resultList").ToString());
                    var rsLists = JArray.Parse(resultList);
                    foreach (JObject rsList in rsLists)
                    {
                        string partList = rsList.GetValue("PartList").ToString();
                        var prLists = JArray.Parse(partList);
                        foreach (JObject prList in prLists)
                        {
                            string partNum = prList.GetValue("partNum").ToString();
                            string desc = prList.GetValue("desc").ToString();
                            string mfr = string.Concat("[",prList.GetValue("manufacturer").ToString(),"]");
                            string mfrName = "";
                            var mfrs = JArray.Parse(mfr);
                            foreach (JObject manfr in mfrs)
                            {
                                mfrName = manfr.GetValue("mfrName").ToString();
                            }
                            PrintLine();
                            PrintRow(partNum, mfrName, desc);
                        }
                    }
                    PrintLine();
                }
                sr.Close();
                Console.ReadLine();

            }

        }
        public static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }
        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }

        }
    }
}
