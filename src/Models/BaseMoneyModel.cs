using System.Collections;
using System.Text.RegularExpressions;

namespace Financial.Models
{
    [Serializable]
    public class BaseMoneyModel
    {
        public string UserEmail { get; set; }
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
    public class UserModel
    {
        private string _email = "";
        private string _password = "";
        public string Email { get => _email; set
            {
                Regex validateEmail = new Regex("^\\S+@\\S+\\.\\S+$");
                if (value != "" && validateEmail.IsMatch(value)) _email = value;
                else _email = "no";
            } 
        }
        public string Password
        {
            get => _password; set
            {
                Regex validatePassword = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                if (value != "" && validatePassword.IsMatch(value)) _password = value;
                else _password = "no";
                
            }
        }
        public UserModel(){}
    }

    /*
        public struct Category
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public static int Amount { get; private set; } = 0;

            public Category()
            {
                Name = "";
                Description = "";
                Amount = 0;
            }

            public Category(string Type, string About)
            {
                Name = Type;
                Description = About;
                Amount++;
            }
        }
    */

    public class BaseMoneyListModel : IEnumerable
    {
        private List<BaseMoneyModel> _list = new();

        public BaseMoneyListModel()
        {

        }
        public BaseMoneyListModel(BaseMoneyModel[] array)
        {
            _list = array.ToList();
        }
        public BaseMoneyListModel(List<BaseMoneyModel> list)
        {
            _list = list;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public BaseAllMoneyEnum GetEnumerator()
        {
            return new BaseAllMoneyEnum(_list);
        }
        public void Add(BaseMoneyModel bmm)
        {
            _list.Add(bmm);
        }
        public void Remove(BaseMoneyModel bmm)
        {
            _list.Remove(bmm);
        }
        public BaseMoneyModel[] ToArray(){
            return _list.ToArray();
        }
        public List<BaseMoneyModel> ToList()
        {
            return _list;
        }
        public BaseMoneyModel Get(int id)
        {
            return _list.ElementAt(id);
        }
        public int Count()
        {
            return _list.Count;
        }
    }
    public class BaseAllMoneyEnum : IEnumerator
    {
        private List<BaseMoneyModel> _list = new();
        int _index = -1;
        public BaseAllMoneyEnum(List<BaseMoneyModel> list)
        {
            _list = list;
        }
        public bool MoveNext()

        {
            _index++;
            return(_index < _list.Count);
        }
        public void Reset()
        {
            _index = -1;
        }
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }
        public BaseMoneyModel Current
        {
            get
            {
                return _list.ElementAt(_index);
            }
        }

        public List<BaseMoneyModel> Get_list()
        {
            return _list;
        }
    }
}