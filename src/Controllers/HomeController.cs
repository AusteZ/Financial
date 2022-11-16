using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections;
using NuGet.Packaging.Signing;
using Microsoft.Data.SqlClient;
using Financial.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static FinanceModel wholeProgram = new FinanceModel();
        private static SettingsModel settings = new SettingsModel();
        private FinanceContext _context;
        private DbSet<UserModel> _users;
        private DbSet<BaseMoneyModel> _finances;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, FinanceContext context)
        {
            _logger = logger;
            _context = context;
            _users = context.users;
            _finances = context.finances;
        }

        public IActionResult Index()
        {
            if (wholeProgram.User.Email == "" || wholeProgram.User.Password == "")
            {
                return RedirectToAction(nameof(Login));
            }
            if(wholeProgram.UserFinanceList.Count() == 0)
            {
                var _list = from one in _finances
                            where one.Email == wholeProgram.User.Email
                            select one;
                wholeProgram.UserFinanceList = new List<BaseMoneyModel>(_list);
                settings.PresentList = wholeProgram.UserFinanceList;
            }
            
            wholeProgram.Statistics.SetList(wholeProgram.UserFinanceList.ToList());
            SettingsModel.ErrorFlag = false;

            return View(settings);
        }
        public IActionResult StatisticsSettings(StatisticsModel sm)
        {
            wholeProgram.Statistics = sm;
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Login()
        {
            return View(new UserModel());
        }
        public IActionResult LoginForm(UserModel _user)
        {
            var _userTemp = from one in _users
                            where one.Email == _user.Email && one.Password == _user.Password
                            select one;
            if (_userTemp.Any()) wholeProgram.User = _userTemp.First();
            else
            {
                wholeProgram.User = new UserModel();
                SettingsModel.ErrorFlag = true;
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Expenses()
        {
            return View(wholeProgram.UserFinanceList);
        }
        [Route("Home/ExpensesForm/{id?}")]
        public IActionResult ExpensesForm(string id, int type = 0)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            foreach(var exp in wholeProgram.UserFinanceList)
            {
                if(exp.Id.ToString() == id)
                {
                    expense = exp;
                    wholeProgram.UserFinanceList.Remove(expense);
                    _finances.Remove(expense);
                    id = "";
                    break;
                }
            }
            if (id != "") expense.Id = new Guid();
            if (type == 1) expense.isExpense = false;
            return View(expense);
        }

        public IActionResult ExpenseLine(BaseMoneyModel bmm)
        {
            bmm.Email = wholeProgram.User.Email;
            if (bmm.isExpense) bmm.Amount = -Math.Abs(bmm.Amount);
            bmm.Category ??= "";
            bmm.Place ??= "";
            wholeProgram.UserFinanceList.Add(bmm);
            _finances.Add(bmm);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExpenseLineDelete(string id, int type = 0)
        {
            foreach(var exp in wholeProgram.UserFinanceList)
            {
                if(exp.Id.ToString() == id)
                {
                    wholeProgram.UserFinanceList.Remove(exp);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Sort(SettingsModel sm)
        {
            settings.SortType = sm.SortType;
            SettingsModel.Sort(settings);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Filter(SettingsModel sm)
        {
            settings.From = sm.From;
            settings.To = sm.To;
            settings.IsExpense = sm.IsExpense;
            settings.IsIncome = sm.IsIncome;
            settings.PresentList = new List<BaseMoneyModel>(wholeProgram.UserFinanceList);
            SettingsModel.Filter(settings);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            wholeProgram = new FinanceModel();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult RegisterForm(UserModel um)
        {
            var _usertemp = from one in _users
                            where one.Email == um.Email
                            select one;
            if (!_usertemp.Any())
            {
                wholeProgram.User = um;
                wholeProgram.User.Id = Guid.NewGuid();
                _users.Add(um);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}