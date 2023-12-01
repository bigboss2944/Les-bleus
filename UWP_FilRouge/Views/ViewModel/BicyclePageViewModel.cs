using Entities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Database;
using UWP_FilRouge.Entities;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;

namespace UWP_FilRouge.Views.ViewModel
{
    public class BicyclePageViewModel : ViewModelBase
    {
        
        private readonly INavigationService navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToAboutPage { get; private set; }
        public RelayCommand MoveToContactPage { get; private set; }
        private bool _isLoading = false;
        public BicyclePageAccessor DataBicycle { get; set; }
        private DatabaseService databaseService;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");

            }
        }
        private string _title;
        public string Title
        {

            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public BicyclePageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            SetupBicycleDatas();
            Title = "Contact Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
            MoveToSellerPage = new RelayCommand(ToSellerPage);
            //MoveToCustomerPage = new RelayCommand(ToCustomerPage);
        }

        private async Task<Bicycle> HttpClientCaller<TItem>(String url, Bicycle item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("UrlDeMonSite");
                client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("http://localhost:44343/Bicycles/Index");
                item = await HandleResponse(item,response);
            }
            return item;
        }

        private async void SetupBicycleDatas()
        {
            DataBicycle = new BicyclePageAccessor();
            //SetupBicycleEdit();
            await SetupBicycleList();
            //SetupBicycleUpdate();
        }

        private async Task SetupBicycleList()
        {
            //StorageFile certificateFile = await Package.Current.InstalledLocation.GetFileAsync(@"client.p12");
            X509Certificate2 cer = new X509Certificate2(File.ReadAllBytes("client.pfx"), "000000");

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cer);

            //Create an HTTP client object
            HttpClient client = new HttpClient(handler);

            //Add a user-agent header to the GET request. 
            var headers = client.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }
            //Uri requestUri = new Uri("https://www.google.fr/");
            Uri requestUri = new Uri("https://localhost:44343/Bicycles/");

            //Send the GET request asynchronously and retrieve the response as a string.
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await client.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("{0}", httpResponseBody);
                DataBicycle.bicycleList.bicycles.Add(JsonConvert.DeserializeObject<Bicycle>(httpResponseBody));
            }
            catch (HttpRequestException ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                navigationService.NavigateTo("Home Page");
            }
        }

        private Byte[] StringToByteArray(string thumbprint)
        {
            //reading all characters as byte and storing them to byte[]
            byte[] barr = Encoding.ASCII.GetBytes(thumbprint);

            //printing characters with byte values
            for (int loop = 0; loop < barr.Length - 1; loop++)
            {
                Console.WriteLine("Byte of char \'" + thumbprint[loop] + "\' : " + barr[loop]);
            }
            return barr;
        }

        private async Task<TItem> HandleResponse<TItem>(TItem item, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                String result = await response.Content.ReadAsStringAsync();
                item = JsonConvert.DeserializeObject<TItem>(result);
            }
            return item;
        }

        private void ToHomePage()
        {
            // Do Something
            navigationService.NavigateTo("Home Page");
        }

        private void ToAboutPage()
        {
            // Do Something
            navigationService.NavigateTo("About Page");
        }

        private void ToRegisterPage()
        {
            // Do Something
            navigationService.NavigateTo("Register Page");
        }

        private void ToLoginPage()
        {
            // Do Something
            navigationService.NavigateTo("Login Page");
        }

        private void ToSellerPage()
        {
            // Do Something
            navigationService.NavigateTo("Seller Main Page");
        }

        private void ToOrderPage()
        {
            // Do Something
            navigationService.NavigateTo("Order Main Page");
        }
    }
}
