using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Data
{

    //classe principale d'accès aux données
    public class EatDbContext : DbContext
    {
        //constructeur qui appel le constructeur parent 
        public EatDbContext(DbContextOptions options) : base(options)
        {
            //super(options); JAVA
        }

        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<Customer> Customers { get; set; }
    }
}
