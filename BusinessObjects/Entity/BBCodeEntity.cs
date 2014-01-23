namespace AwfulMetro.Core.Entity
{
    public class BBCodeEntity
    {
        public BBCodeEntity(string title, string code)
        {
            Title = title;
            Code = code;
        }

        public string Title { get; private set; }

        public string Code { get; private set; }
    }
}