using Azure.Communication.Email;
using MauiForIoTDevices.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiForIoTDevices.Models;
using Microsoft.Maui.Controls;
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

		private async Task<bool> SendEmailNotificationAsync(string deviceName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(SettingsViewModel.SavedEmailAddress) ||
					string.IsNullOrWhiteSpace(SettingsViewModel.SavedEmailServiceConnectionString))
				{
					await Application.Current.MainPage.DisplayAlert("Error", "Email Service Connection String or Email Address is not set. Please configure it in the settings.", "OK");
					return false;
				}

				var connectionString = SettingsViewModel.SavedEmailServiceConnectionString;
				EmailClient emailClient = new EmailClient(connectionString);
				string senderAddress = "DoNotReply@fb9ff88f-1a4b-4978-b8ce-56d9e651a9a0.azurecomm.net";
				string recipientEmail = SettingsViewModel.SavedEmailAddress;

				var recipients = new EmailRecipients(new List<EmailAddress>
		{
			new EmailAddress(recipientEmail)
		});

				var emailMessage = new Azure.Communication.Email.EmailMessage(
					senderAddress: senderAddress,
					content: new EmailContent("IoT Device Removed Notification")
					{
						PlainText = $"The device {deviceName} has been removed from your IoT list."
					},
					recipients: recipients
				);

				await Application.Current.MainPage.DisplayAlert("Info", "Starting email send operation...", "OK");

				EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);
				EmailSendResult result = emailSendOperation.Value;

				if (result.Status == EmailSendStatus.Succeeded)
				{
					await Application.Current.MainPage.DisplayAlert("Success", "Email sent successfully!", "OK");
					return true;
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Failure", "Failed to send email.", "OK");
					return false;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
				return false;
			}
		}


		private async void RemoveDevice(IoTDevice device)
		{
			bool emailSent = await SendEmailNotificationAsync(device.DeviceName);

			if (emailSent)
			{
				IoTDevices.Remove(device);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
