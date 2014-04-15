using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.ViewModels
{
    public class MainForumsPageViewModel : NotifierBase
    {
        private readonly ForumManager _forumManager = new ForumManager();
        private List<long> _forumIds = new List<long>();
        private ForumCategoryEntity _favoritesEntity;
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private ObservableCollection<ForumCategoryEntity> _forumGroupList;
        private ObservableCollection<ForumCategoryEntity> _favoriteForumGroupList;
        private bool _isLoading;
        public MainForumsPageViewModel()
        {
            Initialize();
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ForumCategoryEntity> ForumGroupList
        {
            get { return _forumGroupList; }
            set
            {
                SetProperty(ref _forumGroupList, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ForumCategoryEntity> FavoriteForumGroupList
        {
            get { return _favoriteForumGroupList; }
            set
            {
                SetProperty(ref _favoriteForumGroupList, value);
                OnPropertyChanged();
            }
        }

        public void SetFavoriteForums(ObservableCollection<ForumCategoryEntity> favoriteList)
        {
            FavoriteForumGroupList = favoriteList;
        }

        private void GetFavoriteForums()
        {
            if (!_localSettings.Values.ContainsKey("_forumIds")) return;
            DeserializeXmlToList((string)_localSettings.Values["_forumIds"]);
            var forumEntities = new List<ForumEntity>();
            foreach (var f in ForumGroupList.Select(forumGroup => forumGroup.ForumList.Where(forum => _forumIds.Contains(forum.ForumId)).ToList()))
            {
                forumEntities.AddRange(f);
            }
            _favoritesEntity = new ForumCategoryEntity("Favorites", "forumid=48")
            {
                ForumList = forumEntities
            };
            SetFavoriteForums(new ObservableCollection<ForumCategoryEntity> { _favoritesEntity });
        }

        private async void Initialize()
        {
            IsLoading = true;
            ForumGroupList = await _forumManager.GetForumCategoryMainPage();
            GetFavoriteForums();
            IsLoading = false;
        }

        private static string SerializeListToXml(List<long> list)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof(List<long>));
                var writer = new StringWriter();
                xmlIzer.Serialize(writer, list);
                Debug.WriteLine(writer.ToString());
                return writer.ToString();
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                return String.Empty;
            }
        }

        private void DeserializeXmlToList(string listAsXml)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof(List<long>));
                var strReader = new StringReader(listAsXml);
                _forumIds = (xmlIzer.Deserialize(strReader)) as List<long>;
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                _forumIds = new List<long>();
            }
        }
    }
}
