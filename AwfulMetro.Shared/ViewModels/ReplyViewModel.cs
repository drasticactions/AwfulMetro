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
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Tools;
using Newtonsoft.Json;

namespace AwfulMetro.ViewModels
{
    public class ReplyViewModel : NotifierBase
    {
        public ReplyViewModel()
        {
            BbCodeList = BBCodeManager.BBCodes;
            var bbList = new List<BBCodeEntity>();
            foreach (var category in BbCodeList)
            {
                bbList.AddRange(category.List);
            }
            BbCodes = bbList;
        }

        private bool _isLoading;
        private ForumThreadEntity _forumThread;
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private IEnumerable<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private IEnumerable<SmileEntity> _smileList;
        private IEnumerable<BBCodeEntity> _bbCodes; 
        private ForumReplyEntity _forumReplyEntity;
        private string _html;

        public IEnumerable<BBCodeEntity> BbCodes
        {
            get { return _bbCodes; }
            set
            {
                SetProperty(ref _bbCodes, value);
                OnPropertyChanged();
            }
        }


        public IEnumerable<SmileEntity> SmileList
        {
            get { return _smileList; }
            set
            {
                SetProperty(ref _smileList, value);
                OnPropertyChanged();
            }
        }

        public string Html
        {
            get { return _html; }
            set
            {
                SetProperty(ref _html, value);
                OnPropertyChanged();
            }
        }

        public IEnumerable<BBCodeCategoryEntity> BbCodeList
        {
            get { return _bbCodeList; }
            set
            {
                SetProperty(ref _bbCodeList, value);
                OnPropertyChanged();
            }
        }

        public IEnumerable<SmileCategoryEntity> FullSmileCategoryEntities = new List<SmileCategoryEntity>();
        public IEnumerable<SmileCategoryEntity> SmilieCategoryList
        {
            get { return _smileCategoryList; }
            set
            {
                SetProperty(ref _smileCategoryList, value);
                OnPropertyChanged();
            }
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

        public ForumReplyEntity ForumReplyEntity
        {
            get { return _forumReplyEntity; }
            set
            {
                SetProperty(ref _forumReplyEntity, value);
                OnPropertyChanged();
            }
        }

        public async void GetSmilies()
        {
            var smiliesManager = new SmileManager();
            try
            {
                SmilieCategoryList = await smiliesManager.GetSmileList();

            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Can't get smilies list. :(", ex);
                return;
            }
            FullSmileCategoryEntities = SmilieCategoryList;
        }

        public async void GetSmiliesPhone()
        {
            var smiliesManager = new SmileManager();
            var smileList = new List<SmileEntity>();
            try
            {
                SmilieCategoryList = await smiliesManager.GetSmileList();

            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Can't get smilies list. :(", ex);
                return;
            }
            foreach (var category in SmilieCategoryList)
            {
                smileList.AddRange(category.List);
            }
            SmileList = smileList;
        }

        public async Task<bool> Initialize(string jsonObjectString)
        {
            IsLoading = true;
            long threadId = 0;
            var replyManager = new ReplyManager();
            try
            {
               _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            }
            catch (Exception)
            {
                threadId = Convert.ToInt64(jsonObjectString);
            }

            try
            {
                if (_forumThread != null)
                {
                    ForumReplyEntity = await replyManager.GetReplyCookies(_forumThread);
                }
                else
                {
                    ForumReplyEntity = await replyManager.GetReplyCookies(threadId);
                }
            }
            catch (Exception ex)
            {
                ForumReplyEntity = null;
            }
            IsLoading = false;
            return ForumReplyEntity != null;

        }

        public async Task<bool> InitializeEdit(string jsonObjectString)
        {
            IsLoading = true;
            long threadId = Convert.ToInt64(jsonObjectString); ;
            try
            {
                var replyManager = new ReplyManager();
                ForumReplyEntity = await replyManager.GetReplyCookiesForEdit(threadId);
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("You can't edit this post!", ex);
            }
            IsLoading = false;
            return ForumReplyEntity != null;

        }


        public async Task<bool> GetPreviewPost(string replyText)
        {
            Html = string.Empty;
            IsLoading = true;
            ForumReplyEntity.MapMessage(replyText);
            var replyManager = new ReplyManager();
            try
            {
                Html = await replyManager.CreatePreviewPost(ForumReplyEntity);
            }
            catch (Exception ex)
            {
                 AwfulDebugger.SendMessageDialogAsync("Could not create preview HTML", ex);
            }
            IsLoading = false;
            return !string.IsNullOrEmpty(Html);
        }

        public async Task<bool> GetPreviewEditPost(string replyText)
        {
            Html = string.Empty;
            IsLoading = true;
            ForumReplyEntity.MapMessage(replyText);
            var replyManager = new ReplyManager();
            try
            {
                Html = await replyManager.CreatePreviewEditPost(ForumReplyEntity);
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Could not create preview HTML", ex);
            }
            IsLoading = false;
            return !string.IsNullOrEmpty(Html);
        }

        public void GetPreviousPosts()
        {
            Html = ForumReplyEntity.PreviousPostsRaw;
        }
    }
}
