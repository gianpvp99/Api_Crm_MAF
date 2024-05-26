using Api_Crm_MAF.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;


namespace Api_Crm_MAF.Controllers
{
    [Authorize]
    [System.Web.Http.RoutePrefix("api/Motivo")]
    public class MotivoController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetMotivo(string Tipo)
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mMotivo> listOfMotivo = new List<mMotivo>();

                string strValor;
                strValor = "0";
                if (Tipo == "C")  //Consulta
                {
                    strValor = "8";
                }
                if (Tipo == "S")  //Solicitud
                {
                    strValor = "10";
                }
                if (Tipo == "R")  //Reclamo
                {
                    strValor = "1";
                }
                if (Tipo == "Q")  //Queja
                {
                    strValor = "9";
                }
                string strFetch = @"<fetch distinct=""true"">
                    <entity name=""onetoone_tipificacion"">
                        <filter>
                            <condition attribute=""onetoone_categoria"" operator=""eq"" value=""{0}"" />
                            <condition attribute=""statuscode"" operator=""eq"" value=""1"" />
                        </filter>
                        <link-entity name=""onetoone_asunto"" from=""onetoone_asuntoid"" to=""onetoone_asunto"">
                            <attribute name=""onetoone_name"" alias=""Motivo"" />
                            <attribute name=""onetoone_asuntoid"" alias=""Id"" />
                        </link-entity>
                    </entity>
                </fetch>";
                strFetch = string.Format(strFetch, strValor);
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mMotivo vMotivo = new mMotivo();
                        if (eEntidad.Attributes.Contains("Id"))
                        {
                            vMotivo.Codigo = ((Guid)eEntidad.GetAttributeValue<AliasedValue>("Id").Value).ToString();
                        }
                        if (eEntidad.Attributes.Contains("Motivo"))
                        {
                            vMotivo.Nombre = (string)eEntidad.GetAttributeValue<AliasedValue>("Motivo").Value; 
                        }
                        listOfMotivo.Add(new mMotivo { Codigo = vMotivo.Codigo, Nombre = vMotivo.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfMotivo);
                }
            }
            catch (ArgumentException ex)
            {
                strJson = ex.Message;
            }
            catch (Exception ex)
            {
                strJson = ex.Message;
            }
            finally
            {

            }
            return Ok(strJson);
        }
    }
}
