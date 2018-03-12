using Catel.Services;
using DLR.WPF.DlrServer;
using DLR.WPF.Views;

namespace DLR.WPF.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        private readonly IPleaseWaitService _pleaseWaitService;

        public MainWindowViewModel(IMessageService messageService, IPleaseWaitService pleaseWaitService)
        {
            _messageService = messageService;
            _pleaseWaitService = pleaseWaitService;
            AuthenticateCommand = new Command(Authenticate, () => true);
        }

        public override string Title { get { return "DLR.WPF"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }

        private void Authenticate()
        {
            Token token;
            var authWindow = new AuthWindow();
            //_pleaseWaitService.Show("Авторизация");
            var result = authWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var authServiceClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                if (authWindow.DataContext is AuthWindowViewModel authVm)
                {
                    token = authServiceClient.Authenticate(authVm.Login, authVm.Password);
                    _messageService.ShowAsync("Авторизован");
                }
                
            }
            _messageService.ShowAsync("Не авторизован");
            //_pleaseWaitService.Hide();
        }

        public Command AuthenticateCommand { get; set; }
    }
}
