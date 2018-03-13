using Catel.Windows;
using DLR.WPF.ViewModels;

namespace DLR.WPF.Views
{
    public partial class AuthWindow
    {
        public AuthWindow()
        {
            //AddCustomButton(new DataWindowButton("Войти", "AuthenticateCommand"));
            InitializeComponent();
            Loaded += AuthWindow_Loaded            ;
            
        }

        private void AuthWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is AuthWindowViewModel vm)
            {
                vm.Window = this;
            }
        }
    }
}
