using System.Collections;

namespace Financial.Models
{
    [Serializable]
    public class BaseMoneyModel
    {
        private static int Number = -1;
        public readonly int Index;
        public string What { get; set; } = "Product";
        public string Where { get; set; } = "Place";
        public decimal Amount { get; set; } = 0;
        public Currency Currency = Currency.Euro;
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

    public class BaseAllMoneyModel : IEnumerable
    {
        private LinkedList<BaseMoneyModel> _list = new ();
        public BaseAllMoneyModel()
        {
            
        }
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
        public void Add(BaseMoneyModel bmm)
        {
            _list.AddLast(bmm);
        }
        public void Remove(BaseMoneyModel bmm)
        {
            _list.Remove(bmm);
        }
        public BaseMoneyModel Get(int id)
        {
            return _list.ElementAt(id);
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

        public LinkedList<BaseMoneyModel> Get_list()
        {
            return _list;
        }
    }
}