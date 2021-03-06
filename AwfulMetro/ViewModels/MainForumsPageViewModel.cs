﻿#region copyright
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
using AwfulMetro.Command;
using AwfulMetro.Commands;
using AwfulMetro.Common;
using AwfulMetro.Context;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

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

        private AddAsFavoriteCommand _addAsFavoriteCommand = new AddAsFavoriteCommand();

        public AddAsFavoriteCommand AddAsFavorite
        {
            get { return _addAsFavoriteCommand; }
            set { SetProperty(ref _addAsFavoriteCommand, value); }
        }

        private RemoveFavoriteCommand _removeFavoriteCommand = new RemoveFavoriteCommand();

        public RemoveFavoriteCommand RemoveFavorite
        {
            get { return _removeFavoriteCommand; }
            set { SetProperty(ref _removeFavoriteCommand, value); }
        }

        public void SetFavoriteForums(ObservableCollection<ForumCategoryEntity> favoriteList)
        {
            FavoriteForumGroupList = favoriteList;
        }

        public async void GetFavoriteForums()
        {
            using (var db = new FavoriteForumContext())
            {
                var forumEntities = db.Forums.ToList();
                _favoritesEntity = new ForumCategoryEntity("Favorites", "forumid=48")
                {
                    ForumList = forumEntities
                };
                SetFavoriteForums(new ObservableCollection<ForumCategoryEntity> { _favoritesEntity });
            }  
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
