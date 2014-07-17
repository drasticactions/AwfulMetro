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
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AwfulMetro.Core.Tools
{
    /// <summary>
    ///     Used to bind Webviews to lists.
    /// </summary>
    public static class WebViewHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof (string), typeof (WebViewHelper), new PropertyMetadata(null, OnHtmlChanged));

        public static readonly DependencyProperty HtmlUriProperty = DependencyProperty.RegisterAttached("HtmlUri",
            typeof (Uri),
            typeof (
                WebViewHelper
                ),
            new PropertyMetadata
                (null,
                    OnHtmlUriChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string) dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;
            if (browser == null)
                return;
            string html = string.Empty;
            if (e.NewValue != null)
            {
                html = e.NewValue.ToString();
            }
            browser.NavigateToString(html);
        }

        public static void SetHtmlUri(DependencyObject dependencyObject, Uri value)
        {
            dependencyObject.SetValue(HtmlUriProperty, value);
        }

        public static Uri GetHtmlUri(DependencyObject dependencyObject)
        {
            return (Uri) dependencyObject.GetValue(HtmlUriProperty);
        }

        private static void OnHtmlUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;
            if (browser == null)
                return;

            var htmlUri = (Uri) e.NewValue;
            browser.Navigate(htmlUri);
        }

        public static async Task<bool> HtmlExists(string htmlFile)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(htmlFile);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
    }
}