using Restup.DemoControllers;
using Restup.Webserver.File;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using PIFace_II;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Restup.HeadedDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PiFaceMain piFaceMain { get; set; } = new PiFaceMain();
        private HttpServer _httpServer;

        public MainPage()
        {
            this.InitializeComponent();
            Switch0.AddHandler(PointerPressedEvent, new PointerEventHandler(Switch0_PointerPressed), true);
            Switch0.AddHandler(PointerReleasedEvent, new PointerEventHandler(Switch0_PointerReleased), true);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebServer();
        }

        private async Task InitializeWebServer()
        {
            var restRouteHandler = new RestRouteHandler();

            restRouteHandler.RegisterController<AsyncControllerSample>();
            restRouteHandler.RegisterController<FromContentControllerSample>();
            restRouteHandler.RegisterController<PerCallControllerSample>();
            restRouteHandler.RegisterController<SimpleParameterControllerSample>(piFaceMain);
            restRouteHandler.RegisterController<SingletonControllerSample>();
            restRouteHandler.RegisterController<ThrowExceptionControllerSample>();
            restRouteHandler.RegisterController<WithResponseContentControllerSample>();

            var configuration = new HttpServerConfiguration()
                .ListenOnPort(8800)
                .RegisterRoute("api", restRouteHandler)
                .RegisterRoute(new StaticFileRouteHandler(@"Restup.DemoStaticFiles\Web"))
                .EnableCors(); // allow cors requests on all origins
                               //.EnableCors(x => x.AddAllowedOrigin("http://specificserver:<listen-port>"));

            var httpServer = new HttpServer(configuration);
            _httpServer = httpServer;
            await httpServer.StartServerAsync();


            // Don't release deferral, otherwise app will stop
        }




        private void Switch0_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PiFaceMain.WritePiFaceInput(0, true);
            Switch0.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        }


        private void Switch0_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PiFaceMain.WritePiFaceInput(0, false);
            Switch0.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        }

    }
}
