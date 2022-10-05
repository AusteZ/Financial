using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static BaseAllMoneyModel expenselist = new BaseAllMoneyModel();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Expenses()
        {
            return View(expenselist);
        }
        [Route("Home/ExpensesForm/{id?}")]
        public IActionResult ExpensesForm(int id = -1)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            if (id != -1)
            {
                foreach(var exp in expenselist)
                {
                    if(exp.Index == id)
                    {
                        expense = exp;
                        expenselist.Remove(exp);
                        break;
                    }
                }
            }
            
            return View(expense);
        }
        public IActionResult ExpenseLine(BaseMoneyModel bmm)
        {
            expenselist.Add(bmm);
            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult ExpenseLineDelete(int id = -1)
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
            return RedirectToAction(nameof(Expenses));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}