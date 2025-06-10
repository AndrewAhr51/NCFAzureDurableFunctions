using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using NCFAzureDurableFunctions.Src.Auth;
using NCFAzureDurableFunctions.src.Data.Entities;

namespace NCFAzureDurableFunctions.src.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connection = new SqlConnection
            {
                ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:NCFDonorDb"),
                AccessToken = new AzureIdentityTokenProvider().GetAccessToken()
            };

            optionsBuilder.UseSqlServer(connection);
        }
    }

    public DbSet<Donor> Donors { get; set; }
    public DbSet<CharitableOrganization> Organizations { get; set; }
}
