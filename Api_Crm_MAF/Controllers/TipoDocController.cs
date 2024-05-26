using Api_Crm_MAF.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [Authorize]
    [RoutePrefix("api/TipoDoc")]
    public class TipoDocController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAll()
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mTipoDoc> listOfTipoDoc = new List<mTipoDoc>();

                string strFetch = @"<fetch>
                <entity name=""onetoone_parametro"">
                <attribute name=""onetoone_valorparametro"" />
                <attribute name=""onetoone_name"" />
                <filter type=""and"">
                <condition attribute=""onetoone_entidadproceso"" operator=""eq"" value=""{0}"" />
                <condition attribute=""statecode"" operator=""eq"" value=""0"" />
                </filter>
                </entity>
                </fetch>";
                strFetch = string.Format(strFetch, "TipoDoc");
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mTipoDoc vPaises = new mTipoDoc();
                        if (eEntidad.Attributes.Contains("onetoone_valorparametro"))
                        {
                            vPaises.Codigo = (string)eEntidad.Attributes["onetoone_valorparametro"];
                        }
                        if (eEntidad.Attributes.Contains("onetoone_name"))
                        {
                            vPaises.Nombre = (string)eEntidad.Attributes["onetoone_name"];
                        }
                        listOfTipoDoc.Add(new mTipoDoc { Codigo = vPaises.Codigo, Nombre = vPaises.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfTipoDoc);
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
