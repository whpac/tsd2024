using GoldSavings.App.Model;
using GoldSavings.App.Client;
using System.Xml.Linq;
namespace GoldSavings.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Saver!");

        GoldClient goldClient = new GoldClient();

        GoldPrice currentPrice = goldClient.GetCurrentGoldPrice().GetAwaiter().GetResult();
        Console.WriteLine($"The price for today is {currentPrice.Price}");

        List<GoldPrice> thisMonthPrices = goldClient.GetGoldPrices(new DateTime(2024, 03, 01), new DateTime(2024, 03, 11)).GetAwaiter().GetResult();
        foreach(var goldPrice in thisMonthPrices)
        {
            Console.WriteLine($"The price for {goldPrice.Date} is {goldPrice.Price}");
        }


        // 3.
        var lastYearPrices = goldClient.GetGoldPrices(new DateTime(2023, 1, 1), new DateTime(2023, 12, 31)).GetAwaiter().GetResult();
        var maxLastYearPrices = (from price in lastYearPrices
                                orderby price.Price descending
                                select price).Take(3);
        maxLastYearPrices = lastYearPrices.OrderByDescending(p => p.Price).Take(3);

        var minLastYearPrices = (from price in lastYearPrices
                                 orderby price.Price
                                 select price).Take(3);
        minLastYearPrices = lastYearPrices.OrderBy(p => p.Price).Take(3);

        foreach(var price in minLastYearPrices) {
            Console.WriteLine($"[min] Price for {price.Date} is {price.Price}");
        }

        foreach (var price in maxLastYearPrices)
        {
            Console.WriteLine($"[max] Price for {price.Date} is {price.Price}");
        }


        // 4.
        var jan2020prices = goldClient.GetGoldPrices(new DateTime(2020, 1, 1), new DateTime(2020, 1, 31)).GetAwaiter().GetResult();
        var over5percentGain = from price in jan2020prices
                                where currentPrice.Price / price.Price > 1.05
                                select price;
        over5percentGain = jan2020prices.Where(p => currentPrice.Price / p.Price > 1.05);
        foreach (var price in over5percentGain)
        {
            Console.WriteLine($"[>5%] Price for {price.Date} is {price.Price} (+{(int)(100 * currentPrice.Price / price.Price - 100)}%)");
        }


        // 5.
        List<GoldPrice> prices19_23 = [];
        for(int y = 2019; y <= 2023; y++)
        {
            prices19_23.AddRange(
                goldClient.GetGoldPrices(new DateTime(y, 1, 1), new DateTime(y, 12, 31)).GetAwaiter().GetResult()
            );
        }
        var prices19_22 = prices19_23.Where(p => p.Date < new DateTime(2023, 1, 1));
        var secondTen = (from price in prices19_22
                         orderby price.Price descending
                         select price).Take(13).Skip(10);
        secondTen = prices19_22.OrderByDescending(p => p.Price).Take(13).Skip(10);

        foreach (var price in secondTen)
        {
            Console.WriteLine($"[secondTen] Price for {price.Date} is {price.Price}");
        }


        // 6.
        var prices21_23 = prices19_23.Where(p => p.Date >= new DateTime(2021, 1, 1));
        var avg_prices = from price in prices21_23
                         group price by price.Date.Year into yearPrices
                         select new { Year = yearPrices.Key, Avg = yearPrices.Average(p => p.Price) };
        avg_prices = prices21_23.GroupBy(p => p.Date.Year).Select(g => new { Year = g.Key, Avg = g.Average(p => p.Price) });

        foreach (var price in avg_prices)
        {
            Console.WriteLine($"Average for {price.Year} is {price.Avg}");
        }


        // 7.
        var minPrice = (from p in prices19_23
                       orderby p.Price
                       select p).First();
        minPrice = prices19_23.OrderBy(p => p.Price).First();
        var maxPrice = (from p in prices19_23
                        orderby p.Price descending
                        select p).First();
        maxPrice = prices19_23.OrderByDescending(p => p.Price).First();

        Console.WriteLine($"Min price: {minPrice.Price} ({minPrice.Date}), max price: {maxPrice.Price} ({maxPrice.Date})");


        // 8.
        var xmlDoc = new XDocument(new XElement("GoldPrices",
                                        from p in thisMonthPrices
                                        select new XElement("GoldPrice",
                                            new XElement("Amount", p.Price),
                                            new XElement("Date", p.Date.ToString()))));
        // Alternate:
        thisMonthPrices.Select(p => new XElement("GoldPrice",
                                    new XElement("Amount", p.Price),
                                    new XElement("Date", p.Date.ToString())));

        xmlDoc.Declaration = new XDeclaration("1.0", "utf-8", "true");
        xmlDoc.Save(@"D:\prices.xml");


        // 9.
        var goldPrices = from goldPrice in XElement.Load(@"D:\prices.xml").Elements("GoldPrice")
                         select new GoldPrice()
                         {
                             Date = DateTime.Parse(goldPrice.Element("Date")?.Value ?? "0000-00-00"),
                             Price = double.Parse(goldPrice.Element("Amount")?.Value ?? "0")
                         };
        goldPrices = XElement.Load(@"D:\prices.xml").Elements("GoldPrice").Select(goldPrice => new GoldPrice()
            {
                Date = DateTime.Parse(goldPrice.Element("Date")?.Value ?? "0000-00-00"),
                Price = double.Parse(goldPrice.Element("Amount")?.Value ?? "0")
            });
    }
}
