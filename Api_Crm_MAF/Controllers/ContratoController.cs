using Api_Crm_MAF.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [Authorize]
    [RoutePrefix("api/contrato")]
    public class ContratoController : ApiController
    {
        // GET api/contrato/ObtenerPorDocumento?documento=20519203414
        [HttpGet]
        [Route("ObtenerPorDocumento")]
        public IHttpActionResult ObtenerPorDocumento(string documento)
        {
            if (string.IsNullOrEmpty(documento))
            {
                return BadRequest("El documento no puede estar vacío.");
            }

            try
            {
                IOrganizationService servicio = CrmConexion.ConnectToCRM();

                // Prepara FetchXML para ambos tipos de entidades: Contacto y Cuenta.
                string fetchXmlContact = $@"
            <fetch version='1.0' mapping='logical' distinct='true'>
                <entity name='onetoone_contrato'>
                    <attribute name='statecode'/>
                    <attribute name='onetoone_numerocontrato'/>
                    <attribute name='createdon'/>
                    <order attribute='createdon' descending='true'/>
                    <attribute name='onetoone_producto'/>
                    <attribute name='onetoone_fechaactivacion'/>
                    <attribute name='onetoone_estadocontrato'/>
                    <attribute name='onetoone_customerid'/>
                    <link-entity alias='cliente_relacionado' name='contact' from='contactid' to='onetoone_customerid' link-type='inner'>
                        <attribute name='onetoone_numerodedocumento'/>
                        <filter type='and'>
                            <condition attribute='onetoone_numerodedocumento' operator='eq' value='{documento}'/>
                        </filter>
                    </link-entity>
                </entity>
            </fetch>";

                string fetchXmlAccount = $@"
            <fetch version='1.0' mapping='logical' distinct='true'>
                <entity name='onetoone_contrato'>
                    <attribute name='statecode'/>
                    <attribute name='onetoone_numerocontrato'/>
                    <attribute name='createdon'/>
                    <order attribute='createdon' descending='true'/>
                    <attribute name='onetoone_producto'/>
                    <attribute name='onetoone_fechaactivacion'/>
                    <attribute name='onetoone_estadocontrato'/>
                    <attribute name='onetoone_customerid'/>
                    <link-entity alias='cuenta_relacionada' name='account' from='accountid' to='onetoone_customerid' link-type='inner'>
                        <attribute name='accountnumber'/>
                        <filter type='and'>
                            <condition attribute='accountnumber' operator='eq' value='{documento}'/>
                        </filter>
                    </link-entity>
                </entity>
            </fetch>";

                // Intenta primero con contacto
                EntityCollection resultadoContratos = servicio.RetrieveMultiple(new FetchExpression(fetchXmlContact));
                if (!resultadoContratos.Entities.Any())
                {
                    // Si no se encuentra nada, intenta con cuenta
                    resultadoContratos = servicio.RetrieveMultiple(new FetchExpression(fetchXmlAccount));
                }

                var contratos = resultadoContratos.Entities.Select(e => new
                {
                    //ContratoId = e.Id,
                    NumeroContrato = e.GetAttributeValue<string>("onetoone_numerocontrato"),
                    Producto = e.GetAttributeValue<EntityReference>("onetoone_producto")?.Name,
                    //FechaActivacion = e.GetAttributeValue<DateTime?>("onetoone_fechaactivacion"),
                    //Estado = e.FormattedValues["statecode"]
                }).ToList();

                if (!contratos.Any())
                {
                    return NotFound();
                }

                string jsonResponse = JsonConvert.SerializeObject(contratos);
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
