using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WCFArchitect.Interface.Project
{
	public partial class Screen : ContentControl
	{
		public Projects.Project Project { get { return (Projects.Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Projects.Project), typeof(Screen));

		public object ActivePage { get { return (object)GetValue(ActivePageProperty); } set { SetValue(ActivePageProperty, value); } }
		public static readonly DependencyProperty ActivePageProperty = DependencyProperty.Register("ActivePage", typeof(object), typeof(Screen));

		public Screen()
		{
			InitializeComponent();
		}

		public Screen(Projects.Project Project)
		{
			InitializeComponent();

			this.Project = Project;
			this.Project.Namespace.Children.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Services.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Data.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Enums.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Bindings.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Security.CollectionChanged += Project_CollectionChanged;
			this.Project.Namespace.Hosts.CollectionChanged += Project_CollectionChanged;
		}

		void Project_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void NewItem_Click(object sender, RoutedEventArgs e)
		{

		}

		private void SaveProject_Click(object sender, RoutedEventArgs e)
		{

		}

		private void BuildProject_Click(object sender, RoutedEventArgs e)
		{

		}
	}

	public class ProjectList : ItemsControl
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new System.Windows.Controls.Primitives.ToggleButton();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return false;
		}
	}

	public class ProjectItemList : ItemsControl
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new System.Windows.Controls.Button();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return false;
		}
	}

	public class ProjectListSecurityTemplateSelector : DataTemplateSelector
	{
		public DataTemplate BasicHTTPTemplate { get; set; }
		public DataTemplate BasicHTTPSTemplate { get; set; }
		public DataTemplate MSMQTemplate { get; set; }
		public DataTemplate MSMQIntegrationTemplate { get; set; }
		public DataTemplate NamedPipeTemplate { get; set; }
		public DataTemplate PeerTCPTemplate { get; set; }
		public DataTemplate TCPTemplate { get; set; }
		public DataTemplate WebHTTPTemplate { get; set; }
		public DataTemplate WSDualHTTPTemplate { get; set; }
		public DataTemplate WSFederationHTTPTemplate { get; set; }
		public DataTemplate WSHTTPTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item.GetType() == typeof(Projects.BindingSecurityBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityBasicHTTP)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityTCP)) return TCPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(Projects.BindingSecurityWSHTTP)) return WSHTTPTemplate;
			return BasicHTTPTemplate;
		}
	}

	public class ProjectListBindingTemplateSelector : DataTemplateSelector
	{
		public DataTemplate BasicHTTPTemplate { get; set; }
		public DataTemplate BasicHTTPSTemplate { get; set; }
		public DataTemplate NetHTTPTemplate { get; set; }
		public DataTemplate NetHTTPSTemplate { get; set; }
		public DataTemplate MSMQTemplate { get; set; }
		public DataTemplate MSMQIntegrationTemplate { get; set; }
		public DataTemplate NamedPipeTemplate { get; set; }
		public DataTemplate PeerTCPTemplate { get; set; }
		public DataTemplate TCPTemplate { get; set; }
		public DataTemplate WebHTTPTemplate { get; set; }
		public DataTemplate WSDualHTTPTemplate { get; set; }
		public DataTemplate WSFederationHTTPTemplate { get; set; }
		public DataTemplate WS2007FederationHTTPTemplate { get; set; }
		public DataTemplate WSHTTPTemplate { get; set; }
		public DataTemplate WS2007HTTPTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item.GetType() == typeof(Projects.ServiceBindingBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingBasicHTTPS)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingNetHTTP)) return NetHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingNetHTTPS)) return NetHTTPSTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingTCP)) return TCPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWS2007FederationHTTP)) return WS2007FederationHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWSHTTP)) return WSHTTPTemplate;
			if (item.GetType() == typeof(Projects.ServiceBindingWS2007HTTP)) return WS2007HTTPTemplate;
			return BasicHTTPTemplate;
		}
	}

	public class ProjectListOperationTemplateSelector : DataTemplateSelector
	{
		public DataTemplate MethodTemplate { get; set; }
		public DataTemplate PropertyTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item.GetType() == typeof(Projects.Method)) return MethodTemplate;
			if (item.GetType() == typeof(Projects.Property)) return PropertyTemplate;
			return MethodTemplate;
		}
	}
}