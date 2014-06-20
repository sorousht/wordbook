namespace Wordbook.Data
{
    public class PronounciationService:JsonListService<Pronounciation>
    {
        public PronounciationService(string uri) : base(uri)
        {}
    }
}