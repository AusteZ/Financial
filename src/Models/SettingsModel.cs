using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Drawing.Printing;

namespace Financial.Models
{
    [Serializable]
    public class SettingsModel
    {
        [JsonIgnore]
        public static bool ErrorFlag { get; set; } = false;
        [JsonIgnore]
        public List<BaseMoneyModel> PresentList = new List<BaseMoneyModel>();
        private string _sortType = "Date";
        public string SortType { get => _sortType; set
            {
                _sortType = value;
                OnPriceChanged(EventArgs.Empty);
            } 
        }
        private bool sortDec = false, isExpense = true, isIncome = true;
        public bool SortDec { get => sortDec; set
            {
                sortDec = value;
                OnPriceChanged(EventArgs.Empty);
            }
        }
        public bool IsExpense
        {
            get => isExpense; set
            {
                isExpense = value;
                OnPriceChanged(EventArgs.Empty);
            }
        }
        public bool IsIncome
        {
            get => isIncome; set
            {
                isIncome = value;
                OnPriceChanged(EventArgs.Empty);
            }
        }
        private DateTime _from = DateTime.Now.AddDays(1), _to = DateTime.Now.AddDays(1);
        public DateTime From {
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
        private EventHandler _priceChanged;
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


        protected virtual void OnPriceChanged(EventArgs e)
        {
            _priceChanged?.Invoke(this, e);
        }
        private DateTime checkDate(DateTime dm)
        {
            if (dm.CompareTo(DateTime.Now) > 0) return DateTime.Now.AddDays(1);
            return dm;
        }
        static public void Sort (SettingsModel sm){
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
        }
        static public void Filter(SettingsModel sm)
        {
            var query = from s in sm.PresentList select s;
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
        }
    }
}