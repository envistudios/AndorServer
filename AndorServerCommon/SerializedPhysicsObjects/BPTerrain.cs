using System;
using AndorServerCommon.MessageObjects;

namespace AndorServerCommon.SerializedPhysicsObjects
{
    [Serializable]
    public class BPTerrain
    {
        public PositionData Center { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float[] HeightData { get; set; }
        public float HeightScale { get; set; }

        public PositionData LocalScale { get; set; }
        public PositionData Rotation { get; set; }

        public BPTerrain()
        {
            Center = new PositionData();
            LocalScale = new PositionData();
            Rotation = new PositionData();
        }
    }
}