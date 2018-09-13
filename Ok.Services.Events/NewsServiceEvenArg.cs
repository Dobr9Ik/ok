namespace Ok.Services.Events
{
    public class NewsServiceEvenArg
    {
        public string Message { get; }

        public NewsServiceEvenArg(string message)
        {
            Message = message;
        }
    }
}
