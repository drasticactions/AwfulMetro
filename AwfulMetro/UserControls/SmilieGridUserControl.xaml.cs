using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using AwfulMetro.ViewModels;

namespace AwfulMetro.UserControls
{
    public sealed partial class SmilieGridUserControl : UserControl
    {
        public SmilieGridUserControl()
        {
            this.InitializeComponent();
        }

        private void SmilieGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FilterBox_OnSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {

        }

        private void FilterBox_OnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {

        }

        private void FilterBox_OnQueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args)
        {

        }

        public void OpenPopup()
        {
            ParentPopup.IsOpen = true;
        }

        public void ClosePopup()
        {
            ParentPopup.IsOpen = false;
        }

        public void SetOffset()
        {
            ParentPopup.HorizontalOffset = (Window.Current.Bounds.Width - 400) / 2;
            ParentPopup.VerticalOffset = (Window.Current.Bounds.Height - 500) / 2;
        }

        public void SetContext(ReplyViewModel vm)
        {
            DataContext = vm;
        }
    }
}
