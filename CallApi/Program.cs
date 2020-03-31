using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CallApi
{
    class Program
    {
        static int tableWidth = 73;
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
                // Console.WriteLine(sr.ReadToEnd()); // Raw Result
                Console.Clear();
                PrintLine();
                PrintRow("Part No", "Manufacture", "Description", "Sources");
                PrintLine();
                PrintRow("", "", "", "");
                PrintRow("", "", "", "");
                PrintLine();
                Console.ReadLine();
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
