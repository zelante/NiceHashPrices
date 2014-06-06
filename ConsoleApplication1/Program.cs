using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Globalization;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;


namespace NiceHashPrices
{
    public class Options
    {

      [Option('w', "wallet", Required = false, HelpText = "Set BTC wallet for payout.")]
      public string wallet { get; set; }

      [HelpOption]
      public string GetUsage()
      {
          // this without using CommandLine.Text
          var usage = new StringBuilder();
          usage.AppendLine("NiceHashPrices v 0.3 by Oleksandr Sukhina (zelante)");
          usage.AppendLine("----------------------------------------");
          usage.AppendLine("Usage: NiceHashPrices.exe [Options]");
          usage.AppendLine("Options:");
          usage.AppendLine("\t -w, --wallet \t Your BTC Wallet address for NiceHash payout");
          return usage.ToString();
      }
    }

    public class Stat
    {
        public string price { get; set; }
        public int algo { get; set; }
        public string speed { get; set; }
    }

    public class Result
    {
        public List<Stat> stats { get; set; }
    }

    public class RootObject
    {
        public Result result { get; set; }
        public string method { get; set; }
    }

    public class StatAccount
    {
        public string balance { get; set; }
        public int algo { get; set; }
        public string rejected_speed { get; set; }
        public string accepted_speed { get; set; }
    }

    public class ResultAccount
    {
        public List<StatAccount> stats { get; set; }
        public string addr { get; set; }
    }

    public class RootObjectAccount
    {
        public ResultAccount result { get; set; }
        public string method { get; set; }
    }

    public static class Program
    {
        static string datetime;
        static double[] prices = new double[6];
        static double[] normilizedPrices = new double[6];
        static double[] accountBalance = new double[6];
        static double[] rejectedSpeed = new double[6];
        static double[] acceptedSpeed = new double[6];

        public static int Main(string[] args)
        {
            string wallet=" ";
            datetime = DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss]");
            
            ///Parse Command Line
            Options options = new Options();
            CommandLine.Parser parser = new Parser();
            if (parser.ParseArguments(args, options))
            {
                // consume Options type properties
                if (options.wallet != null)
                    wallet = options.wallet;
                else
                {
                    Console.WriteLine(options.GetUsage());
                    return 1;
                }
            }

            var url = "http://www.nicehash.com/api?method=stats.provider&addr=" + wallet;
            var nicehashAccount = _download_serialized_json_data<RootObjectAccount>(url);
            if (nicehashAccount == null) System.Environment.Exit(1);
            url = "http://www.nicehash.com/api?method=stats.global.current";
            var currencyRates = _download_serialized_json_data<RootObject>(url);
            if (currencyRates == null) System.Environment.Exit(1);
  

            //  Algorithms are marked with following numbers:
            //      0 = Scrypt
            //      1 = SHA256
            //      2 = Scrypt-A.-Nfactor
            //      3 = X11
            //      4 = X13
            //      5 = Keccak

            //  Algorithm	Profitability	Auto-Switching port
            //  Scrypt	refference  x1   BTC/GH/Day   profitability	4333
            //  Scrypt-N	        x0.5 BTC/GH/Day   profitability	4335
            //  X11	                x4   BTC/GH/Day   profitability	4336
            //  X13	                x3   BTC/GH/Day   profitability	4337
            //  Keccak	            x500 BTC/GH/Day   profitability	4338

            foreach (Stat stat in currencyRates.result.stats)
            {
                prices[stat.algo] = double.Parse(currencyRates.result.stats[stat.algo].price, CultureInfo.InvariantCulture);
                
                switch (stat.algo)
                {
                    case 0:
                        normilizedPrices[stat.algo] = prices[stat.algo];
                        break;
                    case 2:
                        normilizedPrices[stat.algo] = prices[stat.algo]*0.5;
                        break;
                    case 3:
                        normilizedPrices[stat.algo] = prices[stat.algo]*4;
                        break;
                    case 4:
                        normilizedPrices[stat.algo] = prices[stat.algo]*3;
                        break;
                    case 5:
                        normilizedPrices[stat.algo] = prices[stat.algo]*500;
                        break;
                }
              
            }
            
            foreach (StatAccount stat in nicehashAccount.result.stats)
            {
                accountBalance[stat.algo] = double.Parse(stat.balance, CultureInfo.InvariantCulture);
                rejectedSpeed[stat.algo] = double.Parse(stat.rejected_speed, CultureInfo.InvariantCulture);
                acceptedSpeed[stat.algo] = double.Parse(stat.accepted_speed, CultureInfo.InvariantCulture);
            }

            switch (getNormilizedMax(normilizedPrices))
            {
                case 0:
                    return algoInfo(0);
                case 2:
                    return algoInfo(2);
                case 3:
                    return algoInfo(3);
                case 4:
                    return algoInfo(4);
                case 5:
                    return algoInfo(5);
            }
              
            return 0;
        }

        static int getNormilizedMax(double[] array) 
        {
            double max = array[0];
            int index = 0;
            for (int i = 0; i < array.Length; i++)
                if (max < array[i])
                {
                    max = array[i];
                    index = i;
                }
            return index;  
        }

        private static int algoInfo(int indexAlgo)
        {
            string[] algo = {"Scrypt", "SHA256" ,"ScryptN", "x11", "x13", "Keccak"};

            Console.WriteLine("Normalized prices {0}:{1} {2}:{3} {4}:{5} {6}:{7} {8}:{9}",
                algo[0], normilizedPrices[0],
                algo[2], normilizedPrices[2],
                algo[3], normilizedPrices[3],
                algo[4], normilizedPrices[4],
                algo[5], normilizedPrices[5]);

            Console.WriteLine("----------------------[ {0} ]---------------------------------------------------", algo[indexAlgo]);
            Console.WriteLine("{0} Currently Paying BTC/GH/Day: {1}", datetime, prices[indexAlgo]);
            Console.WriteLine("{0} Accepted Speed GH/s: {1}", datetime, acceptedSpeed[indexAlgo]);
            Console.WriteLine("{0} Rejected Speed GH/s: {1}", datetime, rejectedSpeed[indexAlgo]);
            Console.WriteLine("{0} Unpaid Balance BTC: {1}", datetime, accountBalance[indexAlgo].ToString("N8"));
            return indexAlgo;
        }
        
        private static T _download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception)
                {
                    Console.WriteLine("{0} Error: Can't get API from nicehash!", datetime);
                    System.Environment.Exit(1);
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
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
