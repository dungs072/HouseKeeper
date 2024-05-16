using HouseKeeper.Enum;
using HouseKeeper.IServices;
using HouseKeeper.Models;
using HouseKeeper.Models.DB;
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
        public HomeController(ILogger<HomeController> logger, 
                IAccountTypeRespository accountTypeRespository, 
                IEmployeeRespository employeeRespository,
                IFirebaseService firebaseService
            )
        {
            _logger = logger;
            this.accountTypeRespository = accountTypeRespository;
            this.employeeRespository = employeeRespository;
            this.firebaseService = firebaseService;
        }

        public IActionResult Index()
        {
            List<LOAITK> accountTypes = accountTypeRespository.GetAccounts().Result;
            return View(accountTypes);
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
        public IActionResult IndexEmployer()
        {
            return View();
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
            else if(result== (int)AccountEnum.LoginResult.WrongPassword)
            {
                TempData["Error"] = "Your password is wrong.";
                return RedirectToAction("ReturnToLogin", model);
            }
            var loginInfor = await accountTypeRespository.GetEmployerOrEmployee(result);
            HttpContext.Session.SetString("UserId", loginInfor.Id.ToString());
            if (loginInfor.ViewIndex == (int)AccountEnum.AccountType.Employer)
            {
                return View("IndexEmployer");
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Employee)
            {
                return RedirectToAction("DisplayList", "Employee",1);
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Staff)
            {
                return RedirectToAction("Index","Staff");
            }
            else if(loginInfor.ViewIndex == (int)AccountEnum.AccountType.Admin)
            {
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
            model.Districts = accountTypeRespository.GetDistricts().Result;
            return View(model);
        }
        public IActionResult ReturnToSignUp(SignUpViewModels model)
        {
            List<TINHTHANHPHO> cities = accountTypeRespository.GetCities().Result;
            model.Cities = cities;
            model.Districts = accountTypeRespository.GetDistricts().Result;
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
            account.Gmail = model.Gmail;
            account.Password = model.Password;
            account.AvatarUrl = "data:image/jpeg;base64,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";
            DANHTINH identity = new DANHTINH();
            identity.CitizenNumber = model.CitizenNumber;


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
                NGUOITHUE employer = new NGUOITHUE();
                employer.Address = model.Address;
                employer.FirstName = model.FirstName;
                employer.LastName = model.LastName;
                employer.District = await accountTypeRespository.GetDistrict(model.DistrictId);
                employer.Account = account;
                identity.FrontImage = await firebaseService.UploadImage(frontImage, AccountEnum.AccountType.Employer, ImageEnum.ImageType.Identity);
                identity.BackImage = await firebaseService.UploadImage(backImage, AccountEnum.AccountType.Employer, ImageEnum.ImageType.Identity);

                employer.Identity = identity;
                employer.IdentityState = await accountTypeRespository.GetIdentityState((int)IdentityEnum.IdentiyStatus.Waiting);
                int result = await accountTypeRespository.CreateEmployerAccount(account, employer, identity);
               
                if (result == (int)AccountEnum.CreateAccountResult.Success)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if(result == (int)AccountEnum.CreateAccountResult.PhoneDuplicated)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if(result == (int)AccountEnum.CreateAccountResult.GmailDuplicated)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateAccountResult.ServerError)
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
                NGUOIGIUPVIEC employee = new NGUOIGIUPVIEC();
                if(model.BirthDate.Year!=1)
                {
                    employee.BirthDate = model.BirthDate;
                }
                //employee.City = city;
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.Account = account;
                employee.District = await accountTypeRespository.GetDistrict(model.DistrictId);
                employee.Gender = model.Gender;
                identity.FrontImage = await firebaseService.UploadImage(frontImage, AccountEnum.AccountType.Employee, ImageEnum.ImageType.Identity);
                identity.BackImage = await firebaseService.UploadImage(backImage, AccountEnum.AccountType.Employee, ImageEnum.ImageType.Identity);

                employee.Identity = identity;

                int result = await accountTypeRespository.CreateEmployeeAccount(account, employee, identity);
                if (result == (int)AccountEnum.CreateAccountResult.Success)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if (result == (int)AccountEnum.CreateAccountResult.PhoneDuplicated)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateAccountResult.GmailDuplicated)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == (int)AccountEnum.CreateAccountResult.ServerError)
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
            LoginViewModel model = new LoginViewModel();
            return View("Login",model);
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        public IActionResult Privacy()
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
