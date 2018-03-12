using System;
using System.Collections.ObjectModel;
using System.Linq;
using Catel.Collections;
using Catel.Data;
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
        private Token _token;

        public MainWindowViewModel(IMessageService messageService, IPleaseWaitService pleaseWaitService)
        {
            _messageService = messageService;
            _pleaseWaitService = pleaseWaitService;
            AuthenticateCommand = new Command(Authenticate, () => true);
            UserRegion = "Не авторизовано";
            EditingEnabled = false;
            ActTypes = new ObservableCollection<string>();
            ActTypes.AddRange(Enum.GetNames(typeof(ActType)));
            ActTypes.Add("Не выбрано");
            SelectedActIndex = ActTypes.IndexOf("Не выбрано");
        }

        public override string Title { get { return "DLR.WPF"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets


        public bool Authenticated { get; set; }

        public bool EditingEnabled { get; set; }

        public string UserRegion { get; set; }

        public ObservableCollection<string> ActTypes { get; set; }

        public int SelectedActIndex { get; set; }

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
            
            var authWindow = new AuthWindow();
            //_pleaseWaitService.Show("Авторизация");
            var result = authWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var authServiceClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                if (authWindow.DataContext is AuthWindowViewModel authVm)
                {
                    _token = authServiceClient.Authenticate(authVm.Login, authVm.Password);
                    if (_token != null)
                    {
                        _messageService.ShowAsync("Авторизован");
                        UserRegion = GetRegionName(_token.UserRegion);
                        Authenticated = true;
                    }
                }
                
            }
            else
            _messageService.ShowAsync("Не авторизован");
            //_pleaseWaitService.Hide();
        }

        private string GetRegionName(Region region)
        {
            switch (region)
            {
                    case Region.All: return "Центральный";
                case Region.Dzerzhinsky: return "Дзержинский";
                case Region.Industrial: return "Индустриальный";
                case Region.Kirov: return "Кировский";
                case Region.Leninsky: return "Ленинский";
                case Region.Motovilikhinsky: return "Мотовилихинский";
                case Region.NewLyads: return "Новые ляды";
                case Region.Ordzhonikidzevsky: return "Орджоникидзевский";
                case Region.Sverdlovsky: return "Свердловский";
                    default: return "";
            }
        }

        public Command AuthenticateCommand { get; set; }
    }
}
