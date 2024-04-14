﻿using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Recruitments
{

    public class PriceTagViewModel
    {
        public TINTUYENDUNG Recruitment { get; set; }
        public List<GIAGOITIN> PriceTags { get; set; }
        public List<RecruitmentBidViewModel> OnlineRecruitments { get; set; }
        public int PricePacketId { get; set; }
        public int SalaryId { get;set; }
        public int experienceId { get; set; }
        public int districtId { get; set; }
        public decimal BidPrice { get; set; }
        public string JobIds { get; set; }
        public string PricePacketName { get;set; }
        public decimal Price { get; set; }
        public int NumberDays { get; set; }
    }
}
