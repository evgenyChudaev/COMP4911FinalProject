/*
 Home controller class
 Author: Evgeny Chudaev
 Purpose: Implements controller class responsible for handling requests coming from the home page
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginTest.Models;
using Microsoft.AspNetCore.Http;
using LoginTest.Data;

namespace LoginTest.Controllers
{


    public class HomeController : Controller
    {
        private readonly CrudContext _contextAdmin;

        // Controller constructor
        public HomeController(CrudContext context)
        {
            _contextAdmin = context;
        }

        //Index method - responsible for displaying a list of options when user is at the main (home) page
        public IActionResult Index()
        {
            // When user id is not available assing "Anomymous" id
            string userId = null;

            if (User.Identity.Name == null || User.Identity.Name == "" || User.Identity.Name == "Anomymous user")
            {
                userId = "Anomymous";
            }
            else
            {
                userId = User.Identity.Name;
            }

           

            Admins myAdmin = _contextAdmin.Admins.SingleOrDefault(x => x.login == userId); // make sure analysts can see all tickets and other users can only see tickets they submitted
            string viewName = null;

            if(myAdmin != null && (myAdmin.role == "Manager" || myAdmin.role == "Superuser")) // managers and superusers should have access tickets and anaytics areas
            {
                viewName = "Index";
            }
            else  // analysts and general users should have access only to ticket areas
            {
                /* use standardIndexView instead when using IIS web server on premises. 
                  Index view is used temporarily for demo purposes
                given that we can't do windows authenticatin from Azure cloud
                 */

                viewName = "Index"; 
                //viewName = "standardIndex";
            }         

            return View(viewName);
        }      

        // Method that routes requests from home page to Service tickets area
        public IActionResult RedirectToTickets()
        {
            return RedirectToAction("Index", "Ticket");
        }

        // Method that routes requests from home page to Incident tickets area
        public IActionResult RedirectToIncidents()
        {
            return RedirectToAction("Index", "Incident");
        }

        // Method that routes requests from home page to Adhoc tickets area
        public IActionResult RedirectToAdhocs()
        {
            return RedirectToAction("Index", "Adhoc");
        }

        // Method that routes requests from home page to Analytics area
        public IActionResult RedirectToAnalytics()
        {
            return RedirectToAction("Index", "Analytics");
        }       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
