using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catel.Collections;
using Catel.MVVM;
using Catel.Windows;
using DLR.WPF.DlrServer;
using DLR.WPF.Models;

namespace DLR.WPF.ViewModels
{
    public class JournalWindowViewModel : ViewModelBase
    {
        private readonly Token _token;
        
        public JournalWindowViewModel(Token token, ActType? filter = null)
        {
            _token = token;
            DisplayingActs = new ObservableCollection<DisplayingJournalAct>();
        }

        public ObservableCollection<DisplayingJournalAct> DisplayingActs { get; set; }

        public Command LoadAllActs => new Command(() =>
        {
            int num = 10, offset = 0;

            var AuthClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
            DisplayingActs.Clear();
            ActBase[] acts;

            do
            {
                acts = AuthClient.GetActs(_token, num, offset);
                offset += num;
                if (acts != null && acts.Length > 0) DisplayingActs.AddRange(acts.Select(x=>new DisplayingJournalAct(x)));
            } while (acts != null && acts.Length > 0);

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
    }
}