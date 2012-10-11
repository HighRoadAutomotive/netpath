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
using Prospective.Controls.Dialogs;
using WCFArchitect.Projects;

namespace WCFArchitect.Interface
{
	public partial class Navigator : ContentControl
	{
		public Projects.Project Project { get { return (Projects.Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Projects.Project), typeof(Navigator));

		public object ActivePage { get { return GetValue(ActivePageProperty); } set { SetValue(ActivePageProperty, value); } }
		public static readonly DependencyProperty ActivePageProperty = DependencyProperty.Register("ActivePage", typeof(object), typeof(Navigator));

		public int? ErrorCount { get { return (int?)GetValue(ErrorCountProperty); } set { SetValue(ErrorCountProperty, value); } }
		public static readonly DependencyProperty ErrorCountProperty = DependencyProperty.Register("ErrorCount", typeof(int?), typeof(Navigator), new PropertyMetadata(0));

		public ObservableCollection<FindReplaceResult> FindResults { get { return (ObservableCollection<FindReplaceResult>)GetValue(FindResultsProperty); } set { SetValue(FindResultsProperty, value); } }
		public static readonly DependencyProperty FindResultsProperty = DependencyProperty.Register("FindResults", typeof(ObservableCollection<FindReplaceResult>), typeof(Navigator));

		public int? FindResultsCount { get { return (int?)GetValue(FindResultsCountProperty); } set { SetValue(FindResultsCountProperty, value); } }
		public static readonly DependencyProperty FindResultsCountProperty = DependencyProperty.Register("FindResultsCount", typeof(int?), typeof(Navigator), new PropertyMetadata(0));

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand ChangeActivePageCommand = new RoutedCommand();

		public Compiler Compiler { get; private set; }

		static Navigator()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(ChangeActivePageCommand, OnChangeActivePageCommandExecuted));
		}

