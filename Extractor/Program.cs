using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.IO;
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

            var table = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='row_titles']");
            var teste = table.SelectNodes("ul|h3");

            foreach (var item in teste)
            {
                if(item.Name == "ul")
                {
                    var parent = item.PreviousSibling;
                }
            }
            var dsad = "";







        }
      
    }
}
