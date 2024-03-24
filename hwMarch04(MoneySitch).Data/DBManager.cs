using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace hwMarch04_MoneySitch_.Data
{
    public class DBManager
    {
        private readonly string _connectionString;
        public DBManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributor";
            connection.Open();
            List<Contributor> contributors = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var contributor = new Contributor()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                };
                var payed = GetContributions().Where(c => c.ContributorId == contributor.Id).Sum(c => c.Amount);
                var deposited = GetDeposits().Where(d => d.ContributorID == contributor.Id).Sum(d => d.Amount);
                contributor.Balance = deposited - payed;
                contributors.Add(contributor);
            }
            return contributors;
        }

        public List<Simcha> GetSimchas()
        {
            int contribCount = GetContributors().Count;
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Simcha";
            connection.Open();
            List<Simcha> simchas = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var simcha = new Simcha()
                {
                    Id = (int)reader["ID"],
                    Name = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                    ContributorCount = new ContributorCount { 
                        TotalContributors = contribCount
                    },
                };
                var contributions = GetContributions().Where(c => c.SimchaID == simcha.Id);
                simcha.Total = contributions.Sum(c => c.Amount);
                simcha.ContributorCount.HowManyContributed = contributions.Count();
                simchas.Add(simcha);
            }
            return simchas;
        }

        public int AddContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributor
                                    VALUES (@firstName, @lastName, @cellNumber, @alwaysInclude)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }

        public void AddSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Simcha
                                    VALUES (@name, @date)";
            command.Parameters.AddWithValue("@name", simcha.Name);
            command.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void EditContributor(Contributor contributor)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Contributor SET FirstName = @firstName, LastName = @lastName,
                                    CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude
                                    WHERE Id = @id";
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            command.Parameters.AddWithValue("@id", contributor.Id);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void AddDeposit(Deposit deposit)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Deposit
                                    VALUES (@id, @amount, @date)";
            command.Parameters.AddWithValue("@id", deposit.ContributorID);
            command.Parameters.AddWithValue("@amount", deposit.Amount);
            command.Parameters.AddWithValue("@date", deposit.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public string GetSimchaName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "select SimchaName from Simcha where Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            return (string)reader["SimchaName"];
        }

        public void UpdateContribution(List<Contribution> contributions, int simchaId)
        {
            if(contributions.Count < 1)
            {
                return;
            }

            ClearSimcha(simchaId);

            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contribution
                                    VALUES (@simchaId, @conId, @amount, @date)";
            connection.Open();
            var date = GetDateById(simchaId);
            foreach (var contribution in contributions)
            {
                if (contribution.Include)
                {
                    command.Parameters.AddWithValue("@simchaId", simchaId);
                    command.Parameters.AddWithValue("@conId", contribution.ContributorId);
                    command.Parameters.AddWithValue("@amount", contribution.Amount);
                    command.Parameters.AddWithValue("@date", date);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }

        private DateTime GetDateById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DATE FROM SIMCHA WHERE ID = @ID";
            command.Parameters.AddWithValue("@ID", id);
            connection.Open();
            return (DateTime)command.ExecuteScalar();
        }

        private void ClearSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Contribution WHERE SimchaID = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
         
        public List<Contribution> GetContributions()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contribution";
            connection.Open();
            List<Contribution> contributions = new List<Contribution>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new()
                {
                    Amount = (decimal)reader["Amount"],
                    SimchaID = (int)reader["SimchaId"],
                    ContributorId = (int)reader["ContributorId"],
                    Date = (DateTime)reader["date"],
                    Include = true
                });
            }
            return contributions;
        }

        public List<Deposit> GetDeposits()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Deposit";
            connection.Open();
            List<Deposit> deposits = new();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                deposits.Add(new()
                {
                    ContributorID = (int)reader["ContributorId"],
                    Amount = (decimal)reader["DepositAmount"],
                    Date = (DateTime)reader["DateDeposited"]
                });
            }
            return deposits;
        }

        public List<History> GetHistories(int id)
        {
            List<Contribution> contributions = GetContributions().Where(c => c.ContributorId == id).ToList();
            List<Deposit> deposits = GetDeposits().Where(d => d.ContributorID == id).ToList();
            List<History> histories = new();
            foreach (var contribution in contributions)
            {
                histories.Add(new()
                {
                    Name = $"Contribution for the {GetSimchaName(contribution.SimchaID)}",
                    Amount = -contribution.Amount,
                    Date = contribution.Date
                });
            }
            foreach (var deposit in deposits)
            {
                histories.Add(new()
                {
                    Name = "Deposit",
                    Amount = deposit.Amount,
                    Date = deposit.Date
                });
            }

            return histories.OrderBy(h => h.Date).ToList();
        }
    }
}
