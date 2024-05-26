using System.Web.Http;
using System.Web.Http.Cors;  // Asegúrate de incluir esta directiva usando para CORS
using Api_Crm_MAF.Security;

namespace Api_Crm_MAF
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configura CORS para permitir solicitudes desde cualquier origen (para desarrollo)
            // Especifica dominios, métodos y cabeceras específicos para producción
            // Habilita CORS solamente para  dominio de GitHub Pages TEST
            //var cors = new EnableCorsAttribute("https://sroldang.github.io", "*", "*");
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Configuración y servicios de API web

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
