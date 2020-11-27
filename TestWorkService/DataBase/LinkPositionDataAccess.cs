
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestWorkService.DataBase.Model;
using TestWorkService.Model;

namespace TestWorkService.DataBase
{
    public class LinkPositionDataAccess
    {
        private RegexCrawler regexCrawler = new RegexCrawler();
        private WebClient webClient = new WebClient();
        public async Task AddLink(Result result, string searchQueue)
        {
            using (var context = new SearchEngineContext())
            {
                var getResult = context.LinkTracker.Where(r => r.Keywords.Equals(searchQueue) && r.Link.Equals(result.Link)).ToList();
                List<string> externalLinks = await regexCrawler.CrawlLinks(result.Link);
                if (getResult.Count == 0)
                {
                    context.LinkTracker.Add(new LinkPositionTracker
                    {
                        Keywords = searchQueue,
                        Link = result.Link
                    });
                    context.LinkDetails.Add(new LinkDetails()
                    {
                        Link = result.Link,
                        Title = result.Title,
                        Snippet = result.Snippet,
                        Index = result.Index
                    });
                }

                var linkId = context.LinkDetails.Where(p => p.Link.Equals(result.Link)).Select(p => p.Id).FirstOrDefault();

                foreach (string link in externalLinks)
                {
                    context.ExternalLinks.Add(new ExternalLinks
                    {
                        Id = linkId,
                        date = DateTime.Now,
                        externalLink = link
                    });
                }

                string webString = webClient.DownloadString(result.Link);
                int wordCount = 0;
                double jsPercent = 0;
                double cssPercent = 0;
                string text = "";

                regexCrawler.getLinkStats(webString, ref cssPercent, ref jsPercent, ref wordCount, ref text);
                context.PositonAndDates.Add(new PositonAndDate
                {
                    Keywords = searchQueue,
                    Link = result.Link,
                    Position = result.Index + 1,
                    WordCount = wordCount,
                    Css = cssPercent,
                    Js = jsPercent,
                    MeaningfulText = text,
                    Date = DateTime.Now
                });
               
                context.SaveChanges();
            }
        }



        public string[] getAllKeywords()
        {
            using (var context = new SearchEngineContext())
            {
                var result = context.Keywords.Select(r => r.Keyword).ToArray();
                return result;
            }
        }

        public void CloseConnection()
        {
            using (var context = new SearchEngineContext())
            {
                context.Database.CloseConnection();
            }
        }
    }
}
