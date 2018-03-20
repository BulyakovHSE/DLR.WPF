using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catel.Collections;
using Catel.MVVM;
using Catel.Services;
using Catel.Windows;
using DLR.WPF.DlrServer;
using DLR.WPF.Models;
using DLR.WPF.Services;

namespace DLR.WPF.ViewModels
{
    public class JournalWindowViewModel : ViewModelBase
    {
        private readonly Token _token;
        private readonly IMessageService _messageService;
        private readonly IPleaseWaitService _pleaseWaitService;

        public JournalWindowViewModel(Token token, IMessageService messageService, IPleaseWaitService pleaseWaitService, ActType? filter = null)
        {
            _token = token;
            _messageService = messageService;
            _pleaseWaitService = pleaseWaitService;
            DisplayingActs = new ObservableCollection<DisplayingJournalAct>();
            IsCentral = token.UserRegion == Region.All;
            ActTypes = new ObservableCollection<string>();
            Regions = new ObservableCollection<string>();
            for (int i = 0; i < Enum.GetNames(typeof(ActType)).Length; i++)
            {
                if (i == 0) continue;
                ActTypes.Add(DlrStaticMethods.GetFullActName((ActType)i));
            }
            for (int i = 0; i < Enum.GetNames(typeof(Region)).Length; i++)
            {
                if (i == 0) continue;
                Regions.Add(DlrStaticMethods.GetRegionName((Region)i));
            }
            Regions.Add("Все");
            SelectedRegionIndex = Regions.IndexOf("Все");
            ActTypes.Add("Все");
            SelectedActTypeIndex = ActTypes.IndexOf("Все");
        }

        public ObservableCollection<DisplayingJournalAct> DisplayingActs { get; set; }

        public bool IsCentral { get; set; }

        public ObservableCollection<string> ActTypes { get; set; }

        public int SelectedRegionIndex { get; set; }

        public int SelectedActTypeIndex { get; set; }

        public ObservableCollection<string> Regions { get; set; }

        public Command LoadAllActs => new Command(() =>
        {
            _pleaseWaitService.Show("Загрузка");
            int num = 1, offset = 0;
            try
            {
                var AuthClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                bool loadAllRegions = Enum.GetNames(typeof(Region)).Length - 1 == SelectedRegionIndex;
                Region region = (Region) SelectedRegionIndex + 1;
                DisplayingActs.Clear();
                ActBase[] acts;
                do
                {
                    _pleaseWaitService.Pop();
                    acts = AuthClient.GetActs(_token, num, offset);

                    offset += num;
                    if (acts != null && acts.Length > 0)
                    {
                        if (IsCentral && loadAllRegions)
                            DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                        else if(IsCentral)
                            DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                        else
                            DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                    }

                } while (acts != null && acts.Length > 0);
            }
            catch (Exception e)
            {
                _messageService.ShowWarningAsync("Не удалось загрузить акты");
            }
            _pleaseWaitService.Hide();
        });

        public Command<DataGrid> MouseDoubleClickCommand => new Command<DataGrid>((d) =>
        {
            if (d.SelectedItem is DisplayingJournalAct a)
            {
                var currentWindow = Application.Current.GetActiveWindow();
                var w = Application.Current.Windows;
                foreach (var window1 in w)
                {
                    if (window1 is Catel.Windows.Window window)
                    {
                        if (window.DataContext is MainWindowViewModel vm)
                        {
                            vm.ShowAct((ActBase)a);
                        }
                    }
                }
                currentWindow.Close();
            }
        });

        public Command<DataGrid> DeleteAct => new Command<DataGrid>(d =>
        {
            if (d.SelectedItem is DisplayingJournalAct a)
            {
                try
                {
                    var AuthClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                    if (AuthClient.DeleteActById(a.Id, _token))
                    {
                        _messageService.ShowInformationAsync("Акт успешно удален!", "Успешно");
                        DisplayingActs.Remove(a);
                    }
                    else
                    {
                        _messageService.ShowWarningAsync("Не удалось удалить акт из-за ошибки на сервере!",
                            "Не удалось удалить акт");
                    }
                }
                catch (Exception e)
                {
                    _messageService.ShowErrorAsync("Не удалось установить соединение с сервером",
                        "Не удалось удалить акт");
                }
            }
        });

        public Command<DataGrid> OpenActFile => new Command<DataGrid>(d =>
        {
            if (d.SelectedItem is DisplayingJournalAct a)
            {
                var act = (ActBase)a;
                try
                {
                    if (act.DocumentBytes.Length > 0)
                    {

                    }
                }
                catch (Exception e)
                {

                }
            }
        });

        public Command ComboBoxSelectionChanged => new Command(() =>
        {
            int num = 1, offset = 0;
            ActType actType = (ActType)SelectedActTypeIndex + 1;
            Region region = (Region) SelectedRegionIndex + 1;
            bool loadAllRegions = Enum.GetNames(typeof(Region)).Length - 1 == SelectedRegionIndex;
            try
            {
                var authClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
                ActBase[] acts;
                DisplayingActs.Clear();
                switch (actType)
                {
                    case ActType.АктОбследования:
                        {
                            do
                            {
                                acts = authClient.GetActInspection(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.АктПроверкиФизЛица:
                        {
                            do
                            {
                                acts = authClient.GetActsInpectationFl(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.АктПроверкиЮл:
                        {
                            do
                            {
                                acts = authClient.GetActsInspectationUlIp(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ЖурналУчетаПроверокЮл:
                        {
                            do
                            {
                                acts = authClient.GetCheckingJournals(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ЗаявлениеСоглВнеплВыездПроверки:
                        {
                            do
                            {
                                acts = authClient.GetAgreementStatements(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ОбмерПлощадиЗу:
                        {
                            do
                            {
                                acts = authClient.GetAreaMeasurements(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ПланПроверокГраждан:
                        {
                            do
                            {
                                acts = authClient.GetCitizensCheckPlans(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ПредписаниеУтсрНарушЗемЗакона:
                        {
                            do
                            {
                                acts = authClient.GetRegulations(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ПротоколАдмПравонарушения:
                        {
                            do
                            {
                                acts = authClient.GetProtocols(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.РаспоряжениеПроверкиЮл:
                        {
                            do
                            {
                                acts = authClient.GetOrdersInspectionUlIp(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    case ActType.ФотоТаблица:
                        {
                            do
                            {
                                acts = authClient.GetPhotoTables(_token, num, offset);

                                offset += num;
                                if (acts != null && acts.Length > 0)
                                {
                                    if (IsCentral && loadAllRegions)
                                        DisplayingActs.AddRange(acts.Select(x => new DisplayingJournalAct(x)));
                                    else if(IsCentral)
                                        DisplayingActs.AddRange(acts.Where(x=>x.Region == region).Select(x => new DisplayingJournalAct(x)));
                                    else
                                        DisplayingActs.AddRange(acts.Where(x => x.Region == _token.UserRegion).Select(x => new DisplayingJournalAct(x)));
                                }

                            } while (acts != null && acts.Length > 0);
                        }
                        break;
                    default: LoadAllActs.Execute(); break;
                }
            }
            catch (Exception e)
            {
                _messageService.ShowWarningAsync("Не удалось загрузить акты");
            }
        });
    }
}