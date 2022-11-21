using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Financial.Models
{
    [Serializable]
    public class BaseMoneyModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }
        public bool isExpense { get; set; } = true;
        
        public string Product { get; set; } = "Product";
        public string? Category { get; set; } = "Type";
        public string? Place { get; set; } = "";
        public decimal Amount { get; set; } = 0;
        public Currency Currency = Currency.Euro;
        public DateTime? Date { get; set; } = DateTime.Now;
    }
    public enum Currency
    {
        Euro,
        Dollar,
        Pound
    }
}