using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MauiForIoTDevices.ViewModels
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		private string _connectionString;
		private string _emailAddress;

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
			Application.Current.MainPage.DisplayAlert("Settings", "Settings have been saved", "OK");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
