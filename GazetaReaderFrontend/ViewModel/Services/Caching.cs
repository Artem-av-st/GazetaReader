using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using GazetaReaderFrontend.Model;

namespace GazetaReaderFrontend.ViewModel
{
    class Caching
    {
        /// <summary>
        /// метод, сериализующий данные и записывающий их в файл
        /// </summary>
        public static async void PutXmlAsync(ObservableCollection<NewsCategory> categories)
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<NewsCategory>));

            using (var stream = 
                await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("data.xml", CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, categories);
            }
        }
        /// <summary>
        /// метод, вычитывающий данные из файла и десериализующий их
        /// </summary>
        public static async Task<ObservableCollection<NewsCategory>>  ReadXmlAsync()
        {
            var serializer = new DataContractSerializer(typeof(ObservableCollection<NewsCategory>));

            // проверка наличия файла
            var existed = await FileExists(ApplicationData.Current.LocalFolder, "data.xml");
            if (existed)
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("data.xml"))
                {
                    return  (ObservableCollection<NewsCategory>)serializer.ReadObject(stream);
                }
            }
            throw new FileNotFoundException("Нет данных для отображения");
         }

        /// <summary>
        /// Проверка наличия файла в директории
        /// </summary>
        /// <param name="folder">Директория</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        public static async Task<bool> FileExists(StorageFolder folder, string fileName)
        {
            return (await folder.GetFilesAsync()).Any(x => x.Name == fileName);
        }


    }
}
