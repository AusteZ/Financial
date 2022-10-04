using System.Collections;

namespace Financial.Models
{
    public class BaseMoneyModel
    {
        public string What { get; set; } = "Product";
        public string Category { get; set; } = "Type";
        public string Where { get; set; } = "Place";
        public decimal Amount { get; set; } = 0;
        public Currency Currency = Currency.Euro;
    }
    public enum Currency
    {
        Euro,
        Dollar,
        Pound
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



    public class BaseAllMoneyModel : IEnumerable
    {
        private LinkedList<BaseMoneyModel> _list = new ();
        public BaseAllMoneyModel(LinkedList<BaseMoneyModel> list)
        {
            foreach(var bmm in list)
            {
                _list.AddLast(bmm);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public BaseAllMoneyEnum GetEnumerator()
        {
            return new BaseAllMoneyEnum(_list);
        }
    }
    public class BaseAllMoneyEnum : IEnumerator
    {
        private LinkedList<BaseMoneyModel> _list = new();
        int _index = -1;
        public BaseAllMoneyEnum(LinkedList<BaseMoneyModel> list)
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
    }
}