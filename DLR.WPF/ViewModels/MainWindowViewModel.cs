using System;
using System.Collections.ObjectModel;
using System.Linq;
using Catel.Collections;
using Catel.Data;
using Catel.Services;
using Catel.Windows.Interactivity;
using DLR.WPF.DlrServer;
using DLR.WPF.Models;
using DLR.WPF.Services;
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
        private ActBase _journalAct;

        public MainWindowViewModel(IMessageService messageService, IPleaseWaitService pleaseWaitService, IUIVisualizerService uiVisualizerService)
        {
            _messageService = messageService;
            _pleaseWaitService = pleaseWaitService;
            _uiVisualizerService = uiVisualizerService;
            InitialiseProperties();
        }

        public bool Authenticated { get; set; }

        public bool EditingEnabled { get; set; }

        public string UserRegion { get; set; }

        public ObservableCollection<ActBase> SelectedAct { get; set; }

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
                if (e.Result.HasValue && authVm.Token != null)
                {
                    _token = authVm.Token;
                    UserRegion = DlrStaticMethods.GetRegionName(_token.UserRegion);
                    Authenticated = true;
                }
            });
        }

        public Command AuthenticateCommand { get; set; }

        public Command CreateNewCommand => new Command(() =>
        {
            if (!Enum.TryParse((SelectedActIndex + 1).ToString(), out ActType actType)) return;
            ActBase act = null;
            switch (actType)
            {
                case ActType.АктОбследования:
                    {
                        act = new ActInspection();
                        break;
                    }
                case ActType.АктПроверкиФизЛица:
                    {
                        act = new ActInpectationFl();
                        break;
                    }
                case ActType.АктПроверкиЮл:
                    {
                        act = new ActInspectationUlIp();
                        break;
                    }
                case ActType.ЖурналУчетаПроверокЮл:
                    {
                        act = new CheckingJournal();
                        break;
                    }
                case ActType.ЗаявлениеСоглВнеплВыездПроверки:
                    {
                        act = new AgreementStatement();
                        break;
                    }
                case ActType.ОбмерПлощадиЗу:
                    {
                        act = new AreaMeasurement();
                        break;
                    }
                case ActType.ПланПроверокГраждан:
                    {
                        act = new CitizensCheckPlan();
                        break;
                    }
                case ActType.ПредписаниеУтсрНарушЗемЗакона:
                    {
                        act = new Regulation();
                        break;
                    }
                case ActType.ПротоколАдмПравонарушения:
                    {
                        act = new Protocol();
                        break;
                    }
                case ActType.РаспоряжениеПроверкиЮл:
                    {
                        act = new OrderInspectionUlIp();
                        break;
                    }
                case ActType.ФотоТаблица:
                    {
                        act = new PhotoTable();
                        break;
                    }
            }
            ShowAct(act);
            _journalAct = null;
        });

        public Command SaveChangesCommand => new Command(() =>
        {
            if (!Authenticated)
            {
                _messageService.ShowWarningAsync("Вы не авторизованы в системе!");
                return;
            }
            try
            {
                var AuthClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                if (_journalAct == null)
                {
                    _messageService.ShowWarningAsync(
                        "Акт еще не создан в базе данных.\nДля его создания нажмите на кнопку \"Создать акт\"");
                    return;
                }
                if (SelectedAct.Count == 0)
                {
                    _messageService.ShowWarningAsync("Акт не выбран!\nВыберите акт для редактирования из журнала");
                    return;
                }

                if (AuthClient.UpdateAct(SelectedAct.First(), _token))
                    _messageService.ShowInformationAsync("Сохранено");
                else
                    _messageService.ShowWarningAsync("Не удалось сохранить изменения на сервере");
            }
            catch (Exception e)
            {
                _messageService.ShowErrorAsync("Произошла непредвиденная ошибка.");
            }
        });

        public Command OpenJournalCommand => new Command(() =>
        {
            if (!Authenticated)
            {
                _messageService.ShowWarningAsync("Вы не авторизованы в системе!");
                return;
            }
            var vm = new JournalWindowViewModel(_token, _messageService, _pleaseWaitService);
            _uiVisualizerService.ShowDialogAsync(vm);
        });

        public Command CreateAct => new Command(() =>
        {
            if (!Authenticated)
            {
                _messageService.ShowWarningAsync("Вы не авторизованы в системе!");
                return;
            }
            try
            {
                var AuthClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                var act = SelectedAct.First();
                if (act == null) return;
                try
                {
                    WordTemplateFillingService.FillWordBookmarks(ref act);
                }
                catch (Exception e)
                {
                    _messageService.ShowWarningAsync("Не удалось создать файл.\n" + e.Message + "\nРекомендуется закрыть все процессы Word в диспетчере задач Windows");
                }

                if (AuthClient.AddAct(act, _token))
                    _messageService.ShowInformationAsync("Акт успешно создан!");
                else
                    _messageService.ShowWarningAsync("Акт не был создан изза ошибки на сервере!");
            }
            catch (Exception e)
            {
                _messageService.ShowErrorAsync("Произошла непредвиденная ошибка.\n" + e.Message);
            }
        }, () => true);

        public void ShowAct(ActBase act)
        {
            if (act == null) return;
            _journalAct = act;
            SelectedAct.Clear();
            SelectedAct.Add(act);
            SelectedActIndex = (int)DlrStaticMethods.GetActType(act);
        }

        private void InitialiseProperties()
        {
            AuthenticateCommand = new Command(Authenticate, () => true);
            UserRegion = "Не авторизовано";
            EditingEnabled = true;
            ActTypes = new ObservableCollection<string>();
            for (int i = 0; i < Enum.GetNames(typeof(ActType)).Length; i++)
            {
                if(i == 0) continue;
                ActTypes.Add(DlrStaticMethods.GetFullActName((ActType)i));
            }
            ActTypes.Add("Не выбрано");
            SelectedActIndex = ActTypes.IndexOf("Не выбрано");
            SelectedAct = new ObservableCollection<ActBase>();
        }
    }
}
