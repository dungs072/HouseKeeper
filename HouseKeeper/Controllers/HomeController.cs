﻿using HouseKeeper.Models;
using HouseKeeper.Models.DB;
using HouseKeeper.Models.Views;
using HouseKeeper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HouseKeeper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountTypeRespository accountTypeRespository;
        public HomeController(ILogger<HomeController> logger, 
                IAccountTypeRespository accountTypeRespository)
        {
            _logger = logger;
            this.accountTypeRespository = accountTypeRespository;
        }

        public IActionResult Index()
        {
            List<LOAITK> accountTypes = accountTypeRespository.GetAccounts().Result;
            return View(accountTypes);
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            TempData["Success"] = null;
            TempData["Error"] = null;
            SignUpViewModels model = new SignUpViewModels();
            List<TINHTHANHPHO> cities = accountTypeRespository.GetCities().Result;
            model.Cities = cities;
            return View(model);
        }
        public IActionResult ReturnToSignUp(SignUpViewModels model)
        {
            
            return View("SignUp",model);
        }
        [HttpPost]
        public async Task<IActionResult> HandleSignUp(SignUpViewModels model)
        {
            var value1 = Request.Form["isEmployer"];
            var value2 = Request.Form["isEmployee"];
            bool isEmployer = value1=="on";
            bool isEmployee = value2 == "on";
            TAIKHOAN account = new TAIKHOAN();
            account.PhoneNumber = model.PhoneNumber;
            account.Gmail = model.Gmail;
            account.Password = model.Password;
            model.IsEmployer = isEmployer;
            model.IsEmployee = isEmployee;
            if (model.Password != model.ConfirmedPassword)
            {
                TempData["Error"] = "Your confirmed password does not match.";
                return RedirectToAction("ReturnToSignUp", model);
            }

            if (isEmployer)
            {
                var accountType = await accountTypeRespository.GetSpecificAccountType(2);
                account.AccountType = accountType;
                NGUOITHUE employer = new NGUOITHUE();
                employer.Address = model.Address;
                employer.FirstName = model.FirstName;
                employer.LastName = model.LastName;
                employer.Account = account;
                int result = await accountTypeRespository.CreateEmployerAccount(account, employer);
                List<TINHTHANHPHO> cities = accountTypeRespository.GetCities().Result;
                model.Cities = cities;
                if (result==1)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if(result==2)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if(result==3)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result==4)
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
                employee.BirthDate = model.BirthDate;
                employee.City = city;
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.Account = account;
                int result = await accountTypeRespository.CreateEmployeeAccount(account, employee);
                if (result == 1)
                {
                    TempData["Success"] = "Create new account successfully.";
                }
                else if (result == 2)
                {
                    TempData["Error"] = "Phone number is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == 3)
                {
                    TempData["Error"] = "Gmail is duplicated.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
                else if (result == 4)
                {
                    TempData["Error"] = "Server error.";
                    return RedirectToAction("ReturnToSignUp", model);
                }
            }
            return RedirectToAction("Login");
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