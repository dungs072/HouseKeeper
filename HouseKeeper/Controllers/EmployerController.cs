using Castle.Core.Resource;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views.Employer;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.BillingPortal;


namespace HouseKeeper.Controllers
{
    public class EmployerController : Controller
    {
        private readonly IEmployerRespository employerRespository;
        private readonly IEmployeeRespository employeeRespository;
        private readonly StripeSetting _stripeSettings;
        public string SessionId { get; set; }
        public EmployerController(IEmployerRespository employerRespository, IEmployeeRespository employeeRespository, IOptions<StripeSetting> stripeSetting)
        {
            this.employerRespository = employerRespository;
            this.employeeRespository = employeeRespository;
            this._stripeSettings = stripeSetting.Value;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Recruitment()
        {
            CreateRecruitmentsViewModel model = new CreateRecruitmentsViewModel();
            List<HINHTHUCTRALUONG> paidTypes = await employerRespository.GetPaidTypes();
            List<KINHNGHIEM> experiences = await employerRespository.GetExperiences();
            List<TINHTHANHPHO> cities = await employerRespository.GetCities();
            List<HUYEN> districts = await employerRespository.GetDistricts();
            List<LOAICONGVIEC> jobs = await employerRespository.GetJobs();
            model.PaidTypes = paidTypes;
            model.Experiences = experiences;
            model.Cities = cities;
            model.jobs = jobs;
            model.Districts = districts;
            model.MaxSalary = 25000;
            model.NumberVacancies = 1;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> HandleCreateRecruitment(CreateRecruitmentsViewModel model)
        {
            var value0 = Request.Form["SelectedJobs"];
           // string[] selectedJobs = value0.ToString().Split(',');  
            var value1 = Request.Form["isFullTime"];
            bool isFullTime = value1 == "on";
            var value2 = Request.Form["min-age"];
            var value3 = Request.Form["max-age"];
            var value4 = Request.Form["PaidTypeId"];
            var value5 = Request.Form["ExperienceId"];
            var value6 = Request.Form["DistrictId"];
            var value7 = Request.Form["Gender"];
            var value8 = Request.Form["min-salary"].ToString();
            var value9 = Request.Form["max-salary"].ToString();
            TINTUYENDUNG recruitment = new TINTUYENDUNG();
            recruitment.MinSalary = int.Parse(value8.Replace(".",""));
            recruitment.MaxSalary = int.Parse(value9.Replace(".", ""));
            recruitment.Age = value2+"-"+value3;
            recruitment.Note = model.TakeNotes;
            recruitment.FullTime = isFullTime;
            recruitment.MaxApplications = model.NumberVacancies;
            recruitment.Address = model.Address;
            //recruitment.SalaryForm = await employerRespository.GetPaidType(int.Parse(value4));
            //recruitment.Experience = await employerRespository.GetExperience(int.Parse(value5));
            //recruitment.City = await employerRespository.GetCity(int.Parse(value6));
            if(value7=="Null")
            {
                recruitment.Gender = null;
            }
            else
            {
                recruitment.Gender = value7;
            }
            recruitment.PostTime = DateTime.Now;
            recruitment.RecruitDeadlineDate = DateTime.Now;
            PriceTagViewModel priceModel = new PriceTagViewModel();
            priceModel.Recruitment = recruitment;
            priceModel.JobIds = value0;
            priceModel.SalaryId = int.Parse(value4);
            priceModel.experienceId = int.Parse(value5);
            priceModel.districtId = int.Parse(value6);
           
            //var result = employerRespository.CreateRecruitment(recruitment,selectedJobs);
            //if(result.Result)
            //{
            //    TempData["Success"] = "Please choose your price you want!";
            //}
            //else
            //{
            //    TempData["Error"] = "Server error. Failed to create new recruitment!";
            //}
            string priceTagViewModelJson = JsonConvert.SerializeObject(priceModel);
            HttpContext.Session.SetString("PriceTagViewModel",priceTagViewModelJson);
            TempData["Success"] = "Please choose your price you want!";
            return RedirectToAction("PriceTag");
        }
        public async Task<IActionResult> PriceTag()
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                model.PriceTags = await employerRespository.GetPriceTags();
                TempData["Success"] = "Please set your price to display above other recruitments!";
                return View(model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> BidPrice(int pricePacketId)
        {

            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                var onlineRecruitment = await employerRespository.GetOnlineRecruitments();
                List<RecruitmentBidViewModel> bids = new List<RecruitmentBidViewModel>();
                foreach(var recruitment in onlineRecruitment)
                {
                    var bid = new RecruitmentBidViewModel();
                    bid.MaxSalary = recruitment.MaxSalary;
                    bid.MinSalary = recruitment.MinSalary;
                    bid.BidPrice = recruitment.BidPrice;
                    bid.MaxApplications = recruitment.MaxApplications;
                    bid.RecruitDeadlineDate = recruitment.RecruitDeadlineDate;
                    bid.PostTime = recruitment.PostTime;
                    bid.RecruiterName = recruitment.Employer.LastName+" "+recruitment.Employer.FirstName;
                    bids.Add(bid);

                }
               
                model.OnlineRecruitments = bids;
                model.PricePacketId = pricePacketId;
                string temp = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("PriceTagViewModel", temp);
                return View(model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> HandleSendBidPrice(PriceTagViewModel tempModel)
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                var value2 = Request.Form["bidAmount"].ToString();
                model.Recruitment.BidPrice = int.Parse(value2.Replace(".",""));
                var pricePacket = await employerRespository.GetPricePacket(model.PricePacketId);
                model.PricePacketName = pricePacket.PricePacketName;
                model.Price = pricePacket.Price;
                model.NumberDays = pricePacket.NumberDays;
                string temp = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("PriceTagViewModel", temp);
                TempData["Success"] = "Please finish your payment";
                return View("CheckOut",model);
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> CheckOut()
        {
            string priceTagViewModelJson = HttpContext.Session.GetString("PriceTagViewModel") as string;
            if (!string.IsNullOrEmpty(priceTagViewModelJson))
            {
                PriceTagViewModel model = JsonConvert.DeserializeObject<PriceTagViewModel>(priceTagViewModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Server error!";
                    return RedirectToAction("Index", "Home");
                }
                string[] selectedJobs = model.JobIds.ToString().Split(',');
                model.Recruitment.SalaryForm = await employerRespository.GetPaidType(model.SalaryId);
                model.Recruitment.Experience = await employerRespository.GetExperience(model.experienceId);
                model.Recruitment.District = await employerRespository.GetDistrict(model.districtId);
                int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
                model.Recruitment.Employer = await employerRespository.GetEmployer(employerId);
                var pricePacket = await employerRespository.GetPricePacket(model.PricePacketId);
                int amount = (int)pricePacket.Price + (int)model.Recruitment.BidPrice;
                var result = HandlePaidBill(amount, model.Recruitment.Employer.EmployerId);
                if (!result)
                {
                    TempData["Error"] = "Fail to paid by credit card";
                    return View("CheckOut", model);
                }
                var state = await employerRespository.CreateRecruitment(model.Recruitment, selectedJobs, model.PricePacketId);
                if(state)
                {
                    TempData["Success"] = "Create new recruitment successfully";
                    return RedirectToAction("ListRecruitment");
                }
                else
                {
                    TempData["Error"] = "Fail to create new recruitment";
                    return View("CheckOut", model);
                }
            }
            TempData["Error"] = "Server error!";
            return RedirectToAction("Index", "Home");
        }
        private bool HandlePaidBill(int amount, int customerId)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "vnd",
                Description = "Customer with id " + customerId.ToString() + " has paid",
                PaymentMethodTypes = new List<string> { "card" },
                CaptureMethod = "manual",
                Customer = "cus_Q6mvpowkYb0Rql",
            };

            var paymentIntent = service.Create(options);
            switch (paymentIntent.Status)
            {
                case "requires_action":
                case "requires_source_action":
                    // Payment requires authentication
                    Console.WriteLine("Payment requires authentication. Handle 3D Secure flow.");
                    return false;
                case "succeeded":
                    // Payment succeeded
                    Console.WriteLine("Payment succeeded!");
                    return true;
                case "requires_capture":
                    // Payment failed or needs action
                    Console.WriteLine("Payment failed or needs action. Handle accordingly.");
                    return false;
                default:
                    Console.WriteLine($"Unknown PaymentIntent status: {paymentIntent.Status}");
                    return true;
            }
        }
        private void HandleRefund(int amount)
        {
            //StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            //string chargeId = "cus_Q6mvpowkYb0Rql";

            //var refundOptions = new RefundCreateOptions
            //{
            //    Amount = amount,
            //    Currency = "vnd",
            //    Customer = "cus_Q6mvpowkYb0Rql",
            //};

            //var refundService = new RefundService();
            //Refund refund = refundService.Create(refundOptions);
            
            //Console.WriteLine("Refund processed successfully!");
        }
        public async Task<IActionResult> ListRecruitment()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            ListRecruitmentViewModel model = await employerRespository.GetEmployerRecruitments(employerId);
            return View(model);
        }
        public async Task<IActionResult> DeleteRecruitment(int recruitmentId)
        {
            var amount = await employerRespository.GetAmountMoneyForRecruitment(recruitmentId);
            HandleRefund(amount);
            var result = await employerRespository.DeleteSpecificRecruitment(recruitmentId);  
            if (result>0)
            {
                TempData["Success"] = "Your money will be back to your payment account in 3 to 5 days!";
                return RedirectToAction("ListRecruitment");
            }
            else
            {
                TempData["Error"] = "Server error!";
                return RedirectToAction("ListRecruitment");
            }
        }
        public async Task<IActionResult> EditRecruitment(int recruitmentId)
        {
            EditRecruitmentViewModel model = new EditRecruitmentViewModel();
            TINTUYENDUNG recruitment = await employerRespository.GetRecruitment(recruitmentId);
            List<HINHTHUCTRALUONG> paidTypes = await employerRespository.GetPaidTypes();
            List<KINHNGHIEM> experiences = await employerRespository.GetExperiences();
            List<TINHTHANHPHO> cities = await employerRespository.GetCities();
            List<LOAICONGVIEC> jobs = await employerRespository.GetJobs();
            List<HUYEN> districts = await employerRespository.GetDistricts();
           
            model.PaidTypes = paidTypes;
            model.Experiences = experiences;
            model.Cities = cities;
            model.Districts = districts;
            model.jobs = jobs;
            model.MaxSalary = recruitment.MaxSalary;
            model.MinSalary = recruitment.MinSalary;
            model.AgeRange = recruitment.Age;
            model.Gender = recruitment.Gender;
            model.IsFulltime = recruitment.FullTime;
            model.PostTime = recruitment.PostTime;
            model.TakeNotes = recruitment.Note;
            model.NumberVacancies = recruitment.MaxApplications;
            model.ExperienceId = recruitment.Experience.ExperienceId;
            model.PaidTypeId = recruitment.SalaryForm.SalaryFormId;
            model.DistrictId = recruitment.District.DistrictId;
            model.CityId = recruitment.District.City.CityId;
            model.RecruitmentId = recruitmentId;
            model.Address = recruitment.Address;
            List<LOAICONGVIEC> selectedJobs = new List<LOAICONGVIEC>();
            foreach(var c in recruitment.HouseworkDetails.ToList())
            {
                selectedJobs.Add(c.Job);
            }
            model.SelectedJobs = selectedJobs;

            return View(model);
        }
        public async Task<IActionResult> BackToEditRecruitment(EditRecruitmentViewModel model)
        {
            TINTUYENDUNG recruitment = await employerRespository.GetRecruitment(model.RecruitmentId);
            List<HINHTHUCTRALUONG> paidTypes = await employerRespository.GetPaidTypes();
            List<KINHNGHIEM> experiences = await employerRespository.GetExperiences();
            List<TINHTHANHPHO> cities = await employerRespository.GetCities();
            List<HUYEN> districts = await employerRespository.GetDistricts();
            List<LOAICONGVIEC> jobs = await employerRespository.GetJobs();
            model.PaidTypes = paidTypes;
            model.Experiences = experiences;
            model.Cities = cities;
            model.Districts = districts;
            model.jobs = jobs;

            List<LOAICONGVIEC> selectedJobs = new List<LOAICONGVIEC>();
            foreach (var c in recruitment.HouseworkDetails.ToList())
            {
                selectedJobs.Add(c.Job);
            }
            model.SelectedJobs = selectedJobs;

            return View("EditRecruitment",model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRecruitment(EditRecruitmentViewModel model)
        {
            var value0 = Request.Form["SelectedJobs"];
            // string[] selectedJobs = value0.ToString().Split(',');  
            var value1 = Request.Form["isFullTime"];
            bool isFullTime = value1 == "on";
            var value2 = Request.Form["min-age"];
            var value3 = Request.Form["max-age"];
            var value4 = Request.Form["PaidTypeId"];
            var value5 = Request.Form["ExperienceId"];
            var value6 = Request.Form["DistrictId"];
            var value7 = Request.Form["Gender"];
            
            var value8 = Request.Form["min-salary"].ToString().Replace(".", "");
            var value9 = Request.Form["max-salary"].ToString().Replace(".", "");
            if (value7 == "Null")
            {
                model.Gender = null;
            }
            else
            {
                model.Gender = value7;
            }
            model.PostTime = DateTime.Now;
            model.JobIds = value0;
            model.AgeRange = value2 + "-" + value3;
            model.PaidTypeId = int.Parse(value4);
            model.ExperienceId = int.Parse(value5);
            model.DistrictId = int.Parse(value6);
            model.IsFulltime = isFullTime;
            model.MinSalary = int.Parse(value8);
            model.MaxSalary = int.Parse(value9);
            bool result = await employerRespository.EditRecruitment(model);
            if (result)
            {
                TempData["Success"] = "Update recruitment successfully!";
                return RedirectToAction("ListRecruitment");
            }
            else
            {
                TempData["Error"] = "Server error!";
                return RedirectToAction("BackToEditRecruitment",model);
            }
        }

        public async Task<ActionResult> JobDetail(int recruitmentId)
        {
            Models.Views.Employee.JobDetailViewModel model = new Models.Views.Employee.JobDetailViewModel();
            model.Recruitment = await employerRespository.GetRecruitment(recruitmentId);
            return View(model);
        }

        public async Task<ActionResult> HideRecruitment(int recruitmentId)
        {
            var result = await employerRespository.HideRecruitment(recruitmentId);
            if (result)
            {
                TempData["Success"] = "Hide recruitment successfully!";
                
            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ListRecruitment");
        }
        public async Task<ActionResult> UnHideRecruitment(int recruitmentId)
        {
            var result = await employerRespository.UnHideRecruitment(recruitmentId);
            if (result)
            {
                TempData["Success"] = "UnHide recruitment successfully!";

            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ListRecruitment");
        }
        public async Task<ActionResult> ShowBidPrice(int recruitmentId)
        {
            var model = new BidPriceSettingViewModel();
            var onlineRecruitments = await employerRespository.GetOnlineRecruitments();
            model.OnlineRecruitments = new List<RecruitmentBidViewModel>();
            model.RecruitmentId = recruitmentId;
            var r = await employerRespository.GetRecruitment(recruitmentId);
            model.CurrentBidPrice = r.BidPrice;
            foreach (var recruitment in onlineRecruitments)
            {
                var bid = new RecruitmentBidViewModel();
                bid.MaxSalary = recruitment.MaxSalary;
                bid.MinSalary = recruitment.MinSalary;
                bid.BidPrice = recruitment.BidPrice;
                bid.MaxApplications = recruitment.MaxApplications;
                bid.RecruitDeadlineDate = recruitment.RecruitDeadlineDate;
                bid.PostTime = recruitment.PostTime;
                bid.RecruiterName = recruitment.Employer.LastName + " " + recruitment.Employer.FirstName;
                model.OnlineRecruitments.Add(bid);
            }
            return View("AddBidPrice", model);
        }
        public async Task<ActionResult> AddBidPrice(BidPriceSettingViewModel model)
        {
            TempData["Success"] = "Please check out!";
            var value1 = Request.Form["bidAmount"].ToString();
            model.Price = decimal.Parse(value1.Replace(".",""));
         
            return View("AddCheckOut",model);
        }
        public async Task<ActionResult> AddBidPriceCheckOut(int recruitmentId,decimal bidPrice)
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            bool result0 = HandlePaidBill((int)bidPrice, employerId);
            if (!result0)
            {
                TempData["Error"] = "Fail to paid by credit card";
                return RedirectToAction("ShowBidPrice", recruitmentId);
            }
            var result = await employerRespository.AddBidPrice(recruitmentId,bidPrice);
            
            if (result)
            {
                TempData["Success"] = "Add bid price to recruitment successfully!";

            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ListRecruitment");
        }

        public async Task<ActionResult> ShowPricePacket(int recruitmentId)
        {
            var model = new PricePacketSettingViewModel();
            var priceTags = await employerRespository.GetPriceTags();
            model.RecruitmentId = recruitmentId;
            model.PriceTags = priceTags;
            return View("AddPricePacket", model);
        }
        public async Task<ActionResult> AddPricePacket(int recruitmentId, int pricePacketId)
        {
            TempData["Success"] = "Please check out!";
            var model = new PricePacketSettingViewModel();
            model.PricePacket = await employerRespository.GetPricePacket(pricePacketId);
            model.PricePacketId = pricePacketId;
            model.RecruitmentId = recruitmentId;

            return View("AddPricePacketCheckOut", model);
        }
        public async Task<ActionResult> AddPricePacketCheckOut(int recruitmentId, int pricePacketId)
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            var pricePacket = await employerRespository.GetPricePacket(pricePacketId);
            bool result0 = HandlePaidBill((int)pricePacket.Price, employerId);
            if (!result0)
            {
                TempData["Error"] = "Fail to paid by credit card";
                return RedirectToAction("ShowPricePacket", recruitmentId);
            }

            var result = await employerRespository.ExtendDeadLine(recruitmentId,pricePacketId);
            if (result)
            {
                TempData["Success"] = "Extend deadline to recruitment successfully!";

            }
            else
            {
                TempData["Error"] = "Server error!";
            }
            return RedirectToAction("ListRecruitment");
        }

        public async Task<IActionResult> EditReasonRecruitment(int recruitmentId)
        {
            EditReasonRecruitmentViewModel model = new EditReasonRecruitmentViewModel();
            TINTUYENDUNG recruitment = await employerRespository.GetRecruitment(recruitmentId);
            List<HINHTHUCTRALUONG> paidTypes = await employerRespository.GetPaidTypes();
            List<KINHNGHIEM> experiences = await employerRespository.GetExperiences();
            List<TINHTHANHPHO> cities = await employerRespository.GetCities();
            List<LOAICONGVIEC> jobs = await employerRespository.GetJobs();
            List<HUYEN> districts = await employerRespository.GetDistricts();
            Dictionary<DateTime,List<CHITIETTUCHOI>> rejectionDetails = await employerRespository.GetRejectionDetails(recruitmentId);
            var sortedDictionary = rejectionDetails.OrderByDescending(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
            model.PaidTypes = paidTypes;
            model.Experiences = experiences;
            model.Cities = cities;
            model.Districts = districts;
            model.jobs = jobs;
            model.MaxSalary = recruitment.MaxSalary;
            model.MinSalary = recruitment.MinSalary;
            model.AgeRange = recruitment.Age;
            model.Gender = recruitment.Gender;
            model.IsFulltime = recruitment.FullTime;
            model.PostTime = recruitment.PostTime;
            model.TakeNotes = recruitment.Note;
            model.NumberVacancies = recruitment.MaxApplications;
            model.ExperienceId = recruitment.Experience.ExperienceId;
            model.PaidTypeId = recruitment.SalaryForm.SalaryFormId;
            model.DistrictId = recruitment.District.DistrictId;
            model.CityId = recruitment.District.City.CityId;
            model.RecruitmentId = recruitmentId;
            model.Address = recruitment.Address;
            model.RejectionDetails = sortedDictionary;
            List<LOAICONGVIEC> selectedJobs = new List<LOAICONGVIEC>();
            foreach (var c in recruitment.HouseworkDetails.ToList())
            {
                selectedJobs.Add(c.Job);
            }
            model.SelectedJobs = selectedJobs;

            return View(model);
        }

        public async Task<IActionResult> Profile()
        {
            EmployerProfileViewModel model = new EmployerProfileViewModel();
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            model.Employer = await employerRespository.GetEmployer(employerId);
            return View(model);
        }

        public async Task<IActionResult> ShowSuitableCandidates()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            ListCandidatesViewModel model = await employerRespository.GetSuitableCandidates(employerId);
            return View("EmployeeProposer", model);
        }
        public async Task<IActionResult> ShowDetailCandidateProfile(int employeeId)
        {
            Models.Views.Employee.EmployeeProfileViewModel model = new Models.Views.Employee.EmployeeProfileViewModel();
            model.Employee = await employeeRespository.GetEmployee(employeeId);
            model.Districts = await employeeRespository.GetWorkplacesForEmployee(employeeId);
            model.Jobs = await employeeRespository.GetJobsForEmployee(employeeId);
            return View("CandidateProfile", model);
        }
       

    }
}
