using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwMarch04_MoneySitch_.Data
{
    public class Deposit
    {
        public int ContributorID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
