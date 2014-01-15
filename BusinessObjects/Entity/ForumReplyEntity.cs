﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

    }
}