		private static void OnChangeActivePageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			var s = sender as Navigator;
			if (s == null) return;
			var d = e.Parameter as OpenableDocument;
			s.OpenProjectItem(d);
		}

		public Navigator()
		{
			InitializeComponent();

			FindResults = new ObservableCollection<FindReplaceResult>();
			FindResultsCount = null;
			ErrorCount = null;
			Compiler = new Compiler(this);
		}

		public Navigator(Projects.Project Project)
		{
			InitializeComponent();

			FindResults = new ObservableCollection<FindReplaceResult>();
			FindResultsCount = null;
			ErrorCount = null;
			this.Project = Project;
			Compiler = new Compiler(this);

			OpenProjectItem(this.Project);
		}

		private void NewItem_Click(object sender, RoutedEventArgs e)
		{
			DialogService.ShowContentDialog(Project, "New Item", new Dialogs.NewItem(Project, OpenProjectItem));
		}

		private void SaveProject_Click(object sender, RoutedEventArgs e)
		{
			Projects.Project.Save(Project);
		}

		private void DeleteProject_Click(object sender, RoutedEventArgs e)
		{
			DialogService.ShowMessageDialog(Project, "Permanently Delete Project?", "This project will be removed from the solution. Would you like to delete it from the disk as well?", new DialogAction("Yes", KillProject), new DialogAction("No", RemoveProject, true));
		}

		private void BuildProject_Click(object sender, RoutedEventArgs e)
		{
			ShowOutput.IsChecked = true;
			Compiler.Build();
		}

		private void OpenProjectItem(OpenableDocument Item)
		{
			if (Item == null) return;

			if (Item.GetType() == typeof(Projects.Project))
				ActivePage = new Project.Project(Item as Projects.Project);

			//Open Service Page Here

			if (Item.GetType() == typeof(Projects.Data))
				ActivePage = new Data.Data(Item as Projects.Data);

			//Need to make all item pages before this function can be completed.
		}

		private void RemoveProject()
		{
			Globals.Projects.Remove(Project);
			Globals.Solution.Projects.Remove(Globals.GetRelativePath(Globals.SolutionPath, Project.AbsolutePath));

			if (Globals.Projects.Count == 0) Globals.MainScreen.ShowHomeScreen();
			else
			{
				var t = Globals.MainScreen.ScreenButtons.Items[0] as SolutionItem;
				if (t != null)
					Globals.MainScreen.SelectProjectScreen(t.Content as Navigator);
			}
		}

		private void KillProject()
		{
			RemoveProject();

			if(System.IO.File.Exists(Project.AbsolutePath))
				System.IO.File.Delete(Project.AbsolutePath);
		}

		private void ShowProject_Click(object sender, RoutedEventArgs e)
		{
			ShowFindReplace.IsChecked = false;
			ShowOutput.IsChecked = false;
			ShowErrors.IsChecked = false;
			FindReplaceGrid.Visibility = Visibility.Collapsed;
			ErrorGrid.Visibility = Visibility.Collapsed;
			OutputGrid.Visibility = Visibility.Collapsed;
		}

		private void ShowFindReplace_Checked(object sender, RoutedEventArgs e)
		{
			ShowOutput.IsChecked = false;
			ShowErrors.IsChecked = false;
			FindReplaceGrid.Visibility = Visibility.Visible;
			ErrorGrid.Visibility = Visibility.Collapsed;
			OutputGrid.Visibility = Visibility.Collapsed;
		}

		private void ShowErrors_Checked(object sender, RoutedEventArgs e)
		{
			ShowFindReplace.IsChecked = false;
			ShowOutput.IsChecked = false;
			FindReplaceGrid.Visibility = Visibility.Collapsed;
			ErrorGrid.Visibility = Visibility.Visible;
			OutputGrid.Visibility = Visibility.Collapsed;
		}

		private void ShowOutput_Checked(object sender, RoutedEventArgs e)
		{
			ShowFindReplace.IsChecked = false;
			ShowErrors.IsChecked = false;
			FindReplaceGrid.Visibility = Visibility.Collapsed;
			ErrorGrid.Visibility = Visibility.Collapsed;
			OutputGrid.Visibility = Visibility.Visible;
		}

		#region " Find / Replace "

		private void FindNext_Click(object sender, RoutedEventArgs e)
		{
			if (FindResultsCount == 0) FindAll_Click(null, null);
			if (FindList.SelectedIndex == FindResults.Count - 1) FindList.SelectedIndex = 0;
			else FindList.SelectedIndex++;
		}

		private void FindReplace_Click(object sender, RoutedEventArgs e)
		{
			Globals.IsFinding = true;

			FindReplaceResult frr = FindResults[FindList.SelectedIndex];
			Type valueType = frr.Item.GetType();
			if (valueType == typeof(Projects.Project) || valueType == typeof(DependencyProject))
			{
				var T = frr.Item as Projects.Project;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Namespace))
			{
				var T = frr.Item as Namespace;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Service))
			{
				var T = frr.Item as Service;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Operation))
			{
				var T = frr.Item as Operation;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(MethodParameter))
			{
				var T = frr.Item as MethodParameter;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Property))
			{
				var T = frr.Item as Property;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Projects.Data))
			{
				var T = frr.Item as Projects.Data;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(DataElement))
			{
				var T = frr.Item as DataElement;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Projects.Enum))
			{
				var T = frr.Item as Projects.Enum;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(EnumElement))
			{
				var T = frr.Item as EnumElement;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(ServiceBinding))
			{
				var T = frr.Item as ServiceBinding;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(BindingSecurity))
			{
				var T = frr.Item as BindingSecurity;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(Host))
			{
				var T = frr.Item as Host;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(HostEndpoint))
			{
				var T = frr.Item as HostEndpoint;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}
			if (valueType == typeof(HostBehavior))
			{
				var T = frr.Item as HostBehavior;
				if (T != null) T.Replace(new FindReplaceInfo(FindItems.Any, FindLocations.EntireSolution, FindValue.Text, FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked.Value, FindReplaceValue.Text), frr.Field);
			}

			Globals.IsFinding = false;
		}

		private void FindAll_Click(object sender, RoutedEventArgs e)
		{
			Globals.IsFinding = true;

			FindResults.Clear();
			var findInfo = new FindReplaceInfo((FindItems)FindItem.SelectedIndex, (FindLocations)FindLocation.SelectedIndex, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value);
			var results = new List<FindReplaceResult>();

			if (findInfo.Location == FindLocations.EntireSolution)
			{
				foreach (Projects.Project p in Globals.Projects)
					results.AddRange(p.FindReplace(findInfo));
			}
			if (findInfo.Location == FindLocations.CurrentProject)
			{
				results.AddRange(Project.FindReplace(findInfo));
			}

			foreach (FindReplaceResult frr in results)
				FindResults.Add(frr);
			FindResultsCount = FindResults.Count;
			if (FindResultsCount == 0) FindResultsCount = null;

			Globals.IsFinding = false;
		}

		private void FindReplaceAll_Click(object sender, RoutedEventArgs e)
		{
			Globals.IsFinding = true;

			FindResults.Clear();
			var findInfo = new FindReplaceInfo((FindItems)FindItem.SelectedIndex, (FindLocations)FindLocation.SelectedIndex, FindValue.Text, FindMatchCase.IsChecked != null && FindMatchCase.IsChecked.Value, FindUseRegex.IsChecked != null && FindUseRegex.IsChecked.Value, FindReplaceValue.Text);
			var results = new List<FindReplaceResult>();

			if (findInfo.Location == FindLocations.EntireSolution)
			{
				foreach (Projects.Project p in Globals.Projects)
					results.AddRange(p.FindReplace(findInfo));
			}
			if (findInfo.Location == FindLocations.CurrentProject)
			{
				results.AddRange(Project.FindReplace(findInfo));
			}

			foreach (FindReplaceResult frr in results)
				FindResults.Add(frr);
			FindResultsCount = FindResults.Count;
			if (FindResultsCount == 0) FindResultsCount = null;

			Globals.IsFinding = false;
		}

		private void FindList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//TODO: Add code to open the selected item when all screens are finished.
		}

		#endregion

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
			return new Button();
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
			if (item.GetType() == typeof(BindingSecurityBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(BindingSecurityBasicHTTP)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(BindingSecurityMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(BindingSecurityMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(BindingSecurityNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(BindingSecurityPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(BindingSecurityTCP)) return TCPTemplate;
			if (item.GetType() == typeof(BindingSecurityWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(BindingSecurityWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(BindingSecurityWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(BindingSecurityWSHTTP)) return WSHTTPTemplate;
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
			if (item == null) return BasicHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingBasicHTTP)) return BasicHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingBasicHTTPS)) return BasicHTTPSTemplate;
			if (item.GetType() == typeof(ServiceBindingNetHTTP)) return NetHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingNetHTTPS)) return NetHTTPSTemplate;
			if (item.GetType() == typeof(ServiceBindingMSMQ)) return MSMQTemplate;
			if (item.GetType() == typeof(ServiceBindingMSMQIntegration)) return MSMQIntegrationTemplate;
			if (item.GetType() == typeof(ServiceBindingNamedPipe)) return NamedPipeTemplate;
			if (item.GetType() == typeof(ServiceBindingPeerTCP)) return PeerTCPTemplate;
			if (item.GetType() == typeof(ServiceBindingTCP)) return TCPTemplate;
			if (item.GetType() == typeof(ServiceBindingWebHTTP)) return WebHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingWSDualHTTP)) return WSDualHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingWSFederationHTTP)) return WSFederationHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingWS2007FederationHTTP)) return WS2007FederationHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingWSHTTP)) return WSHTTPTemplate;
			if (item.GetType() == typeof(ServiceBindingWS2007HTTP)) return WS2007HTTPTemplate;
			return BasicHTTPTemplate;
		}
	}

	public class ProjectListOperationTemplateSelector : DataTemplateSelector
	{
		public DataTemplate MethodTemplate { get; set; }
		public DataTemplate PropertyTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item != null && item.GetType() == typeof(Method)) return MethodTemplate;
			if (item != null && item.GetType() == typeof(Property)) return PropertyTemplate;
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
			if (lt == CompileMessageSeverity.ERROR) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Error.png";
			if (lt == CompileMessageSeverity.WARN) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Warning.png";
			if (lt == CompileMessageSeverity.INFO) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Message.png";
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
			if (valueType == typeof(Projects.Project))
			{
				var T = value as Projects.Project;
				if (T != null) return string.IsNullOrEmpty(T.Name) ? "Project Settings" : T.Name;
			}
			if (valueType == typeof(ServiceBinding))
			{
				var T = value as ServiceBinding;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Service Binding";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(BindingSecurity))
			{
				var T = value as BindingSecurity;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Binding Security";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Host))
			{
				var T = value as Host;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Host";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Namespace))
			{
				var T = value as Namespace;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Namespace";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Service))
			{
				var T = value as Service;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Service";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Method))
			{
				var T = value as Operation;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Operation";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(MethodParameter))
			{
				var T = value as MethodParameter;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Operation Parameter";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Property))
			{
				var T = value as Property;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Property";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(Projects.Data))
			{
				var T = value as Projects.Data;
				if (T != null && string.IsNullOrEmpty(T.Name))
					return "Data";
				if (T != null) return T.Name;
			}
			if (valueType == typeof(DataElement))
			{
				var T = value as DataElement;
				if (T != null && string.IsNullOrEmpty(T.DataType.Name))
					return "Data Value";
				if (T != null) return T.DataType.Name;
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
			if (valueType == typeof(ServiceBinding))
			{
				var T = value as ServiceBinding;
				if (T != null && string.IsNullOrEmpty(T.Parent.Name))
					return "Service Binding";
				if (T != null) return T.Parent.Name;
			}
			if (valueType == typeof(BindingSecurity))
			{
				var T = value as BindingSecurity;
				if (T != null && string.IsNullOrEmpty(T.Parent.Name))
					return "Binding Security";
				if (T != null) return T.Parent.Name;
			}
			if (valueType == typeof(Host))
			{
				var T = value as Host;
				if (T != null && (T.Parent != null && string.IsNullOrEmpty(T.Parent.Name)))
					return "Host";
				if (T != null && T.Parent != null) return T.Parent.Name;
			}
			if (valueType == typeof(Namespace))
			{
				var T = value as Namespace;
				if (T != null && string.IsNullOrEmpty(T.Owner.Name))
					return "Namespace";
				if (T != null) return T.Owner.Name;
			}
			if (valueType == typeof(Service))
			{
				var T = value as Service;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Service";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(Projects.Data))
			{
				var T = value as Projects.Data;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Data";
				if (T != null) return T.Parent.Owner.Name;
			}
			if (valueType == typeof(Projects.Enum))
			{
				var T = value as Projects.Enum;
				if (T != null && string.IsNullOrEmpty(T.Parent.Owner.Name))
					return "Enum";
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
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Project.png";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.Project)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Project.png";
			if (valueType == typeof(DependencyProject)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/DependencyProject.png";
			if (valueType == typeof(Namespace)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Namespace.png";
			if (valueType == typeof(Service)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Service.png";
			if (valueType == typeof(Operation)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Operation.png";
			if (valueType == typeof(MethodParameter)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			if (valueType == typeof(Property)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			if (valueType == typeof(Projects.Data)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Data.png";
			if (valueType == typeof(DataElement)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			if (valueType == typeof(Projects.Enum)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Enum.png";
			if (valueType == typeof(EnumElement)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/X16/Project.png";
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