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
    [RoutePrefix("api/SubMotivo")]
    public class SubMotivoController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetSubMotivo(string Tipo, string Motivo)
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
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
                if (Tipo == "R") //Reclamo
                    strValor = "1";

                if (Tipo == "Q") //Queja
                    strValor = "9";

                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mSubMotivo> listOfSubMotivo = new List<mSubMotivo>();
                string strFetch = @"<fetch distinct=""true"">
                  <entity name=""onetoone_tipificacion"">                                         
                    <filter>
                      <condition attribute=""onetoone_categoria"" operator=""eq"" value=""{0}"" />
                      <condition attribute=""onetoone_asunto"" operator=""eq"" value=""{1}"" />
                      <condition attribute=""statuscode"" operator=""eq"" value=""1"" />
                    </filter>
                    <link-entity name=""onetoone_tema"" from=""onetoone_temaid"" to=""onetoone_tema"">
                      <attribute name=""onetoone_temaid"" alias=""Id"" />
                      <attribute name=""onetoone_name"" alias=""Nombre"" />
                    </link-entity>
                  </entity>
               </fetch>";
                strFetch = string.Format(strFetch, strValor, Motivo);
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mSubMotivo vSubMotivo = new mSubMotivo();
                        if (eEntidad.Attributes.Contains("Id"))
                        {
                            vSubMotivo.Codigo = ((Guid)eEntidad.GetAttributeValue<AliasedValue>("Id").Value).ToString();
                        }
                        if (eEntidad.Attributes.Contains("Nombre"))
                        {
                            vSubMotivo.Nombre = (string)eEntidad.GetAttributeValue<AliasedValue>("Nombre").Value;
                        }
                        listOfSubMotivo.Add(new mSubMotivo { Codigo = vSubMotivo.Codigo, Nombre = vSubMotivo.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfSubMotivo);
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
