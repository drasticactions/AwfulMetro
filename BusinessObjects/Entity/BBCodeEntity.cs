namespace BusinessObjects.Entity
{
    public class BBCodeEntity
    {
        public string Title { get; private set; }

        public string Code { get; private set; }

        public BBCodeEntity(string title, string code)
        {
            this.Title = title;
            this.Code = code;
        }
    }
}
