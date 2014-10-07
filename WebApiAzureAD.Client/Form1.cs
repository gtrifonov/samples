using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace WebApiAzureAD.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private static readonly string AadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static readonly string Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        readonly Uri _redirectUri = new Uri(ConfigurationManager.AppSettings["ida:RedirectUri"]);

        private static readonly string Authority = String.Format(AadInstance, Tenant);

        private static readonly string ApiResourceId = ConfigurationManager.AppSettings["ApiResourceId"];
        private static readonly string ApiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];

        private async void button1_Click(object sender, EventArgs e)
        {

            _authContext = new AuthenticationContext(Authority);
            
            AuthenticationResult authResult = _authContext.AcquireToken(ApiResourceId, ClientId, _redirectUri);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

            HttpResponseMessage response = await client.GetAsync(ApiBaseAddress + "api/orders");
          
            string responseString = await response.Content.ReadAsStringAsync();
           
            MessageBox.Show(responseString);

        }
    }
}
