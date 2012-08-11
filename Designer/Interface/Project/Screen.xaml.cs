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
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand ChangeActivePageCommand = new RoutedCommand();

		static Screen()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Screen), new CommandBinding(Screen.ChangeActivePageCommand, OnChangeActivePageCommandExecuted));
		}

		private static void OnChangeActivePageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Screen s = e.Source as Screen;
			if (s == null) return;
			Projects.OpenableDocument d = e.Parameter as Projects.OpenableDocument;
			s.OpenProjectItem(d);
		}

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

		}

		private void NewItem_Click(object sender, RoutedEventArgs e)
		{
			Globals.ShowDialogBox(Project, "New Item", new Dialogs.NewItem(Project, new Action<Projects.OpenableDocument>(OpenProjectItem)));
		}

		private void SaveProject_Click(object sender, RoutedEventArgs e)
		{
			Projects.Project.Save(Project);
		}

		private void BuildProject_Click(object sender, RoutedEventArgs e)
		{
			//Need the compiler driver for this to work.
		}

		private void OpenProjectItem(Projects.OpenableDocument Item)
		{
			//Need to make all item pages before this function can be completed.
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

	public class NewItemType : DependencyObject
	{
		public string ImageSource { get { return (string)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(NewItemType));

		public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NewItemType));

		public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(NewItemType));

		public string Frameworks { get { return (string)GetValue(FrameworksProperty); } set { SetValue(FrameworksProperty, value); } }
		public static readonly DependencyProperty FrameworksProperty = DependencyProperty.Register("Frameworks", typeof(string), typeof(NewItemType));

		public int DataType { get { return (int)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(int), typeof(NewItemType));
	}
}