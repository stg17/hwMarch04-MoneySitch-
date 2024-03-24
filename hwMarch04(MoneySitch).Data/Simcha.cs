using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwMarch04_MoneySitch_.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public ContributorCount ContributorCount { get; set; } = new ContributorCount();
        public decimal Total { get; set; }
    }

    public class ContributorCount
    {
        public int HowManyContributed { get; set; }
        public int TotalContributors { get; set; }
    }
}
