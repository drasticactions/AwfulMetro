using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Tools
{
    public class HtmlButtonBuilder
    {
        /// <summary>
        /// Create HTML Submit button for Web Views.
        /// </summary>
        /// <param name="buttonName">Name of button.</param>
        /// <param name="buttonClick">Click handler to be applied to button.</param>
        /// <returns>HTML Submit Button String.</returns>
        public static string CreateSubmitButton(string buttonName, string buttonClick)
        {
            return WebUtility.HtmlDecode(
                       string.Format(
                           "<li><input type=\"submit\" value=\"{0}\" onclick=\"{1}\";></input></li>",
                           buttonName, buttonClick));
        }
    }
}
