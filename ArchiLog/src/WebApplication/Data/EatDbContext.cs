using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        // Factory : créer différentes classes et interfaces où les interfaces sont les modèles : préparer l'objet pour le besoin
        public static readonly ILoggerFactory SqlLogger = LoggerFactory.Create(builder => builder.AddConsole());
        // instancier objet pour faire des log --  ""fournir un instance parametré à notre besoin""
        /* 
         * pk log factory :
         * - donner un format 
         * - (faire un choix) comment on va enregistrer
         */

        // base(...) constructeur qui appel le constructeur parent
        public EatDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
