﻿using HouseKeeper.Models.DB;

namespace HouseKeeper.Models.Views.Employee
{
    public class JobDetailViewModel
    {
        public TINTUYENDUNG Recruitment { get; set; }
        public CHITIETAPPLY? ApplyDetail { get; set; }
        public TAIKHOAN Account { get; set; }
    }
}
