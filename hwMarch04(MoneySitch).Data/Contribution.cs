using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwMarch04_MoneySitch_.Data
{
    public class Contribution
    {
        public int SimchaID { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool Include { get; set; }
    }
}
