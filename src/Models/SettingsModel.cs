using Microsoft.Extensions.Logging.Abstractions;

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
    }
}