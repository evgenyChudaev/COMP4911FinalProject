/*
 GeoStatItem model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of geolocational record that is used for Google Maps
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class GeoStatItem
    {
        public double latitude, longtitude;
        public string locName;
        public long tickets, incidents, adhocs;
        public static long totalRequests = 0, totalTickets, totalIncidents, totalAdhocs; // static variables needed to store cumulative numbers
        
        public GeoStatItem(double latitude, double longtitude, string locName, long tickets, long incidents, long adhocs)
        {
            this.latitude = latitude;
            this.longtitude = longtitude;
            this.locName = locName;
            this.tickets = tickets;
            this.incidents = incidents;
            this.adhocs = adhocs;
            totalRequests++;
            totalTickets += tickets;
            totalIncidents += incidents;
            totalAdhocs += adhocs;
        }
        
    }
}
