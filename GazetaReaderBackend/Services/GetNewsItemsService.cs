using GazetaReaderFrontend.Model;
using GazetaReaderBackend.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace GazetaReaderFrontend.Services
{
    public static class GetNewsItemsService
    {
        /// <summary>
        /// К уже полученной коллекции категорий и новостей, добавляем недостающие поля (изображения, основной текст новости и "связанные" новости"
        /// </summary>
        public static async Task<ObservableCollection<NewsCategory>> GetNewsItemsAsync(ObservableCollection<NewsCategory> categories)
        {
            // Восстанавливаем предыдущее состояние.
            var oldNewsCollection = HttpServices.ReadXmlAsync();
            NewsCategory existedCategory=null;
            NewsCategory.NewsItem existedItem = null;
           
            
            foreach (var category in categories)
            {
                // Пытаемся восстановить новости из предыдущего состояния, если такая новость уже была в нашем списке,
                // чтобы уменьшить количество запросов к внешнему ресурсу.
                if (oldNewsCollection != null)
                {
                    existedCategory = oldNewsCollection.FirstOrDefault(x => x.CategoryName == category.CategoryName);
                }
                foreach (var newsItem in category.Items)
                {
                    if (existedCategory != null)
                    {
                        existedItem = existedCategory.Items.FirstOrDefault(x => x.NewsTitle == newsItem.NewsTitle);
                    }
                    if (existedItem != null)
                    {
                        newsItem.NewsImagePath = existedItem.NewsImagePath;
                        newsItem.NewsArticle = existedItem.NewsArticle;
                        newsItem.RelatedNewsItems = existedItem.RelatedNewsItems;
                    }
                    // Если новость "новая"
                    else
                    {
                        var response = await HttpServices.GetHttpResponseAsync(newsItem.NewsLink);
                        newsItem.NewsImagePath = HttpServices.ParseImage(response);
                        newsItem.NewsArticle = HttpServices.ParseNewsBody(response);
                        newsItem.RelatedNewsItems = await GetRelatedNews(response);
                    }
                    
                }
            }
            return categories;
        }

        /// <summary>
        /// Ищет на странице "связанные" новости и возвращает их коллекцию, либо пустую, если не найдены
        /// </summary>
        public static async Task<ObservableCollection<NewsCategory.NewsItem>> GetRelatedNews(string htmlString)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlString);
            var items = new ObservableCollection<NewsCategory.NewsItem>();

            var articleFullText = html.GetElementbyId("article_body");
            
            //TODO: некоторые статьи в разделе "спорт" выглядят по другому, поэтому для нахождения в них "связанных" новостей, нужно немного другое решение.
            if (articleFullText == null)
            {
               
                    Console.WriteLine("тратата");
                    return items;
                
            }
            

            var relatedNews = articleFullText.Descendants("article");
            try
            {
                foreach (var item in relatedNews)
                {
                    string description;
                    string title;

                    // Еще немного магии парсинга.
                    var link = "https://gazeta.ru" + item.Element("a").GetAttributeValue("href", "");

                    var imgLink = "https:" + item.Element("a").Element("img").GetAttributeValue("src", "").Trim();

                    using (var sw = new StringWriter())
                    {
                        HtmlToText.ConvertTo(item.Element("p").Element("a"), sw);
                        sw.Flush();
                        description = sw.ToString();
                    }

                    using (var sw = new StringWriter())
                    {
                        HtmlToText.ConvertTo(item.Element("h3").Element("a"), sw);
                        sw.Flush();
                        title = sw.ToString();
                    }
                   
                    var article = HttpServices.ParseNewsBody(await HttpServices.GetHttpResponseAsync(link));

                    items.Add(new NewsCategory.NewsItem
                    {
                        NewsArticle = article,
                        NewsTitle = title.Trim(),
                        NewsPubDate = DateTime.Now,
                        NewsDescription = description.Trim(),
                        NewsImagePath = imgLink,
                        NewsLink = link
                    });
                }

            }
            catch (Exception)
            {
                Console.WriteLine("что-то пошло не так");
                return items;
            }
           
            return items;

        }
    }
}
