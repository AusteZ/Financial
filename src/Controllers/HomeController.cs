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
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using Org.BouncyCastle.Utilities;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        private static List<BaseMoneyModel> _userFinanceList = new();
        private static Guid _currentUser { get; set; }
        private static SettingsModel _settings = new SettingsModel();
        //private static SettingsModel settings = new SettingsModel();
        private static FinanceContext _context;
        private static DbSet<UserModel> _users;
        private static DbSet<BaseMoneyModel> _finances;

        private readonly ILogger<HomeController> _logger;

        public delegate TResult SaveTo<TResult>(string json);

        public HomeController(ILogger<HomeController> logger, FinanceContext context)
        {
            //test += "Controller -> ";
            //_currentUser = Guid.Empty;
            _logger = logger;
            _context = context;
            _users = context.users;
            _finances = context.finances;
            _settings.PriceChanged += SaveSettings;
        }

        public IActionResult Index()
        {
            if (_currentUser == Guid.Empty)
            {
                return RedirectToAction(nameof(Login));
            }
            if(!_userFinanceList.Any())
            {
                var _list = from one in _finances
                            where one.UserId == _currentUser
                            select one;
                _userFinanceList = new List<BaseMoneyModel>(_list);
                _settings.PresentList = _userFinanceList;
            }
            SettingsModel.ErrorFlag = false;

            return View(_settings);
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
            if (_userTemp.Any()) _currentUser = _userTemp.First().Id;
            else
            {
                _currentUser = Guid.Empty;
                SettingsModel.ErrorFlag = true;
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Expenses()
        {
            return View(_userFinanceList);
        }
        [Route("Home/ExpensesForm/{id?}")]
        public IActionResult ExpensesForm(string id, int type = 0)
        {
            BaseMoneyModel expense = new BaseMoneyModel();
            foreach(var exp in _userFinanceList)
            {
                if(exp.Id.ToString() == id)
                {
                    expense = exp;
                    //_userFinanceList.Remove(expense);
                    //_finances.Remove(expense);
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
            if(bmm.UserId != _currentUser)
            {
                bmm.UserId = _currentUser;
                _userFinanceList.Add(bmm);
                _finances.Add(bmm);
            } 
            if (bmm.isExpense) bmm.Amount = -Math.Abs(bmm.Amount);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExpenseLineDelete(string id, int type = 0)
        {
            foreach(var exp in _userFinanceList)
            {
                if(exp.Id.ToString() == id)
                {
                    _userFinanceList.Remove(exp);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Sort(SettingsModel sm)
        {
            _settings.SortType = sm.SortType;
            SettingsModel.Sort(_settings);
            //_users.Settings
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Filter(SettingsModel sm)
        {
            _settings.From = sm.From;
            _settings.To = sm.To;
            _settings.IsExpense = sm.IsExpense;
            _settings.IsIncome = sm.IsIncome;
            _settings.PresentList = new List<BaseMoneyModel>(_userFinanceList);
            SettingsModel.Filter(_settings);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            _userFinanceList = new List<BaseMoneyModel>();
            _currentUser = Guid.Empty;
            _settings = new SettingsModel();

            //wholeProgram = new FinanceModel();

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
                um.Id = Guid.NewGuid();
                _users.Add(um);
                _currentUser = um.Id;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        public async void SaveSettings(object o, EventArgs e)
        {
            //Thread e = new Thread();
            if(SettingsModel.ErrorFlag == false)
            {
                SettingsModel.ErrorFlag = true;
                await Task.Delay(60000);
                SaveTo<string> _saveTo = (json) =>
                {
                    return json;
                };
                try
                {
                    Monitor.Enter(_context.users);
                    try
                    {
                        _context.users.FirstOrDefault(x => x.Id.ToString() == _currentUser.ToString()).settings = Save<SettingsModel, string>(_settings, _saveTo);
                    }
                    finally
                    {
                        Monitor.Exit(_context.users);
                    }
                }
                catch (SynchronizationLockException SyncEx)
                {
                    var b = "";
                }
                catch (Exception ex){
                    var b = "";
                }
                
                _context.SaveChanges();
                SettingsModel.ErrorFlag = false;
            }
        }
        public UserModel GetUserInstance()
        {
            return _context.users.FirstOrDefault(x => x.Id.ToString() == _currentUser.ToString());
        }
        public IActionResult Download()
        {
            SaveTo<byte[]> _saveTo = (x) => Encoding.UTF8.GetBytes(x);
            return File(Save(_finances.ToArray(), _saveTo), "application/json", "finances.json");
        }
        public TResult Save<T, TResult>(T savee, SaveTo<TResult> st)
        {
            try
            {
                var jsonstring = JsonConvert.SerializeObject(savee, Formatting.Indented);
                return st(jsonstring);
            }
            catch (NullReferenceException nre)
            {
                return default(TResult);
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}