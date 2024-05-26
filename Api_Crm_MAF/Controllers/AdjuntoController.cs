using Api_Crm_MAF.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [System.Web.Http.RoutePrefix("api/adjunto")]
    public class AdjuntoController : ApiController
    {
        [System.Web.Http.HttpPost]
        public Respuesta Post([FromBody] Adjunto Js_Adjunto)
        {
            int INDICADOR_EXITO = 1;
            int INDICADOR_ERROR = 0;

            Respuesta Resultado = new Respuesta();
            Resultado.indicadorExito = INDICADOR_ERROR;
            Resultado.descripcionError = "";

            IOrganizationService servicio;
            try
            {
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;

                Guid attachmentId = Guid.Empty;
                string entitytype = "incident";
                Entity note = new Entity("annotation");
                note["objectid"] = new EntityReference(entitytype, new Guid(Js_Adjunto.gEntidad));
                note["subject"] = Js_Adjunto.Asunto;
                note["filename"] = Js_Adjunto.NombreArchivo;
                note["documentbody"] = Js_Adjunto.Data;
                note["objecttypecode"] = entitytype;
                note["isdocument"] = true;
                attachmentId = servicio.Create(note);

                Resultado.indicadorExito = INDICADOR_EXITO;
                Resultado.descripcionError = "Archivo adjuntado correctamente";
            }
            catch (ArgumentException ex)
            {
                Resultado.indicadorExito = INDICADOR_ERROR;
                Resultado.descripcionError = ex.Message;
            }
            catch (Exception ex)
            {
                Resultado.indicadorExito = INDICADOR_ERROR;
                Resultado.descripcionError = ex.Message;
            }
            finally
            {

            }
            return Resultado;
        }
    }
}
