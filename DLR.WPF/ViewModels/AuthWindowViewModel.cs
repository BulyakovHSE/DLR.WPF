using System.Collections.Generic;
using Catel.Data;

namespace DLR.WPF.ViewModels
{
    using Catel.MVVM;
    using System.Threading.Tasks;

    public class AuthWindowViewModel : ViewModelBase
    {
        public AuthWindowViewModel()
        {
        }

        public override string Title { get { return "View model title"; } }


        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        
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
