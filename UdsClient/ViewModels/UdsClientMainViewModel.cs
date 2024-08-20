
using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Standard;
using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Printing;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UdsClient.ViewModels
{
    public class UdsClientMainViewModel: ObservableObject
    {
		public string Version { get; set; }

		public ObservableCollection<string> UDSSessionsList { get; set; }

		public UdsClientMainViewModel()
		{
			Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
				"Negative Response",
			};
		}
	}
}
