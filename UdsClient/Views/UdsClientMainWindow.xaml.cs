using MahApps.Metro.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UdsClient.ViewModels;

namespace UdsClient.Views
{
	/// <summary>
	/// Interaction logic for UdsClientMainWindow.xaml
	/// </summary>
	public partial class UdsClientMainWindow : MetroWindow
	{
		public UdsClientMainWindow()
		{
			InitializeComponent();
			DataContext = new UdsClientMainViewModel();
		}
	}
}