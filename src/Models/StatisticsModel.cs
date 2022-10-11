namespace Financial.Models
{
    public class StatisticsModel
    {
        private static List<BaseMoneyModel> _expenselist = new List<BaseMoneyModel>();
        private static List<BaseMoneyModel> _incomelist = new List<BaseMoneyModel>();

        public DateTime From { get; set; } = DateTime.Now.AddDays(1);
        public bool IsExpense { get; set; } = true;
        public bool IsIncome { get; set; } = true;
        public string Type { get; set; } = "Total";
        public int Period { get; set; } = 2; //1 == day, 2 == month, 3 == year

        public StatisticsModel()
        {
            From = DateTime.Now.AddDays(1);
        }
        public StatisticsModel(List<BaseMoneyModel> list)
        {
            SetList(list);
        }
        public void SetList(List<BaseMoneyModel> list)
        {
            foreach (var item in list)
            {
                _incomelist.Clear();
                _expenselist.Clear();
                if (item.isExpense == false)
                {
                    _incomelist.Add(item);
                }
                else
                {
                    item.Amount = -Math.Abs(item.Amount);
                    _expenselist.Add(item);
                }
            }
        }
        public decimal Count()
        {
            var list = new List<BaseMoneyModel>();
            if (IsExpense) list.AddRange(_expenselist);

            if (IsIncome) list.AddRange(_incomelist);

            foreach (var item in list)
            {
                if (From.CompareTo(DateTime.Now) < 0 && From.CompareTo(item.Date) < 0) list.Remove(item);
            }

            
            if (Type == "Average") return TotalAverage(list);
            else return TotalSum(list);

        }
        public decimal TotalSum(List<BaseMoneyModel> list)
        {
            decimal sum = 0;


            foreach (var item in list)
            {
                sum += item.Amount;
            }

            return sum;
        }
        public decimal TotalAverage(List<BaseMoneyModel> list)
        {
            decimal sum = 0;
            int count = 0;
            foreach(var item in list)
            {
                ++count;
                sum += item.Amount;
                foreach(var item2 in list)
                {
                    switch (Period)
                    {
                        case 1:
                            if (item.Date.Day == item2.Date.Day)
                            {
                                sum += item2.Amount;
                                list.Remove(item2);
                            }
                            break;
                        case 2:
                            if (item.Date.Month == item2.Date.Month)
                            {
                                sum += item2.Amount;
                                list.Remove(item2);
                            }
                            break;
                        case 3:
                            if (item.Date.Year == item2.Date.Year)
                            {
                                sum += item2.Amount;
                                list.Remove(item2);
                            }
                            break;
                    }
                }
            }
            
            return sum/count;
        }
    }
}
