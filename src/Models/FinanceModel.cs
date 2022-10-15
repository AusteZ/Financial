namespace Financial.Models
{
    public class FinanceModel
    {
        public BaseMoneyListModel AllOtherUsersFinanceList = new BaseMoneyListModel();
        public BaseMoneyListModel UserFinanceList = new BaseMoneyListModel();
        public StatisticsModel Statistics;
        public UserModel User = new UserModel();

        public FinanceModel()
        {
            Statistics = new StatisticsModel(UserFinanceList.ToList());
        }
    
    }
}
