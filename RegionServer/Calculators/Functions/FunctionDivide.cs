using RegionServer.Model;
using RegionServer.Model.Interfaces;

namespace RegionServer.Calculators.Functions
{
    public class FunctionDivide : IFunction
    {
        private readonly ILambda _lambda;

        public FunctionDivide(IStat stat, int order, CObject owner, ILambda lamda)
        {
            Stat = stat;
            Order = order;
            Owner = owner;
            _lambda = lamda;
        }

        #region Implementation of IFunction

        public IStat Stat { get; private set; }
        public int Order { get; private set; }
        public CObject Owner { get; set; }
        public ILambda Lamda { get; set; }
        public ICondition Condition { get; set; }

        public void Calc(Environment env)
        {
            if (Condition == null || Condition.Test(env))
            {
                env.Value /= _lambda.Calculate(env);
            }
        }

        #endregion
    }
}