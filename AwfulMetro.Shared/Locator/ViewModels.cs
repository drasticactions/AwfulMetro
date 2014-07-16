using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;
using Autofac;
using AwfulMetro.Common;
using AwfulMetro.ViewModels;

namespace AwfulMetro.Locator
{
    public class ViewModels
    {
        public ViewModels()
        {
            if (DesignMode.DesignModeEnabled)
            {
                App.Container = AutoFacConfiguration.Configure();
            }
        }

        public static MainForumsPageViewModel MainForumsPageVm
        {
            get { return App.Container.Resolve<MainForumsPageViewModel>(); }
        }

        public static ThreadViewModel ThreadVm
        {
            get { return App.Container.Resolve<ThreadViewModel>(); }
        }

        public static ThreadListPageViewModel ThreadListPageVm
        {
            get { return App.Container.Resolve<ThreadListPageViewModel>(); }
        }

        public static PrivateMessageViewModel PrivateMessageVm
        {
            get { return App.Container.Resolve<PrivateMessageViewModel>(); }
        }

    }
}
