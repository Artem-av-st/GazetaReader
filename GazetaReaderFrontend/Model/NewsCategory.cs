using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace GazetaReaderFrontend.Model
{
    [DataContractAttribute]
    public class NewsCategory
    {
        [DataMemberAttribute]
        public string CategoryName { get; set; }
        [DataMemberAttribute]
        public string CategoryImagePath { get; set; }
        [DataMemberAttribute]
        public ObservableCollection<NewsItem> Items { get; set; }
        
        public override string ToString()
        {
            return this.CategoryName;
        }

        [DataContractAttribute]
        public class NewsItem
        {
            [DataMemberAttribute]
            public string NewsTitle { get; set; }
            [DataMemberAttribute]
            public DateTime NewsPubDate { get; set; }
            [DataMemberAttribute]
            public string NewsDescription { get; set; }
            [DataMemberAttribute]
            public string NewsImagePath { get; set; }
            [DataMemberAttribute]
            public string NewsLink { get; set; }
            [DataMemberAttribute]
            public string NewsArticle { get;  set; }
            [DataMemberAttribute]
            public ObservableCollection<NewsItem> RelatedNewsItems { get; set; }

            public override string ToString()
            {
                return this.NewsTitle;
            }
        }
    }
    
}
