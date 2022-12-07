using Financial.Data;
using Financial.Interfaces;
using Financial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;

namespace Financial.Controllers
{
    public class HomeController : Controller
    {
        public static List<BaseMoneyModel> _userFinanceList;
        public static Guid _currentUser { get; set; }
        public static SettingsModel _settings;
        public static FinanceContext _context;
        public static DbSet<UserModel> _users;
        public static DbSet<BaseMoneyModel> _finances;

        private readonly ILogger<HomeController> _logger;
        readonly IBufferedFileUploadService _bufferedFileUploadService;

        public delegate TResult SaveTo<TResult>(string json); //delegate + generic

        public HomeController(ILogger<HomeController> logger, FinanceContext context, List<BaseMoneyModel> _list, SettingsModel sm, IBufferedFileUploadService bufferedFileUploadService)
        {
            _logger = logger;
            _context = context;
            _users = context.users;
            _finances = context.finances;
            _userFinanceList = _list;
            _settings = sm;
            _bufferedFileUploadService = bufferedFileUploadService;
            _settings.PriceChanged += SaveSettings;
        }

        public IActionResult Index()
        {
            if (_currentUser == Guid.Empty)
            {
                return RedirectToAction(nameof(Login));
            }
            if (!_userFinanceList.Any())
            {
                var _list = from one in _finances
                            where one.UserId == _currentUser
                            select one;
                _userFinanceList.AddRange(_list);
                try
                {
                    var temp = _context.users.FirstOrDefault(x => x.Id.ToString() == _currentUser.ToString());
                    _settings = JsonConvert.DeserializeObject<SettingsModel>(temp.settings);
                }
                catch(Exception ex){
                    var b = "";
                }
                
            }
            _settings.PresentList = _userFinanceList;
            _settings.Sort(_settings);
            SettingsModel.ErrorFlag = false;

            if (_settings.budget == 0) _settings.monthlyExpenses = 0;
            else _settings.CountExpenses(_userFinanceList);

            return View(_settings);
        }

        public IActionResult Login()
        {
            return View(new UserModel());
        }
        public IActionResult LoginForm(UserModel _user) //testfinx
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
            foreach (var exp in _userFinanceList)
            {
                if (exp.Id.ToString() == id)
                {
                    expense = exp;
                    id = "";
                    break;
                }
            }
            if (id != "") expense.Id = Guid.NewGuid();
            if (type == 1) expense.isExpense = false;
            return View(expense);
        }

