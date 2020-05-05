namespace DCAPST.Interfaces
{
    
    public interface IAssimilation
    {
        AssimilationFunction GetFunction(AssimilationPathway pathway, TemperatureResponse leaf);

        void UpdateIntercellularCO2(AssimilationPathway pathway, double gt, double waterUseMolsSecond);

        void UpdatePartialPressures(AssimilationPathway pathway, TemperatureResponse leaf, AssimilationFunction function);
    }
}
