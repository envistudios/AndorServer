using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class ChatItem
    {
        public string TellPlayer { get; set; }
        public string Text { get; set; }
        public ChatType Type { get; set; }
    }
}