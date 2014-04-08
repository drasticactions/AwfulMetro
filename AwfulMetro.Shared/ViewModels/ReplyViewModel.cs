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
        }

        private bool _isLoading;
        private ForumThreadEntity _forumThread;
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private IEnumerable<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private ForumReplyEntity _forumReplyEntity;
        public IEnumerable<BBCodeCategoryEntity> BbCodeList
        {
            get { return _bbCodeList; }
            set
            {
                SetProperty(ref _bbCodeList, value);
                OnPropertyChanged();
            }
        }

        public IEnumerable<SmileCategoryEntity> SimileCategoryList
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

        public async Task<bool> Initialize(string jsonObjectString)
        {
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

            return ForumReplyEntity != null;

        }
    }
}
