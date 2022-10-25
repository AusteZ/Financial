using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections;
using NuGet.Packaging.Signing;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static FinanceModel wholeProgram = new FinanceModel();
        private static SettingsModel Settings = new SettingsModel();
        

        //private static UserModel user = new UserModel();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (wholeProgram.User.Email == "" || wholeProgram.User.Password == "")
            {
                return RedirectToAction(nameof(Login));
            }
            if(wholeProgram.UserFinanceList.Count() == 0)
            {
                List<BaseMoneyModel> foruser, forothers;
                Load<BaseMoneyModel>("data.json", out foruser, out forothers);
                wholeProgram.UserFinanceList = foruser;
                wholeProgram.AllOtherUsersFinanceList = forothers;
                Settings.PresentList = wholeProgram.UserFinanceList;
            }
            
            wholeProgram.Statistics.SetList(wholeProgram.UserFinanceList.ToList());
            SettingsModel.ErrorFlag = false;

            return View(Settings);
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
            wholeProgram.User = _user;
            List<UserModel> foruser;
            Load<UserModel>("users.json", out foruser, out _);
            if (foruser.Count != 1 || foruser[0].Password != wholeProgram.User.Password)
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
        public IActionResult ExpensesForm(int id = -1, int type = 0)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            if (id != -1)
            {
                foreach(var exp in wholeProgram.UserFinanceList)
                {
                    if(exp.Index == id)
                    {
                        expense = exp;
                        wholeProgram.UserFinanceList.Remove(expense);
                        break;
                    }
                }
            }
            if (type == 1) expense.isExpense = false;
            return View(expense);
        }

        public IActionResult CategoriesForm(BaseMoneyModel category = null)
        {
            category = category ?? new BaseMoneyModel();
            return View(category);
        }
        public IActionResult ExpenseLine(BaseMoneyModel bmm)
        {
            bmm.Email = wholeProgram.User.Email;
            if (bmm.isExpense) bmm.Amount = -Math.Abs(bmm.Amount);
            wholeProgram.UserFinanceList.Add(bmm);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExpenseLineDelete(int id = -1, int type = 0)
        {
            foreach(var exp in wholeProgram.UserFinanceList)
            {
                if(exp.Index == id)
                {
                    wholeProgram.UserFinanceList.Remove(exp);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Save()
        {
            List<BaseMoneyModel> list = new List<BaseMoneyModel>();
            if (wholeProgram.UserFinanceList.Count() > 0) list.AddRange(wholeProgram.UserFinanceList.ToList());
            if (wholeProgram.AllOtherUsersFinanceList.Count() > 0) list.AddRange(wholeProgram.AllOtherUsersFinanceList.ToList());
            var a = JsonConvert.SerializeObject(list.ToArray());
            try
            {
                System.IO.File.WriteAllText("data.json", a);
            }
            catch(Exception e)
            {
                var b = " d";
            }
            return RedirectToAction(nameof(Index));
        }
        public void Load<T>(string filename, out List<T> foruser, out List<T> forothers) where T : LinkingEmail
        {
            foruser = new List<T>();
            forothers = new List<T>();
            IEnumerable list;
            if (System.IO.File.Exists(filename) && (new FileInfo(filename)).Length != 0)
            {
                var filestream = System.IO.File.ReadAllText(filename);
                list = (JsonConvert.DeserializeObject<T[]>(filestream)).ToList();
                foreach (T item in list)
                {
                    if (item.Email == wholeProgram.User.Email)
                    {
                        foruser.Add(item);
                    }
                    else
                    {
                        forothers.Add(item);
                    }
                }
            }
            else
            {
                (System.IO.File.Create(filename)).Close();
            }
        }
        public IActionResult Sort(SettingsModel sm)
        {
            Settings.SortType = sm.SortType;
            var _list = new List<BaseMoneyModel>(wholeProgram.UserFinanceList);
            var orderByResult = from s in _list select s;
            if (Settings.SortType == "Name")
            {
                orderByResult = from s in _list
                                orderby s.Product descending
                                select s;
            }
            else if(Settings.SortType == "Place")
            {
                orderByResult = from s in _list
                                orderby s.Place descending
                                select s;
            }
            else if (Settings.SortType == "Amount")
            {
                orderByResult = from s in _list
                                orderby s.Amount descending
                                select s;
            }
            else if (Settings.SortType == "Date")
            {
                orderByResult = from s in _list
                                orderby s.Date descending
                                select s;
            }
            wholeProgram.UserFinanceList.Clear();
            wholeProgram.UserFinanceList.AddRange(orderByResult);
            Settings.PresentList = wholeProgram.UserFinanceList;
            //return RedirectToAction(nameof(Expenses));
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Filter(SettingsModel sm)
        {
            Settings.From = sm.From;
            Settings.To = sm.To;
            Settings.IsExpense = sm.IsExpense;
            Settings.IsIncome = sm.IsIncome;
            var query = from s in wholeProgram.UserFinanceList select s;
            if (Settings.From.CompareTo(DateTime.Now) < 0)
            {
                query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) > 0
                        select finance;
            }
            if (Settings.To.CompareTo(DateTime.Now) < 0)
            {
                query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) < 0
                        select finance;
            }
             
            query = from finance in query
                    where (finance.isExpense == true && Settings.IsExpense == true) || (finance.isExpense == false && Settings.IsIncome == true)
                    select finance;

            Settings.PresentList = new List<BaseMoneyModel>(query);

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
            List<UserModel> foruser, forall;
            Load<UserModel>("users.json", out foruser, out forall);
            if (foruser.Count == 0) {
                wholeProgram.User = um;
                if (wholeProgram.User.ConfirmPassword == wholeProgram.User.Password)
                {
                    forall.Add(wholeProgram.User);
                    System.IO.File.WriteAllText("users.json", JsonConvert.SerializeObject(forall));
                }
                else
                {
                    SettingsModel.ErrorFlag = true;
                    return RedirectToAction(nameof(Register));
                }
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