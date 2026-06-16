using DepremTakipAPP.ViewModels;

namespace DepremTakipAPP
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            // XAML tarafında BindingContext zaten verilmiş olsa da,
            // runtime güvenliği için ViewModel'i burada da set edelim.
            _viewModel = BindingContext as MainPageViewModel ?? new MainPageViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel.Earthquakes.Count == 0)
            {
                await _viewModel.LoadEarthquakesAsync();
            }
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            await _viewModel.LoadEarthquakesAsync();
        }
    }
}
