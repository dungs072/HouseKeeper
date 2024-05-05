using HouseKeeper.Enum;
using System;
using System.Runtime.Serialization;

namespace HouseKeeper.Models.Views.Admin
{
    //DataContract for Serializing Data - required to serve in JSON format
    [DataContract]
    public class DataPoint
    {
        public DataPoint(EnumAdmin.ChartMonth x, decimal y)
        {
            this.X = x;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<EnumAdmin.ChartMonth> X = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<decimal> Y = null;
    }
}