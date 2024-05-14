using HouseKeeper.Enum;

namespace HouseKeeper.Configs
{
    public class StatisticConfig
    {
        public static int startYearStatistic { get; private set; } = 2018;
        public static int endYearStatistic { get; private set; } = DateTime.Now.Year;

        public readonly static AdminEnum.ChartMonth[] monthsArray = {
            AdminEnum.ChartMonth.Jan,
            AdminEnum.ChartMonth.Feb,
            AdminEnum.ChartMonth.Mar,
            AdminEnum.ChartMonth.Apr,
            AdminEnum.ChartMonth.May,
            AdminEnum.ChartMonth.Jun,
            AdminEnum.ChartMonth.Jul,
            AdminEnum.ChartMonth.Aug,
            AdminEnum.ChartMonth.Sep,
            AdminEnum.ChartMonth.Oct,
            AdminEnum.ChartMonth.Nov,
            AdminEnum.ChartMonth.Dec
        };
    }
}
