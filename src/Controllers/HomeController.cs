using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static 
            List<BaseMoneyModel> expenseList = new List<BaseMoneyModel>();

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
            return View(expenseList);
        }

        public IActionResult Categories()
        {
            return View();
        }

        public IActionResult ExpensesForm(BaseMoneyModel expense = null)
        {
            expense = expense ?? new BaseMoneyModel();
            return View(expense);
        }

        public IActionResult CategoriesForm(BaseMoneyModel category = null)
        {
            category = category ?? new BaseMoneyModel();
            return View(category);
        }
        

        public IActionResult ExpenseLine(BaseMoneyModel bmm)
        {
            expenseList.Add(bmm);
            return RedirectToAction(nameof(Expenses));
        }
        public IActionResult ExpenseLineDelete(BaseMoneyModel removedLine)
        {
            expenseList.Remove(removedLine);
            return RedirectToAction(nameof(Expenses));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}