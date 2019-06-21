/*
 Analytics controller class
 Author: Evgeny Chudaev
 Purpose: Implements controller class responsible for handling requests for analytics area
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LoginTest.Data;
using LoginTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LoginTest.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly CrudContext _context;           


        // Controller constructor
        public AnalyticsController(CrudContext context, IHttpContextAccessor httpContextAccessor)
        {           
           _context = context;
        }

        public IActionResult Index()
        {
            return View("GeneralTickets");
        }       

        // Redirects request to General tickets view 
        public IActionResult GeneralTickets()
        {
            return View();
        }

        // Redirects request to "Index" view
        public IActionResult Filter(AnalyticsParameters myParams)
        {           
            return View("Index");
        }      

        // Method returns JSON data file for stacked area chart
        public JsonResult JsonDataLine(string year, string location, string service, string status, string overdue)
        {
            var data = ModelHelper.MultiLineData(_context, year, location, service, status, overdue);
            return Json(data);
        }

        // Method returns JSON data file for doughnut charts - volume by requestor and volume by analyst
        public JsonResult JsonDataPie(string year, string location, string service, string status, string overdue, string actor)
        {
            var data = ModelHelper.PiechartData(_context, year, location, service, status, overdue, actor);
            return Json(data);
        }

        // Method returns JSON data file for Tree chart
        public JsonResult JsonDataTree(string year, string location, string service, string status, string overdue)
        {
            var data = ModelHelper.TreeChartData(_context, year, location, service, status, overdue);
            return Json(data);
        }

        // Method returns JSON data file for Calendar chart
        public JsonResult JsonDataCalendar(string year, string location, string service, string status, string overdue)
        {
            var data = ModelHelper.CalendarChartData(_context, year, location, service, status, overdue);
            return Json(data);
        }

        // Method returns JSON data file for Raw Data table
        public JsonResult JsonDataTable(string year, string location, string service, string status, string overdue)
        {
            List<object> data = ModelHelper.TableChartData(_context, year, location, service, status, overdue); 
            return Json(data);
        }

        // Method returns JSON data file for Google Map
        public JsonResult JsonMapTable(string year, string location, string service, string status, string overdue)
        {
            var data = ModelHelper.MapChartData(_context, year, location, service, status, overdue);
            return Json(data);
        }

        // method returns JSON files for Service Requests, Incidents and Adhocs. 
        public JsonResult JsonTicketRawData(string type)
        {
            List<Requests> myList = new List<Requests>();
            if (type == "service")
            {
                myList = _context.Tickets.ToList().ConvertAll(x => (Requests)x);
            }
            else if (type == "incident")
            {
                myList = _context.Incidents.ToList().ConvertAll(x => (Requests)x);
            }
            else
            {
                myList = _context.Adhocs.ToList().ConvertAll(x => (Requests)x);
            }

            return Json(myList);
        }

        
    }
}