namespace Financial.Models
{
    public class FinanceModel
    {
        public List<BaseMoneyModel> AllOtherUsersFinanceList = new();
        public List<BaseMoneyModel> UserFinanceList = new();
        public StatisticsModel Statistics;
        public UserModel User = new UserModel();
        public SettingsModel Settings = new SettingsModel();

        public FinanceModel()
        {
            Statistics = new StatisticsModel(UserFinanceList);
        }
    
    }
}
