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
        private readonly IUIVisualizerService _uiVisualizerService;
        private Token _token;

        public MainWindowViewModel(IMessageService messageService, IPleaseWaitService pleaseWaitService, IUIVisualizerService uiVisualizerService)
        {
            _messageService = messageService;
            _pleaseWaitService = pleaseWaitService;
            _uiVisualizerService = uiVisualizerService;
            AuthenticateCommand = new Command(Authenticate, () => true);
            UserRegion = "Не авторизовано";
            EditingEnabled = true;
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

        private async void Authenticate()
        {
            var authVm = new AuthWindowViewModel(_messageService);
            await _uiVisualizerService.ShowDialogAsync(authVm, (o, e) =>
            {
                if (e.Result.HasValue && authVm.Token!=null)
                {
                    _token = authVm.Token;
                    UserRegion = GetRegionName(_token.UserRegion);
                    Authenticated = true;
                }
            });
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
