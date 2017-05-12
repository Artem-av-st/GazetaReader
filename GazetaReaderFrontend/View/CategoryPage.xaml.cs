using System.Collections.ObjectModel;
using GazetaReaderFrontend.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GazetaReaderFrontend.Model;
using GazetaReaderFrontend.ViewModel;
using System.ComponentModel;

namespace GazetaReaderFrontend
{
    /// <summary>
    /// Страница, на которой отображаются новости выбранной категории после перехода с главной страницы.
    /// </summary>
    public partial class CategoryPage:INotifyPropertyChanged
    {
        #region Public properties
        /// <summary>
        /// NavigationHelper используется на каждой странице для облегчения навигации и 
        /// управление жизненным циклом процесса
        /// </summary>
        public NavigationHelper NavigationHelper { get; }
        public ObservableCollection<NewsCategory.NewsItem> Items { get; set; }
        public NewsCategory Category { get; set; }
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

        public CategoryPage()
        {
            this.InitializeComponent();

            this.NavigationHelper = new NavigationHelper(this);
            this.NavigationHelper.LoadState += navigationHelper_LoadState;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации.  Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; обычно <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, object)"/> при первоначальном запросе этой страницы и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса.  Это состояние будет равно NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Category = (NewsCategory) e.NavigationParameter;
        }

        #region Регистрация NavigationHelper

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
            NavigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// При клике на одну из новостоей (элемент GridView) переход на страницу с этой новостью (NewsItemPage).
        /// </summary>
        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (NewsCategory.NewsItem)e.ClickedItem;
            Frame.Navigate(typeof(NewsItemPage), item);
        }

        private  void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Items=Category.Items;
            OnPropertyChanged("Items");
            OnPropertyChanged("Category");
        }
    }
}
