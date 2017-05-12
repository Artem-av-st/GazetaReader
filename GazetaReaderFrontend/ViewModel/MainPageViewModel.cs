using System.Threading.Tasks;
using GazetaReaderFrontend.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;

namespace GazetaReaderFrontend.ViewModel
{
    public class MainPageViewModel
    {
        public enum ApplicationStatus:int
        {
            Crashed=0,
            Offline=1,
            Online=2
        }

        //private static ApplicationStatus AppStatus;
      
        /// <summary>
        /// Возвращает список категорий новостей, пытаясь получить их от сервера, либо восстановить из 
        /// локального файла. 
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<NewsCategory>> GetCategoriesAsync()
        {
           
           Stream result=null;

            // Проверка состояния подключения к сети. Попытка получить ответ от сервера.
            if (ViewModelServices.CheckInternetConnection())
            {
                result = await ViewModelServices.GetHttpResponseAsync("http://localhost:5000/");
                MainPage.AppStatus = ApplicationStatus.Online;
            }
            
           // Если нет ответа или он плохой, то пытаемся восстановить данные от прошлой сессии.
           if (result == null)
           {
               MainPage.AppStatus = ApplicationStatus.Offline;
               try
               {
                    return await Caching.ReadXmlAsync();
               }
               catch (FileNotFoundException)
               {
                    /*await new MessageDialog("К сожалению, не удалось восстановить предыдущее состояние приложения, и оно будет завершено.").ShowAsync();
                    Application.Current.Exit();*/
                    MainPage.AppStatus = ApplicationStatus.Crashed;
                }
           }

           var serializer = new DataContractSerializer(typeof (ObservableCollection<NewsCategory>));
           
            // Десериализуем ответ, полученный от сервера.
           ObservableCollection<NewsCategory> categories = (ObservableCollection<NewsCategory>) serializer.ReadObject(result);
           
           // Записываем в local storage коллекцию категорий.
           Caching.PutXmlAsync(categories);

           return categories;

            
        }

        





    }

}

