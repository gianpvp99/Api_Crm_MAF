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
    [System.Web.Http.RoutePrefix("api/CasosPortal")]
    public class CasosPortalController : ApiController
    {
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetCasosPortal(string NroDocumento)
        {
            string strJson = "";
            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;
                List<mLstPortal> listOfLstPortal = new List<mLstPortal>();

                string strFetch = "";
                if (NroDocumento.Trim().Length == 11){
                    strFetch = @"<fetch>
                    <entity name=""incident"">
                    <attribute name=""onetoone_comentarios"" alias=""Comentarios"" />
                    <attribute name=""ticketnumber"" alias=""CodigoCaso"" />
                    <attribute name=""casetypecode"" alias=""TipoCaso"" />
                    <attribute name=""onetoone_diasabiertosp"" alias=""DiasTranscurridos"" />
                    <attribute name=""statecode"" alias=""Estado"" />
                    <attribute name=""onetoone_fechaderecepcion"" alias=""FechaRecepcion"" />
                    <link-entity name=""onetoone_tema"" from=""onetoone_temaid"" to=""onetoone_tema"" link-type=""outer"">
                        <attribute name=""onetoone_name"" alias=""SubMotivo"" />
                    </link-entity>
                    <link-entity name=""onetoone_asunto"" from=""onetoone_asuntoid"" to=""onetoone_asunto"" link-type=""outer"">
                        <attribute name=""onetoone_name"" alias=""Motivo"" />
                    </link-entity>
                    <link-entity name=""account"" from=""accountid"" to=""customerid"">
                        <attribute name=""accountnumber"" alias=""onetoone_numerodedocumento"" />
                        <filter>
                        <condition attribute=""accountnumber"" operator=""eq"" value=""{0}"" />
                        </filter>
                    </link-entity>
                    </entity>
                    </fetch>";
                }
                else {
                    strFetch = @"<fetch>
                  <entity name=""incident"">
                    <attribute name=""onetoone_comentarios"" alias=""Comentarios"" />
                    <attribute name=""ticketnumber"" alias=""CodigoCaso"" />
                    <attribute name=""casetypecode"" alias=""TipoCaso"" />
                    <attribute name=""onetoone_diasabiertosp"" alias=""DiasTranscurridos"" />
                    <attribute name=""statecode"" alias=""Estado"" />
                    <attribute name=""onetoone_fechaderecepcion"" alias=""FechaRecepcion"" />
                    <link-entity name=""onetoone_tema"" from=""onetoone_temaid"" to=""onetoone_tema"" link-type=""outer"">
                      <attribute name=""onetoone_name"" alias=""SubMotivo"" />
                    </link-entity>
                    <link-entity name=""onetoone_asunto"" from=""onetoone_asuntoid"" to=""onetoone_asunto"" link-type=""outer"">
                      <attribute name=""onetoone_name"" alias=""Motivo"" />
                    </link-entity>
                    <link-entity name=""contact"" from=""contactid"" to=""customerid"">
                      <attribute name=""onetoone_numerodedocumento"" />
                      <filter>
                        <condition attribute=""onetoone_numerodedocumento"" operator=""eq"" value=""{0}"" />
                      </filter>
                    </link-entity>
                  </entity>
                </fetch>";
                }                
                strFetch = string.Format(strFetch, NroDocumento);
                EntityCollection ecEntidades = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecEntidades.Entities.Count == 0)
                {
                    strJson = "No contiene información";
                }
                else
                {
                    int intValor = 0;
                    string strDato = "";
                    foreach (var eEntidad in ecEntidades.Entities)
                    {
                        mLstPortal vLstPortal = new mLstPortal();
                        if (eEntidad.Attributes.Contains("CodigoCaso"))
                        {
                            vLstPortal.CodigoCaso = (string)eEntidad.GetAttributeValue<AliasedValue>("CodigoCaso").Value;
                        }
                        if (eEntidad.Attributes.Contains("TipoCaso"))
                        {
                            intValor = ((OptionSetValue)eEntidad.GetAttributeValue<AliasedValue>("TipoCaso").Value).Value;
                            switch (intValor)
                            {
                                case 5:
                                    strDato = "Arco";
                                    break;
                                case 8:
                                    strDato = "Consulta";
                                    break;
                                case 11:
                                    strDato = "Denuncia";
                                    break;
                                case 4:
                                    strDato = "Entidad Gubernamental";
                                    break;
                                case 7:
                                    strDato = "Interacción";
                                    break;
                                case 6:
                                    strDato = "Otros";
                                    break;
                                case 2:
                                    strDato = "Queja - (Opción ya no usada)";
                                    break;
                                case 9:
                                    strDato = "Quejas";
                                    break;
                                case 1:
                                    strDato = "Reclamo";
                                    break;
                                case 12:
                                    strDato = "Reclamo Indecopi";
                                    break;
                                case 3:
                                    strDato = "Requerimiento";
                                    break;
                                case 10:
                                    strDato = "Solicitud";
                                    break;

                            }
                            vLstPortal.TipoCaso = strDato;
                        }
                        if (eEntidad.Attributes.Contains("DiasTranscurridos"))
                        {
                            vLstPortal.DiasTranscurridos = (int)eEntidad.GetAttributeValue<AliasedValue>("DiasTranscurridos").Value;
                        }
                        if (eEntidad.Attributes.Contains("Estado"))
                        {
                            intValor = ((OptionSetValue)eEntidad.GetAttributeValue<AliasedValue>("Estado").Value).Value;
                            switch (intValor)
                            {
                                case 0:
                                    strDato = "Activo";
                                    break;
                                case 1:
                                    strDato = "Resuelto";
                                    break;
                                case 2:
                                    strDato = "Cancelado";
                                    break;
                            }
                            vLstPortal.Estado = strDato;
                        }
                        if (eEntidad.Attributes.Contains("FechaRecepcion"))
                        {
                            vLstPortal.FechaRecepcion = ((DateTime)eEntidad.GetAttributeValue<AliasedValue>("FechaRecepcion").Value).ToString("dd-MM-yyyy");
                        }
                        if (eEntidad.Attributes.Contains("Motivo"))
                        {
                            vLstPortal.Motivo = (string)eEntidad.GetAttributeValue<AliasedValue>("Motivo").Value;
                        }
                        if (eEntidad.Attributes.Contains("SubMotivo"))
                        {
                            vLstPortal.SubMotivo = (string)eEntidad.GetAttributeValue<AliasedValue>("SubMotivo").Value;
                        }                        
                        
                        listOfLstPortal.Add(new mLstPortal { CodigoCaso = vLstPortal.CodigoCaso, 
                            TipoCaso = vLstPortal.TipoCaso, 
                            DiasTranscurridos = vLstPortal.DiasTranscurridos,
                            Estado = vLstPortal.Estado,
                            FechaRecepcion = vLstPortal.FechaRecepcion,
                            Motivo = vLstPortal.Motivo,
                            SubMotivo = vLstPortal.SubMotivo
                        });
                    }
                    //Pase a Json
                    strJson = JsonConvert.SerializeObject(listOfLstPortal);
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
