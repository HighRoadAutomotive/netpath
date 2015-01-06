using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EllipticBit.Controls.WPF.Dialogs;
using NETPath.Projects;
using NETPath.Projects.Wcf;
using NETPath.Projects.WebApi;

namespace NETPath.Interface
{
	public partial class Navigator : ContentControl
	{
		public Projects.Project Project { get { return (Projects.Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Projects.Project), typeof(Navigator));

		public object ActivePage { get { return GetValue(ActivePageProperty); } set { SetValue(ActivePageProperty, value); } }
		public static readonly DependencyProperty ActivePageProperty = DependencyProperty.Register("ActivePage", typeof(object), typeof(Navigator));

		public int? ErrorCount { get { return (int?)GetValue(ErrorCountProperty); } set { SetValue(ErrorCountProperty, value); } }
		public static readonly DependencyProperty ErrorCountProperty = DependencyProperty.Register("ErrorCount", typeof(int?), typeof(Navigator), new PropertyMetadata(0));

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand ChangeActivePageCommand = new RoutedCommand();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand DeleteCommand = new RoutedCommand();

		static Navigator()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(ChangeActivePageCommand, OnChangeActivePageCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(DeleteCommand, OnDeleteCommand));
		}

		private static void OnDeleteCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var s = sender as Navigator;
			if (s == null) return;

