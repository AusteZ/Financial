using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static FinanceModel wholeProgram = new FinanceModel();

        private static UserModel user = new UserModel();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(wholeProgram.User.Email == "" || wholeProgram.User.Password == "")
            {
                return RedirectToAction(nameof(Login));
            }
            if(wholeProgram.UserFinanceList.Count() == 0)
            {
                List<BaseMoneyModel> foruser, forothers;
                Load<BaseMoneyModel>("data.txt", out foruser, out forothers);
                wholeProgram.UserFinanceList = new BaseMoneyListModel(foruser);
                wholeProgram.AllOtherUsersFinanceList = new BaseMoneyListModel(forothers);
            }
            
            wholeProgram.Statistics.SetList(wholeProgram.UserFinanceList.ToList());
            return View(wholeProgram.Statistics);

            //return RedirectToAction(nameof(Index1));
        }
        public IActionResult Index1()
        {
            return View(wholeProgram);
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
            Load<UserModel>("users.txt", out foruser, out _);
            if (foruser.Count != 1 || foruser[0].Password != wholeProgram.User.Password)
                wholeProgram.User = new UserModel();
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
            return RedirectToAction(nameof(Expenses));
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

            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult Save()
        {
            List<BaseMoneyModel> list = new List<BaseMoneyModel>();
            if (wholeProgram.UserFinanceList.Count() > 0) list.AddRange(wholeProgram.UserFinanceList.ToList());
            if (wholeProgram.AllOtherUsersFinanceList.Count() > 0) list.AddRange(wholeProgram.AllOtherUsersFinanceList.ToList());
            var a = JsonConvert.SerializeObject(list.ToArray());
            try
            {
                System.IO.File.WriteAllText("data.txt", a);
            }
            catch(Exception e)
            {
                var b = " d";
            }
            return RedirectToAction(nameof(Expenses));
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
        public IActionResult Sort()
        {
            var _list = wholeProgram.UserFinanceList.ToList();
            var orderByResult = from s in _list
                                orderby s.Amount descending
                                select s;
            wholeProgram.UserFinanceList = new BaseMoneyListModel(orderByResult);
            return RedirectToAction(nameof(Expenses));
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
            Load<UserModel>("users.txt", out foruser, out forall);
            if (foruser.Count == 0) {
                wholeProgram.User = um;
                if (wholeProgram.User.Email != "" && wholeProgram.User.Password != "")
                {
                    forall.Add(wholeProgram.User);
                    System.IO.File.WriteAllText("users.txt", JsonConvert.SerializeObject(forall));
                }
                else
                {
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