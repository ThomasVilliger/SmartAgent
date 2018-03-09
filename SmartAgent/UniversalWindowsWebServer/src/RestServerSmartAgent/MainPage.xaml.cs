using Restup.SmartAgent;
using Restup.Webserver.File;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SmartAgent;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Restup.SmartAgentWebServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SmartAgentMain piFaceMain { get; set; } = new SmartAgentMain();
        private HttpServer _httpServer;

        public MainPage()
        {
            this.InitializeComponent();
          
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebServer();
        }

        private async Task InitializeWebServer()
        {
            var restRouteHandler = new RestRouteHandler();

            restRouteHandler.RegisterController<SmartAgentRestController>(piFaceMain);

            var configuration = new HttpServerConfiguration()
                .ListenOnPort(8800)
                .RegisterRoute("api", restRouteHandler)
                .RegisterRoute(new StaticFileRouteHandler(@"Restup.DemoStaticFiles\Web"))
                .EnableCors(); // allow cors requests on all origins
                               //.EnableCors(x => x.AddAllowedOrigin("http://specificserver:<listen-port>"));

            var httpServer = new HttpServer(configuration);
            _httpServer = httpServer;
            await httpServer.StartServerAsync();

        }
    }
}
