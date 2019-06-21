/*
 ModelHelper class
 Author: Evgeny Chudaev
 Purpose: Implements business/data processing logic for analytics area
 */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginTest.Data;
using LoginTest.Models;
using System.Globalization;

namespace LoginTest.Models
{
    public class ModelHelper
    {
        static List<LoginTest.Models.Ticket> myTicketsList;
        static List<LoginTest.Models.Incident> myIncidentsList;
        static List<LoginTest.Models.Adhoc> myAdhocsList;
        static int year;
        static bool success;

        /*
         * Method sets 4 filter variables - Year Submitted, Location, Service Category, Request Status, Overdue Status
         * This prevents from writing repetitive code for all methods responsible for visiuals in the Analytics Area as
         * all visuals are affected by the same set of filters.
         */
        public static void setFilterParameters(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {
            myTicketsList = myContext.Tickets.ToList();
            myIncidentsList = myContext.Incidents.ToList();
            myAdhocsList = myContext.Adhocs.ToList();
            
            success = Int32.TryParse(pYear, out year);

            // 1.Filter by Year Submitted
            if (pYear != "All")
            {
                if (success)
                {
                    myTicketsList = myTicketsList.FindAll(x => x.SubmittedDate.Year == year);
                    myIncidentsList = myIncidentsList.FindAll(x => x.SubmittedDate.Year == year);
                    myAdhocsList = myAdhocsList.FindAll(x => x.SubmittedDate.Year == year);
                }
            }
            // 2. Filter by Location
            if (location != "All")
            {
                myTicketsList = myTicketsList.FindAll(x => x.requestLocation == location);
                myIncidentsList = myIncidentsList.FindAll(x => x.requestLocation == location);
                myAdhocsList = myAdhocsList.FindAll(x => x.requestLocation == location);
            }
            // 3. Filter by Status
            if (status != "All")
            {
                myTicketsList = myTicketsList.FindAll(x => x.Status == status);
                myIncidentsList = myIncidentsList.FindAll(x => x.Status == status);
                myAdhocsList = myAdhocsList.FindAll(x => x.Status == status);
            }

            DateTime todayDate = System.DateTime.Today;
            // 4. Filter by overdue status
            if (overdue != "All")
            {
                myTicketsList = myTicketsList.FindAll(x => (x.CompletedDate > x.RequiredByDate) || (x.Status != "Closed" && x.Status != "Cancelled" && todayDate > x.RequiredByDate));
                myIncidentsList = myIncidentsList.FindAll(x => x.CompletedDate > x.SubmittedDate.AddHours(4));
                myAdhocsList = myAdhocsList.FindAll(x => (x.CompletedDate > x.RequiredByDate) || (x.Status != "Closed" && x.Status != "Cancelled" && todayDate > x.RequiredByDate));
            }
        } 

        // Method implements logic for stacked area chart
        public static List<object> MultiLineData(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {
            // initialize filters
            setFilterParameters(myContext, pYear, location, service, status, overdue);
            List<object> chartData = new List<object>();        
            List<Tuple<DateTime, string>> timePoint = new List<Tuple<DateTime, string>>();
            
            // 5. Filter by request category - Service tickets, Incidents, Adhoc
            switch (service)
            {
                case "Service requests":
                    foreach (Ticket ticket in myTicketsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "service"));
                    }

                    //--

                    var grpTimeSeriesData = timePoint.GroupBy(i => i.Item1.ToString("yyyy-MM"))
                    .Select(grp => new
                    {
                        SDate = grp.Key,
                        nService = grp.Where(i => i.Item2 == "service").Count()                      
                       
                    })
                    .OrderBy(o => o.SDate);


                    chartData.Add(new[] { "x", "Service"});

                    foreach (var ticket in grpTimeSeriesData)
                    {
                        chartData.Add(new object[] { ticket.SDate, ticket.nService});
                    }

                    //--

                    break;
                case "Adhoc":
                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "adhoc"));
                    }

                    // --
                    var grpTimeSeriesData3 = timePoint.GroupBy(i => i.Item1.ToString("yyyy-MM"))
                      .Select(grp => new
                      {
                          SDate = grp.Key,                         
                          nAdhoc = grp.Where(i => i.Item2 == "adhoc").Count(),
                          total = grp.Count()
                      })
                      .OrderBy(o => o.SDate);


                    chartData.Add(new[] { "x", "Adhoc" });

                    foreach (var ticket in grpTimeSeriesData3)
                    {
                        chartData.Add(new object[] { ticket.SDate, ticket.nAdhoc });
                    }

                    //--

