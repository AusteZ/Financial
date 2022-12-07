using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Financial.Models
{
    [Serializable]
    public class SettingsModel
    {
        [JsonIgnore]
        public static bool ErrorFlag { get; set; } = false;
        [JsonIgnore]
        public List<BaseMoneyModel> PresentList = new List<BaseMoneyModel>();
        public string SortType { get; set; } = "";
        public bool SortDec { get; set; }
        public bool IsExpense {get; set;}
        public bool IsIncome { get; set; }
        
        public decimal budget { get; set; }

        public decimal monthlyExpenses { get; set; }
        
        private EventHandler _priceChanged; //events
        public event EventHandler PriceChanged
        {
            add
            {
                if (_priceChanged == null || !_priceChanged.GetInvocationList().Contains(value))
                {
                    _priceChanged += value;
                }
            }
            remove
            {
                _priceChanged -= value;
            }
        }

        private DateTime _from = DateTime.Now.AddDays(1), _to = DateTime.Now.AddDays(1);
        public DateTime From
        {
            get => _from;
            set => _from = _to.CompareTo(value) < 0 ? checkDate(value) : DateTime.Now.AddDays(1);
        }
        public DateTime To
        {
            get => _to;
            set
            {
                _to = checkDate(value);
                _from = _to.CompareTo(_from) < 0 ? _from : DateTime.Now.AddDays(1);
            }
        }
        protected virtual void OnPriceChanged(EventArgs e)
        {
            _priceChanged?.Invoke(this, e);
        }
        private DateTime checkDate(DateTime dm)
        {
            if (dm.CompareTo(DateTime.Now) > 0) return DateTime.Now.AddDays(1);
            return dm;
        }
        public void Sort (SettingsModel sm){
            var _list = new List<BaseMoneyModel>(sm.PresentList);
            var orderByResult = from s in _list select s;
            if (sm.SortType == "Name")
            {
                orderByResult = from s in _list
                                orderby s.Product descending
                                select s;
            }
            else if (sm.SortType == "Place")
            {
                orderByResult = from s in _list
                                orderby s.Place descending
                                select s;
            }
            else if (sm.SortType == "Amount")
            {
                orderByResult = from s in _list
                                orderby s.Amount descending
                                select s;
            }
            else if (sm.SortType == "Date")
            {
                orderByResult = from s in _list
                                orderby s.Date descending
                                select s;
            }
            sm.PresentList = new List<BaseMoneyModel>(orderByResult);
            OnPriceChanged(EventArgs.Empty);
        }
        public void Filter(SettingsModel sm, List<BaseMoneyModel> li)
        {
            var query = from s in li select s;
            if (sm.From.CompareTo(DateTime.Now) < 0)
            {
                /*query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) > 0
                        select finance;*/
            }
            if (sm.To.CompareTo(DateTime.Now) < 0)
            {
                /*query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) < 0
                        select finance;*/
            }

            query = from finance in query
                    where (finance.isExpense == true && sm.IsExpense == true) || (finance.isExpense == false && sm.IsIncome == true)
                    select finance;

            sm.PresentList = new List<BaseMoneyModel>(query);
            OnPriceChanged(EventArgs.Empty);
        }
        public void CountExpenses(List<BaseMoneyModel> _list)
        {
            monthlyExpenses = 0;
            var query = from i in _list
                    where i.Date.HasValue && i.isExpense == true && i.Date.Value.Month == DateTime.Now.Month
                    select i;
            foreach(var t in query){
                monthlyExpenses += t.Amount;
            }
            OnPriceChanged(EventArgs.Empty);
        }
    }
}