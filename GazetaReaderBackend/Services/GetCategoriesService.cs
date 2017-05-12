using GazetaReaderFrontend.Model;
using GazetaReaderBackend.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace GazetaReaderFrontend.Services
{
    public static class GetCategoriesService
    {
        public static async Task<ObservableCollection<NewsCategory>> GetCategoriesAsync()
        {
            // Получаем список адресов рсс-каналов.
            var uriList = GetRssUriList();

            // Получаем список объектов класса Rss, по сути, рсс-ленты.
            List<Rss> rssList = await GetRssListAsync(uriList);

            // Заполняем данными категории и новости
            var categories = await GetCategoriesAsync(rssList);
           
            return categories;
        }

        /// <summary>
        /// Метод возвращает список URI-адресов RSS-каналов с которыми предстоит работать.
        /// </summary>
        /// <returns>Cписок URI-адресов</returns>
        private static List<Uri> GetRssUriList()
        {
            return new List<Uri>
            {
                new Uri("https://www.gazeta.ru/export/rss/first.xml"),
                new Uri("https://www.gazeta.ru/export/rss/politics.xml"),
                new Uri("https://www.gazeta.ru/export/rss/business.xml"),
                new Uri("https://www.gazeta.ru/export/rss/social.xml"),
                new Uri("https://www.gazeta.ru/export/rss/lifestyle.xml"),
                new Uri("https://www.gazeta.ru/export/rss/tech_articles.xml"),
                new Uri("https://www.gazeta.ru/export/rss/realty.xml"),
                new Uri("https://www.gazeta.ru/export/rss/comments.xml"),
                new Uri("https://www.gazeta.ru/export/rss/culture.xml"),
                new Uri("https://www.gazeta.ru/export/rss/science.xml"),
                new Uri("https://www.gazeta.ru/export/rss/sport.xml"),
                new Uri("https://www.gazeta.ru/export/rss/auto.xml"),
                new Uri("https://www.gazeta.ru/export/rss/kolonka.xml")
            };
        }

        /// <summary>
        /// По списку uri адресов всех рсс-лент получаем все Xml файлы, и сериализуем их, 
        /// возвращаем список элементов класса Rss.
        /// </summary>
        /// <param name="uriList">Cписок URI-адресов RSS-каналов</param>
        /// <returns></returns>
        private static async Task<List<Rss>> GetRssListAsync(List<Uri> uriList)
        {
            var rssList = new List<Rss>();

            var xml = new XmlSerializer(typeof (Rss));

            foreach (var uri in uriList)
            {
                var result = await HttpServices.GetHttpResponseAsync(uri.ToString());

                var memoryStream = new MemoryStream(Encoding.GetEncoding("windows-1251").GetBytes(result));

                var rss = (Rss) xml.Deserialize(memoryStream);

                rssList.Add(rss);
            }

            return rssList;
        }

        /// <summary>
        /// Форматирует данные для получения итогового списка категорий и новостей
        /// </summary>
        /// <param name="rssList"></param>
        /// <returns></returns>
        private static async Task<ObservableCollection<NewsCategory>> GetCategoriesAsync(List<Rss> rssList)
        {
            var categories = new ObservableCollection<NewsCategory>();

            foreach (var rss in rssList)
            {
                var category = new NewsCategory
                {
                    CategoryName = rss.Channel.Title.Substring(12),
                    CategoryImagePath = await GetCategoryImgAsync(rss),
                    Items = new ObservableCollection<NewsCategory.NewsItem>()
                };

                foreach (var item in rss.Channel.Items)
                {
                    DateTime date;
                    DateTime.TryParse(item.PubDate, out date);

                    category.Items.Add(new NewsCategory.NewsItem
                    {
                        NewsTitle = item.Title,
                        NewsPubDate = date,
                        NewsDescription = item.Description,
                        NewsLink = item.Link,
                        RelatedNewsItems = new ObservableCollection<NewsCategory.NewsItem>()
                    });

                }
                categories.Add(category);

            }

            return categories;
        }

        /// <summary>
        /// Возвращает изображение для новостной категории
        /// </summary>
        /// <param name="rss">Рсс-лента, для которой нужно получить изображение категории</param>
        /// <returns></returns>
        private static async Task<string> GetCategoryImgAsync(Rss rss)
        {
            var itemNumber = 0;
            string imageLink=null;
            do
            {
                var uri = rss.Channel.Items[itemNumber].Link;

                imageLink = HttpServices.ParseImage(await HttpServices.GetHttpResponseAsync(uri));

            } while ((imageLink == null) && (itemNumber < 50));

            return imageLink;
        }

       

    }

}
