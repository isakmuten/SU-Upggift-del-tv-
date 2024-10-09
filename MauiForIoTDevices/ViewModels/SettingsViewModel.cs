using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MauiForIoTDevices.ViewModels
{


	public class SettingsViewModel : INotifyPropertyChanged
	{
		private string _connectionString;
		private string _emailAddress;
		
		public static string SavedEmailAddress { get; private set; }
		public static string SavedConnectionString { get; private set; }


		public string ConnectionString
		{
			get => _connectionString;
			set
			{
				_connectionString = value;
				OnPropertyChanged();
			}
		}

		public string EmailAddress
		{
			get => _emailAddress;
			set
			{
				_emailAddress = value;
				OnPropertyChanged();
			}
		}

		public ICommand SaveSettingsCommand { get; }

		public SettingsViewModel()
		{
			SaveSettingsCommand = new Command(SaveSettings);
		}

		private void SaveSettings()
		{
			if (string.IsNullOrWhiteSpace(ConnectionString) || string.IsNullOrWhiteSpace(EmailAddress))
			{
				Application.Current.MainPage.DisplayAlert("Error", "Both Connection String and Email Address are required.", "OK");
				return;
			}
			if (!IsValidEmail(EmailAddress))
			{
				Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid email address.", "OK");
				return;
			}
			if (!ConnectionString.Contains("HostName=") || !ConnectionString.Contains("DeviceId=") || !ConnectionString.Contains("SharedAccessKey="))
			{
				Application.Current.MainPage.DisplayAlert("Error", "Invalid Connection String format. It must contain 'HostName', 'DeviceId', and 'SharedAccessKey'.", "OK");
				return;
			}

			SavedEmailAddress = EmailAddress;
			SavedConnectionString = ConnectionString;

			Application.Current.MainPage.DisplayAlert("Settings", "Settings have been saved", "OK");
		}
		private bool IsValidEmail(string email)
		{
			return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
