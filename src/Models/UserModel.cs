using System.Text.RegularExpressions;

namespace Financial.Models
{
    [Serializable]
    public class UserModel
    {
        //public int Id { get; set; }
        public Guid Id { get; set; }
        public string settings { get; set; } = "";
        private string _email = "";
        private string _password = "";
        public string Email
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
    }
}
