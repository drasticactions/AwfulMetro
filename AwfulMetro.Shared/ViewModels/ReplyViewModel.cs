using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
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
            SmilieCategoryList = await smiliesManager.GetSmileList();
            FullSmileCategoryEntities = SmilieCategoryList;
        }

        public async void GetSmiliesPhone()
        {
            var smiliesManager = new SmileManager();
            var smileList = new List<SmileEntity>();
            SmilieCategoryList = await smiliesManager.GetSmileList();
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

            if (_forumThread != null)
            {
                ForumReplyEntity = await replyManager.GetReplyCookies(_forumThread);
            }
            else
            {
                ForumReplyEntity = await replyManager.GetReplyCookies(threadId);
            }
            IsLoading = false;
            return ForumReplyEntity != null;

        }

        public async Task<bool> InitializeEdit(string jsonObjectString)
        {
            IsLoading = true;
            long threadId = Convert.ToInt64(jsonObjectString); ;
            var replyManager = new ReplyManager();
                ForumReplyEntity = await replyManager.GetReplyCookiesForEdit(threadId);
            IsLoading = false;
            return ForumReplyEntity != null;

        }


        public async Task<bool> GetPreviewPost(string replyText)
        {
            Html = string.Empty;
            IsLoading = true;
            ForumReplyEntity.MapMessage(replyText);
            var replyManager = new ReplyManager();
            Html = await replyManager.CreatePreviewPost(ForumReplyEntity);
            IsLoading = false;
            return !string.IsNullOrEmpty(Html);
        }

        public async Task<bool> GetPreviewEditPost(string replyText)
        {
            Html = string.Empty;
            IsLoading = true;
            ForumReplyEntity.MapMessage(replyText);
            var replyManager = new ReplyManager();
            Html = await replyManager.CreatePreviewEditPost(ForumReplyEntity);
            IsLoading = false;
            return !string.IsNullOrEmpty(Html);
        }

        public void GetPreviousPosts()
        {
            Html = ForumReplyEntity.PreviousPostsRaw;
        }
    }
}
