namespace HouseKeeper.Enum
{
    public class AdminEnum
    {
        public enum ChartMonth : long {
            Jan = 1483209000000,
            Feb = 1485887400000, 
            Mar = 1488306600000,
            Apr = 1490985000000,
            May = 1493577000000,
            Jun = 1496255400000,
            Jul = 1498847400000,
            Aug = 1501525800000,
            Sep = 1504204200000,
            Oct = 1506796200000,
            Nov = 1509474600000,
            Dec = 1512066600000
        }

        public enum RevenueType
        {
            PricePacket = 1,
            Bid = 2,
            Total = 3
        }
    }
}
