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
    [System.Web.Http.RoutePrefix("api/Departamento")]
    public class DepartamentoController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetDepartamento()
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mDepartamento> listOfDepartamento = new List<mDepartamento>();

                string strFetch = @"<fetch>
                <entity name=""onetoone_departamento"">
                <attribute name=""onetoone_departamentoid"" />
                <attribute name=""onetoone_name"" />
                </entity>
                </fetch>";
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mDepartamento vDepartamento = new mDepartamento();
                        if (eEntidad.Attributes.Contains("onetoone_departamentoid"))
                        {
                            vDepartamento.Codigo = ((Guid)eEntidad.Attributes["onetoone_departamentoid"]).ToString();
                        }
                        if (eEntidad.Attributes.Contains("onetoone_name"))
                        {
                            vDepartamento.Nombre = (string)eEntidad.Attributes["onetoone_name"];
                        }
                        listOfDepartamento.Add(new mDepartamento { Codigo = vDepartamento.Codigo, Nombre = vDepartamento.Nombre });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfDepartamento);
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
