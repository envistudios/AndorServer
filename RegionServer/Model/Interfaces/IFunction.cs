using RegionServer.Calculators;

namespace RegionServer.Model.Interfaces
{
    public interface IFunction
    {
        IStat Stat { get; }
        int Order { get; }
        CObject Owner { get; set; }
        ICondition Condition { get; set; }

        void Calc(Environment env);
    }
}