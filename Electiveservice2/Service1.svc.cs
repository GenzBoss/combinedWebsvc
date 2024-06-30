using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Electiveservice2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetSolarsaving (string zipcode , int effitiency, int area)
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

            Double saving = 0;

            double rad = Convert.ToDouble(radiance);
            double electprice = Convert.ToDouble(electricityrateres);

            saving = rad * 365 * electprice * area * effitiency/100;



            string output = String.Format("average electricity cost in your location is {0}$/kWh , the average solar radiation per year is {1}kWh/m2, the saving per year in electricity cost for a panel of area {2}m2 and efficienty {3}% is {4}$/year", electricityrateres, radiance, area, effitiency, saving);


            Debug.Write(output);






            return output;

        }

    }
}
