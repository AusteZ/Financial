using System.Collections;
using System.Text.RegularExpressions;

namespace Financial.Models
{
    public class LinkingEmail
    {
        protected string _email = "";
        public virtual string Email
        {
            get => _email;
            set => _email = value;
        }
    }

    [Serializable]
    public class BaseMoneyModel : LinkingEmail
    {
        public bool isExpense { get; set; } = true;
        private static int Number = -1;
        public readonly int Index;
        public string Product { get; set; } = "Product";
        public string Category { get; set; } = "Type";
        public string Place { get; set; }
        public decimal Amount { get; set; } = 0;
        public Currency Currency = Currency.Euro;
        public DateTime Date { get; set; } = DateTime.Now;
        public BaseMoneyModel()
        {
            Number++;
            Index = Number;
        }
    }
    public enum Currency
    {
        Euro,
        Dollar,
        Pound
    }
    [Serializable]
    public class UserModel : LinkingEmail
    {
        private string _password = "";
        private string _confirmPassword = "";
        public override string Email
        {
            get => _email; set
            {
                Regex validateEmail = new Regex("^\\S+@\\S+\\.\\S+$");
                if (value != "" && validateEmail.IsMatch(value)) _email = value;
                else _email = "";
            }
        }
        public string Password
        {
            get => _password; set
            {
                Regex validatePassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                if (value != "" && validatePassword.IsMatch(value)) _password = value;
                else _password = "";

            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword; set
            {
                Regex validatePassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                if (value != "" && validatePassword.IsMatch(value)) _confirmPassword = value;
                else _confirmPassword = "";

            }
        }
        public UserModel() { }
    }
}