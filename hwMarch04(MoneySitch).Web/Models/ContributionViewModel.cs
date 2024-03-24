using hwMarch04_MoneySitch_.Data;

namespace hwMarch04_MoneySitch_.Web.Models
{
    public class ContributionViewModel
    {
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
        public List<Contributor> Contributors { get; set; } = new List<Contributor>();
        public List<Contribution> Contributions { get; set; } = new List<Contribution>();
    }
}