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
