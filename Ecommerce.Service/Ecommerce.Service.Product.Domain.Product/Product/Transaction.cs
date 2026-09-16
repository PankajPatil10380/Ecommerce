namespace Ecommerce.Service.Product.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual Order Order { get; set; }
    }
}
