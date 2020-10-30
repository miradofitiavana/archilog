using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Data
{

    //classe principale d'accès aux données
    public class EatDbContext : DbContext
    {
        public int Count { get; set; }
        //constructeur qui appel le constructeur parent 
        public EatDbContext(DbContextOptions options) : base(options)
        {
            //super(options); JAVA
        }
    }
}
