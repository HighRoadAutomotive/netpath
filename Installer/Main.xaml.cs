using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WCF_Architect_Installer
{
	public partial class Main : Window
	{
		public Main()
		{
			InitializeComponent();
		}

		private void MinimizeCommand_Click(object sender, RoutedEventArgs e)
		{
			WindowState = System.Windows.WindowState.Minimized;
		}

		private void CloseCommand_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("Closing the installer will cancel this installation. Are you sure you want to close the installer?", "Cancel Installation?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
				return;

			//Stop the install process here!
			
			this.Close();
		}
	}
}