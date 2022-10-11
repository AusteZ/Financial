using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static BaseMoneyListModel expenselist = new BaseMoneyListModel();
        private static StatisticsModel statistics = new StatisticsModel(expenselist.ToList());

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            statistics.SetList(expenselist.ToList());
            return View(statistics);
        }
        public IActionResult StatisticsSettings(StatisticsModel sm)
        {
            statistics = sm;
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Expenses()
        {
            return View(expenselist);
        }
        [Route("Home/ExpensesForm/{id?}")]
        public IActionResult ExpensesForm(int id = -1, int type = 0)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            if (id != -1)
            {
                foreach(var exp in expenselist)
                {
                    if(exp.Index == id)
                    {
                        expense = exp;
                        expenselist.Remove(expense);
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
            expenselist.Add(bmm);
            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult ExpenseLineDelete(int id = -1, int type = 0)
        {
            foreach(var exp in expenselist)
            {
                if(exp.Index == id)
                {
                    expenselist.Remove(exp);
                }
            }

            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult Save()
        {
            var a = JsonConvert.SerializeObject(expenselist.ToArray());
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
            expenselist = new BaseMoneyListModel((JsonConvert.DeserializeObject<BaseMoneyModel[]>(a)).ToList());

            return RedirectToAction(nameof(Expenses));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}