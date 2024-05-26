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
    [System.Web.Http.RoutePrefix("api/Distrito")]
    public class DistritoController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetDistrito(string CodProvincia)
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mDistrito> listOfDistrito = new List<mDistrito>();

                string strFetch = @"<fetch>
                <entity name=""onetoone_distrito"">
                <attribute name=""onetoone_distritoid"" />
                <attribute name=""onetoone_name"" />
                <filter>
                <condition attribute=""onetoone_provinciaid"" operator=""eq"" value=""{0}"" />
                </filter>
                </entity>
                </fetch>";
                strFetch = string.Format(strFetch, CodProvincia);
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mDistrito vDistrito = new mDistrito();
                        if (eEntidad.Attributes.Contains("onetoone_distritoid"))
                        {
                            vDistrito.Codigo = ((Guid)eEntidad.Attributes["onetoone_distritoid"]).ToString();
                        }
                        if (eEntidad.Attributes.Contains("onetoone_name"))
                        {
                            vDistrito.Nombre = (string)eEntidad.Attributes["onetoone_name"];
                        }
                        listOfDistrito.Add(new mDistrito { Codigo = vDistrito.Codigo, Nombre = vDistrito.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfDistrito);
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
