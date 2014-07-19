using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class PlayerMovement
    {
        public int ObjectId { get; set; }
        public int Facing { get; set; }
        public bool Walk { get; set; }
        public bool Jump { get; set; }
        public bool Moving { get; set; }
        public float Forward { get; set; }
        public float Right { get; set; }
    }
}