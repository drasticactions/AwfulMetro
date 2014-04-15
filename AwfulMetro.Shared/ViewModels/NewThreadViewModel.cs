using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using Newtonsoft.Json;

namespace AwfulMetro.ViewModels
{
    public class NewThreadViewModel : NotifierBase
    {
        public NewThreadViewModel() 
        {
            BbCodeList = BBCodeManager.BBCodes;
        }

        private bool _isLoading;
        private ForumEntity _forumEntity;
        private NewThreadEntity _newThreadEntity;
        private readonly ThreadManager _threadManager = new ThreadManager();
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private IEnumerable<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private IEnumerable<PostIconEntity> _postIconList = new List<PostIconEntity>();
        private PostIconEntity _postIcon = new PostIconEntity();
        private string _newThreadTitle = "New Thread";

        public PostIconEntity PostIcon
        {
            get { return _postIcon; }
            set
            {
                SetProperty(ref _postIcon, value);
                OnPropertyChanged();
            }
        }

        public string NewThreadTitle
        {
            get { return _newThreadTitle; }
            set
            {
                SetProperty(ref _newThreadTitle, value);
                OnPropertyChanged();
            }
        }

        public ForumEntity ForumEntity
        {
            get { return _forumEntity; }
            set
            {
                SetProperty(ref _forumEntity, value);
                OnPropertyChanged();
            }
        }

        public NewThreadEntity NewThreadEntity
        {
            get { return _newThreadEntity; }
            set
            {
                SetProperty(ref _newThreadEntity, value);
                OnPropertyChanged();
            }
        }

        public IEnumerable<PostIconEntity> PostIconList
        {
            get { return _postIconList; }
            set
            {
                SetProperty(ref _postIconList, value);
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

        public async void GetSmilies()
        {
            var smiliesManager = new SmileManager();
            SmilieCategoryList = await smiliesManager.GetSmileList();
            FullSmileCategoryEntities = SmilieCategoryList;
        }

        public async void GetPostIcons()
        {
            PostIconManager postIconManager = new PostIconManager();
            if (!PostIconList.Any())
            {
                PostIconList = await postIconManager.GetPostIconList(_forumEntity);
            }
        }

        public async Task<bool> Initialize(string jsonObjectString)
        {
            ForumEntity = JsonConvert.DeserializeObject<ForumEntity>(jsonObjectString);
            if (ForumEntity == null) return false;
            NewThreadTitle = string.Format("New Thread - {0}", ForumEntity.Name);
            NewThreadEntity = await _threadManager.GetThreadCookies(ForumEntity.ForumId);
            GetPostIcons();
            return NewThreadEntity != null;
        }
    }
}
