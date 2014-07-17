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

namespace AwfulMetro.UserControls
{
    public sealed partial class ImageLoader : UserControl
    {
        public static DependencyProperty LoadingContentProperty =
      DependencyProperty.Register("LoadingContent",
        typeof(object),
        typeof(ImageLoader), null);

        public static DependencyProperty FailedContentProperty =
          DependencyProperty.Register("FailedContent",
            typeof(object),
            typeof(ImageLoader), null);

        public static DependencyProperty SourceProperty =
          DependencyProperty.Register("Source",
            typeof(ImageSource),
            typeof(ImageLoader),
            new PropertyMetadata(null, OnSourceChanged));

        public ImageLoader()
        {
            this.InitializeComponent();
        }

        public object LoadingContent
        {
            get
            {
                return (base.GetValue(LoadingContentProperty));
            }
            set
            {
                base.SetValue(LoadingContentProperty, value);
            }
        }
        public object FailedContent
        {
            get
            {
                return (base.GetValue(FailedContentProperty));
            }
            set
            {
                base.SetValue(FailedContentProperty, value);
            }
        }
        public ImageSource Source
        {
            get
            {
                return ((ImageSource)base.GetValue(SourceProperty));
            }
            set
            {
                base.SetValue(SourceProperty, value);
            }
        }
        static void OnSourceChanged(DependencyObject sender,
          DependencyPropertyChangedEventArgs args)
        {
            ImageLoader loader = (ImageLoader)sender;
            VisualStateManager.GoToState(loader, "Loading", true);
        }
        void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Failed", true);
        }
        void OnImageOpened(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Displaying", true);
        }
    }
}
