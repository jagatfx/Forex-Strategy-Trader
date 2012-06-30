namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Data Periods.
    /// The value of each period is equal to its duration in minutes.
    /// </summary>
    public enum DataPeriods
    {
        min1 = 1,
        min5 = 5,
        min15 = 15,
        min30 = 30,
        hour1 = 60,
        hour4 = 240,
        day = 1440,
        week = 10080
    }
}