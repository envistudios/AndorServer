using RegionServer.Model.Interfaces;

namespace RegionServer.Model.Stats
{
    public class MoveSpeed : IStat
    {
        #region Implementation of IStat

        public string Name { get { return "Move Speed"; } }
        public bool IsBaseStat { get { return true; } }
        public bool IsNonZero { get { return false; } }
        public float BaseValue { get { return 6.0f; } set {} }

        #endregion
    }
}