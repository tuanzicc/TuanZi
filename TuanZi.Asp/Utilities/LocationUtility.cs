using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuanZi.Asp.Utilities
{
    public class LocationUtility
    {
        public static GeoLocation GetGeolocation(string address, string city, string province, string postalCode)
        {
            var location = new GeoLocation();
            try
            {
                
                var locationService = new GoogleLocationService("AIzaSyC-Pztde1OpSENwRlGZXdGrjrNX3LdDSFQ");

                var point = locationService.GetLatLongFromAddress(string.Format("{0},{1},{2},{3}", address, city, province, postalCode));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0},{1},{2}", address, city, province));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0},{1}", address, city));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0},{1}", address, postalCode));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0},{1}", city, province));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0}", address));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0}", city));
                if (point == null)
                    point = locationService.GetLatLongFromAddress(string.Format("{0}", province));
                if (point != null)
                {
                    location.Latitude = point.Latitude;
                    location.Longitude = point.Longitude;
                }
            }
            catch { }
            return location;
        }
    }

    public struct GeoLocation
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
