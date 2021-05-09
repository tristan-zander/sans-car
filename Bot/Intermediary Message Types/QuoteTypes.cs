using Bot.Convertors;

namespace Bot.Intermediary_Message_Types
{
    [UnixLikeArgs("message", 'm')]
    public class MessageArg
    {
        public string Content { get; set; }
    }
}