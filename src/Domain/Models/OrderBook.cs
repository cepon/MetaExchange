namespace MetaExchange.Domain.Models
{
    public class OrderBook
    {
        public decimal Balance { get; set; }
        public List<OrderObject> Bids { get; set; }
        public List<OrderObject> Asks { get; set; }
    }
}
