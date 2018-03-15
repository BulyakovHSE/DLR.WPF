using System.Collections.Generic;
using System.Windows;
using Catel.Data;
using Catel.Services;
using Catel.Windows;
using DLR.WPF.DlrServer;

namespace DLR.WPF.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class AuthWindowViewModel : ViewModelBase
    {
        private readonly IMessageService _messageService;

        public AuthWindowViewModel(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        public Token Token { get; set; }
        
        public string Login
        {
            get { return GetValue<string>(LoginProperty); }
            set { SetValue(LoginProperty, value); }
        }

        public static readonly PropertyData LoginProperty = RegisterProperty(nameof(Login), typeof(string), string.Empty);


        public string Password
        {
            get { return GetValue<string>(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly PropertyData PasswordProperty = RegisterProperty(nameof(Password), typeof(string), string.Empty);

        public Command AuthenticateCommand => new Command(async () =>
        {
            var authClient = new AuthServiceClient("BasicHttpBinding_IAuthService");
            try
            {
                var token = await authClient.AuthenticateAsync(Login, Password);
                if (token != null)
                {
                    Token = token;
                    var window = Application.Current.GetActiveWindow();
                    window.Close();
                }
                else
                {
                    await _messageService.ShowWarningAsync("Неудачная попытка авторизации");
                }
            }
            catch
            {
                await _messageService.ShowErrorAsync("Не удалось установить соединение с сервером.");
            }

            
        });

        public bool CanAuthenicate()
        {
            return string.IsNullOrEmpty(Login) && string.IsNullOrEmpty(Password);
        }

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

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if(string.IsNullOrEmpty(Login))
                validationResults.Add(FieldValidationResult.CreateError(LoginProperty, "Логин не можеть быть пустым!"));

            if(string.IsNullOrEmpty(Password))
                validationResults.Add(FieldValidationResult.CreateError(PasswordProperty, "Пароль не можеть быть пустым!"));
        }
    }
}
