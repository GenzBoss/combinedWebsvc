using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CombinedElectiveService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : combelectIService
    {

        //this is service 1 weather forecast

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
                  weather_date + "," + weather_desc + "," + weather_tmax + "," + weather_tmin;

                report[date] = day_report;

                date++;
                if (date == 5)
                    break;


            }
            //return report

            return report;

        }


        //This is service 2 solar saving

        public string GetSolarsaving(string zipcode, int effitiency, int area)
        {


            string electricityrateres, radiance;

            using (var client = new HttpClient())
            {

                string electapi = string.Format("https://developer.nrel.gov/api/utility_rates/v3.json?api_key=xQg2qiN348oltAVLG6iFfGLW9iE4ug4bfcaHGEtI&address={0}", zipcode);

                var request = new HttpRequestMessage(HttpMethod.Get, electapi);

                var response = client.SendAsync(request).Result;

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

                dynamic electricityprice = JsonConvert.DeserializeObject(body);

                //getting the price of electricity in the location for residents. The residential electricity rate ($/kWh)

                electricityrateres = electricityprice.outputs.residential.ToString();

            }


            //now call another api to get solar radiation in the area


            using (var client = new HttpClient())
            {

                string solarapi = string.Format("https://developer.nrel.gov/api/solar/solar_resource/v1.json?api_key=xQg2qiN348oltAVLG6iFfGLW9iE4ug4bfcaHGEtI&address={0}", zipcode);

                var request = new HttpRequestMessage(HttpMethod.Get, solarapi);

                var response = client.SendAsync(request).Result;

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

                dynamic solarpower = JsonConvert.DeserializeObject(body);

                radiance = solarpower.outputs.avg_ghi.annual.ToString();





            }

            //now calculate the saving using formla saving = energer * efficienty * area * price

            Double saving = 0;

            double rad = Convert.ToDouble(radiance);
            double electprice = Convert.ToDouble(electricityrateres);

            saving = rad * 365 * electprice * area * effitiency / 100;



            string output = String.Format("average electricity cost in your location is {0}$/kWh , the average solar radiation per year is {1}kWh/m2, the saving per year in electricity cost for a panel of area {2}m2 and efficienty {3}% is {4}$/year", electricityrateres, radiance, area, effitiency, saving);


            //Debug.Write(output);






            return output;

        }




    }

}
