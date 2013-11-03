namespace AwfulMetro.Core.Entity
{
    public class ThreadReplyEntity
    {
        public ThreadReplyEntity(long threadId, string post, string formKey, string formCookie)
        {
            this.ThreadId = threadId;
            this.Post = post;
            this.FormKey = formKey;
            this.FormCookie = formCookie;
        }

        public long ThreadId {  get;  private set;  }

        public string Post { get; private set; }

        public string FormKey { get; private set; }

        public string FormCookie { get; private set; }
    }
}
