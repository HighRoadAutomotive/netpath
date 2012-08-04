using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WCFArchitect.Interface.Project.Host
{
	internal partial class Host : Grid
	{
		public Projects.Host Data { get; set; }

		public Projects.HostBehavior SelectedBehavior { get; set; }
		public Projects.HostEndpoint SelectedEndpoint { get; set; }

		public Host(Projects.Host Data)
		{
			this.Data = Data;

			InitializeComponent();

			this.OpenTimeout.Value = Data.OpenTimeout;
			this.CloseTimeout.Value = Data.CloseTimeout;
			this.UserNamePasswordCachedLogonTokenLifetime.Value = Data.Credentials.UserNamePasswordCachedLogonTokenLifetime;

			if (Data.Service != null)
				Service.Text = Data.Service.Name;

			EndpointList.ItemsSource = Data.Endpoints;
			BehaviorList.ItemsSource = Data.Behaviors;
		}

		private void CodeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.CodeName = Helpers.RegExs.ReplaceSpaces.Replace(CodeName.Text, "");
		}

		private void CodeName_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchCodeName.IsMatch(CodeName.Text);
		}

		private void Namespace_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.Namespace = Helpers.RegExs.ReplaceSpaces.Replace(Namespace.Text, "");
		}

		private void Namespace_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = Helpers.RegExs.MatchHTTPURI.IsMatch(Namespace.Text);
		}

		private void OpenTimeout_ValueChanged(object sender, C1.WPF.DateTimeEditors.NullablePropertyChangedEventArgs<TimeSpan> e)
		{
			if (IsLoaded == false) return;
			if (e.NewValue.HasValue == false) { Data.OpenTimeout = TimeSpan.Zero; return; }
			Data.OpenTimeout = e.NewValue.Value;
		}

		private void CloseTimeout_ValueChanged(object sender, C1.WPF.DateTimeEditors.NullablePropertyChangedEventArgs<TimeSpan> e)
		{
			if (IsLoaded == false) return;
			if (e.NewValue.HasValue == false) { Data.CloseTimeout = TimeSpan.Zero; return; }
			Data.CloseTimeout = e.NewValue.Value;
		}

		private void AuthorizationImpersonateCallerForAllOperations_Checked(object sender, RoutedEventArgs e)
		{
			AuthorizationImpersonateCallerForAllOperations.Content = "Yes";
		}

		private void AuthorizationImpersonateCallerForAllOperations_Unchecked(object sender, RoutedEventArgs e)
		{
			AuthorizationImpersonateCallerForAllOperations.Content = "No";
		}

		private void ManualFlowControlLimit_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.ManualFlowControlLimit = Convert.ToInt32(e.NewValue);
		}

		private void SelectService_Click(object sender, RoutedEventArgs e)
		{
			Service.SelectService SS = new Service.SelectService(Data.Parent, false);
			if (SS.ShowDialog() == true)
			{
				Data.Service = SS.SelectedService;
				Service.Text = Data.Service.Name;
			}
		}

		private void AddEndpoint_Click(object sender, RoutedEventArgs e)
		{
			string EN = (string)Prospective.Controls.InputBox.ShowDialog(Globals.MainScreen, "Please enter a name for the new Endpoint.", "New Endpoint", true, Prospective.Controls.InputBoxType.String, "");
			if (EN == null || EN == "") { return; }
			else
			{
				Projects.HostEndpoint NHE = new Projects.HostEndpoint(Data, EN);
				Data.Endpoints.Add(NHE);
				EndpointList.SelectedItem = NHE;
			}
		}

		private void DeleteEndpoint_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the endpoint '" + SelectedEndpoint.Name + "' from this host?", "Confirm Endpoint Removal", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				Data.Endpoints.Remove(SelectedEndpoint);
			EndpointList.SelectedItem = null;
		}

		private void EndpointList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (EndpointList.SelectedItem != null)
			{
				EndpointContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				SelectedEndpoint = (Projects.HostEndpoint)EndpointList.SelectedItem;
			}
			else
			{
				EndpointContentEmpty.Visibility = System.Windows.Visibility.Visible;
				EndpointContent.Content = null;
				SelectedEndpoint = null;
				DeleteEndpoint.IsEnabled = false;
				return;
			}
			DeleteEndpoint.IsEnabled = true;

			EndpointContent.Content = new Endpoint((Projects.HostEndpoint)SelectedEndpoint);
		}

		private void BehaviorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (BehaviorList.SelectedItem != null)
			{
				BehaviorContentEmpty.Visibility = System.Windows.Visibility.Collapsed;
				SelectedBehavior = (Projects.HostBehavior)BehaviorList.SelectedItem;
			}
			else
			{
				BehaviorContentEmpty.Visibility = System.Windows.Visibility.Visible;
				BehaviorContent.Content = null;
				SelectedBehavior = null;
				DeleteBehavior.IsEnabled = false;
				return;
			}
			DeleteBehavior.IsEnabled = true;

			if (SelectedBehavior.GetType() == typeof(Projects.HostDebugBehavior)) BehaviorContent.Content = new DebugBehavior((Projects.HostDebugBehavior)SelectedBehavior);
			if (SelectedBehavior.GetType() == typeof(Projects.HostMetadataBehavior)) BehaviorContent.Content = new MetadataBehavior((Projects.HostMetadataBehavior)SelectedBehavior);
			if (SelectedBehavior.GetType() == typeof(Projects.HostThrottlingBehavior)) BehaviorContent.Content = new ThrottlingBehavior((Projects.HostThrottlingBehavior)SelectedBehavior);
		}

		private void AddBehavior_Click(object sender, RoutedEventArgs e)
		{
			NewBehavior NB = new NewBehavior(Data);
			NB.ShowDialog();
			BehaviorList.SelectedIndex = Data.Behaviors.Count - 1;
		}

		private void DeleteBehavior_Click(object sender, RoutedEventArgs e)
		{
			if (Prospective.Controls.MessageBox.Show("Are you sure you want to delete the behavior '" + SelectedBehavior.Name + "' from this host?", "Confirm Behavior Removal", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				Data.Behaviors.Remove(SelectedBehavior);
			BehaviorList.SelectedItem = null;
		}

		private void ClientCertificateAuthenticationIncludeWindowsGroups_Checked(object sender, RoutedEventArgs e)
		{
			ClientCertificateAuthenticationIncludeWindowsGroups.Content = "Yes";
		}

		private void ClientCertificateAuthenticationIncludeWindowsGroups_Unchecked(object sender, RoutedEventArgs e)
		{
			ClientCertificateAuthenticationIncludeWindowsGroups.Content = "No";
		}

		private void ClientCertificateAuthenticationMapClientCertificateToWindowsAccount_Checked(object sender, RoutedEventArgs e)
		{
			ClientCertificateAuthenticationMapClientCertificateToWindowsAccount.Content = "Yes";
		}

		private void ClientCertificateAuthenticationMapClientCertificateToWindowsAccount_Unchecked(object sender, RoutedEventArgs e)
		{
			ClientCertificateAuthenticationMapClientCertificateToWindowsAccount.Content = "No";
		}

		private void IssuedTokenAllowUntrustedRsaIssuers_Checked(object sender, RoutedEventArgs e)
		{
			IssuedTokenAllowUntrustedRsaIssuers.Content = "Yes";
		}

		private void IssuedTokenAllowUntrustedRsaIssuers_Unchecked(object sender, RoutedEventArgs e)
		{
			IssuedTokenAllowUntrustedRsaIssuers.Content = "No";
		}

		private void UserNamePasswordCacheLogonTokens_Checked(object sender, RoutedEventArgs e)
		{
			UserNamePasswordCacheLogonTokens.Content = "Yes";
		}

		private void UserNamePasswordCacheLogonTokens_Unchecked(object sender, RoutedEventArgs e)
		{
			UserNamePasswordCacheLogonTokens.Content = "No";
		}

		private void UserNamePasswordIncludeWindowsGroups_Checked(object sender, RoutedEventArgs e)
		{
			UserNamePasswordIncludeWindowsGroups.Content = "Yes";
		}

		private void UserNamePasswordIncludeWindowsGroups_Unchecked(object sender, RoutedEventArgs e)
		{
			UserNamePasswordIncludeWindowsGroups.Content = "No";
		}

		private void WindowsServiceAllowAnonymousLogons_Checked(object sender, RoutedEventArgs e)
		{
			WindowsServiceAllowAnonymousLogons.Content = "Yes";
		}

		private void WindowsServiceAllowAnonymousLogons_Unchecked(object sender, RoutedEventArgs e)
		{
			WindowsServiceAllowAnonymousLogons.Content = "No";
		}

		private void WindowsServiceIncludeWindowsGroups_Checked(object sender, RoutedEventArgs e)
		{
			WindowsServiceIncludeWindowsGroups.Content = "Yes";
		}

		private void WindowsServiceIncludeWindowsGroups_Unchecked(object sender, RoutedEventArgs e)
		{
			WindowsServiceIncludeWindowsGroups.Content = "No";
		}

		private void UserNamePasswordMaxCachedLogonTokens_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
		{
			if (IsLoaded == false) return;
			Data.Credentials.UserNamePasswordMaxCachedLogonTokens = Convert.ToInt32(e.NewValue);
		}
	}

	[ValueConversion(typeof(Projects.HostBehavior), typeof(string))]
	public class HostBehaviorTypeImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			if (value.GetType() == typeof(Projects.HostDebugBehavior)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorDebug.png";
			if (value.GetType() == typeof(Projects.HostMetadataBehavior)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorMetadata.png";
			if (value.GetType() == typeof(Projects.HostThrottlingBehavior)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorThrottling.png";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}