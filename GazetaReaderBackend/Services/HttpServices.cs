using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using GazetaReaderFrontend.Model;
using System.IO;
using System.Text.RegularExpressions;
using GazetaReaderFrontend.Services;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GazetaReaderBackend.Services
{
    class HttpServices
    {
        
        /// <summary>
        /// Отправляет асинхронно HTTP запрос GET и возвращает строковый ответ
        /// </summary>
        /// <param name="httpUri">Адрес запрашиваемой страницы</param>
        /// <returns>Ответ страницы в виде строки</returns>
        public static async Task<string> GetHttpResponseAsync(string httpUri)
        {
            var http = new HttpClient();

            string result = null;

            try
            {
                var response = await http.GetAsync(new Uri(httpUri));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("От сервера получен ответ " + response.StatusCode);
                    return null;
                }
                    
                result = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);

            }

            return result;


        }
       
        /// <summary>
        /// Возвращает адрес "главного" изображения на странице
        /// </summary>
        /// <param name="htmlString">HTML-страница новости</param>
        /// <returns>Строка с адресом изображения</returns>
        public static string ParseImage(string htmlString)
        {
            var html = new HtmlDocument();

            if ((htmlString == null)||(htmlString=="")) return null;

            html.LoadHtml(htmlString);

            var headNode = html.DocumentNode.ChildNodes.FindFirst("head");

                // Ссылка на "главное" изображение страницы лежит в head в теге link с атрибутом image_src
                var imageSrc = headNode.ChildNodes.Where(x => x.Name == "link").ToArray()
                    .FirstOrDefault(x => x.Attributes["rel"].Value == "image_src");

                return imageSrc != null ? imageSrc.Attributes["href"].Value : null;
        }

        /// <summary>
        /// Возвращает основной текст новости
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static string ParseNewsBody(string htmlString)
        {
            List<HtmlNode> paragraphs = new List<HtmlNode>();
            var newsText = String.Empty;
            var html = new HtmlDocument();
            html.LoadHtml(htmlString);

            // Чаще всего текст новости лежит здесь (div с id "article_body")
            var articleFullText = html.GetElementbyId("article_body");
            
            if (articleFullText == null)
            {
                // Но иногда он в теге "main"
                var mainNode = html.DocumentNode.Descendants("main").ToList();
                if (mainNode.Count != 0)
                    articleFullText = mainNode[0];
            }

            // Если все таки не удалось найти основной текст
            if (articleFullText == null)
                return "";

            paragraphs = articleFullText.Descendants("p").ToList();
            

            foreach (var paragraph in paragraphs)
            {
                using (var sw = new StringWriter())
                {
                    // Используется библиотеку, которая извлекает текст из html-тегов
                    HtmlToText.ConvertTo(paragraph, sw);

                    sw.Flush();
                    newsText += sw.ToString();
                }
                
            }
                   
            // Добавляем дополнительные пустые строки после абзацев для большей читаемости.
            newsText = Regex.Replace(newsText, "\n", "\n\n");

            // Убираем лишние пустые строки.
            newsText = Regex.Replace(newsText, "\n\n\n\n", "\n\n");

            return newsText.Trim();

        }

        /// <summary>
        /// Метод, сериализующий данные и записывающий их в файл.
        /// </summary>
        /// <param name="categories">Коллекция категорий, готовая для отправки "клиенту"</param>
        public static void PutXmlAsync(ObservableCollection<NewsCategory> categories)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<NewsCategory>));

            using (var stream = File.Create("data.xml"))
            {
                serializer.WriteObject(stream, categories);
            }


        }

        /// <summary>
        /// метод, вычитывающий данные из файла и десериализующий их
        /// </summary>
        public static ObservableCollection<NewsCategory> ReadXmlAsync()
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<NewsCategory>));

            if(File.Exists("data.xml"))
                try
                {
                    using (var stream = File.Open("data.xml", FileMode.Open))
                    {
                        return (ObservableCollection<NewsCategory>)serializer.ReadObject(stream);
                    }
                }
                catch (Exception)
                {
                    
                    return null;
                }
            
            return null;



        }

      

    }

}
