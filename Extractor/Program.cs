using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Extractor
{
    class Program
    {

        const string SRC_URL = "https://www.tudocelular.com/compare/6443-5715.html";
        static async Task Main(string[] args)
        {

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            // Create a new page and go to Bing Maps
            Page page = await browser.NewPageAsync();
            await page.GoToAsync(SRC_URL);
            await page.WaitForTimeoutAsync(2000);
            var html = await page.EvaluateExpressionAsync("document.documentElement.outerHTML");
            await browser.CloseAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html.ToString());

            var lines = new List<string> { };



            var titles = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='row_titles']");
            var titles2 = titles.SelectNodes("ul|h3");

            var header = "Id; Nome;";
            foreach (var item in titles2)
            {
                if(item.Name == "ul")
                {

                    var featureNames = item.SelectNodes("li");
                    foreach (var featureName in featureNames)
                    {
                        if (featureName.InnerText.Contains("- "))
                        {
                            header += featureName.InnerText.Replace("- ", "").Trim();
                        }
                        else 
                        {
                            header += featureName.InnerText.Trim();
                        }

                        header += ";";
                    }
                }
            }
            header = header.Substring(0, header.Length - 1);
            header += "\n";
            lines.Add(header);
            var phone_columns = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='phone_columns']").SelectNodes("div[@class='phone_column']");
            foreach (var phone_column in phone_columns)
            {
                var column_data = phone_column.GetAttributeValue("id", "none") +";";
                var cel_name = phone_column.SelectSingleNode("div[@class='big_phone']").SelectSingleNode("h2").InnerText.Trim() + ";";
                column_data += cel_name != null ? cel_name : "none";
                var featureULs = phone_column.SelectNodes("ul");

                foreach (var featureUL in featureULs)
                {
                    var featureLIs = featureUL.SelectNodes("li");
                    foreach (var featureLI in featureLIs)
                    {
                        var icon = "";

                        if (featureLI.InnerHtml.Contains("ok")) icon = "SIM";
                        if (featureLI.InnerHtml.Contains("wrong")) icon = "NAO";

                        var featureName = featureLI.InnerText.Trim();
                        featureName = featureName != null ? featureName : "none";
                        featureName = featureName != "none" && icon != "" ? icon + " - " + featureName : featureName;
                        column_data += ";";

                    }
                }
                column_data = column_data.Substring(0, column_data.Length - 1);
                column_data += "\n";
                lines.Add(column_data);

            }

            File.WriteAllText("C:\\Users\\joaoa\\Documents\\out2.csv", string.Join("",lines));
        }
        


    }
}
