using MauiForIoTDevices.Views;


namespace MauiForIoTDevices
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private async void OnSettingsButtonClicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new SettingsPage());
		}
	}
}
