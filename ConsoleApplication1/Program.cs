using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Diagnostics;
using CommandLine;
using CommandLine.Text;

namespace NiceHashPrices
{
    public class Options
    {

      [Option('1', "scryptPrice", Required = false, HelpText = "Set x11 price.")]
      public float scryptPrice { get; set; }
      
      [Option('2', "scryptNprice", Required = false, HelpText = "Set x11 diffcult.")]
      public float scryptNprice { get; set; }

      [Option('3', "x11price", Required = false, HelpText = "Set x11 miner exe file.")]
      public float x11price { get; set; }

      [HelpOption]
      public string GetUsage()
      {
        // this without using CommandLine.Text
        var usage = new StringBuilder();
        usage.AppendLine("Quickstart Application 1.0");
        usage.AppendLine("Read user manual for usage instructions...");
        return usage.ToString();
      }
    }
    public static class Program
    {
        public static int Main(string[] args)
        {
            float scryptPrice = 0F;
            float scryptNprice = 0F;
            float x11price = 0F;
            float priceX11NH = 0F;
            float priceScryptNH = 0F;
            float priceScryptNNH = 0F;

            ///Parse Command Line
            Options options = new Options();
            CommandLine.Parser parser = new Parser();
            if (parser.ParseArguments(args, options))
            {
            // consume Options type properties
                scryptPrice = options.scryptPrice;
                scryptNprice = options.scryptNprice;
                x11price = options.x11price;
            }

            string url = "http://www.nicehash.com";
            string result = null;

                try
                {
                    Console.Write("Trying to get prices from {0} ... ", url);
                    WebClient client = new WebClient();
                    result = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error!");
                    return 1;
                }

                finally
                {
                    if (result != null)
                    {
                        Console.WriteLine("Ok!");
                    }
                }


                int first = result.CustomIndexOf("<td>", 1);
                int last = result.CustomIndexOf("</td>", 3);

                string str2 = result.Substring(first + 4, last - first - 4);
                priceScryptNH = Convert.ToSingle(str2.Replace(".", ","));
                //Console.Write("Scrypt: {0} ", priceScryptNH);

                first = result.CustomIndexOf("<td>", 5);
                last = result.CustomIndexOf("</td>", 13);
            
                str2 = result.Substring(first + 4, last - first - 4);
                priceScryptNNH = Convert.ToSingle(str2.Replace(".", ","));
                //Console.Write("ScryptN: {0} ", priceScryptNNH);

            
                first = result.CustomIndexOf("<td>", 7);
                last = result.CustomIndexOf("</td>", 18);

                str2 = result.Substring(first + 4, last - first - 4);
                priceX11NH = Convert.ToSingle(str2.Replace(".", ","));
                //Console.Write("x11: {0}", priceX11NH);

                if (scryptPrice != 0)
                    if (scryptPrice < priceScryptNH)
                        return 3;
                if (scryptNprice != 0)
                    if (scryptNprice < priceScryptNNH)
                        return 3;
                if (x11price != 0)
                    if (x11price < priceX11NH)
                        return 3;
                return 0;
        }
        
        public static int CustomIndexOf(this string source, string toFind, int position)
        {
            int index = -1;
            for (int i = 0; i < position; i++)
            {
                index = source.IndexOf(toFind, index + 1);

                if (index == -1)
                    break;
            }

            return index;
        }
    }
}
