using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiForIoTDevices.Models;
using Microsoft.Maui.Controls;
using Azure.Communication.Email;
using System.Threading.Tasks;
using Azure;


namespace MauiForIoTDevices.ViewModels
{

	public class MainPageViewModel : INotifyPropertyChanged
	{
		string emailAddress = SettingsViewModel.SavedEmailAddress;

		public ObservableCollection<IoTDevice> IoTDevices { get; set; }

		private string _connectionString;
		public string ConnectionString
		{
			get => _connectionString;
			set
			{
				_connectionString = value;
				OnPropertyChanged();
			}
		}

		public ICommand RemoveDeviceCommand { get; }

		public MainPageViewModel()
		{
			IoTDevices = new ObservableCollection<IoTDevice>
			{
				new IoTDevice { DeviceId = "1", DeviceName = "Fan 1", Status = "Online" },
				new IoTDevice { DeviceId = "2", DeviceName = "Fan 2", Status = "Offline" }
			};

			RemoveDeviceCommand = new Command<IoTDevice>(RemoveDevice);
		}

		private async void RemoveDevice(IoTDevice device)
		{
			bool emailSent = await SendEmailNotificationAsync(device.DeviceName);

			if (emailSent)
			{
				IoTDevices.Remove(device);
			}
		}

		private async Task<bool> SendEmailNotificationAsync(string deviceName)
		{
			if (string.IsNullOrWhiteSpace(SettingsViewModel.SavedEmailAddress))
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Email address not set. Please configure it in the settings.", "OK");
				return false;
			}

			// Kontrollera om connection string är angiven och korrekt
			if (string.IsNullOrWhiteSpace(SettingsViewModel.SavedConnectionString) ||
				!SettingsViewModel.SavedConnectionString.Contains("endpoint=") ||
				!SettingsViewModel.SavedConnectionString.Contains("accesskey="))
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Invalid Connection String. Please configure it in the settings.", "OK");
				return false;
			}

			var connectionString = SettingsViewModel.SavedConnectionString;
			EmailClient emailClient = new EmailClient(connectionString);

			string emailAddress = SettingsViewModel.SavedEmailAddress;

			var recipients = new EmailRecipients(new List<EmailAddress>
			{
				new EmailAddress(emailAddress)
			});

			var emailMessage = new Azure.Communication.Email.EmailMessage(
				senderAddress: "isak.muten.kyh@gmail.com",
				content: new EmailContent("IoT Device Removed Notification")
				{
					PlainText = $"The device {deviceName} has been removed from your IoT list."
				},
				recipients: recipients
			);

			try
			{
				EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Started, emailMessage);
				EmailSendResult result = emailSendOperation.Value;

				if (result.Status == EmailSendStatus.Succeeded)
				{
					await Application.Current.MainPage.DisplayAlert("Email Sent", "Notification sent successfully.", "OK");
					return true;
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Email Failed", "Failed to send email notification.", "OK");
					return false;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
				return false;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
