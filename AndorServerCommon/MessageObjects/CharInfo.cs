using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class CharInfo
    {
        public PositionData Position { get; set; }
        public string Name { get; set; }
    }
}