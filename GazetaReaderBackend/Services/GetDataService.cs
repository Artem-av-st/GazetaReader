using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Quartz;
using GazetaReaderFrontend.Model;
using GazetaReaderBackend.Services;

namespace GazetaReaderFrontend.Services
{
    // Реализуем интерфейс IJob для работы планировщика.
    public class GetDataService:IJob
    {
        /// <summary>
        /// Метод, запускаемый планировщиком по расписанию.
        /// </summary>
        /// <param name="context">Содержит контекст планировщика, в т.ч. параметры job и trigger, а также, возможно, дополнительные параметры или данные</param>
        public async void Execute(IJobExecutionContext context)
        {
            await GetDataAsync();
        }

        /// <summary>
        /// Основной метод приложения, вызывающий методы для получения данных и записывающий данные в файл.
        /// </summary>
        /// <returns></returns>
        public static async Task GetDataAsync()
        {
            // Получаем коллекцию категорий новостей.
            ObservableCollection<NewsCategory> categories = await GetCategoriesService.GetCategoriesAsync();

            // Заполняем категории новостями.
            categories =await GetNewsItemsService.GetNewsItemsAsync(categories);
          
            // Записываем в файл данные категорий.
            HttpServices.PutXmlAsync(categories);

            Console.WriteLine(DateTime.Now);
            
        }

       

        
    }
}
