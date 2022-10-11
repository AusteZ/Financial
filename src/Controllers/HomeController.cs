using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static BaseMoneyListModel allotheruserlist = new BaseMoneyListModel();
        private static BaseMoneyListModel userlist = new BaseMoneyListModel();
        private static StatisticsModel statistics = new StatisticsModel(userlist.ToList());


        private static UserModel user = new UserModel();
        //private string email = "";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(user.Email == "" || user.Email == "no")
            {
                return RedirectToAction(nameof(Login));
            }
            if(userlist.Count() == 0)
            {
                Load();
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
            bmm.UserEmail = user.Email;
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
        public IActionResult Load()
        {
            var a = System.IO.File.ReadAllText("data.txt");
            List<BaseMoneyModel> list = (JsonConvert.DeserializeObject<BaseMoneyModel[]>(a)).ToList();

            foreach(var item in list)
            {
                if(item.UserEmail == user.Email)
                {
                    userlist.Add(item);
                }
                else
                {
                    allotheruserlist.Add(item);
                }
            }
            //userlist = new BaseMoneyListModel((JsonConvert.DeserializeObject<BaseMoneyModel[]>(a)).ToList());
            

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}