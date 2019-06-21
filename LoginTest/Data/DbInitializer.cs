/*
 DBInitializaer class
 Author: Evgeny Chudaev
 Purpose: Initializes the database (if none exists in the connection string provided in appsettings.json file)
 */

using LoginTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Data
{
    public class DbInitializer
    {
        public static void Initialize(CrudContext context)
        {
            context.Database.EnsureCreated();

            
        }
    }
}
