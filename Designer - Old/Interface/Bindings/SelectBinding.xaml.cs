using System;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace WCFArchitect.Interface.Project.Bindings
{
	internal partial class SelectBinding : Window
	{

		private Projects.Project Project { get; set; }

		public Projects.ServiceBinding SelectedBinding { get; private set; }

		public SelectBinding(Projects.Project Project)
		{
			this.Project = Project;

			InitializeComponent();

			BindingItems.ItemsSource = Project.Bindings;
		}

		public SelectBinding(Projects.Project Project, Type SecurityType)
		{
			this.Project = Project;

			InitializeComponent();

			ObservableCollection<Projects.ServiceBinding> ItemsList = new ObservableCollection<Projects.ServiceBinding>();
			BindingItems.ItemsSource = ItemsList;

			foreach (Projects.ServiceBinding S in Project.Bindings)
			{
				if (SecurityType == typeof(Projects.ServiceBindingBasicHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityBasicHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWSHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWS2007HTTP)) if (S.GetType() == typeof(Projects.ServiceBindingWS2007HTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWSDualHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSDualHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWSFederationHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSFederationHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWS2007FederationHTTP)) if (S.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingTCP)) if (S.GetType() == typeof(Projects.BindingSecurityTCP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingNamedPipe)) if (S.GetType() == typeof(Projects.BindingSecurityNamedPipe)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingMSMQ)) if (S.GetType() == typeof(Projects.BindingSecurityMSMQ)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingPeerTCP)) if (S.GetType() == typeof(Projects.BindingSecurityPeerTCP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingWebHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWebHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.ServiceBindingMSMQIntegration)) if (S.GetType() == typeof(Projects.BindingSecurityMSMQIntegration)) ItemsList.Add(S);
			}
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			SelectedBinding = (Projects.ServiceBinding)BindingItems.SelectedItem;

			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void BindingItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Projects.ServiceBinding t = (Projects.ServiceBinding)BindingItems.SelectedItem;
			BindingName.Text = t.Name;
			if (t.GetType() == typeof(Projects.ServiceBindingBasicHTTP)) BindingType.Text = "Basic HTTP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWSHTTP)) BindingType.Text = "WS HTTP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWS2007HTTP)) BindingType.Text = "WS 2007 HTTP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWSDualHTTP)) BindingType.Text = "WS Dual HTTP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP)) BindingType.Text = "WS Federation Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP)) BindingType.Text = "WS 2007 Federation Security";
			if (t.GetType() == typeof(Projects.ServiceBindingTCP)) BindingType.Text = "TCP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingNamedPipe)) BindingType.Text = "Named Pipe Security";
			if (t.GetType() == typeof(Projects.ServiceBindingMSMQ)) BindingType.Text = "MSMQ Security";
			if (t.GetType() == typeof(Projects.ServiceBindingPeerTCP)) BindingType.Text = "Peer TCP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingWebHTTP)) BindingType.Text = "Web HTTP Security";
			if (t.GetType() == typeof(Projects.ServiceBindingMSMQIntegration)) BindingType.Text = "MSMQ Integration Security";
		}
	}
}