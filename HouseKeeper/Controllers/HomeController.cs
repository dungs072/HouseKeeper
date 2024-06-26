﻿using Firebase.Auth;
using HouseKeeper.Configs;
using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views;
using HouseKeeper.Models.Views.Employee;
using HouseKeeper.Models.Views.OutPage;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HouseKeeper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountTypeRespository accountTypeRespository;
        private readonly IEmployeeRespository employeeRespository;
        private readonly IFirebaseService firebaseService;
        private readonly ITokenService tokenService;
        private int page = 1;
        private readonly IPasswordService passwordService;
        public HomeController(ILogger<HomeController> logger, 
                IAccountTypeRespository accountTypeRespository, 
                IEmployeeRespository employeeRespository,
                IFirebaseService firebaseService,
                IPasswordService passwordService,
                ITokenService tokenService)
        {
            _logger = logger;
            this.accountTypeRespository = accountTypeRespository;
            this.employeeRespository = employeeRespository;
            this.firebaseService = firebaseService;
            this.passwordService = passwordService;
            this.tokenService = tokenService;
        }

        public async Task<IActionResult> Index()
        {
            //List<LOAITK> accountTypes = accountTypeRespository.GetAccounts().Result;
            ListRecruitmentViewModel model = new ListRecruitmentViewModel();
            model.Recruitments = await employeeRespository.GetRecruitments(page, "", null, null);
            model.Cities = await employeeRespository.GetCities();
            model.Districts = await employeeRespository.GetDistricts();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ShowSearch(ListRecruitmentViewModel model)
        {
            model.Cities = await employeeRespository.GetCities();
            model.Districts = await employeeRespository.GetDistricts();

            return View("Index",model);
        }
        public async Task<ActionResult> LoadMoreItems(int currentPage, string searchKey, int? cityId, int? districtId)
        {
            cityId = cityId == -999 ? null : cityId;
            districtId = districtId == -999 ? null : districtId;
            var items = await employeeRespository.GetRecruitments(currentPage, searchKey, cityId, districtId);
            Models.Views.Employee.ListRecruitmentViewModel model = new Models.Views.Employee.ListRecruitmentViewModel();
            model.Recruitments = items;
            model.SearchKey = searchKey;
            model.CityId = cityId;
            model.DistrictId = districtId;
            return PartialView("ListRecruitmentPartital", model);
        }
        public async Task<ActionResult> SearchJob(string keyword, int? cityId, int? districtId)
        {
            cityId = cityId == -999 ? null : cityId;
            districtId = districtId == -999 ? null : districtId;
            ListRecruitmentViewModel listRecruitmentViewModel = new ListRecruitmentViewModel();
            listRecruitmentViewModel.Recruitments = await employeeRespository.GetRecruitments(1, keyword, cityId, districtId);
            listRecruitmentViewModel.Cities = await employeeRespository.GetCities();
            listRecruitmentViewModel.Districts = await employeeRespository.GetDistricts();
            listRecruitmentViewModel.CityId = cityId;
            listRecruitmentViewModel.DistrictId = districtId;
            listRecruitmentViewModel.SearchKey = keyword;
            return View("Index", listRecruitmentViewModel);
        }
        public IActionResult Login()
        {
            LoginViewModel viewModel = new LoginViewModel();
            return View(viewModel);
        }
        public IActionResult ReturnToLogin(LoginViewModel model)
        {
            return View("Login", model);
        }
        public async Task<IActionResult> IndexEmployer()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int employerId);
            NGUOITHUE employer = await accountTypeRespository.GetEmployer(employerId);
            TempData["Error"] = null;
            TempData["Success"] = null;
            return View(employer);
        }
        [HttpPost]
        public async Task<IActionResult> HandleLogin(LoginViewModel model)
        {
            int result = await accountTypeRespository.Login(model);
            if(result==(int)AccountEnum.LoginResult.PhoneAndGmailNotRegistered)
            {
                TempData["Error"] = "Your email or phone number is not registered. Account does not exist";
                return RedirectToAction("ReturnToLogin", model);
            }
            else if (result == (int)AccountEnum.LoginResult.Locked)
            {
                TempData["Error"] = "Your account is locked. Please contact us for more information.";
                return RedirectToAction("ReturnToLogin", model);
            }
            else if(result== (int)AccountEnum.LoginResult.WrongPassword)
            {
                TempData["Error"] = "Your password is wrong.";
                return RedirectToAction("ReturnToLogin", model);
            }
            var loginInfor = await accountTypeRespository.GetEmployerOrEmployee(result);
            
            HttpContext.Session.SetString("UserId", loginInfor.Id.ToString());
            string token = tokenService.GenerateToken(loginInfor.Id.ToString());
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
            if (loginInfor.ViewIndex == (int)AccountEnum.AccountType.Employer)
            {
                NGUOITHUE employer = await accountTypeRespository.GetEmployer(loginInfor.Id);
               
                Response.Cookies.Append("Role", "Employer", new CookieOptions
                {
                    HttpOnly=true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                return View("IndexEmployer",employer);
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Employee)
            {
                Response.Cookies.Append("Role", "Employee", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                return RedirectToAction("DisplayList", "Employee",1);
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Staff)
            {
                Response.Cookies.Append("Role", "Staff", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                return RedirectToAction("Index","Staff");
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Admin)
            {
                Response.Cookies.Append("Role", "Admin", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                return RedirectToAction("Index","Admin");
            }
            return View();
        }
        public IActionResult SignUp()
        {
            TempData["Success"] = null;
            TempData["Error"] = null;
            SignUpViewModels model = new SignUpViewModels();
            List<TINHTHANHPHO> cities = accountTypeRespository.GetCities().Result;
            model.Cities = cities;
            model.FrontImage = DefaultImageUrlConfig.FrontImage;
            model.BackImage = DefaultImageUrlConfig.BackImage;
            model.Districts = accountTypeRespository.GetDistricts().Result;
            return View(model);
        }
        public IActionResult ReturnToSignUp(SignUpViewModels model)
        {
            List<TINHTHANHPHO> cities = accountTypeRespository.GetCities().Result;
            model.Cities = cities;
            model.Districts = accountTypeRespository.GetDistricts().Result;
            model.FrontImage = DefaultImageUrlConfig.FrontImage;
            model.BackImage = DefaultImageUrlConfig.BackImage;
            return View("SignUp",model);
        }



        [HttpPost]
        public async Task<IActionResult> HandleSignUp(SignUpViewModels model, IFormFile frontImage, IFormFile backImage)
        {
            var value1 = Request.Form["isEmployer"];
            //var value2 = Request.Form["isEmployee"];
            bool isEmployer = value1=="True";
            bool isEmployee = !isEmployer;
            TAIKHOAN account = new TAIKHOAN();
            account.PhoneNumber = model.PhoneNumber;
            if(model.Gmail!=null)
            {
                account.Gmail = model.Gmail.Trim();
            }
            
            account.Password = passwordService.HashPassword(model.Password);
            account.Status = true;
            DANHTINH identity = new DANHTINH();
            identity.CitizenNumber = model.CitizenNumber.Trim();


            model.IsEmployer = isEmployer;
            model.IsEmployee = isEmployee;
            if (model.Password != model.ConfirmedPassword)
            {
                TempData["Error"] = "Your confirmed password does not match.";
                return RedirectToAction("ReturnToSignUp", model);
            }

            if (isEmployer)
            {
                var accountType = await accountTypeRespository.GetSpecificAccountType((int)AccountEnum.AccountType.Employer);
                account.AccountType = accountType;
                account.AvatarUrl = DefaultImageUrlConfig.DefaultAvatarEmployer;
                NGUOITHUE employer = new NGUOITHUE();
                employer.Address = model.Address.Trim();
                employer.FirstName = model.FirstName.Trim();
                employer.LastName = model.LastName.Trim();
                employer.District = await accountTypeRespository.GetDistrict(model.DistrictId);
                employer.Account = account;
                identity.FrontImage = await firebaseService.UploadImage(frontImage, AccountEnum.AccountType.Employer, ImageEnum.ImageType.Identity);
                identity.BackImage = await firebaseService.UploadImage(backImage, AccountEnum.AccountType.Employer, ImageEnum.ImageType.Identity);

                employer.Identity = identity;
                employer.IdentityState = await accountTypeRespository.GetIdentityState((int)IdentityEnum.IdentiyStatus.Waiting);
                int result = await accountTypeRespository.CreateEmployerAccount(account, employer, identity);
               
                if (result == (int)AccountEnum.CreateEditAccountResult.Success)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if(result == (int)AccountEnum.CreateEditAccountResult.PhoneDuplicated)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if(result == (int)AccountEnum.CreateEditAccountResult.GmailDuplicated)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateEditAccountResult.ServerError)
                {
                    TempData["Error"] = "Server error.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
            }
            else if(isEmployee)
            {
                string selectedValue = Request.Form["cityOrProvince"];
                var accountType = await accountTypeRespository.GetSpecificAccountType(3);
                var city = await accountTypeRespository.GetSpecificCity(int.Parse(selectedValue));
                account.AccountType = accountType;
                account.AvatarUrl = DefaultImageUrlConfig.DefaultAvatarEmployee;
                NGUOIGIUPVIEC employee = new NGUOIGIUPVIEC();
                if(model.BirthDate.Year!=1)
                {
                    employee.BirthDate = model.BirthDate;
                }
                //employee.City = city;
                employee.FirstName = model.FirstName.Trim();
                employee.LastName = model.LastName.Trim();
                employee.Address = model.Address.Trim();
                employee.Account = account;
                employee.District = await accountTypeRespository.GetDistrict(model.DistrictId);
                employee.Gender = model.Gender;
                identity.FrontImage = await firebaseService.UploadImage(frontImage, AccountEnum.AccountType.Employee, ImageEnum.ImageType.Identity);
                identity.BackImage = await firebaseService.UploadImage(backImage, AccountEnum.AccountType.Employee, ImageEnum.ImageType.Identity);

                employee.Identity = identity;
                employee.IdentityState = await accountTypeRespository.GetIdentityState((int)IdentityEnum.IdentiyStatus.Waiting);
                int result = await accountTypeRespository.CreateEmployeeAccount(account, employee, identity);
                if (result == (int)AccountEnum.CreateEditAccountResult.Success)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if (result == (int)AccountEnum.CreateEditAccountResult.PhoneDuplicated)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateEditAccountResult.GmailDuplicated)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateEditAccountResult.ServerError)
                {
                    TempData["Error"] = "Server error.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
            }
            return RedirectToAction("Login");
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("UserId", "-1");
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("Role");
            //LoginViewModel model = new LoginViewModel();
            return RedirectToAction("Index");
        }
        public IActionResult ForgetPassword()
        {
            ForgetPasswordViewModel model = new ForgetPasswordViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            string gmail = Request.Form["email"];
            model.Gmail = gmail;
            var result = await accountTypeRespository.HandleForgetPassword(model);
            if(result)
            {
                TempData["Success"] = "We send you the random password to your email. Please check carefully.";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["Error"] = "Gmail is not exist or registered.";
                return View(model);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult TermConditions()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       


    }
}
