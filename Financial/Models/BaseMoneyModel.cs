namespace Financial.Models
{
    public struct BaseMoneyModel
    {
        public string What { get; set; } = "Product";
        public string Where { get; set; } = "Place";
        public decimal Amount { get; set; } = 0;

        public BaseMoneyModel() { }
    }
}