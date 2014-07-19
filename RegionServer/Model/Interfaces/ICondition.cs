using RegionServer.Calculators;

namespace RegionServer.Model.Interfaces
{
    public interface ICondition
    {
        bool Test(Environment env);
        void NotifiyChanged();
    }
}