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
using System.Windows.Shapes;

namespace WCFArchitect.Backstage
{
	public partial class ProjectInfo : Grid
	{
		public ProjectInfo()
		{
			InitializeComponent();

			RefreshProjectInfo();
		}

		public void RefreshProjectInfo()
		{
			if (Globals.ActiveProjectInfo == null) return;

			InfoHeader.Text = "Information about " + Globals.Solution.Name;

			ProjectName.Content = Globals.Solution.Name;
			ServiceList.Children.Clear();
			foreach(WCFArchitect.Projects.Project P in Globals.Projects)
			{
				foreach (WCFArchitect.Projects.Namespace TN in P.Namespaces)
					CreateServiceList(TN);
			}

			ProjectCreated.Content = System.IO.File.GetCreationTime(Globals.ActiveProjectInfo.Path).ToString("dddd, dd MMMM yyyy h:mm tt");
			ProjectAccessed.Content = System.IO.File.GetLastAccessTime(Globals.ActiveProjectInfo.Path).ToString("dddd, dd MMMM yyyy h:mm tt");
			ProjectModified.Content = System.IO.File.GetLastWriteTime(Globals.ActiveProjectInfo.Path).ToString("dddd, dd MMMM yyyy h:mm tt");

			if (Globals.ProjectSpace.OfType<WCFArchitect.Projects.Developer>().Count<WCFArchitect.Projects.Developer>() > 0)
			{
				DeveloperList.Children.Clear();
				foreach (WCFArchitect.Projects.Developer TD in Globals.ProjectSpace.OfType<WCFArchitect.Projects.Developer>())
				{
					Label NL = new Label();
					NL.Content = TD.UserName + " (" + TD.ComputerName + ")";
					NL.Foreground = Brushes.Gray;
					DeveloperList.Children.Add(NL);
					if (TD.LastEditedBy == true) LastEditedBy.Content = NL.Content;
				}
			}
		}

		private void CreateServiceList(WCFArchitect.Projects.Namespace N)
		{
			foreach (WCFArchitect.Projects.Service TS in N.Services)
			{
				Label NL = new Label();
				NL.Content = TS.Name;
				NL.Foreground = Brushes.Gray;
				ServiceList.Children.Add(NL);
				foreach (WCFArchitect.Projects.Namespace TN in N.Children)
					CreateServiceList(TN);
			}
		}
	}
}
