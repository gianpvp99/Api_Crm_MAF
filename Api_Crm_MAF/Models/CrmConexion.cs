using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using System.Net;

namespace Api_Crm_MAF.Models
{
    public class CrmConexion
    {
        public static IOrganizationService ConnectToCRM()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            CrmServiceClient service = new CrmServiceClient(ConfigurationManager.ConnectionStrings["crm"].ConnectionString);
            IOrganizationService _orgService = (IOrganizationService)service.OrganizationWebProxyClient != null ? (IOrganizationService)service.OrganizationWebProxyClient : (IOrganizationService)service.OrganizationServiceProxy;

            return _orgService;
        }
    }
}