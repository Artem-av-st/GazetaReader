using System;
using Windows.UI.Xaml;
using GazetaReaderFrontend.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using GazetaReaderFrontend.Model;


namespace GazetaReaderFrontend
{
    /// <summary>
    /// Страница, на которой развернуто отображаются отдельная новость.
    /// </summary>
    public sealed partial class NewsItemPage
    {
        #region Public properties

        public NewsCategory.NewsItem Item { get; set; }

        public ObservableCollection<NewsCategory.NewsItem> RelatedItems { get; set; }

        #endregion

        public NewsItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            // Регистрация share-контракта.
            RegisterForShare();
        }

        #region NavigationHelper
        private NavigationHelper navigationHelper;

        /// <summary>
        /// NavigationHelper используется на каждой странице для облегчения навигации и 
        /// управление жизненным циклом процесса
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        

        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper мог откликаться на методы навигации страницы.
        /// 
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// и <see cref="Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// При клике на одну из "связанных" новостей (элемент GridView) переход на страницу с этой новостью (NewsItemPage).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RelatedGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (NewsCategory.NewsItem)e.ClickedItem;
            Frame.Navigate(typeof(NewsItemPage), item);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; обычно <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            
            Item = (NewsCategory.NewsItem)e.NavigationParameter;
            
        }
        
        #region ShareButtons
        private void ShareVkBtn_OnClick(object sender, RoutedEventArgs e)
        {
             ShareVkBtn.NavigateUri = new Uri(
               String.Format("http://vkontakte.ru/share.php?url={0}&title={1}&image={2}&noparse=true",
               Item.NewsLink,
               Item.NewsTitle,
               Item.NewsImagePath));
        }

        private void ShareFbBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ShareFbBtn.NavigateUri = new Uri(
              String.Format("https://www.facebook.com/sharer.php?src=sp&u={0}", Item.NewsLink));
        }
        

        private void ShareTwBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ShareTwBtn.NavigateUri = new Uri(String.Format("https://twitter.com/intent/tweet?text={1}&url={0}&hashtags=#ГазетаРу",
                Item.NewsLink,
                Item.NewsTitle));
        }
        #endregion
        
        #region AppBarButtons
        private void ToMainBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void ShareBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
            
        }
        #endregion

        #region ShareContract
        private DataTransferManager _dataTransferManager = DataTransferManager.GetForCurrentView();

        private void RegisterForShare()
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += NewsItemPage_DataRequested;
        }

        /// <summary>
        /// Конфигурация share-контракта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NewsItemPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {

            if (!string.IsNullOrEmpty(Item.NewsTitle))
            {
                args.Request.Data.Properties.Title = Item.NewsTitle;
                args.Request.Data.SetBitmap(Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(Item.NewsImagePath)));
                args.Request.Data.SetText(Item.NewsArticle);
                args.Request.Data.SetWebLink(new Uri(Item.NewsLink));
            }
            else
            {
                args.Request.FailWithDisplayText("Nothing to share");
            }
        }
        #endregion

    }
}
