using System;
using System.Collections.Generic;

namespace Financial.Models
{
    public partial class Finance
    {
        public Guid Id { get; set; }
        public bool IsExpense { get; set; }
        public string Product { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Place { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; } = null!;
    }
}
