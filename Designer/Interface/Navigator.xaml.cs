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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")]
		public static readonly RoutedCommand CollapseCommand = new RoutedCommand();

		static Navigator()
		{
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(ChangeActivePageCommand, OnChangeActivePageCommandExecuted));
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(CollapseCommand, OnCollapseCommand));
			CommandManager.RegisterClassCommandBinding(typeof(Navigator), new CommandBinding(DeleteCommand, OnDeleteCommand));
		}

		private static void OnCollapseCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var s = sender as Navigator;
			if (s == null) return;

			Type dt = e.Parameter.GetType();
			if (dt == typeof(WebApiNamespace))
			{
				var t = e.Parameter as WebApiNamespace;
				if (t == null) return;
				t.Collapsed = !t.Collapsed;
			}
		}

		private static void OnDeleteCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var s = sender as Navigator;
			if (s == null) return;

			Type dt = e.Parameter.GetType();
			if (dt == typeof(Projects.Enum))
			{
				var t = e.Parameter as Projects.Enum;
				var tp = t?.Parent;
				if (t == null || tp == null) return;
				DialogService.ShowMessageDialog("NETPath", "Delete Enumeration?", "Are you sure you want to delete the '" + t.Declaration + "' enumeration?", new DialogAction("Yes", () => { s.OpenProjectItemBelow(t); tp.Enums.Remove(t); foreach (var x in t.Parent.Owner.ServerGenerationTargets) x.TargetTypes.Remove(t); foreach (var x in t.Parent.Owner.ClientGenerationTargets) x.TargetTypes.Remove(t); }, true), new DialogAction("No", false, true));
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

			if (Project.GetType() == typeof(WebApiProject))
				OpenProjectItem((Project as WebApiProject)?.Namespace.GetLastSelectedItem() ?? this.Project);
		}

		public void AddNewItem()
		{
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


			if (Item.GetType() == typeof(Projects.Enum))
				ActivePage = new Enum.Enum(Item as Projects.Enum);

			if (Item.GetType() == typeof(WebApiProject))
				ActivePage = new WebApi.Project(Item as WebApiProject);

			if (Item.GetType() == typeof(WebApiNamespace))
				ActivePage = new WebApi.Namespace.Namespace(Item as WebApiNamespace);

			if (Item.GetType() == typeof(WebApiService))
				ActivePage = new WebApi.Service.Service(Item as WebApiService);

			if (Item.GetType() == typeof(WebApiData))
				ActivePage = new WebApi.Data.Data(Item as WebApiData);

			if (Project.GetType() == typeof(WebApiProject))
				(Project as WebApiProject)?.Namespace.SetSelectedItem(Item);
		}

		private void OpenProjectItemBelow(OpenableDocument Item)
		{
			if (Item == null) return;
			Type dt = Item.GetType();
			if (dt == typeof(WebApiNamespace))
			{
				var t = Item as WebApiNamespace;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Children.IndexOf(t);
				int ic = tp.Children.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Children[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Children[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WebApiProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WebApiService))
			{
				var t = Item as WebApiService;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Services.IndexOf(t);
				int ic = tp.Services.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Services[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Services[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WebApiProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(WebApiData))
			{
				var t = Item as WebApiData;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Data.IndexOf(t);
				int ic = tp.Data.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Data[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Data[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WebApiProject)?.Namespace)) OpenProjectItem(t.Parent);
					else OpenProjectItem(Project);
				}
			}
			if (dt == typeof(Projects.Enum))
			{
				var t = Item as Projects.Enum;
				var tp = t?.Parent as WebApiNamespace;
				if (t == null || tp == null) return;
				int idx = tp.Enums.IndexOf(t);
				int ic = tp.Enums.Count - 1;
				if (idx < ic)
					OpenProjectItem(tp.Enums[idx + 1]);
				else if (idx == ic && tp.Services.Count > 0)
					OpenProjectItem(tp.Enums[0]);
				else
				{
					if (t.Parent != null && !Equals(t.Parent, (t.Parent.Owner as WebApiProject)?.Namespace)) OpenProjectItem(t.Parent);
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

	public class ProjectListItemTemplateSelector : DataTemplateSelector
	{
		public DataTemplate WebApiNamespaceTemplate { get; set; }
		public DataTemplate WebApiServiceTemplate { get; set; }
		public DataTemplate WebApiDataTemplate { get; set; }
		public DataTemplate EnumTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) return EnumTemplate;
			if (item.GetType() == typeof(Projects.Enum)) return EnumTemplate;
			if (item.GetType() == typeof(WebApiNamespace)) return WebApiNamespaceTemplate;
			if (item.GetType() == typeof(WebApiService)) return WebApiServiceTemplate;
			if (item.GetType() == typeof(WebApiData)) return WebApiDataTemplate;
			return EnumTemplate;
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
			if (valueType == typeof(WebApiProject) || valueType == typeof(WebApiProject)) return "pack://application:,,,/NETPath;component/Icons/X16/Project.png";
			if (valueType == typeof(WebApiNamespace) || valueType == typeof(WebApiNamespace)) return "pack://application:,,,/NETPath;component/Icons/X16/Namespace.png";
			if (valueType == typeof(WebApiService) || valueType == typeof(WebApiService)) return "pack://application:,,,/NETPath;component/Icons/X16/Service.png";
			//if (valueType == typeof(WebApiService)) return "pack://application:,,,/NETPath;component/Icons/X16/Rest.png";
			if (valueType == typeof(WebApiMethod)) return "pack://application:,,,/NETPath;component/Icons/X16/Method.png";
			if (valueType == typeof(WebApiMethodParameter) || valueType == typeof(WebApiRouteParameter) || valueType == typeof(WebApiMethodParameter)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
			if (valueType == typeof(WebApiData) || valueType == typeof(WebApiData)) return "pack://application:,,,/NETPath;component/Icons/X16/Data.png";
			if (valueType == typeof(WebApiDataElement) || valueType == typeof(WebApiDataElement)) return "pack://application:,,,/NETPath;component/Icons/X16/Property.png";
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

	[ValueConversion(typeof(CompileMessageSeverity), typeof(string))]
	public class NamespaceCollapsedStateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			var lt = (bool)value;
			return lt ? "pack://application:,,,/NETPath;component/Icons/X16/ListExpand.png" : "pack://application:,,,/NETPath;component/Icons/X16/ListCollapse.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}