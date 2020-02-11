using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sample
{
    public class MyDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         // => optionsBuilder.UseSqlite(@"Data Source=:memory:");
         => optionsBuilder.UseSqlite(@"Data Source=sample.db");
    }

    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
