using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Cloudrocket.Interfaces;
using Orleans;
using Orleans.Providers; 

namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class ServiceGrainBingLocation : Grain<IServiceGrainBingLocationState>, IServiceGrainBingLocation
    {
        // Bing REST service: http://msdn.microsoft.com/en-us/library/hh534080.aspx

        private string _bingMapsKey;
        private StringBuilder _queryBuilder;
        private UriBuilder _uriBuilder;

        public override Task OnActivateAsync()
        {
            // Initialize grain state
            // TODO: Get Bing configuration from a stateless configuration grain.
            this._bingMapsKey = ConfigurationManager.AppSettings["BingMapsKey"];
            this._queryBuilder = new StringBuilder();
            this._uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = ConfigurationManager.AppSettings["BingMapsHost"],
                Path = ConfigurationManager.AppSettings["BingMapsLocationService"],
            };

            return base.OnActivateAsync();
        }

        public async Task<GeoLocation> GetGeocodeAsync()
        {
            return await Task.FromResult(new GeoLocation());
        }

        public async Task<GeoLocation> GetGeocodeAsync(string addressLine, string locality, string adminDistrict, string postalCode, string countryRegion)
        {
            State.BingLocation.addressLine = addressLine;
            State.BingLocation.locality = locality;
            State.BingLocation.adminDistrict = adminDistrict;
            State.BingLocation.postalCode = postalCode;
            State.BingLocation.countryRegion = countryRegion;

            return await GetGeocodeAsync(State.BingLocation);
        }

        public async Task<GeoLocation> GetGeocodeAsync(GeoLocation geoLocation)
        {
            State.BingLocation = geoLocation;

            _queryBuilder.Clear();
            _queryBuilder.Append("addressLine=" + State.BingLocation.addressLine + "&");
            _queryBuilder.Append("locality=" + State.BingLocation.locality + "&"); // City
            _queryBuilder.Append("adminDistrict=" + State.BingLocation.adminDistrict + "&"); //State
            _queryBuilder.Append("postalCode=" + State.BingLocation.postalCode + "&");
            _queryBuilder.Append("countryRegion=" + State.BingLocation.countryRegion + "&");
            _queryBuilder.Append("output=xml" + "&");
            _queryBuilder.Append("key=" + _bingMapsKey);

            // "#" breaks REST Uris, but is common in addresses (1234 Your Street #42).
            _uriBuilder.Query = _queryBuilder.ToString().Replace("#", String.Empty);

            HttpWebRequest request = WebRequest.Create(_uriBuilder.Uri) as HttpWebRequest;

            try
            {
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(response.GetResponseStream());

                    State.BingLocation.latitude = Convert.ToDecimal(xmlDoc.GetElementsByTagName("Latitude").Item(0).InnerText);
                    State.BingLocation.longitude = Convert.ToDecimal(xmlDoc.GetElementsByTagName("Longitude").Item(0).InnerText);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return await Task.FromResult(State.BingLocation);
        }
    }

    public interface IServiceGrainBingLocationState : IGrainState
    {
        GeoLocation BingLocation { get; set; }
    }
}