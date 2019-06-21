/*
 CrudContext class
 Author: Evgeny Chudaev
 Purpose: Represents the CrudContext, an extension of DBCOntext which handles database CRUD operations
 */

using LoginTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Data
{
    public class CrudContext : DbContext
    {

        public CrudContext(DbContextOptions<CrudContext> options) : base(options)
        {

        }        

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Adhoc> Adhocs { get; set; }
        public DbSet<Admins> Admins { get; set; }
    }
}
