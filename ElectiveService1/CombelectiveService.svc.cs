using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using RestSharp;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using System.Diagnostics;

namespace ElectiveService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : comelectIService
    {
        public string[] Weather5day(string zipcode)
        {
            //i am using web api to create my weather forecast service, alt would have been to use xml loader to make calls to soap service
           
            var client = new HttpClient();

            //i make queries based on zip code
           
            string api = string.Format(" https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{0}?unitGroup=metric&key=GMBSTCVYTATAWSMEYJZFL584C&contentType=json", zipcode);

            var request = new HttpRequestMessage(HttpMethod.Get, api);

            var response = client.SendAsync(request).Result;

            //if zip code is wrong i create an exception

            try
            {
                response.EnsureSuccessStatusCode();
            }

            catch (Exception ex)
            {   
                
                 //if there is an error, i return null object
                Debug.WriteLine(ex);
                return null; 
            }


            //else i read the json result

            var body = response.Content.ReadAsStringAsync().Result;

            //then i parse the json

            dynamic weather = JsonConvert.DeserializeObject(body);

             Int32 date = 0;

            string[] report = new string[5];

            //store 5day forecast as csv string

             foreach (var day in weather.days)
             {

                 string weather_date = day.datetime;
                 string weather_desc = day.description;
                 string weather_tmax = day.tempmax;
                 string weather_tmin = day.tempmin;

                string day_report =
                  weather_date + "," + weather_desc + "," + weather_tmax + "," +  weather_tmin;

                report[date] = day_report;

                date++;
                if (date == 5)
                    break;


             } 
             //return report

            return report ;
            
        }

        
    }
}