                    break;
                case "Incidents":
                    foreach (Incident ticket in myIncidentsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "incident"));
                    }

                    // --
                    var grpTimeSeriesData4 = timePoint.GroupBy(i => i.Item1.ToString("yyyy-MM"))
                      .Select(grp => new
                      {
                          SDate = grp.Key,                        
                          nIncident = grp.Where(i => i.Item2 == "incident").Count(),                          
                          total = grp.Count()
                      })
                      .OrderBy(o => o.SDate);


                    chartData.Add(new[] { "x", "Incident" });

                    foreach (var ticket in grpTimeSeriesData4)
                    {
                        chartData.Add(new object[] { ticket.SDate, ticket.nIncident});
                    }

                    //--

                    break;
                default:
                    foreach (Ticket ticket in myTicketsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "service"));

                    }

                    foreach (Incident ticket in myIncidentsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "incident"));
                    }

                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        timePoint.Add(Tuple.Create(ticket.SubmittedDate, "adhoc"));
                    }

                    // --
                    var grpTimeSeriesData2 = timePoint.GroupBy(i => i.Item1.ToString("yyyy-MM"))
                      .Select(grp => new
                      {
                          SDate = grp.Key,
                          nService = grp.Where(i => i.Item2 == "service").Count(),
                          nIncident = grp.Where(i => i.Item2 == "incident").Count(),
                          nAdhoc = grp.Where(i => i.Item2 == "adhoc").Count(),
                          total = grp.Count()
                      })
                      .OrderBy(o => o.SDate);


                    chartData.Add(new[] { "x", "Service", "Incident", "Adhoc" });

                    foreach (var ticket in grpTimeSeriesData2)
                    {
                        chartData.Add(new object[] { ticket.SDate, ticket.nService, ticket.nIncident, ticket.nAdhoc });
                    }

                    //--

                    break;
            }         
         

            return chartData;
        }


        // Method implements logic for pie/doughnut chart
        public static List<object> PiechartData(CrudContext myContext, string pYear, string location, string service, string status, string overdue, string actor)
        {
            setFilterParameters(myContext, pYear, location, service, status, overdue);

            List<object> chartData = new List<object>();
            List<string> timePoint = new List<string>();
          
           
            // 5. Filter by request category - Service tickets, Incidents, Adhoc
            if (service != "All")
            {
                switch (service)
                {
                    case "Service requests":
                        foreach (Ticket ticket in myTicketsList)
                        {
                            if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                            else timePoint.Add(ticket.SupportAnalyst == null? "NA": ticket.SupportAnalyst);

                        }
                        break;
                    case "Incidents":
                        foreach (Incident ticket in myIncidentsList)
                        {
                            if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                            else timePoint.Add(ticket.SupportAnalyst == null ? "NA" : ticket.SupportAnalyst);
                        }
                        break;
                    case "Adhoc":
                        foreach (Adhoc ticket in myAdhocsList)
                        {
                            if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                            else timePoint.Add(ticket.SupportAnalyst == null ? "NA" : ticket.SupportAnalyst);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                foreach (Ticket ticket in myTicketsList)
                {
                    if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                    else timePoint.Add(ticket.SupportAnalyst == null ? "NA" : ticket.SupportAnalyst);
                }
                foreach (Incident ticket in myIncidentsList)
                {
                    if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                    else timePoint.Add(ticket.SupportAnalyst == null ? "NA" : ticket.SupportAnalyst);
                }
                foreach (Adhoc ticket in myAdhocsList)
                {
                    if (actor == "requestor") timePoint.Add(ticket.RequestorID);
                    else timePoint.Add(ticket.SupportAnalyst == null ? "NA" : ticket.SupportAnalyst);
                }
            }

            chartData.Add(new object[]
            {
            "Requestor", "Quantity"
            });
          

            var numberGroups = timePoint.GroupBy(i => i)
                     .Select(grp => new
                     {
                         Status = grp.Key,
                         total = grp.Count()
                     });                  
          

            foreach (var ticket in numberGroups)
            {
                chartData.Add(new object[] {ticket.Status, ticket.total });
            }  

            return chartData;
        }


        // Method implements logic for tree chart
        public static List<object> TreeChartData(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {
            setFilterParameters(myContext, pYear, location, service, status, overdue);

            List<object> chartData = new List<object>();
            chartData.Add(new object[]  { "ID", "Parent", "Number Of Lines"});
            chartData.Add(new object[] { "All Requests", null, 0 });
            chartData.Add(new object[] { "Tickets", "All Requests", null});
            chartData.Add(new object[] { "Incidents", "All Requests", null });
            chartData.Add(new object[] { "Adhoc", "All Requests", null });            

            chartData.Add(new object[] { "Submitted Tickets", "Tickets", null });
            chartData.Add(new object[] { "Completed Tickets", "Tickets", null });
            chartData.Add(new object[] { "Cancelled Tickets", "Tickets", null });
            chartData.Add(new object[] { "On hold Tickets", "Tickets", null });
            chartData.Add(new object[] { "Other Tickets", "Tickets", null });

            chartData.Add(new object[] { "Submitted Incidents", "Incidents", null });
            chartData.Add(new object[] { "Completed Incidents", "Incidents", null });
            chartData.Add(new object[] { "Cancelled Incidents", "Incidents", null });
            chartData.Add(new object[] { "Other Incidents", "Incidents", null });

            chartData.Add(new object[] { "Submitted Adhoc", "Adhoc", null });
            chartData.Add(new object[] { "Completed Adhoc", "Adhoc", null });
            chartData.Add(new object[] { "Cancelled Adhoc", "Adhoc", null });
            chartData.Add(new object[] { "On hold Adhoc", "Adhoc", null });
            chartData.Add(new object[] { "Other Adhoc", "Adhoc", null });
          
            // 5. Filter by Service            
                switch (service)
                {
                    case "Service requests":
                        foreach (Ticket ticket in myTicketsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Tickets";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Tickets";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Tickets";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Tickets";
                                    break;
                                default:
                                    tStatus = "Other Tickets";
                                    break;
                            }
                            chartData.Add(new object[] { "ticket: " + ticket.ID, tStatus, 1 });
                        }
                        break;
                    case "Incidents":
                        foreach (Incident ticket in myIncidentsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Incidents";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Incidents";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Incidents";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Incidents";
                                    break;
                                default:
                                    tStatus = "Other Incidents";
                                    break;
                            }
                            chartData.Add(new object[] { "incident: " + ticket.ID, tStatus, 1 });
                        }
                        break;
                    case "Adhoc":
                        foreach (Adhoc ticket in myAdhocsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Adhoc";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Adhoc";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Adhoc";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Adhoc";
                                    break;
                                default:
                                    tStatus = "Other Adhoc";
                                    break;
                            }
                            chartData.Add(new object[] { "adhoc: " + ticket.ID, tStatus, 1 });
                        }
                        break;


                    default:
                        foreach (Ticket ticket in myTicketsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Tickets";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Tickets";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Tickets";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Tickets";
                                    break;
                                default:
                                    tStatus = "Other Tickets";
                                    break;
                            }
                            chartData.Add(new object[] { "ticket: " + ticket.ID, tStatus, 1 });
                        }

                        foreach (Incident ticket in myIncidentsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Incidents";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Incidents";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Incidents";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Incidents";
                                    break;
                                default:
                                    tStatus = "Other Incidents";
                                    break;
                            }
                            chartData.Add(new object[] { "incident: " + ticket.ID, tStatus, 1 });
                        }

                        foreach (Adhoc ticket in myAdhocsList)
                        {
                            string tStatus = ticket.Status;

                            switch (tStatus)
                            {
                                case "Submitted":
                                    tStatus = "Submitted Adhoc";
                                    break;
                                case "Cancelled":
                                    tStatus = "Cancelled Adhoc";
                                    break;
                                case "On hold":
                                    tStatus = "On hold Adhoc";
                                    break;
                                case "Completed":
                                    tStatus = "Completed Adhoc";
                                    break;
                                default:
                                    tStatus = "Other Adhoc";
                                    break;
                            }
                            chartData.Add(new object[] { "adhoc: " + ticket.ID, tStatus, 1 });
                        }
                        break;
                }        
            
            return chartData;
        }

        // Method implements logic for calendar chart
        public static List<object> CalendarChartData(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {
            // initialize filters
            setFilterParameters(myContext, pYear, location, service, status, overdue);

            List<object> chartData = new List<object>();
            List<Requests> combinedList = new List<Requests>();
                      
            // 5. Filter by request category - Service tickets, Incidents, Adhoc
            switch (service)
            {
                case "Service requests":
                    foreach (Ticket ticket in myTicketsList)
                    {
                        combinedList.Add(ticket);
                    }
                    break;
                case "Adhoc":
                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        combinedList.Add(ticket);
                    }
                    break;
                case "Incidents":
                    foreach (Incident ticket in myIncidentsList)
                    {
                        combinedList.Add(ticket);
                    }
                    break;
                default:
                    foreach (Ticket ticket in myTicketsList)
                    {
                        combinedList.Add(ticket);
                    }

                    foreach (Incident ticket in myIncidentsList)
                    {
                        combinedList.Add(ticket);
                    }

                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        combinedList.Add(ticket);
                    }
                    break;
            }            

            var numberGroups = combinedList.GroupBy(i => i.SubmittedDate.Date)
            .Select(grp => new
            {
                SDate = grp.Key,
                total = grp.Count()
            });


            chartData.Add(new object[] { "Year", "Month", "Day", "Instances" });

            foreach (var ticket in numberGroups)
            {
                chartData.Add(new object[] { ticket.SDate.Year, ticket.SDate.Month-1, ticket.SDate.Day, ticket.total });
               
            }          

            return chartData;
        }

        // Method implements logic for Google map
        public static List<object> MapChartData(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {
            // initialize filters
            setFilterParameters(myContext, pYear, location, service, status, overdue);

            List<object> chartData = new List<object>();
            List<GeoStatItem> locationStats = new List<GeoStatItem>();                 

            // 5. Filter by request category - Service tickets, Incidents, Adhoc
            switch (service)
            {
                case "Service requests":
                    foreach (Ticket ticket in myTicketsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 1, 0, 0));
                    }
                    break;

                case "Adhoc":
                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 0, 0, 1));
                    }
                    break;

                case "Incidents":
                    foreach (Incident ticket in myIncidentsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 0, 1, 0));
                    }
                    break;

                default:
                    foreach (Ticket ticket in myTicketsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 1, 0, 0));
                    }

                    foreach (Incident ticket in myIncidentsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 0, 1, 0));
                    }

                    foreach (Adhoc ticket in myAdhocsList)
                    {
                        locationStats.Add(new GeoStatItem(ticket.latitute, ticket.longtitude, ticket.requestLocation, 0, 0, 1));
                    }

                    break;
            }


            var groupedLocationStats = locationStats.GroupBy(l => new { l.locName, l.latitude, l.longtitude, })
                 .Select(grp => new {
                     location = grp.Key.locName,
                     lat = grp.Key.latitude,
                     lon = grp.Key.longtitude,
                     totalTickets = grp.Sum(i => i.tickets),
                     totalIncidents = grp.Sum(j => j.incidents),
                     totalAdhocs =  grp.Sum(k => k.adhocs),
                     totalAll = grp.Sum(i => i.tickets) + grp.Sum(j => j.incidents) + grp.Sum(k => k.adhocs)
                 }).ToList();         
                                         

            int numTickets = myTicketsList.Count + myIncidentsList.Count + myAdhocsList.Count;          

            chartData.Add(new object[] { "Lat", "Long", "Name" });

            foreach (var ticket in groupedLocationStats)
            {
                string details = String.Format("{0}: {1} total requests ({2} % of all locations); {3} service requests, {4} incidents, {5} adhoc. ",
                    ticket.location, ticket.totalAll, (((double)ticket.totalAll / (double)numTickets) * 100.00).ToString("#.##"), ticket.totalTickets,
                    ticket.totalIncidents, ticket.totalAdhocs);

                chartData.Add(new object[] { ticket.lat, ticket.lon, details });
            }         

            return chartData;
        }

        // Method implements logic data table
        public static List<object> TableChartData(CrudContext myContext, string pYear, string location, string service, string status, string overdue)
        {   

            List<object> chartData = new List<object>();

            // 5. Filter by request category - Service tickets, Incidents, Adhoc

            chartData.Add(new object[] { "Request Category", "ID", "Submitted On",  "Requestor",
                "Service/Priority", "Due Date", "Status", "Support Analyst", "Completed On"});

            switch (service)
            {
                case "Service requests":
                    foreach (var item in myTicketsList)
                    {
                        chartData.Add(new object[] { "Service request", item.ID, item.SubmittedDate,
                        item.RequestorID, item.RequiredService, item.RequiredByDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }
                    break;

                case "Adhoc":
                    foreach (var item in myAdhocsList)
                    {
                        chartData.Add(new object[] { "Adhoc", item.ID, item.SubmittedDate,
                        item.RequestorID, item.Type, item.SubmittedDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }
                    break;

                case "Incidents":
                    foreach (var item in myIncidentsList)
                    {
                        chartData.Add(new object[] { "Incident", item.ID, item.SubmittedDate,
                        item.RequestorID, item.Severity, item.SubmittedDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }
                    break;

                default:
                    foreach (var item in myTicketsList)
                    {
                        chartData.Add(new object[] { "Service request", item.ID, item.SubmittedDate,
                        item.RequestorID, item.RequiredService, item.RequiredByDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }
                    foreach (var item in myIncidentsList)
                    {
                        chartData.Add(new object[] { "Incident", item.ID, item.SubmittedDate,
                        item.RequestorID, item.Severity, item.SubmittedDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }

                    foreach (var item in myAdhocsList)
                    {
                        chartData.Add(new object[] { "Adhoc", item.ID, item.SubmittedDate,
                        item.RequestorID, item.Type, item.SubmittedDate, item.Status, item.SupportAnalyst, item.CompletedDate });
                    }
                    break;
            }           

            return chartData;
        }

        // Method implements logic for raw data extract for REST API Get Request
        public static List<Requests> GetRawData(CrudContext myContext)
        {          
            List<Requests> myRequestList = myContext.Tickets.ToList().ConvertAll(x => (Requests)x);
            return myRequestList;
        }

    }
}
