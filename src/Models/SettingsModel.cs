using Microsoft.Extensions.Logging.Abstractions;
using NuGet.Configuration;

namespace Financial.Models
{
    public class SettingsModel
    {
        public static bool ErrorFlag { get; set; } = false;
        public List<BaseMoneyModel> PresentList = new List<BaseMoneyModel>();
        public string SortType { get; set; } = "Date";
        public bool SortDec { get; set; } = false;
        public bool IsExpense { get; set; } = true;
        public bool IsIncome { get; set; } = true;
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
                query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) > 0
                        select finance;
            }
            if (sm.To.CompareTo(DateTime.Now) < 0)
            {
                query = from finance in query
                        where finance.Date.CompareTo(DateTime.Now) < 0
                        select finance;
            }

            query = from finance in query
                    where (finance.isExpense == true && sm.IsExpense == true) || (finance.isExpense == false && sm.IsIncome == true)
                    select finance;

            sm.PresentList = new List<BaseMoneyModel>(query);
        }
    }
}