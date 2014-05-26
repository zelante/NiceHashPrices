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

      [Option('1', "scryptPrice", Required = false, HelpText = "Set x11 price.")]
      public string scryptPrice { get; set; }
      
      [Option('2', "scryptNprice", Required = false, HelpText = "Set x11 diffcult.")]
      public string scryptNprice { get; set; }

      [Option('3', "x11price", Required = false, HelpText = "Set x11 miner exe file.")]
      public string x11price { get; set; }

      [Option('w', "wallet", Required = false, HelpText = "Set BTC wallet for payout.")]
      public string wallet { get; set; }

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
        public static int Main(string[] args)
        {
            double scryptPrice = 0;
            double scryptNprice = 0;
            double x11price = 0;
            string wallet = "1AiT1j185rZ7cokrGrszZJLWAktaFszCUR";
            double[] prices = new double[4];
            double[] accountBalance = new double[4];
            double[] rejectedSpeed = new double[4];
            double[] acceptedSpeed = new double[4];

            ///Parse Command Line
            Options options = new Options();
            CommandLine.Parser parser = new Parser();
            if (parser.ParseArguments(args, options))
            {
                // consume Options type properties
                double.TryParse(options.scryptPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out scryptPrice);
                double.TryParse(options.scryptNprice, NumberStyles.Float, CultureInfo.InvariantCulture, out scryptNprice);
                double.TryParse(options.x11price, NumberStyles.Float, CultureInfo.InvariantCulture, out x11price);
                if (options.wallet != null) wallet = options.wallet;
            }

            var url = "https://www.nicehash.com/api?method=stats.provider&addr=" + wallet;
            var nicehashAccount = _download_serialized_json_data<RootObjectAccount>(url);

            url = "http://www.nicehash.com/api?method=stats.global.current";
            var currencyRates = _download_serialized_json_data<RootObject>(url);
            
            //  Algorithms are marked with following numbers:
            //      0 = Scrypt
            //      1 = SHA256
            //      2 = Scrypt-A.-Nfactor
            //      3 = X11


            foreach (Stat stat in currencyRates.result.stats)
                prices[stat.algo] = double.Parse(currencyRates.result.stats[stat.algo].price, CultureInfo.InvariantCulture);
            
            foreach (StatAccount stat in nicehashAccount.result.stats)
            {
                accountBalance[stat.algo] = double.Parse(stat.balance, CultureInfo.InvariantCulture);
                rejectedSpeed[stat.algo] = double.Parse(stat.rejected_speed, CultureInfo.InvariantCulture);
                acceptedSpeed[stat.algo] = double.Parse(stat.accepted_speed, CultureInfo.InvariantCulture);
            }

            string datetime = DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss]");
            Console.Write("{0} Currently Paying BTC/GH/Day ", datetime);
            if (scryptPrice != 0)
                if (scryptPrice < prices[0])
                {
                    Console.WriteLine("(Scrypt): {0} ", prices[0]);
                    Console.WriteLine("{0} Accepted Speed GH/s: {1}", datetime, acceptedSpeed[0]);
                    Console.WriteLine("{0} Rejected Speed GH/s: {1}", datetime, rejectedSpeed[0]);
                    Console.WriteLine("{0} Unpaid Balance BTC (Scrypt): {1}", datetime, accountBalance[0].ToString("N8"));
                    return 3;
                }
            if (scryptNprice != 0)
                if (scryptNprice < prices[2])
                {
                    Console.WriteLine("(ScryptN): {0} ", prices[2]);
                    Console.WriteLine("{0} Accepted Speed GH/s: {1}", datetime, acceptedSpeed[2]);
                    Console.WriteLine("{0} Rejected Speed GH/s: {1}", datetime, rejectedSpeed[2]);
                    Console.WriteLine("{0} Unpaid Balance BTC (ScryptN): {1}", datetime, accountBalance[2].ToString("N8"));
                    return 3;
                }
            if (x11price != 0)
                if (x11price < prices[3])
                {
                    Console.WriteLine("(x11): {0} ", prices[3]);
                    Console.WriteLine("{0} Accepted Speed GH/s: {1}", datetime, acceptedSpeed[3]);
                    Console.WriteLine("{0} Rejected Speed GH/s: {1}", datetime, rejectedSpeed[3]);
                    Console.WriteLine("{0} Unpaid Balance BTC (x11): {1}", datetime, accountBalance[3].ToString("N8"));
                    return 3;
                }
            Console.WriteLine("Scrypt {0} ScryptN {1} x11 {2}", prices[0], prices[2], prices[3]);
                
            return 0;
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
                catch (Exception) { }
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
