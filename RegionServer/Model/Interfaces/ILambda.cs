using RegionServer.Calculators;

namespace RegionServer.Model.Interfaces
{
    public interface ILambda
    {
        float Calculate(Environment env);
    }
}