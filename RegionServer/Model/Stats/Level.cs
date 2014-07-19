using RegionServer.Model.Interfaces;

namespace RegionServer.Model.Stats
{
    public class Level : IStat
    {    
        #region Implementation of IStat

        public string Name { get { return "Level"; } }
        public bool IsBaseStat { get { return true; } }
        public bool IsNonZero { get { return true; } }
        public float BaseValue { get; set; }

        #endregion
    }
}