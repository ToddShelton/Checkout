using System;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    // Bing REST service schema: http://rbrundritt.wordpress.com/2012/01/06/bing-maps-rest-service-net-libraries/

    /// <summary>
    /// Orleans grain communication interface IGeocodeGrain
    /// </summary>

    public interface IServiceGrainBingLocation : IGrainWithGuidKey
    {
        /// <summary>
        /// Gets the latitude and longitude (geocode) of the supplied location parameters.
        /// </summary>
        /// <param name="addressLine">Address.  Can include city, state zip.  </param>
        /// <param name="locality">City or town. </param>
        /// <param name="adminRegion">State or province</param>
        /// <param name="postalCode">Zip or postal code. </param>
        /// <param name="countryRegion">ISO country code.  Defaults to US.  </param>
        /// <returns>Geocode(Latitude, Longitude)</returns>
        Task<GeoLocation> GetGeocodeAsync(
             string addressLine = null, string locality = null, string adminDistrict = null, string postalCode = null, string countryRegion = "US");

        Task<GeoLocation> GetGeocodeAsync(GeoLocation geoLocation);

        Task<GeoLocation> GetGeocodeAsync();
    }

    public interface IGeoLocationGrainState : IGrainState
    {
        GeoLocation BingLocation { get; set; }
    }

    [Serializable]
    public class GeoLocation : BaseAddress
    {
        public GeoLocation()
        {
        }

        public GeoLocation(BaseAddress baseAddress)
        {
            this.addressLine = baseAddress.AddressLine1;
            this.adminDistrict = baseAddress.StateProvince;
            this.countryRegion = baseAddress.Country;
            this.locality = baseAddress.City;
            this.postalCode = baseAddress.ZipPostalCode;
            this.latitude = baseAddress.Latitude;
            this.longitude = baseAddress.Longitude;
        }

        public string addressLine
        {
            get { return base.AddressLine1; }
            set { base.AddressLine1 = value; }
        }

        public string adminDistrict
        {
            get { return base.StateProvince; }
            set { base.StateProvince = value; }
        }

        public string countryRegion
        {
            get { return base.Country; }
            set { base.Country = value; }
        }

        public string locality
        {
            get { return base.City; }
            set { base.City = value; }
        }

        public string postalCode
        {
            get { return base.ZipPostalCode; }
            set { base.ZipPostalCode = value; }
        }

        public decimal latitude
        {
            get { return base.Latitude; }
            set { base.Latitude = value; }
        }

        public decimal longitude
        {
            get { return base.Longitude; }
            set { base.Longitude = value; }
        }
    }
}