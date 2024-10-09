using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiForIoTDevices.Models;
using Microsoft.Maui.Controls;

namespace MauiForIoTDevices.ViewModels
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
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

		private void RemoveDevice(IoTDevice device)
		{
			IoTDevices.Remove(device);
			// send email when removed here
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
