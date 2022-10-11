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
        int u = 0;


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
            _incomelist = new List<BaseMoneyModel>();
            _expenselist = new List<BaseMoneyModel>();
            foreach (var item in list)
            {
                
                if (item.isExpense)
                {
                    item.Amount = -Math.Abs(item.Amount);
                    _expenselist.Add(item);
                }
                else
                {
                    _incomelist.Add(item);
                }
            }
        }
        public decimal Count()
        {
            var list = new List<BaseMoneyModel>();
            if (IsIncome)  list.AddRange(_incomelist);
            if (IsExpense) list.AddRange(_expenselist);

            //if (IsIncome) list.AddRange(_incomelist);
            //if (IsExpense) list.AddRange(_expenselist);
            //u = list.Count;


            //if (IsIncome) list.AddRange(_incomelist);
            List<BaseMoneyModel> indexes = new List<BaseMoneyModel>();

            foreach (var item in list)
            {
                if (From.CompareTo(DateTime.Now) < 0 && From.CompareTo(item.Date) > 0) indexes.Add(item);
            }
            foreach(var item in indexes)
            {
                list.Remove(item);
            }
            u = list.Count;


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
            decimal sol = 0;
            int count = 0;
            //int i = 0;
            list.Sort((x, y) => x.Date.CompareTo(y.Date));
            for(int i = 0; i < list.Count; )
            {
                count++;
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].Date.Year == list[j].Date.Year) { 
                        if (Period == 3)
                        {
                            continue;
                        }
                        else if (list[i].Date.Month == list[j].Date.Month)
                        {
                            if (Period == 2 || (list[i].Date.Day == list[j].Date.Day && Period == 1))
                            {
                                continue;
                            }
                        }
                    }
                    i = j;
                    break;
                }
            }
            sol = count == 0 ? 0 : TotalSum(list) / count;
            
            return count;
            //return u;
        }
    }
}
