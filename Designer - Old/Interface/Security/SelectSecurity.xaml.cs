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

namespace WCFArchitect.Interface.Project.Security
{
	internal partial class SelectSecurity : Window
	{
		private Projects.Project Project { get; set; }

		public Projects.BindingSecurity SelectedSecurity { get; private set; }

		public SelectSecurity(Projects.Project Project)
		{
			this.Project = Project;

			InitializeComponent();

			SecurityItems.ItemsSource = Project.Security;
		}

		public SelectSecurity(Projects.Project Project, Type SecurityType)
		{
			this.Project = Project;

			InitializeComponent();

			ObservableCollection<Projects.BindingSecurity> ItemsList = new ObservableCollection<Projects.BindingSecurity>();
			SecurityItems.ItemsSource = ItemsList;

			foreach (Projects.BindingSecurity S in Project.Security)
			{
				if (SecurityType == typeof(Projects.BindingSecurityBasicHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityBasicHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityWSHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityWSDualHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSDualHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityWSFederationHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWSFederationHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityTCP)) if (S.GetType() == typeof(Projects.BindingSecurityTCP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityNamedPipe)) if (S.GetType() == typeof(Projects.BindingSecurityNamedPipe)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityMSMQ)) if (S.GetType() == typeof(Projects.BindingSecurityMSMQ)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityPeerTCP)) if (S.GetType() == typeof(Projects.BindingSecurityPeerTCP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityWebHTTP)) if (S.GetType() == typeof(Projects.BindingSecurityWebHTTP)) ItemsList.Add(S);
				if (SecurityType == typeof(Projects.BindingSecurityMSMQIntegration)) if (S.GetType() == typeof(Projects.BindingSecurityMSMQIntegration)) ItemsList.Add(S);
			}
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			SelectedSecurity = (Projects.BindingSecurity)SecurityItems.SelectedItem;

			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void SecurityItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Projects.BindingSecurity t = (Projects.BindingSecurity)SecurityItems.SelectedItem;
			SecurityName.Text = t.Name;
			if (t.GetType() == typeof(Projects.BindingSecurityBasicHTTP)) SecurityType.Text = "Basic HTTP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityWSHTTP)) SecurityType.Text = "WS HTTP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityWSDualHTTP)) SecurityType.Text = "WS Dual HTTP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityWSFederationHTTP)) SecurityType.Text = "WS Federation Security";
			if (t.GetType() == typeof(Projects.BindingSecurityTCP)) SecurityType.Text = "TCP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityNamedPipe)) SecurityType.Text = "Named Pipe Security";
			if (t.GetType() == typeof(Projects.BindingSecurityMSMQ)) SecurityType.Text = "MSMQ Security";
			if (t.GetType() == typeof(Projects.BindingSecurityPeerTCP)) SecurityType.Text = "Peer TCP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityWebHTTP)) SecurityType.Text = "Web HTTP Security";
			if (t.GetType() == typeof(Projects.BindingSecurityMSMQIntegration)) SecurityType.Text = "MSMQ Integration Security";
		}
	}
}