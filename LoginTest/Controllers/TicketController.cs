/*
 Ticket controller class
 Author: Evgeny Chudaev
 Purpose: Implements controller class responsible for handling requests and business logic for the service tickets
 */

using LoginTest.Data;
using LoginTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace LoginTest.Controllers
{
    public class TicketController : Controller
    {

        private readonly CrudContext _context;            
        private string ipAddr, city;
        double latitude, longtitude;       

        // Controller constructor
        public TicketController(CrudContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;           
            this.ipAddr = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();

            if (ipAddr == "::1") // when using localhost, IP is ::1. 
            {
                this.ipAddr = "142.134.88.171"; // default IP Address for HRM in place for localhost
            }
            else
            {
                this.ipAddr = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            }

            // use ipstack web service to translate ip address to location
            string url = "http://api.ipstack.com/" + ipAddr + "?access_key=<your key>";
            var request = WebRequest.Create(url);

            using (WebResponse wrs = request.GetResponse())
            using (Stream stream = wrs.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var obj = JObject.Parse(json);
                city = (string)obj["city"];
                latitude = (double)obj["latitude"];
                longtitude = (double)obj["longitude"];
                ipAddr = city + " " + latitude + " " + longtitude;
            }
        }

        // Index method - responsible for displaying a list of tickets
        public async System.Threading.Tasks.Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {           
           
            string userId = null;

            // When user id is not available assing "Anomymous" id
            if (User.Identity.Name == null || User.Identity.Name == "" || User.Identity.Name == "Anomymous user")
            {
                userId = "Anomymous";
            }
            else
            {
                userId = User.Identity.Name;
            }    
            
            var students = from s in _context.Tickets
                           select s;

            Admins myAdmin = _context.Admins.SingleOrDefault(x => x.login == userId); // make sure analysts can see all tickets and other users can only see tickets they submitted

            ViewData["Role"] = null;
            if (myAdmin != null && (myAdmin.role == "Analyst" || myAdmin.role == "Superuser"))
            {
               // do nothing - for analysts - simply show all tickets , for general users & managers limit list to show only requests they have submitted
                if(myAdmin.role == "Superuser") // let superuser edit & delete tickets after status changed to something other than "Submitted" even after ticket is closed. 
                {
                    ViewData["Role"] = "Superuser";
                }
            }
            else
            {
                students = students.Where(user => user.RequestorID.Equals(userId)); // for general users & managers limit list to show only requests they have submitted, they can see a list of all tickets in the Analytics area without being able to delete/modify
            }

            // remainder of this method deals with pagination of the list of tickets
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Status.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Status);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.SubmittedDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.SubmittedDate);
                    break;
                default:
                    students = students.OrderBy(s => s.Status);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Ticket>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
         
        }

        // route request to Index
        public IActionResult RedirectToTickets()
        {        
            return View(_context.Tickets.ToList());
        }      

        //Method responsible for displaying ticket details
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = _context.Tickets.SingleOrDefault(x => x.ID == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        //Method responsible for rounting to ticket edit page
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }            

            Ticket ticket = _context.Tickets.SingleOrDefault(x => x.ID == id);        

            if (ticket == null)
            {
                return NotFound();
            }


            return View(ticket);
        }

        //Method responsible editing of ticket information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Ticket ticket)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = null;

            // When user id is unavailable, display user as "Anomymous"
            if (User.Identity.Name == null || User.Identity.Name == "" || User.Identity.Name == "Anomymous user")
            {
                userId = "Anomymous";
            }
            else
            {
                userId = User.Identity.Name;
            }        


            if (ModelState.IsValid)
            {
               // When status is changed by analyst to closed/cancelled, update analyst id (whoever closed the ticket)
               if (ticket.Status == "Closed" || ticket.Status == "Cancelled")
               {
                    Ticket oticket = new Ticket { ID = ticket.ID, Status = ticket.Status, SupportAnalyst = userId, TicketDetails = ticket.TicketDetails, RequiredByDate = ticket.RequiredByDate };
                    oticket.CompletedDate = System.DateTime.Now;
                    _context.Entry(oticket).Property("CompletedDate").IsModified = true;
                    _context.Entry(oticket).Property("Status").IsModified = true;
                    _context.Entry(oticket).Property("SupportAnalyst").IsModified = true;
                    _context.Entry(oticket).Property("TicketDetails").IsModified = true;
                    _context.Entry(oticket).Property("RequiredByDate").IsModified = true;
                }
                else  
                {
                    Ticket oticket = new Ticket { ID = ticket.ID, Status = ticket.Status, SupportAnalyst = userId, TicketDetails = ticket.TicketDetails, RequiredByDate = ticket.RequiredByDate };                   
                    _context.Entry(oticket).Property("Status").IsModified = true;
                    _context.Entry(oticket).Property("SupportAnalyst").IsModified = true;
                    _context.Entry(oticket).Property("TicketDetails").IsModified = true;
                    _context.Entry(oticket).Property("RequiredByDate").IsModified = true;
                }
               
                _context.SaveChanges();               

                TempData["message"] = "Person edited!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "There has been errors");
            return View(ticket);
        }

        //Method responsible for creating new ticket
        public IActionResult Create(int? id)
        {
            return View();
        }

        //Method responsible for creating new ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.SubmittedDate = System.DateTime.Now;
                ticket.Status = "Submitted";
                
                ticket.RequestorID = User.Identity.Name == null ? "Anomymous" : User.Identity.Name;
              

                ticket.requestLocation = city;
                ticket.latitute = latitude;
                ticket.longtitude = longtitude;
                _context.Add(ticket);
                _context.SaveChanges();

                TempData["message"] = "Ticket Created!";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "There has been errors");
            return View(ticket);

        }

        //Method responsible for deletion of the ticket
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = _context.Tickets.SingleOrDefault(x => x.ID == id);

            if (ticket == null)
            {
                return NotFound();
            }

            _context.Remove(ticket);
            _context.SaveChanges();

            TempData["message"] = "Ticket deleted!";
            return RedirectToAction("Index");

        }

        //Method responsible for routing request to the filtered index view
        public IActionResult Statusview(string filter)
        {
            return View(_context.Tickets.ToList().FindAll(x => (x.Status == filter) && (x.RequestorID == User.Identity.Name)));
        }
    }
}