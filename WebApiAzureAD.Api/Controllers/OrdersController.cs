using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Web.Http;
using System.IdentityModel.Tokens;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;

namespace WebApiAzureAD.Api.Controllers
{
    //Uncomment this if you want auth to be performed by Owin.WindowsAzureActiveDirectoryBearerAuthenticationExtensions
    //For this sample we are manually doing validation
    //[System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        static string audience = ConfigurationManager.AppSettings["ida:Audience"];
        static string metadataUri = ConfigurationManager.AppSettings["ida:FederationMetaDataUri"];


        /// <summary>
        /// In this Get method we are manually validating JWT token using Signing key from AD metadata 
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.Route("")]
        public IHttpActionResult Get()
        {   
            
            SecurityKey issuerSigningKey = null;

            // You can utilize MetadataSerializer class to read signing key from AD federation metadata
            MetadataSerializer serializer = new MetadataSerializer();
            serializer.CertificateValidationMode = X509CertificateValidationMode.None;
            var metaData = serializer.ReadMetadata(new XmlTextReader(metadataUri));
            issuerSigningKey = metaData.SigningCredentials.SigningKey;
            
            
           
            ////Or you can mannually parse metadata if you want to persist signing key in your service store and use it later
            XPathDocument xmlReader = new XPathDocument(metadataUri);
            XPathNavigator navigator = xmlReader.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("", "urn:oasis:names:tc:SAML:2.0:metadata");
            manager.AddNamespace("ns1", "urn:oasis:names:tc:SAML:2.0:metadata");
            manager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            manager.PushScope();
            XPathNodeIterator nodes = navigator.Select("//ns1:EntityDescriptor/ns1:RoleDescriptor/ns1:KeyDescriptor[@use='signing']/ds:KeyInfo/ds:X509Data/ds:X509Certificate", manager);
            nodes.MoveNext();
            XPathNavigator nodesNavigator = nodes.Current;

            //Cert body is base64 encoded in metadata doc
            X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(nodesNavigator.InnerXml));
            issuerSigningKey = new X509AsymmetricSecurityKey(cert);

           

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = issuerSigningKey,
                ValidAudience = audience,
                ValidateIssuer = false,
                ValidateLifetime = true,
            };
            SecurityToken validated;
            handler.ValidateToken(this.Request.Headers.Authorization.Parameter, validationParameters, out validated);
            return Ok(Order.CreateOrders());

        }
    }

    #region Helpers

    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string ShipperCity { get; set; }
        public Boolean IsShipped { get; set; }

        public static List<Order> CreateOrders()
        {
            List<Order> OrderList = new List<Order> 
            {
                
            };

            return OrderList;
        }
    }

    #endregion

}
