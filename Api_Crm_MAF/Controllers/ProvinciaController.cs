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
    [RoutePrefix("api/Provincia")]
    public class ProvinciaController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetProvincia(string CodDepartamento)
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mProvincia> listOfProvincia = new List<mProvincia>();

                string strFetch = @"<fetch>
                <entity name=""onetoone_provincia"">
                <attribute name=""onetoone_name"" />
                <attribute name=""onetoone_provinciaid"" />
                <filter>
                <condition attribute=""onetoone_departamentoid"" operator=""eq"" value=""{0}"" />
                </filter>
                </entity>
                </fetch>";
                strFetch = string.Format(strFetch, CodDepartamento);
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mProvincia vProvincia = new mProvincia();
                        if (eEntidad.Attributes.Contains("onetoone_provinciaid"))
                        {
                            vProvincia.Codigo = ((Guid)eEntidad.Attributes["onetoone_provinciaid"]).ToString();
                        }
                        if (eEntidad.Attributes.Contains("onetoone_name"))
                        {
                            vProvincia.Nombre = (string)eEntidad.Attributes["onetoone_name"];
                        }
                        listOfProvincia.Add(new mProvincia { Codigo = vProvincia.Codigo, Nombre = vProvincia.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfProvincia);
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
