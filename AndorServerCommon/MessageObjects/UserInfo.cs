using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class UserInfo
    {
        public PositionData Position { get; set; }
        public string Name { get; set; }
    }
}