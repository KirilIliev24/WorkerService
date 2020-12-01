using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TestWorkService.DataBase;
using TestWorkService.Model;

namespace TestWorkService
{
    class CustomGoogleSearch
    {
        private LinkPositionDataAccess linkPosition = new LinkPositionDataAccess();


        public async Task GetWebSearchResul(string searchQueue)
        {
            string cx = "d64fde5f3edd155bf";
            string apiKey = "AIzaSyDRHs-hDvdcdvwZsbO_n3Iwlnk2oJM94pc";

            int[] positions = new int[] {1, 11, 21};

            string[] splitQueue = searchQueue.Split(' ');

            List<string> finishedWords = new List<string>();
            string[] allPermutations = GetAllPermutations(splitQueue, 0, splitQueue.Length - 1, finishedWords);

            //HashSet<Result> uniqueResults = new HashSet<Result>(new ResultComparer());


            for (int i = 0; i < allPermutations.Length; i++)
            {
                int index = 0;

                for (int j = 0; j < 3; j++)
                {
                   
                    try
                    {
                        dynamic jsonData = "";
                        int pos = positions[j];
                        await Task.Run(() => {
                            var request = WebRequest.Create($"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={cx}&q={allPermutations[i]}&start={pos}&num=10");
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            Stream dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseString = reader.ReadToEnd();
                            jsonData = JsonConvert.DeserializeObject(responseString);
                        });
                       

                        foreach (var item in jsonData.items)
                        {
                            string link = item.link;

                            await linkPosition.AddLink(new Result
                            {
                                Title = item.title,
                                Link = item.link,
                                Snippet = item.snippet,
                                Index = index
                            }, searchQueue);//adds position and date also

                            index++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.InnerException);
                    }

                }
                Console.WriteLine($"Number of results: {index + 1}");

            }
        }

        private string[] GetAllPermutations(string[] words, int first, int last, List<string> finishedWords)
        {
            if (first == last)
            {
                finishedWords.Add(JoinWords(words));
            }
            else
            {
                for (int i = first; i <= last; i++)
                {
                    Swap(ref words[first], ref words[i]);
                    GetAllPermutations(words, first + 1, last, finishedWords);
                    Swap(ref words[first], ref words[i]);
                }
            }
            return finishedWords.ToArray();

        }

        static void Swap(ref string a, ref string b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private string[] SwapWords(string[] words, int i, int j)
        {
            string tempWord;

            tempWord = words[i];
            words[i] = words[j];
            words[j] = tempWord;
            return words;
        }

        private string JoinWords(string[] words)
        {
            string final = "";
            for (int i = 0; i < words.Length; i++)
            {
                final += words[i] + " ";
            }
            return final;
        }

    }
}
