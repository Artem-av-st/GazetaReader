using GazetaReaderFrontend.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GazetaReaderFrontend.ViewModel;
using GazetaReaderFrontend.ViewModel.Services;
using Windows.UI.Xaml.Navigation;


namespace GazetaReaderFrontend
{

    /// <summary>
    /// Главная страница приложения, на которой отображаются категории новостей
    /// </summary>
    public partial class MainPage: INotifyPropertyChanged
    {
        public MainPage()
        {
            
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

        }
        
        #region Fields and constants 

        private bool _pageAlreadyLoaded = false;

        #endregion

        #region Public properties

        // Использвется для определения статуса работы приложения: Online - удалось получить ответ от сервера, offline - в случае отсутствия
        // ответа от сервера, удалось восстановить прошлый сеанс из local storage, crashed - не удалось.
        public static MainPageViewModel.ApplicationStatus AppStatus { get; set; }
                
        public MyWeather Weather { get; set; }

        public ObservableCollection<NewsCategory> Categories { get; set; } = new ObservableCollection<NewsCategory>();

        #endregion

        #region  INotifyPropertyChanged declaration
        MainPageViewModel viewModel { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName = null)
        {
            var _handler = PropertyChanged;
            if (_handler != null)
                _handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private async void  Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_pageAlreadyLoaded) return;

            _pageAlreadyLoaded = true;

            Categories = await MainPageViewModel.GetCategoriesAsync();
            OnPropertyChanged("Categories");

            // Отражение в заголовке окна статуса работы приложения
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = AppStatus.ToString();

            Weather = await WeatherService.GetWeather();
            OnPropertyChanged("Weather");
            
            
        }

        /// <summary>
        /// При клике на одну из категорий (элемент GridView) переход на страницу со списком новостей этой категории (CategoryPage)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var category = (NewsCategory) e.ClickedItem;
            Frame.Navigate(typeof (CategoryPage), category);
            
        }

        
        private void RefreshBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _pageAlreadyLoaded = false;
            Page_Loaded(null,null);
        }

       
    }

}