			Type dt = e.Parameter.GetType();
			if (dt == typeof(WcfNamespace))
			{
				var t = e.Parameter as WcfNamespace;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Namespace?", "Are you sure you want to delete the '" + t.FullName + "' namespace?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Children.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WcfService))
			{
				var t = e.Parameter as WcfService;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Service?", "Are you sure you want to delete the '" + t.Declaration + "' service?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Services.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WcfData))
			{
				var t = e.Parameter as WcfData;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Data?", "Are you sure you want to delete the '" + t.Declaration + "' data?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Data.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(Projects.Enum))
			{
				var t = e.Parameter as Projects.Enum;
				var tp = t?.Parent;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Enumeration?", "Are you sure you want to delete the '" + t.Declaration + "' enumeration?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Enums.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WcfBindingBasicHTTP) || dt == typeof(WcfBindingBasicHTTPS) || dt == typeof(WcfBindingNetHTTP) || dt == typeof(WcfBindingNetHTTPS) || dt == typeof(WcfBindingWSHTTP) || dt == typeof(WcfBindingWS2007HTTP) || dt == typeof(WcfBindingWSDualHTTP) || dt == typeof(WcfBindingWSFederationHTTP) || dt == typeof(WcfBindingWS2007FederationHTTP) || dt == typeof(WcfBindingTCP) || dt == typeof(WcfBindingNamedPipe) || dt == typeof(WcfBindingMSMQ) || dt == typeof(WcfBindingPeerTCP) || dt == typeof(WcfBindingWebHTTP) || dt == typeof(WcfBindingMSMQIntegration))
			{
				var t = e.Parameter as WcfBinding;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Binding?", "Are you sure you want to delete the '" + t.Declaration + "' binding?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Bindings.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WcfHost))
			{
				var t = e.Parameter as WcfHost;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Host?", "Are you sure you want to delete the '" + t.Declaration + "' host?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Hosts.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WebApiNamespace))
			{
				var t = e.Parameter as WebApiNamespace;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Namespace?", "Are you sure you want to delete the '" + t.FullName + "' namespace?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Children.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WebApiService))
			{
				var t = e.Parameter as WebApiService;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Service?", "Are you sure you want to delete the '" + t.Declaration + "' service?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Services.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
			if (dt == typeof(WebApiData))
			{
				var t = e.Parameter as WebApiData;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Data?", "Are you sure you want to delete the '" + t.Declaration + "' data?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Data.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
			}
		}

		private static void OnChangeActivePageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var s = sender as Navigator;
			if (s == null) return;
			var d = e.Parameter as OpenableDocument;
			s.OpenProjectItem(d);
			s.Project.SetSelected(d as DataType);
		}

		public Navigator()
		{
			InitializeComponent();

			ErrorCount = null;
			Globals.Compiler = new Compiler(this);
		}

		public Navigator(Projects.Project Project)
		{
			InitializeComponent();

			ErrorCount = null;
			this.Project = Project;
			Globals.Compiler = new Compiler(this);

			if (Project.GetType() == typeof(WcfProject))
				OpenProjectItem((Project as WcfProject)?.Namespace.GetLastSelectedItem() ?? this.Project);
			if (Project.GetType() == typeof(WebApiProject))
				OpenProjectItem((Project as WebApiProject)?.Namespace.GetLastSelectedItem() ?? this.Project);
		}

		public void AddNewItem()
		{
			if (Project.GetType() == typeof(WcfProject))
				DialogService.ShowContentDialog(Project, "New Item", new Wcf.Dialogs.NewItem(Project as WcfProject, OpenProjectItem));
			if (Project.GetType() == typeof(WebApiProject))
				DialogService.ShowContentDialog(Project, "New Item", new WebApi.Dialogs.NewItem(Project as WebApiProject, OpenProjectItem));
		}

		public void BuildProject()
		{
			ShowOutput.IsChecked = true;
			Globals.Compiler.Build();
		}

		internal void OpenProjectItem(OpenableDocument Item)
		{
			if (Item == null) return;

			if (Item.GetType() == typeof(WcfProject))
				ActivePage = new Wcf.Project(Item as WcfProject);

			if (Item.GetType() == typeof(WcfNamespace))
				ActivePage = new Wcf.Namespace.Namespace(Item as WcfNamespace);

			if (Item.GetType() == typeof(WcfService))
				ActivePage = new Wcf.Service.Service(Item as WcfService);

			if (Item.GetType() == typeof(WcfData))
				ActivePage = new Wcf.Data.Data(Item as WcfData);

			if (Item.GetType() == typeof(WcfHost))
				ActivePage = new Wcf.Host.Host(Item as WcfHost);

			if (Item.GetType() == typeof(WcfBindingBasicHTTP))
				ActivePage = new Wcf.Bindings.BasicHTTP(Item as WcfBindingBasicHTTP);

			if (Item.GetType() == typeof(WcfBindingBasicHTTPS))
				ActivePage = new Wcf.Bindings.BasicHTTPS(Item as WcfBindingBasicHTTPS);

			if (Item.GetType() == typeof(WcfBindingNetHTTP))
				ActivePage = new Wcf.Bindings.NetHTTP(Item as WcfBindingNetHTTP);

			if (Item.GetType() == typeof(WcfBindingNetHTTPS))
				ActivePage = new Wcf.Bindings.NetHTTPS(Item as WcfBindingNetHTTPS);

			if (Item.GetType() == typeof(WcfBindingWSHTTP))
				ActivePage = new Wcf.Bindings.WSHTTP(Item as WcfBindingWSHTTP);

			if (Item.GetType() == typeof(WcfBindingWS2007HTTP))
				ActivePage = new Wcf.Bindings.WS2007HTTP(Item as WcfBindingWS2007HTTP);

			if (Item.GetType() == typeof(WcfBindingWSFederationHTTP))
				ActivePage = new Wcf.Bindings.WSFederationHTTP(Item as WcfBindingWSFederationHTTP);

			if (Item.GetType() == typeof(WcfBindingWS2007FederationHTTP))
				ActivePage = new Wcf.Bindings.WS2007FederationHTTP(Item as WcfBindingWS2007FederationHTTP);

			if (Item.GetType() == typeof(WcfBindingWSDualHTTP))
				ActivePage = new Wcf.Bindings.WSDualHTTP(Item as WcfBindingWSDualHTTP);

			if (Item.GetType() == typeof(WcfBindingTCP))
				ActivePage = new Wcf.Bindings.TCP(Item as WcfBindingTCP);

			if (Item.GetType() == typeof(WcfBindingUDP))
				ActivePage = new Wcf.Bindings.UDP(Item as WcfBindingUDP);

			if (Item.GetType() == typeof(WcfBindingNamedPipe))
				ActivePage = new Wcf.Bindings.NamedPipe(Item as WcfBindingNamedPipe);

			if (Item.GetType() == typeof(WcfBindingPeerTCP))
				ActivePage = new Wcf.Bindings.PeerTCP(Item as WcfBindingPeerTCP);

			if (Item.GetType() == typeof(WcfBindingMSMQ))
				ActivePage = new Wcf.Bindings.MSMQ(Item as WcfBindingMSMQ);

			if (Item.GetType() == typeof(WcfBindingMSMQIntegration))
				ActivePage = new Wcf.Bindings.MSMQIntegration(Item as WcfBindingMSMQIntegration);


			if (Item.GetType() == typeof(Projects.Enum))
				ActivePage = new Enum.Enum(Item as Projects.Enum);

			//TODO: Finish adding screens as they are made.

			if (Project.GetType() == typeof(WcfProject))
				(Project as WcfProject)?.Namespace.SetSelectedItem(Item);
			if (Project.GetType() == typeof(WebApiProject))
				(Project as WebApiProject)?.Namespace.SetSelectedItem(Item);
		}

		private void OpenProjectItemBelow(OpenableDocument Item)
		{
			if (Item == null) return;
			Type dt = Item.GetType();
			if (dt == typeof(WcfNamespace))
			{
				var t = Item as WcfNamespace;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Children.IndexOf(t);
				int ic = tp.Children.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Children[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Children[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WcfService))
			{
				var t = Item as WcfService;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Services.IndexOf(t);
				int ic = tp.Services.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Services[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Services[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WcfData))
			{
				var t = Item as WcfData;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Data.IndexOf(t);
				int ic = tp.Data.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Data[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Data[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(Projects.Enum))
			{
				var t = Item as Projects.Enum;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Enums.IndexOf(t);
				int ic = tp.Enums.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Enums[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Enums[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WcfBinding))
			{
				var t = Item as WcfBinding;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Bindings.IndexOf(t);
				int ic = tp.Bindings.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Bindings[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Bindings[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WcfHost))
			{
				var t = Item as WcfHost;
				var tp = t?.Parent as WcfNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Hosts.IndexOf(t);
				int ic = tp.Hosts.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Hosts[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Hosts[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WcfProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
		}

		private void ShowProject_Click(object sender, RoutedEventArgs e)
		{
			ShowOutput.IsChecked = false;
			ShowErrors.IsChecked = false;
			ErrorGrid.Visibility = Visibility.Collapsed;
			OutputGrid.Visibility = Visibility.Collapsed;
		}


		private void ShowErrors_Checked(object sender, RoutedEventArgs e)
		{
			ShowOutput.IsChecked = false;
			ErrorGrid.Visibility = Visibility.Visible;
			OutputGrid.Visibility = Visibility.Collapsed;
		}

		private void ShowOutput_Checked(object sender, RoutedEventArgs e)
		{
			ShowErrors.IsChecked = false;
			ErrorGrid.Visibility = Visibility.Collapsed;
			OutputGrid.Visibility = Visibility.Visible;
		}

		private void ErrorShowErrors_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void ErrorShowWarnings_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void ErrorShowInfo_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void ErrorShowErrors_Unchecked(object sender, RoutedEventArgs e)
		{

		}

		private void ErrorShowWarnings_Unchecked(object sender, RoutedEventArgs e)
		{

		}

		private void ErrorShowInfo_Unchecked(object sender, RoutedEventArgs e)
		{

		}
	}

	public class ProjectList : ItemsControl
	{
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ToggleButton();
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
			return new ToggleButton();
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
			if (item == null) return BasicHTTPTemplate;
			if (item.GetType() == typeof(WcfSecurityBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(WcfSecurityBasicHTTP)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(WcfSecurityMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(WcfSecurityMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(WcfSecurityNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(WcfSecurityPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(WcfSecurityTCP)) return TCPTemplate;
			if (item.GetType() == typeof(WcfSecurityWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(WcfSecurityWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(WcfSecurityWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(WcfSecurityWSHTTP)) return WSHTTPTemplate;
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
		public DataTemplate UDPTemplate { get; set; }
		public DataTemplate WebHTTPTemplate { get; set; }
		public DataTemplate WSDualHTTPTemplate { get; set; }
		public DataTemplate WSFederationHTTPTemplate { get; set; }
		public DataTemplate WS2007FederationHTTPTemplate { get; set; }
		public DataTemplate WSHTTPTemplate { get; set; }
		public DataTemplate WS2007HTTPTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return BasicHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingBasicHTTPS)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(WcfBindingNetHTTP)) return NetHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingNetHTTPS)) return NetHTTPSTemplate;
			if (item.GetType() == typeof(WcfBindingMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(WcfBindingMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(WcfBindingNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(WcfBindingPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(WcfBindingTCP)) return TCPTemplate;
			if (item.GetType() == typeof(WcfBindingUDP)) return UDPTemplate;
			if (item.GetType() == typeof(WcfBindingWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingWS2007FederationHTTP)) return WS2007FederationHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingWSHTTP)) return WSHTTPTemplate;
			if (item.GetType() == typeof(WcfBindingWS2007HTTP)) return WS2007HTTPTemplate;
			return BasicHTTPTemplate;
		}
	}

	public class ProjectListOperationTemplateSelector : DataTemplateSelector
	{
		public DataTemplate MethodTemplate { get; set; }
		public DataTemplate PropertyTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item != null && item.GetType() == typeof(WcfMethod)) return MethodTemplate;
			if (item != null && item.GetType() == typeof(WcfProperty)) return PropertyTemplate;
			return MethodTemplate;
		}
	}

	[ValueConversion(typeof(CompileMessageSeverity), typeof(string))]
	public class CompileMessageSeverityImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			var lt = (CompileMessageSeverity)value;
			if (lt == CompileMessageSeverity.ERROR) return "pack://application:,,,/NETPath;component/Icons/X16/Error.png";
			if (lt == CompileMessageSeverity.WARN) return "pack://application:,,,/NETPath;component/Icons/X16/Warning.png";
			if (lt == CompileMessageSeverity.INFO) return "pack://application:,,,/NETPath;component/Icons/X16/Message.png";
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class ErrorObjectNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Type valueType = value.GetType();

			if (valueType == typeof(string)) return value;
			if (valueType == typeof(WcfProject))
			{
				var T = value as WcfProject;
				if (T != null) return string.IsNullOrEmpty(T.Name) ? "Project Settings" : T.Name;
			}
			if (valueType == typeof(WcfNamespace))
			{
				var T = value as WcfNamespace;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Namespace";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfService))
			{
				var T = value as WcfService;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Service";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfMethod))
			{
				var T = value as WcfOperation;
				if (T != null && string.IsNullOrEmpty(T.ServerName))
					return "Operation";
				if (T != null) return T.ServerName;
			}
			if (valueType == typeof(WcfMethodParameter))
			{
				var T = value as WcfMethodParameter;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Operation Parameter";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfProperty))
			{
				var T = value as WcfProperty;
				if (T != null && string.IsNullOrEmpty(T.ServerName))
					return "Property";
				if (T != null) return T.ServerName;
			}
			if (valueType == typeof(WcfData))
			{
				var T = value as WcfData;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Data";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfDataElement))
			{
				var T = value as WcfDataElement;
				if (T != null && string.IsNullOrEmpty(T.DataType.Name))
					return "Data Value";
				if (T != null) return T.DataType.Name;
			}
			if (valueType == typeof(WcfBinding))
			{
				var T = value as WcfBinding;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Service Binding";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfHost))
			{
				var T = value as WcfHost;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Host";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Projects.Enum))
			{
				var T = value as Projects.Enum;
				return string.IsNullOrEmpty(T.Name) ? "Enum" : T.Name;
			}
			if (valueType == typeof(EnumElement))
			{
				var T = value as EnumElement;
				return string.IsNullOrEmpty(T.Name) ? "Enum Value" : T.Name;
			}
			if (valueType == typeof(WcfProject))
			{
				var T = value as WcfProject;
				if (T != null) return string.IsNullOrEmpty(T.Name) ? "Project Settings" : T.Name;
			}
			if (valueType == typeof(WebApiNamespace))
			{
				var T = value as WebApiNamespace;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Namespace";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WebApiService))
			{
				var T = value as WebApiService;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Service";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WebApiMethod))
			{
				var T = value as WebApiMethod;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Operation";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WebApiMethodParameter))
			{
				var T = value as WebApiMethodParameter;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Operation Parameter";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WebApiData))
			{
				var T = value as WebApiData;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Data";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WebApiDataElement))
			{
				var T = value as WebApiDataElement;
				if (T != null && string.IsNullOrEmpty(T.DataType.Name))
					return "Data Value";
				if (T != null) return T.DataType.Name;
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class ErrorObjectOwnerNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Type valueType = value.GetType();

			if (valueType == typeof(string)) return value;
			if (valueType == typeof(Projects.Project))
			{
				var T = value as Projects.Project;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Project Settings";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(WcfNamespace))
			{
				var T = value as WcfNamespace;
				if (T != null && string.IsNullOrEmpty(T.Owner.Name))
					return "Namespace";
				if (T != null) return T.Owner.Name;
			}
			if (valueType == typeof(WcfService))
			{
				var T = value as WcfService;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Service";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(WcfData))
			{
				var T = value as WcfData;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Data";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(WcfBinding))
			{
				var T = value as WcfBinding;
				if (T != null && string.IsNullOrEmpty(T.Parent.Name))
					return "Service Binding";
				if (T != null) return T.Parent.Name;
			}
			if (valueType == typeof(WcfHost))
			{
				var T = value as WcfHost;
				if (T != null && (T.Parent != null && string.IsNullOrEmpty(T.Parent.Name)))
					return "Host";
				if (T != null && T.Parent != null) return T.Parent.Name;
			}
			if (valueType == typeof(Projects.Enum))
			{
				var T = value as Projects.Enum;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Enum";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(WebApiNamespace))
			{
				var T = value as WebApiNamespace;
				if (T != null && string.IsNullOrEmpty(T.Owner.Name))
					return "Namespace";
				if (T != null) return T.Owner.Name;
			}
			if (valueType == typeof(WebApiService))
			{
				var T = value as WebApiService;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Service";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(WebApiData))
			{
				var T = value as WebApiData;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Data";
				if (T != null) return T.Parent.Owner.Name;
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class DocumentTypeImage16Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/NETPath;component/Icons/X16/Project.png";
			Type valueType = value.GetType();
			if (valueType == typeof(WcfProject) || valueType == typeof(WebApiProject)) return "pack://application:,,,/NETPath;component/Icons/X16/Project.png";
			if (valueType == typeof(WcfNamespace) || valueType == typeof(WebApiNamespace)) return "pack://application:,,,/NETPath;component/Icons/X16/Namespace.png";
			if (valueType == typeof(WcfService)) return "pack://application:,,,/NETPath;component/Icons/X16/Service.png";
			if (valueType == typeof(WebApiService)) return "pack://application:,,,/NETPath;component/Icons/X16/Rest.png";
			if (valueType == typeof(WcfOperation)) return "pack://application:,,,/NETPath;component/Icons/X16/Method.png";
			if (valueType == typeof(WcfMethodParameter) || valueType == typeof(WebApiRouteParameter) || valueType == typeof(WebApiMethodParameter)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
			if (valueType == typeof(WcfProperty)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
			if (valueType == typeof(WcfData) || valueType == typeof(WebApiData)) return "pack://application:,,,/NETPath;component/Icons/X16/Data.png";
			if (valueType == typeof(WcfDataElement) || valueType == typeof(WebApiDataElement)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
			if (valueType == typeof(Projects.Enum)) return "pack://application:,,,/NETPath;component/Icons/X16/Enum.png";
			if (valueType == typeof(EnumElement)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
			return "pack://application:,,,/NETPath;component/Icons/X16/Project.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[System.Windows.Markup.ContentProperty("Pages")]
	public class NewItemType : DependencyObject
	{
		public string ImageSource { get { return (string)GetValue(ImageSourceProperty); } set { SetValue(ImageSourceProperty, value); } }
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(NewItemType));

		public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NewItemType));

		public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(NewItemType));

		public int DataType { get { return (int)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }
		public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(int), typeof(NewItemType));

		public NewItemType() { }

		public NewItemType(string ImageSource, string Title, string Description, int DataType)
		{
			this.ImageSource = ImageSource;
			this.Title = Title;
			this.Description = Description;
			this.DataType = DataType;
		}
	}
}