using Api_Crm_MAF.Models;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [Authorize]
    [System.Web.Http.RoutePrefix("api/Registro")]
    public class RegistroController : ApiController
    {
        [System.Web.Http.HttpPost]
        public Respuesta Post([FromBody] mRegistro Js_Registro)
        {
            int INDICADOR_EXITO = 1;
            int INDICADOR_ERROR = 0;
            string strFetch = "";
            Guid gEmpresa = new Guid();
            Guid gContacto = new Guid();
            int eCia = 0;

            Respuesta Resultado = new Respuesta();
            Resultado.indicadorExito = 0;
            Resultado.descripcionError = "";
            IOrganizationService servicio;
            try
            {
                //Libro de Reclamaciones
                servicio = CrmConexion.ConnectToCRM();
                Guid orgId = ((WhoAmIResponse)servicio.Execute(new WhoAmIRequest())).OrganizationId;

                //Validacion de Empresa
                if (Js_Registro.RepresentaEmpresa == 1)
                {
                    if (Js_Registro.eNroDoc != "" || Js_Registro.eNroDoc == null)
                    {
                        eCia = 1;
                        strFetch = @"<fetch>
                          <entity name=""account"">
                            <attribute name=""accountnumber"" />
                            <attribute name=""name"" />
                            <attribute name=""accountid"" />
                            <filter>
                              <condition attribute=""accountnumber"" operator=""eq"" value=""{0}"" />
                            </filter>
                          </entity>
                        </fetch>";
                        strFetch = string.Format(strFetch, Js_Registro.eNroDoc);
                        EntityCollection ecEmpresa = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                        if (ecEmpresa.Entities.Count == 0)
                        {
                            //Crea Empresa
                            var cEmpresa = new Entity("account");
                            cEmpresa["name"] = Js_Registro.eRazonSocial;
                            cEmpresa["onetoone_nombrecomercial"] = Js_Registro.eRazonSocial;
                            cEmpresa["accountnumber"] = Js_Registro.eNroDoc;
                            gEmpresa = servicio.Create(cEmpresa);
                        }
                        else
                        {
                            Js_Registro.eRazonSocial = "";
                            gEmpresa = (Guid)ecEmpresa.Entities[0].Attributes["accountid"];
                            if (ecEmpresa.Entities[0].Attributes.Contains("name"))
                            {
                                Js_Registro.eRazonSocial = (String)ecEmpresa.Entities[0].Attributes["name"];
                            }                                
                        }
                    }
                }
                //Validacion de Persona                
                if (Js_Registro.NroDoc != "" || Js_Registro.NroDoc == null)
                {                    
                    strFetch = @"<fetch>
                      <entity name=""contact"">
                        <attribute name=""contactid"" />
                        <attribute name=""onetoone_tipodocumento"" />
                        <attribute name=""onetoone_numerodedocumento"" />
                        <filter>
                          <condition attribute=""onetoone_tipodocumento"" operator=""eq"" value=""{0}"" />
                          <condition attribute=""onetoone_numerodedocumento"" operator=""eq"" value=""{1}"" />
                        </filter>
                      </entity>
                    </fetch>";
                    strFetch = string.Format(strFetch, Js_Registro.TipoDoc, Js_Registro.NroDoc);
                    EntityCollection ecContacto = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                    if (ecContacto.Entities.Count == 0)
                    {
                        //Crea Contacto
                        var cContacto = new Entity("contact");
                        cContacto["firstname"] = Js_Registro.Nombres;
                        cContacto["lastname"] = Js_Registro.ApellidoPaterno + " " + Js_Registro.ApellidoMaterno;
                        cContacto["onetoone_tipodocumento"] = new OptionSetValue() { Value = Int32.Parse(Js_Registro.TipoDoc) };
                        cContacto["onetoone_numerodedocumento"] = Js_Registro.NroDoc;                        
                        cContacto["onetoone_primernombre"] = Js_Registro.Nombres;
                        cContacto["onetoone_apellidopaterno"] = Js_Registro.ApellidoPaterno;
                        cContacto["onetoone_apellidomaterno"] = Js_Registro.ApellidoMaterno;
                        cContacto["emailaddress1"] = Js_Registro.Email;
                        cContacto["mobilephone"] = Js_Registro.Celular;
                        cContacto["onetoone_paisid"] = new EntityReference("onetoone_pais", new Guid("ab8b1ecd-2271-e611-80ec-9cb65490a2a4"));
                        cContacto["onetoone_departamentoid"] = new EntityReference("onetoone_departamento", new Guid(Js_Registro.uDepartamento));
                        cContacto["onetoone_provinciaid"] = new EntityReference("onetoone_provincia", new Guid(Js_Registro.uProvincia));
                        cContacto["onetoone_distritoid"] = new EntityReference("onetoone_distrito", new Guid(Js_Registro.uDistrito));
                        cContacto["address1_name"] = Js_Registro.uDireccion;
                        gContacto = servicio.Create(cContacto);
                        
                        if (eCia == 1) {
                            Entity eItem1 = new Entity() { Id = gContacto, LogicalName = "contact" };
                            eItem1["parentcustomerid"] = new EntityReference("account", gEmpresa);
                            eItem1["fullname"] = Js_Registro.Nombres + " " + Js_Registro.ApellidoPaterno + " " + Js_Registro.ApellidoMaterno;
                            servicio.Update(eItem1);
                        }
                    }
                    else
                    {
                        gContacto = (Guid)ecContacto.Entities[0].Attributes["contactid"];
                    }
                }
                //Registro del Caso
                int Valor = 0;
                DateTime Fecha = DateTime.Now.AddHours(-5);
                var cCaso = new Entity("incident");
                string vTitulo = "";
                if (Js_Registro.TipoCaso == "1")
                    vTitulo = "Reclamo";
                if (Js_Registro.TipoCaso == "9")
                    vTitulo = "Quejas";

                cCaso["onetoone_fechaderecepcion"] = Fecha;
                cCaso["casetypecode"] = new OptionSetValue() {Value = Int32.Parse(Js_Registro.TipoCaso) };
                cCaso["onetoone_canal"] = new OptionSetValue() { Value = 2 }; //Vía Internet
                cCaso["caseorigincode"] = new OptionSetValue() { Value = 7 }; //Libro de reclamaciones virtuales
                cCaso["onetoone_asunto"] = new EntityReference("onetoone_asunto", new Guid(Js_Registro.Motivo));
                cCaso["onetoone_tema"] = new EntityReference("onetoone_tema", new Guid(Js_Registro.SubMotivo));
                cCaso["description"] = Js_Registro.DetalleCaso;
                cCaso["onetoone_lineadeatencion"] = new OptionSetValue() { Value = 3 }; //3ra línea
                cCaso["onetoone_arearesponsable"] = new OptionSetValue() { Value = 99 }; //Otros
                cCaso["onetoone_correoelectronico"] = Js_Registro.Email;
                cCaso["onetoone_celular"] = Js_Registro.Celular;                
                String gSbs = "";
                strFetch = @"<fetch>
                <entity name=""onetoone_tipificacion"">
                <filter>
                <condition attribute=""onetoone_asunto"" operator=""eq"" value=""{0}"" />
                <condition attribute=""onetoone_tema"" operator=""eq"" value=""{1}"" />
                <condition attribute=""onetoone_categoria"" operator=""eq"" value=""{2}"" />
                <condition attribute=""statuscode"" operator=""eq"" value=""1"" />
                </filter>
                <link-entity name=""onetoone_motivoreclamosbs"" from=""onetoone_motivoreclamosbsid"" to=""onetoone_motivosbs"" alias=""motivosbs"">
                <attribute name=""onetoone_motivoreclamosbsid"" alias=""sbs"" />
                </link-entity>
                </entity>
                </fetch>";
                strFetch = string.Format(strFetch, Js_Registro.Motivo, Js_Registro.SubMotivo, Js_Registro.TipoCaso);
                EntityCollection ecSBS = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecSBS.Entities.Count > 0)
                {
                    gSbs = ((Guid)ecSBS.Entities[0].GetAttributeValue<AliasedValue>("sbs").Value).ToString();
                    cCaso["onetoone_motivoreclamosbs"] = new EntityReference("onetoone_motivoreclamosbs", new Guid(gSbs));
                }
                //Finalidad
                strFetch = @"<fetch>
                <entity name=""onetoone_tipificacion"">
                <attribute name=""onetoone_finalidad"" alias=""finalidad"" />
                <filter>
                <condition attribute=""onetoone_asunto"" operator=""eq"" value=""{0}"" />
                <condition attribute=""onetoone_tema"" operator=""eq"" value=""{1}"" />
                <condition attribute=""onetoone_categoria"" operator=""eq"" value=""{2}"" />
                <condition attribute=""statuscode"" operator=""eq"" value=""1"" />
                </filter>                
                </entity>
                </fetch>";
                strFetch = string.Format(strFetch, Js_Registro.Motivo, Js_Registro.SubMotivo, "1"); // El ultimo parametro de este Format debe ser Js_Registro.TipoCaso pero por el momento estará en duro como "1"
                EntityCollection ecFinalidad = servicio.RetrieveMultiple(new FetchExpression(strFetch));
                if (ecFinalidad.Entities.Count > 0)
                {
                    if (ecFinalidad.Entities[0].Attributes.Contains("finalidad"))
                    {
                        Valor = ((OptionSetValue)ecFinalidad[0].GetAttributeValue<AliasedValue>("finalidad").Value).Value;
                        cCaso["onetoone_finalidad"] = new OptionSetValue() { Value = Valor };
                    }
                }
                cCaso["onetoone_casoregistrado"] = true;

                if (eCia == 1)
                {
                    cCaso["customerid"] = new EntityReference("account", gEmpresa);
                    cCaso["title"] = vTitulo + " " + Js_Registro.eRazonSocial + " " + Fecha.ToString("dd-MM-yyyy");
                    cCaso["primarycontactid"] = new EntityReference("contact", gContacto);
                }
                else
                {
                    cCaso["customerid"] = new EntityReference("contact", gContacto);
                    cCaso["title"] = vTitulo + " " + Js_Registro.Nombres + " " + Js_Registro.ApellidoPaterno+" "+ Js_Registro.ApellidoMaterno+" "+ Fecha.ToString("dd-MM-yyyy");
                }
                //Ultima actualizaciones
                if (Js_Registro.TipoProductoLib == "460710000")
                {
                    cCaso["onetoone_tipoproductolibrecvirtual"] = new OptionSetValue() { Value = 460710000 }; //Crédito vehicular
                }
                if (Js_Registro.TipoProductoLib == "460710001")
                {
                    cCaso["onetoone_tipoproductolibrecvirtual"] = new OptionSetValue() { Value = 460710001 }; //Leasing
                }
                if (Js_Registro.TipoProductoLib == "460710002")
                {
                    cCaso["onetoone_tipoproductolibrecvirtual"] = new OptionSetValue() { Value = 460710002 }; //Crédito de consumo
                }
                if (Js_Registro.TipoProductoLib == "460710003")
                {
                    cCaso["onetoone_tipoproductolibrecvirtual"] = new OptionSetValue() { Value = 460710003 }; //Otros
                }
                cCaso["onetoone_respuestaenlinea"] = true;
                if (Js_Registro.EnvioNotificacion == "C")
                {
                    cCaso["onetoone_viadireccion"] = true;
                    cCaso["onetoone_direccion"] = Js_Registro.uDireccion;
                }
                if (Js_Registro.EnvioNotificacion == "E")
                {
                    cCaso["onetoone_viarespuestacorreo"] = true;
                    cCaso["onetoone_correoelectronico"] = Js_Registro.Email;
                }
                //OVP 26-09-2022
                cCaso["onetoone_envioconstancia"] = true; //OVP 09-10-2022
                Guid gCasos = servicio.Create(cCaso);

                Resultado.indicadorExito = INDICADOR_EXITO;
                Resultado.descripcionError = gCasos.ToString();
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