        public IActionResult ExpenseLine(BaseMoneyModel bmm)
        {
            if (bmm.UserId != _currentUser)
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
            var delexpense = new BaseMoneyModel();
            foreach (var exp in _userFinanceList)
            {
                if (exp.Id.ToString() == id)
                {
                    delexpense = exp;
                }
            }
            _userFinanceList.Remove(delexpense);
            _finances.Remove(delexpense);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ExpensesReport()
        {
            decimal foodExpense = 0, transportationExpense = 0, housingExpense = 0, utilitiesExpense = 0, healthcareExpense = 0, savingsInvestingExpense = 0, clothingExpense = 0, entertainmentExpense = 0;


            foreach (var exp in _userFinanceList)
            {
                if (exp.isExpense == true)
                {

                    if (exp.Category == "Food") foodExpense += exp.Amount;
                    else if (exp.Category == "Transportation") transportationExpense += exp.Amount;
                    else if (exp.Category == "Housing") housingExpense += exp.Amount;
                    else if (exp.Category == "Utilities") utilitiesExpense += exp.Amount;
                    else if (exp.Category == "Healthcare") healthcareExpense += exp.Amount;
                    else if (exp.Category == "Saving&Investing") savingsInvestingExpense += exp.Amount;
                    else if (exp.Category == "Clothing") clothingExpense += exp.Amount;
                    else if (exp.Category == "Entertainment") entertainmentExpense += exp.Amount;

                }

            }

            List<DataPoint> dataPoints = new List<DataPoint>();


            dataPoints.Add(new DataPoint("Food", System.Math.Abs(foodExpense)));
            dataPoints.Add(new DataPoint("Transportation", System.Math.Abs(transportationExpense)));
            dataPoints.Add(new DataPoint("Housing", System.Math.Abs(housingExpense)));
            dataPoints.Add(new DataPoint("Utilities", System.Math.Abs(utilitiesExpense)));
            dataPoints.Add(new DataPoint("Healthcare", System.Math.Abs(healthcareExpense)));
            dataPoints.Add(new DataPoint("Saving and investing", System.Math.Abs(savingsInvestingExpense)));
            dataPoints.Add(new DataPoint("Clothing", System.Math.Abs(clothingExpense)));
            dataPoints.Add(new DataPoint("Entertainment", System.Math.Abs(entertainmentExpense)));


            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View();
        }

        public IActionResult SetBudget(SettingsModel sm)
        {
            _settings.budget = sm.budget > 0m ? sm.budget : 0m;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Sort(SettingsModel sm) //testfinx
        {
            _settings.SortType = sm.SortType;
            _settings.Sort(_settings);
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
            _settings.Filter(_settings, _userFinanceList);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Logout()
        {
            _userFinanceList.Clear();
            _currentUser = Guid.Empty;
            _settings = new SettingsModel();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult RegisterForm(UserModel um)//testfinx
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

        public  async void SaveSettings(object o, EventArgs e)//testfinx
        {
            if (SettingsModel.ErrorFlag == false)
            {
                SettingsModel.ErrorFlag = true;
                //await Task.Delay(60000);
                SaveTo<string> _saveTo = (json) => json;
                try
                {
                    var temp = _context.users.FirstOrDefault(x => x.Id == _currentUser);

                    if (temp != null)
                    {
                        temp.settings = Save<SettingsModel, string>(_settings, _saveTo);
                    }
                    
                    _context.SaveChanges();
                }
                catch (ObjectDisposedException ObjDisEx)
                {
                    LogErrors(ObjDisEx);
                }
                catch(Exception ex)
                {
                    LogErrors(ex);
                }

                SettingsModel.ErrorFlag = false;
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            try
            {
                if (await _bufferedFileUploadService.UploadFile(file))
                {
                    ViewBag.Message = "File Upload Successful";
                    using (StreamReader sr = System.IO.File.OpenText(@"UploadedFiles/fin.json"))
                    {
                        List<BaseMoneyModel> list = JsonConvert.DeserializeObject <List< BaseMoneyModel>>(sr.ReadToEnd());
                        foreach (var i in list)
                        {
                            i.Id = Guid.NewGuid();
                            i.UserId = _currentUser;
                        }
                        _userFinanceList.AddRange(list);
                        _finances.AddRangeAsync(list);
                        _context.SaveChangesAsync();
                    }
                    System.IO.File.Delete(@"UploadedFiles/fin.json");
                }
                else
                {
                    ViewBag.Message = "File Upload Failed";
                }

            }
            catch (Exception ex)
            {
                //Log ex
                ViewBag.Message = "File Upload Failed";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Download()
        {
            SaveTo<byte[]> _saveTo = (x) => Encoding.UTF8.GetBytes(x);
            return File(Save(_finances.ToArray(), _saveTo), "application/json", "finances.json");
        }
        
        public TResult Save<T, TResult>(T savee, SaveTo<TResult> st)//testfinx
        {
            try
            {
                var jsonstring = JsonConvert.SerializeObject(savee, Formatting.Indented);
                return st(jsonstring);
            }
            catch (ExceptionLoggingException ELE)
            {
                return default(TResult);
            }
            catch (Exception ex)
            {
                LogErrors(ex);

                return default(TResult);
            }
        }
        public async void LogErrors(Exception ex) //logging exceptions to file //testfinx
        {
            try
            {
                SaveTo<string> _saveto = (x) => x;
                var error = new Dictionary<string, string>
                {
                    {"Type", ex.GetType().ToString()},
                    {"Message", ex.Message},
                    {"StackTrace", ex.StackTrace}
                };
                var exceptionjson = Save<Dictionary<string, string>, string>(error, _saveto);
                await System.IO.File.AppendAllTextAsync("errors.json", exceptionjson);
            }
            catch (Exception e)
            {
                throw new ExceptionLoggingException("", e);
            }
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}