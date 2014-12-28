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
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Helpers;

namespace NETPath.Interface.Host
{
	internal partial class Host : Grid
	{
		public Projects.Host Data { get { return (Projects.Host)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Projects.Host), typeof(Host));

		public object ActiveEndpoint { get { return GetValue(ActiveEndpointProperty); } set { SetValue(ActiveEndpointProperty, value); } }
		public static readonly DependencyProperty ActiveEndpointProperty = DependencyProperty.Register("ActiveEndpoint", typeof(object), typeof(Host));

		public object ActiveBehavior { get { return GetValue(ActiveBehaviorProperty); } set { SetValue(ActiveBehaviorProperty, value); } }
		public static readonly DependencyProperty ActiveBehaviorProperty = DependencyProperty.Register("ActiveBehavior", typeof(object), typeof(Host));

		public Host(Projects.Host Data)
		{
			this.Data = Data;

			InitializeComponent();

			OpenTimeout.Value = Data.OpenTimeout;
			CloseTimeout.Value = Data.CloseTimeout;
			UserNamePasswordCachedLogonTokenLifetime.Value = Data.Credentials.UserNamePasswordCachedLogonTokenLifetime;

			Service.SelectedItem = Data.Service;
			Service.ItemsSource = Data.Parent.Owner.Namespace.GetServices();

			HostEndpoint s = Data.Endpoints.FirstOrDefault(a => a.IsSelected);
			if (s != null) EndpointList.SelectedItem = s;
			HostBehavior c = Data.Behaviors.FirstOrDefault(a => a.IsSelected);
			if (c != null) BehaviorList.SelectedItem = c;
		}

		private void Service_SelectionChanged(object Sender, SelectionChangedEventArgs E)
		{
			Data.Service = Service.SelectedItem as Projects.Service;
		}

