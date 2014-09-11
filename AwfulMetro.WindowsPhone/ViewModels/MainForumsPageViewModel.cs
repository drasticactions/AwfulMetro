#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Popups;
using AwfulMetro.Commands;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Tools;

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
            if (ForumGroupList == null)
            {
                Initialize();                
            }
        }

        public ThreadListPageViewModel ThreadListPageViewModel = null;

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

        public async void Initialize()
        {
            IsLoading = true;
            try
            {
                ForumGroupList = await _forumManager.GetForumCategoryMainPage();
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Failed to initialize the main forums list.", ex);
                IsLoading = false;
                return;
            }
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
