using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestWorkService
{
    class RegexCrawler
    {
        //maybe pass the full lenght as well if % should be returned
        public int getLinkStats(string webString, ref double cssPercent, ref double jsPercent, ref int wordCount, ref string text)
        {

            if (webString.Length == 0 || webString.Equals(string.Empty))
            {
                cssPercent = -1;
                jsPercent = -1;
                wordCount = -1;
                return -1;
            }
            double mbStart = ((double)Encoding.Unicode.GetByteCount(webString) / 1048576);
            int websiteLenght = webString.Length;
            Console.WriteLine($"Lenght of website: {websiteLenght}");

            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

            string[] array = new string[0];

            //relace css
            webString = Regex.Replace(webString, "<\\s*style[^>]*>.*?<\\s*/\\s*style\\s*>", "", options);
            webString = Regex.Replace(webString, "\\}", "", RegexOptions.IgnoreCase);
            double afterCss = ((double)Encoding.Unicode.GetByteCount(webString) / 1048576);
            
            double justCSS = mbStart - afterCss;//good
            cssPercent = ((justCSS) * 100.0) / mbStart;
            
            //replace js
            webString = Regex.Replace(webString, "<\\s*script[^>]*>.*?<\\s*/\\s*script\\s*>", "", options);
            double afterJs = ((double)Encoding.Unicode.GetByteCount(webString) / 1048576);
            double justJsRemoved = (mbStart - afterJs) - justCSS;//good
            jsPercent = (justJsRemoved) * 100.0 / mbStart;
            
            //replace html tags
            webString = Regex.Replace(webString, "(<.*?>\\s*)+", " ").Trim();

            array = webString.Split(" ");
            wordCount = array.Length;

            text = Zipper.Compress(webString);         
            return 0;
           
        }

        public async Task<List<string>> CrawlLinks(string website)
        {
            HashSet<string> hashSet = new HashSet<string>();
            try
            {
                var client = new HttpClient();
                var websiteString = await client.GetStringAsync(website);

                RegexOptions options = RegexOptions.IgnoreCase;
                MatchCollection colection = await Task.Run(() => Regex.Matches(websiteString, "(http|ftp|https)://([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?", options));

                for (int i = 0; i < colection.Count; i++)
                {
                    string linkUrl = colection[i].Groups[2].Value;
                    hashSet.Add(linkUrl);
                }
                if (hashSet.Count == 0)
                {
                    hashSet.Add("List was empty");
                }
                return hashSet.ToList();

            }
            catch (Exception e)
            {

                if (e is ArgumentNullException)
                    Console.WriteLine(e.Message);
            }
            hashSet.Add("Link could NOT be crawled");
            return hashSet.ToList();
        }

        public async Task<List<string>> getAllMetaTags(string website)
        {
            var client = new HttpClient();
            var websiteString = await client.GetStringAsync(website);
            List<string> metaTags = new List<string>();

            MatchCollection colection = await Task.Run(() => Regex.Matches(websiteString, "<meta .* content=\"(.*)\".*\\/>"));

            for (int i = 0; i < colection.Count; i++)
            {
                string linkUrl = colection[i].Groups[1].Value;
                metaTags.Add(linkUrl);
            }
            if (metaTags.Count == 0)
            {
                metaTags.Add("List was empty");
            }
            return metaTags;
        }
    }
}