		private void Namespace_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded == false) return;
			Data.Namespace = RegExs.ReplaceSpaces.Replace(Namespace.Text, "");
		}

		private void Namespace_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = RegExs.MatchHTTPURI.IsMatch(Namespace.Text);
		}

		#region - Endpoints -

		private void AddEndpointName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddEndpointName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddEndpointName.Text);
			AddEndpoint.IsEnabled = (!string.IsNullOrEmpty(AddEndpointName.Text) && e.IsValid);
		}

		private void AddEndpointName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddEndpointName.Text))
					AddEndpoint_Click(sender, null);
		}

		private void AddEndpointName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddEndpointName.IsInvalid == false)
			{
				AddThrottlingBehavior.IsEnabled = false;
				return;
			}
			AddEndpoint.IsEnabled = AddEndpointName.Text != "";
		}

		private void AddEndpoint_Click(object Sender, RoutedEventArgs E)
		{
			if (AddEndpoint.IsEnabled == false) return;

			var t = new HostEndpoint(Data, AddEndpointName.Text);
			Data.Endpoints.Add(t);

			AddEndpointName.Text = "";

			EndpointList.SelectedItem = t;
		}

		private void EndpointList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (EndpointList.SelectedItem == null) return;
			var sb = EndpointList.SelectedItem as HostEndpoint;
			if (sb == null) return;

			foreach (HostEndpoint he in Data.Endpoints)
				he.IsSelected = false;
			sb.IsSelected = true;

			ActiveEndpoint = new Endpoint(sb);
		}

		private void DeleteEndpoint_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as HostEndpoint;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Endpoint?", "Are you sure you want to delete the '" + OP.Name + "' endpoint?", new DialogAction("Yes", () => Data.Endpoints.Remove(OP), true), new DialogAction("No", false, true));
		}

		#endregion

		#region - Behaviors -

		private void AddBehaviorName_Validate(object sender, EllipticBit.Controls.WPF.ValidateEventArgs e)
		{
			e.IsValid = true;
			if (string.IsNullOrEmpty(AddBehaviorName.Text)) return;

			e.IsValid = RegExs.MatchCodeName.IsMatch(AddBehaviorName.Text);
			AddThrottlingBehavior.IsEnabled = (!string.IsNullOrEmpty(AddBehaviorName.Text) && e.IsValid);
			AddDebugBehavior.IsEnabled = (!string.IsNullOrEmpty(AddBehaviorName.Text) && e.IsValid);
			AddMetadataBehavior.IsEnabled = (!string.IsNullOrEmpty(AddBehaviorName.Text) && e.IsValid);
			AddWebHTTPBehavior.IsEnabled = (!string.IsNullOrEmpty(AddBehaviorName.Text) && e.IsValid);
		}

		private void AddBehaviorName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				if (!string.IsNullOrEmpty(AddBehaviorName.Text))
					AddThrottlingBehavior_Click(sender, null);
		}

		private void AddBehaviorName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (AddBehaviorName.IsInvalid == false)
			{
				AddThrottlingBehavior.IsEnabled = false;
				AddDebugBehavior.IsEnabled = false;
				AddMetadataBehavior.IsEnabled = false;
				AddWebHTTPBehavior.IsEnabled = false;
				return;
			}
			if (AddBehaviorName.Text != "")
			{
				AddThrottlingBehavior.IsEnabled = true;
				AddDebugBehavior.IsEnabled = true;
				AddMetadataBehavior.IsEnabled = true;
				AddWebHTTPBehavior.IsEnabled = true;
			}
			else
			{
				AddThrottlingBehavior.IsEnabled = false;
				AddDebugBehavior.IsEnabled = false;
				AddMetadataBehavior.IsEnabled = false;
				AddWebHTTPBehavior.IsEnabled = false;
			}
		}

		private void AddThrottlingBehavior_Click(object Sender, RoutedEventArgs E)
		{
			if (AddThrottlingBehavior.IsEnabled == false) return;

			var t = new HostThrottlingBehavior(AddBehaviorName.Text, Data);
			Data.Behaviors.Add(t);

			AddBehaviorName.Text = "";

			BehaviorList.SelectedItem = t;
		}

		private void AddDebugBehavior_Click(object Sender, RoutedEventArgs E)
		{
			if (AddDebugBehavior.IsEnabled == false) return;

			var t = new HostDebugBehavior(AddBehaviorName.Text, Data);
			Data.Behaviors.Add(t);

			AddBehaviorName.Text = "";

			BehaviorList.SelectedItem = t;
		}

		private void AddMetadataBehavior_Click(object Sender, RoutedEventArgs E)
		{
			if (AddMetadataBehavior.IsEnabled == false) return;

			var t = new HostMetadataBehavior(AddBehaviorName.Text, Data);
			Data.Behaviors.Add(t);

			AddBehaviorName.Text = "";

			BehaviorList.SelectedItem = t;
		}

		private void AddWebHTTPBehavior_Click(object Sender, RoutedEventArgs E)
		{
			if (AddWebHTTPBehavior.IsEnabled == false) return;

			var t = new HostWebHTTPBehavior(AddBehaviorName.Text, Data);
			Data.Behaviors.Add(t);

			AddBehaviorName.Text = "";

			BehaviorList.SelectedItem = t;
		}
		
		private void BehaviorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (BehaviorList.SelectedItem == null) return;
			var sb = BehaviorList.SelectedItem as HostBehavior;
			if (sb == null) return;

			foreach (HostBehavior hb in Data.Behaviors)
				hb.IsSelected = false;
			sb.IsSelected = true;

			if (sb.GetType() == typeof(HostDebugBehavior)) ActiveBehavior = new DebugBehavior((HostDebugBehavior) sb);
			if (sb.GetType() == typeof(HostMetadataBehavior)) ActiveBehavior = new MetadataBehavior((HostMetadataBehavior)sb);
			if (sb.GetType() == typeof(HostThrottlingBehavior)) ActiveBehavior = new ThrottlingBehavior((HostThrottlingBehavior)sb);
			if (sb.GetType() == typeof(HostWebHTTPBehavior)) ActiveBehavior = new WebHTTPBehavior((HostWebHTTPBehavior)sb);
		}

		private void DeleteBehavior_Click(object sender, RoutedEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(sender);
			var OP = lbi.Content as HostBehavior;
			if (OP == null) return;

			DialogService.ShowMessageDialog("NETPath", "Delete Endpoint?", "Are you sure you want to delete the '" + OP.Name + "' behavior?", new DialogAction("Yes", () => Data.Behaviors.Remove(OP), true), new DialogAction("No", false, true));
		}

		#endregion
	}
}