using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace UdsClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			this.DispatcherUnhandledException += App_DispatcherUnhandledException;

		}

		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			//LoggerService.Error(this, "Un-handled exception caught", "Error", e.Exception);
			e.Handled = true;
		}
	}

}
