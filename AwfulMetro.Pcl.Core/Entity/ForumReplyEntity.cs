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
using System.Net;
using AwfulMetro.Pcl.Core.Entity;

namespace AwfulMetro.Core.Entity
{
    public class ForumReplyEntity
    {
        public string Message { get; private set; }

        public ForumThreadEntity Thread { get; private set; }

        public ForumPostEntity Post { get; private set; }

        public bool ParseUrl { get; private set; }

        public string FormKey { get; private set; }

        public string FormCookie { get; private set; }

        public string Quote { get; private set; }

        public string ThreadId { get; private set; }

        public long PostId { get; private set; }

        public string PreviousPostsRaw { get; set; }

        public string Bookmark { get; set; }

        public void MapMessage(string message)
        {
            Message = message;
            ParseUrl = true;
        }

        public void MapThreadInformation(string formKey, string formCookie, string quote, string threadId)
        {
            FormKey = formKey;
            FormCookie = formCookie;
            ThreadId = threadId;
            Quote = WebUtility.HtmlDecode(quote);
        }

        public void MapEditPostInformation(string quote, long postId, string bookmark)
        {
            Quote = WebUtility.HtmlDecode(quote);
            PostId = postId;
            Bookmark = bookmark;
        }
    }
}