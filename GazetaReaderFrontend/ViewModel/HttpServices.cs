using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace GazetaRuReaderBackend
{
    class HttpServices
    {
        #region GetHttpResponseAsync
        /// <summary>
        /// Возвращает ответ от Http страницы в виде string  по ее адресу
        /// </summary>
        public static async Task<string> GetHttpResponseAsync(string httpUri)
        {
            var http = new HttpClient();
            var result = String.Empty;

            try
            {
                var response = await http.GetAsync(new Uri(httpUri));
                result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch 
            {
                return null;
            }
            
            
        }
        public static async Task<string> GetHttpResponseAsync(Uri httpUri)
        {
            var http = new HttpClient();
            var result = String.Empty;
            try
            {
                var response = await http.GetAsync(httpUri);
                result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch 
            {
                return null;
            }
            
            
            
        }
        #endregion

        public static async Task<List<Article>> ParseHtml(string htmlString)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlString);
           
            //Здесь находятся все новости
            var auho1 = html.GetElementbyId("auho1");
            //список всех новостей
            var articles=auho1.SelectNodes("//article").ToArray().Select(arr => arr.ChildNodes.FindFirst("div").ChildNodes.FindFirst("div")).ToArray();

           var articlesList = new List<Article>();

            
            foreach (var article in articles)
            {

                articlesList.Add(new Article
                {
                    header = 
                        article.ChildNodes.Where(x=>x.Name=="div").ToArray().
                            First(z=>z.Attributes["class"].Value== "ear-textblock pt5 add-info").
                                ChildNodes.FindFirst("h2").InnerText.Trim(),
                    description = 
                        article.ChildNodes.Where(x=>x.Name=="div").ToArray().
                            Last(z => z.Attributes["class"].Value == "ear-textblock pt5 add-info").
                            ChildNodes.Last(y => y.Name=="div").
                                ChildNodes.FindFirst("a").InnerText.Trim(),
                    pubDate=DateTime.Parse(
                        article.ChildNodes.Where(x => x.Name == "div").ToArray().
                            First(z => z.Attributes["class"].Value == "ear-textblock pt5 add-info").
                            ChildNodes.FindFirst("time").Attributes["datetime"].Value),
                    mainLink =
                        new Uri(new Uri("https://www.gazeta.ru"),
                            article.ChildNodes.Where(x => x.Name == "link").ToArray().
                                First(z => z.Attributes["itemprop"].Value == "mainEntityOfPage url")
                                .GetAttributeValue("href", "")),
                    imgLink =
                        new Uri(new Uri("https://www.gazeta.ru"),
                            article.ChildNodes.Where(x=>x.Name=="div").ToArray().
                                First(z=>z.Attributes["class"].Value=="ear-image mb10").
                                    ChildNodes.FindFirst("img").GetAttributeValue("src",""))

                });


            }

           
            return articlesList;
        }
    }
}
