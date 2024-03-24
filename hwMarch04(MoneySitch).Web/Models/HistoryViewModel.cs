using hwMarch04_MoneySitch_.Data;

namespace hwMarch04_MoneySitch_.Web.Models
{
    public class HistoryViewModel
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<History> Histories { get; set; } = new ();
    }
}
