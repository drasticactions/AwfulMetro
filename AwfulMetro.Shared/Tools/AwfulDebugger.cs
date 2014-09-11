using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace AwfulMetro.Tools
{
    public static class AwfulDebugger
    {
        public static async Task SendMessageDialogAsync(string message, Exception ex)
        {
            var dialog = new MessageDialog((string.Concat(message, Environment.NewLine, Environment.NewLine, ex.Message)));
            await dialog.ShowAsync();
        }
    }
}
