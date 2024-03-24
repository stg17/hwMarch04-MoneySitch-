using hwMarch04_MoneySitch_.Data;
using hwMarch04_MoneySitch_.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hwMarch04_MoneySitch_.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=MoneyManagement; Integrated Security=true;";

        public IActionResult Index()
        {
            var manager = new DBManager(_connectionString);
            var vm = new SimchasViewModel()
            {
                Simchas = manager.GetSimchas(),
            };
            if (TempData["new-simcha"] != null)
            {
                ViewBag.Message = (string)TempData["new-simcha"];
            }
            return View(vm);
        }

        public IActionResult Contributors()
        {
            var manager = new DBManager(_connectionString);
            var vm = new ContributorsViewModel()
            {
                Contributors = manager.GetContributors()
            };
            vm.Total = vm.Contributors.Sum(c => c.Balance);
            if (TempData["new-contributor"] != null) 
            {
                ViewBag.Message = (string)TempData["new-contributor"];
            }
            else if (TempData["edit-contributor"] != null)
            {
                ViewBag.Message = (string)TempData["edit-contributor"];
            }
            else if (TempData["deposit"] != null)
            {
                ViewBag.Message = (string)TempData["deposit"];
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult NewContributor(Contributor contributor, decimal initialDeposit, DateTime date)
        {
            var manager = new DBManager(_connectionString);
            int id = manager.AddContributor(contributor);
            manager.AddDeposit(new Deposit { ContributorID = id, Amount = initialDeposit, Date = date });
            TempData["new-contributor"] = "new contributor added!";
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)
        {
            var manager = new DBManager(_connectionString);
            manager.AddSimcha(simcha);
            TempData["new-simcha"] = "new Simcha added!";
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult EditContrib(Contributor contributor)
        {
            var manager = new DBManager(_connectionString);
            manager.EditContributor(contributor);
            TempData["edit-contributor"] = $"{contributor.FirstName} {contributor.LastName} Edited successfully!";
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult AddDeposit(Deposit deposit)
        {
            var manager = new DBManager(_connectionString);
            manager.AddDeposit(deposit);
            TempData["deposit"] = "Deposit successfully recorded";
            return Redirect("/home/contributors");
        }

        public IActionResult Contributions(int id)
        {
            var manager = new DBManager(_connectionString);
            var vm = new ContributionViewModel();
            vm.SimchaId = id;
            vm.SimchaName = manager.GetSimchaName(id);
            vm.Contributors = manager.GetContributors();
            vm.Contributions = manager.GetContributions().Where(c => c.SimchaID == id).ToList();
            return View(vm);
        }

        [HttpPost]
        public IActionResult updatecontributions(List<Contribution> contributions, int simchaId)
        {
            var manager = new DBManager(_connectionString);
            manager.UpdateContribution(contributions, simchaId);
            return Redirect($"/");
        }

        public IActionResult ShowHistory(int id)
        {
            var manager = new DBManager(_connectionString);
            var contributor = manager.GetContributors().FirstOrDefault(c => c.Id == id);
            var historyvm = new HistoryViewModel();
            historyvm.Histories = manager.GetHistories(id);
            historyvm.Name = contributor.FirstName + contributor.LastName;
            historyvm.Balance = contributor.Balance;
            return View(historyvm);
        }
    }
}
