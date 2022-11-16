using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        //public int Id { get; set; }
        
        public Guid Id { get; set; }
        public bool isExpense { get; set; } = true;
        
        public string Product { get; set; } = "Product";
        public string Category { get; set; } = "Type";
        public string Place { get; set; } = "";
        public decimal Amount { get; set; } = 0;
        public Currency Currency = Currency.Euro;
        public DateTime Date { get; set; } = DateTime.Now;
    }
    public enum Currency
    {
        Euro,
        Dollar,
        Pound
    }
}