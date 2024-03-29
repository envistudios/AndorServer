﻿using System;
using System.Collections.Generic;
using AndorServerCommon.MessageObjects;

namespace AndorServerCommon.SerializedPhysicsObjects
{
    [Serializable]
    public class BPMesh
    {
        public PositionData Center { get; set; }
        public PositionData Rotation { get; set; }
        public PositionData LocalScale { get; set; }

        public int NumTris { get; set; }
        public int NumVerts { get; set; }
        public List<int> Triangles { get; set; }
        public List<PositionData> Vertexes { get; set; }

        public BPMesh()
        {
            Center = new PositionData();
            Rotation = new PositionData();
            LocalScale = new PositionData();
            Triangles = new List<int>();
            Vertexes = new List<PositionData>();
        }
    }
}