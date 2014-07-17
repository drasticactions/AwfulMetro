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
using System.Net;
using System.Text;

namespace AwfulMetro.Core.Tools
{
    public static class Extensions
    {
        public static string WithoutNewLines(this string text)
        {
            var sb = new StringBuilder(text.Length);
            foreach (char i in text)
            {
                if (i != '\n' && i != '\r' && i != '\t' && i != '#' && i != '?')
                {
                    sb.Append(i);
                }
                else if (i == '\n')
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }

        public static Dictionary<string, string> ParseQueryString(String query)
        {
            var queryDict = new Dictionary<string, string>();
            foreach (String token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    queryDict[parts[0].Trim()] = WebUtility.UrlDecode(parts[1]).Trim();
                else
                    queryDict[parts[0].Trim()] = "";
            }
            return queryDict;
        }
    }
}