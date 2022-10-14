using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static BaseMoneyListModel allotheruserlist = new BaseMoneyListModel();
        private static BaseMoneyListModel userlist = new BaseMoneyListModel();
        private static StatisticsModel statistics = new StatisticsModel(userlist.ToList());


        private static UserModel user = new UserModel();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(user.Email == "" || user.Email == "no" || user.Password == "" || user.Password == "no")
            {
                return RedirectToAction(nameof(Login));
            }
            if(userlist.Count() == 0)
            {
                List<BaseMoneyModel> foruser, forothers;
                Load<BaseMoneyModel>("data.txt", out foruser, out forothers);
                userlist = new BaseMoneyListModel(foruser);
                allotheruserlist = new BaseMoneyListModel(forothers);
            }
            
            statistics.SetList(userlist.ToList());
            return View(statistics);
        }
        public IActionResult StatisticsSettings(StatisticsModel sm)
        {
            statistics = sm;
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Login()
        {
            return View(new UserModel());
        }
        public IActionResult LoginForm(UserModel _user)
        {
            user = _user;
            List<UserModel> foruser;
            Load<UserModel>("users.txt", out foruser, out _);
            if (foruser.Count != 1 || foruser[0].Password != user.Password)
                user = new UserModel();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Expenses()
        {
            return View(userlist);
        }
        [Route("Home/ExpensesForm/{id?}")]
        public IActionResult ExpensesForm(int id = -1, int type = 0)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            if (id != -1)
            {
                foreach(var exp in userlist)
                {
                    if(exp.Index == id)
                    {
                        expense = exp;
                        userlist.Remove(expense);
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
            bmm.Email = user.Email;
            if (bmm.isExpense) bmm.Amount = -Math.Abs(bmm.Amount);
            userlist.Add(bmm);
            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult ExpenseLineDelete(int id = -1, int type = 0)
        {
            foreach(var exp in userlist)
            {
                if(exp.Index == id)
                {
                    userlist.Remove(exp);
                }
            }

            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult Save()
        {
            List<BaseMoneyModel> list = new List<BaseMoneyModel>();
            if (userlist.Count() > 0) list.AddRange(userlist.ToList());
            if (allotheruserlist.Count() > 0) list.AddRange(allotheruserlist.ToList());
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
                    if (item.Email == user.Email)
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
            var _list = userlist.ToList();
            var orderByResult = from s in _list
                                orderby s.Amount descending
                                select s;
            userlist = new BaseMoneyListModel(orderByResult);
            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult Logout()
        {
            user.Email = "";
            user.Password = "";
            statistics = new StatisticsModel(userlist.ToList());
            userlist = new BaseMoneyListModel();
            allotheruserlist = new BaseMoneyListModel();
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
                user = um;
                if (user.Email != "" && user.Password != "")
                {
                    forall.Add(user);
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