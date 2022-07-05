using ApiDatabaseFirst.Models;
using ApiDatabaseFirst.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDatabaseFirst
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SellInMonthPercentage>(builder =>
            {
                builder.HasNoKey();
            });
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<ActualSales> ActualSales { get; set; }


        public DbSet<SellInMonthPercentage> SellsInMonthPercentage { get; set; }
    }
}
