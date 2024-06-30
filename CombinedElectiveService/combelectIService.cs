using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CombinedElectiveService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface combelectIService
    {


        //this is service 1  this is weather forecast elective 1

        [OperationContract]
        string[] Weather5day(string zipcode);

        

        //this is service 2 solar saving elective

        [OperationContract]
        string GetSolarsaving(string zipcode, int effitiency, int area);




    }   
}
