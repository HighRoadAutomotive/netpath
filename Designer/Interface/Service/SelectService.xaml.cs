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

namespace WCFArchitect.Interface.Service
{
	internal partial class SelectService : Window
	{
		private bool IsCallbackSelect;
		private Projects.Project Project { get; set; }
		private List<Projects.Service> Services { get; set; }

		public Projects.Service SelectedService { get; private set; }

		public SelectService(Projects.Project Project, bool IsCallbackSelect)
		{
			this.IsCallbackSelect = IsCallbackSelect;
			this.Project = Project;
			this.Services = new List<Projects.Service>(Project.Services);

			InitializeComponent();

			foreach (Projects.Namespace N in Project.Namespaces)
			{
				if (IsCallbackSelect == false)
				{
					foreach (Projects.Service S in N.Services)
						if (S.IsCallback == false) Services.Add(S);
				}
				else
				{
					foreach (Projects.Service S in N.Services)
						if (S.IsCallback == true && S.Callback == null) Services.Add(S);
				}
				AddChildServices(N);
			}

			ServiceItems.ItemsSource = Services;
		}

		private void AddChildServices(Projects.Namespace NS)
		{
			foreach (Projects.Namespace N in NS.Children)
			{
				foreach (Projects.Service S in N.Services)
					Services.Add(S);
				AddChildServices(N);
			}
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void ServiceItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedService = (Projects.Service)ServiceItems.SelectedItem;
		}
	}
}