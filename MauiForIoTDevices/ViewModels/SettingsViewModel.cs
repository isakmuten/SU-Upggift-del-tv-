using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MauiForIoTDevices.ViewModels
{


	public class SettingsViewModel : INotifyPropertyChanged
	{


		private string _emailServiceConnectionString;
		private string _emailAddress;

		public static string SavedEmailServiceConnectionString { get; private set; }
		public static string SavedEmailAddress { get; private set; }

		public string EmailServiceConnectionString
		{
			get => _emailServiceConnectionString;
			set
			{
				if (_emailServiceConnectionString != value)
				{
					_emailServiceConnectionString = value;
					OnPropertyChanged();
				}
			}
		}

		public string EmailAddress
		{
			get => _emailAddress;
			set
			{
				if (_emailAddress != value)
				{
					_emailAddress = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand SaveSettingsCommand { get; }

		public SettingsViewModel()
		{
			SaveSettingsCommand = new Command(SaveSettings);
		}

		private void SaveSettings()
		{
			if (string.IsNullOrWhiteSpace(EmailServiceConnectionString) || string.IsNullOrWhiteSpace(EmailAddress))
			{
				Application.Current.MainPage.DisplayAlert("Error", "Both Email Service Connection String and Email Address are required.", "OK");
				return;
			}

			if (!EmailServiceConnectionString.Contains("endpoint=") || !EmailServiceConnectionString.Contains("accesskey="))
			{
				Application.Current.MainPage.DisplayAlert("Error", "Invalid Email Service Connection String format. It must contain 'endpoint' and 'accesskey'.", "OK");
				return;
			}

			SavedEmailServiceConnectionString = EmailServiceConnectionString;
			SavedEmailAddress = EmailAddress;

			Application.Current.MainPage.DisplayAlert("Settings", "Settings have been saved.", "OK");
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
