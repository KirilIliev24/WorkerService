
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
        private List<string> metaTags = new List<string>();

        public async Task AddLink(Result result, string searchQueue)
        {
            using (var context = new SearchEngineContext())
            {
                try
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

                    metaTags = await regexCrawler.getAllMetaTags(result.Link);
                    metaTags = removeSymbols(metaTags);
                    addNoOfKeywordsFromText(result.Link, text, searchQueue);

                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                }
            }
        }

        private void addNoOfKeywordsFromText(string link, string compressedText, string searchQueue)
        {

            using (var context = new SearchEngineContext())
            {
                var linkId = context.LinkDetails.Where(p => p.Link.Equals(link)).Select(p => p.Id).FirstOrDefault();

                string unzippedText = Zipper.Decompress(compressedText);
                unzippedText = unzippedText.Replace(",", "");
                unzippedText = unzippedText.Replace(".", "");
                unzippedText = unzippedText.Replace("!", "");
                unzippedText = unzippedText.Replace("?", "");
                string[] splitText = unzippedText.Split(" ");

                string[] separateKeyword = searchQueue.Split(" ");

                for (int i = 0; i < separateKeyword.Length; i++)
                {
                    var matchQuery = from word in splitText
                                     where word.ToLowerInvariant() == separateKeyword[i].ToLowerInvariant()
                                     select word;
                    int keywordCount = matchQuery.Count();
                    
                    Console.WriteLine($"{separateKeyword[i]} count: {keywordCount}");
                   
                    int noInMetaTags = getNoOfKeywordsFromMetaTags(metaTags, separateKeyword[i]);
                    
                    context.KeywordsInText.Add(new KeywordsInMeaningfulText
                    {
                        Id = linkId,
                        keyword = separateKeyword[i],
                        keywordsInText = keywordCount,
                        keywordsInMetaTags = noInMetaTags,
                        date = DateTime.Now
                    });

                    context.SaveChanges();
                }
            }
        }

        private int getNoOfKeywordsFromMetaTags(List<string> metaTags, string keyword)
        {
            using (var context = new SearchEngineContext())
            {

                Console.WriteLine($"Number of meta tags: {metaTags.Count}");

                if (metaTags.Count == 0)
                {
                    return 0;
                }
                else
                {
                    int keywordCount = 0;
                    foreach (string m in metaTags)
                    {
                        string[] metaContent = m.Split(" ");

                        var matchQuery = from word in metaContent
                                         where word.ToLowerInvariant() == keyword.ToLowerInvariant()
                                         select word;
                        keywordCount += matchQuery.Count();

                    }
                    Console.WriteLine($"{keyword} count from meta tags: {keywordCount}\n");
                    return keywordCount;
                }
            }
        }

        private List<string> removeSymbols(List<string> metaTags)
        {
            var list = new List<string>();
            foreach (string m in metaTags)
            {
                string newString = m.Replace(",", "");
                newString = newString.Replace(".", "");
                newString = newString.Replace("!", "");
                newString = newString.Replace("?", "");
                list.Add(newString);
            }
            return list;
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
