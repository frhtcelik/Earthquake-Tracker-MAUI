using DepremTakipAPP.ViewModels;

namespace DepremTakipAPP
{
    public partial class NotificationSettingsPage : ContentPage
    {
        private readonly NotificationSettingsViewModel _viewModel;

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Helpers.EarthquakeNotificationHelper.CheckAndNotifyFromApiAsync();
        }
        public NotificationSettingsPage()
        {
            InitializeComponent();
            _viewModel = new NotificationSettingsViewModel();
            BindingContext = _viewModel;
        }

        private void TestNotificationButton_Clicked(object sender, EventArgs e)
        {
            if (_viewModel.SendTestNotificationCommand.CanExecute(null))
                _viewModel.SendTestNotificationCommand.Execute(null);
        }
    }
}
