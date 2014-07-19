using System.Collections.Generic;
using RegionServer.Calculators.Functions;
using RegionServer.Calculators.Lambdas;
using RegionServer.Model.Interfaces;

namespace RegionServer.Model.Stats
{
    public class Level5HP : IDerivedStat
    {
        private List<IFunction> _functions;

        public Level5HP()
        {
            _functions = new List<IFunction>()
            {
                new FunctionAdd(this, 0, null, new LambdaConstant(5)),
                new FunctionMultiply(this, 1, null, new LambdaStat(new Level()))
            };
        }

        #region Implementation of IDerivedStat

        public string Name { get { return "HP"; } }
        public bool IsBaseStat { get { return false; } }
        public bool IsNonZero { get { return false; } }
        public float BaseValue { get { return 0; } set {} }
        public List<IFunction> Functions { get { return _functions; } }

        #endregion
    }
}