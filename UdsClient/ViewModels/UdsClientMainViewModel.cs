
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows;
using UdsClient.Services;

namespace UdsClient.ViewModels
{
    public class UdsClientMainViewModel: ObservableObject
    {
		#region Properties and Fields

		public string Version { get; set; }

		public ObservableCollection<string> UDSSessionsList { get; set; }

		private UdsSessionsSender _udsSessionsSender;

		#endregion Properties and Fields

		#region Constructor

		public UdsClientMainViewModel()
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			SendCommand = new RelayCommand<string>(Send);

			UDSSessionsList = new ObservableCollection<string>()
			{
				"Diagnostic Session Control",
				"ECU Reset",
				"Clear Diagnostic Information",
				"Security Access",
				"Communication Control",
				"Authentication",
				"Tester Present",
				"Access Timing Parameters",
				"Secured Data Transmission",
				"Control DTC Settings",
				"Response On Event",
				"Link Control",
				"Read Data By Identifier",
				"Read Memory By Address",
				"Read Scaling Data By Identifier",
				"Read Data By Identifier Periodic",
				"Dynamically Define Data Identifier",
				"Write Data By Identifier",
				"Write Memory By Address",
				"Clear Diagnostic Information",
				"Read DTC Information",
				"Input Output Control By Identifier",
				"Routine Control",
				"Request Download",
				"Request Upload",
				"Transfer Data",
				"Request Transfer Exit",
				"Request File Transfer",
			//	"Negative Response", // TODO: was not found in the PCAN example
			};

			_udsSessionsSender = new UdsSessionsSender();
			_udsSessionsSender.Init();
		}

		#endregion Constructor

		#region Methods

		private void Send(string sessionFromList)
		{
			string session = sessionFromList.ToLower();
			session = session.Replace(" ", string.Empty);

			if (_udsSessionsSender.DescriptionToMethodDict.ContainsKey(session) == false)
			{
				MessageBox.Show($"Session {sessionFromList} not found");
				return;
			}

			MethodInfo method = _udsSessionsSender.DescriptionToMethodDict[session];
			method?.Invoke(
				_udsSessionsSender,
				new object[] 
				{
					_udsSessionsSender.Client_handle,
					_udsSessionsSender.Config,
				});
		}

		#endregion Methods

		#region Commands

		public RelayCommand<string> SendCommand { get; private set; }

		#endregion Commands
	}
}
