using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using C1.WPF.Docking;
using System.Globalization;
using System.Windows.Data;

namespace WCFArchitect.Themes
{
	public class C1DockTabItemWindow : C1DockTabItem
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211")] public static RoutedCommand CloseCommand = new RoutedCommand();

		public Projects.OpenableDocument DocumentData { get { return (Projects.OpenableDocument)GetValue(DocumentDataProperty); } set { SetValue(DocumentDataProperty, value); } }
		public static readonly DependencyProperty DocumentDataProperty = DependencyProperty.Register("DocumentData", typeof(Projects.OpenableDocument), typeof(C1DockTabItemWindow));

		public C1DockTabItemWindow(Projects.OpenableDocument Data)
		{
			CommandManager.RegisterClassCommandBinding(typeof(C1DockTabItemWindow), new CommandBinding(C1DockTabItemWindow.CloseCommand, OnCloseCommandExecuted));

			this.SetResourceReference(C1DockTabItemWindow.HeaderTemplateProperty, "ProjectItemHeaderTemplate");
			this.DocumentData = Data;
			this.Header = Data;

			Type dataType = Data.GetType();

			if (dataType == typeof(Projects.DependencyProject))
			{
				Interface.Project.Project NPW = new Interface.Project.Project(Data as Projects.DependencyProject);
#if STANDARD
				Data.IsLocked = true;
#endif
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
			else if (dataType == typeof(Projects.Project))
			{
				Interface.Project.Project NPW = new Interface.Project.Project(Data as Projects.Project);
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
			else if (dataType == typeof(Projects.Namespace))
			{
				Interface.Namespace.Namespace NPW = new Interface.Namespace.Namespace(Data as Projects.Namespace);
#if STANDARD
				if (NPW.Data.Owner.IsDependencyProject == true) Data.IsLocked = true;
#endif
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
			else if (dataType == typeof(Projects.Service))
			{
				Interface.Service.Service NPW = new Interface.Service.Service(Data as Projects.Service);
#if STANDARD
				if (NPW.Data.Parent.Owner.IsDependencyProject == true) Data.IsLocked = true;
#endif
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
			else if (dataType == typeof(Projects.Data))
			{
				Interface.Data.Data NPW = new Interface.Data.Data(Data as Projects.Data);
#if STANDARD
				if (NPW.PData.Parent.Owner.IsDependencyProject == true) Data.IsLocked = true;
#endif
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
			else if (dataType == typeof(Projects.Enum))
			{
				Interface.Enum.Enum NPW = new Interface.Enum.Enum(Data as Projects.Enum);
#if STANDARD
				if (NPW.Data.Parent.Owner.IsDependencyProject == true) Data.IsLocked = true;
#endif
				Content = NPW;
				MaxWidth = Globals.UserProfile.TabMaximumWidth;
			}
		}

		protected override void OnSelected(RoutedEventArgs e)
		{
			base.OnSelected(e);

			foreach (Projects.OpenableDocument OD in Globals.OpenDocuments)
				OD.IsActive = false;

			if(Globals.IsClosing == false && Globals.IsLoadSorting == false)
				DocumentData.IsActive = true;
		}

		private static void OnCloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			C1DockTabItemWindow ti = e.Source as C1DockTabItemWindow;
			if (ti == null) return;

			Globals.MainScreen.CloseTabWindow(ti);
		}
	}

	[ValueConversion(typeof(bool), typeof(C1.WPF.Docking.DockMode))]
	public class DockModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			C1.WPF.Docking.DockMode lt = (C1.WPF.Docking.DockMode)value;
			if (lt == C1.WPF.Docking.DockMode.Docked) return true;
			if (lt == C1.WPF.Docking.DockMode.Sliding) return false;
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool lt = (bool)value;
			if (lt == true) return C1.WPF.Docking.DockMode.Docked;
			if (lt == false) return C1.WPF.Docking.DockMode.Sliding;
			return true;
		}
	}

	public class DockingItemContainerSelector : StyleSelector
	{
		public Style DockTopStyle { get; set; }
		public Style DockBottomStyle { get; set; }
		public Style DockLeftStyle { get; set; }
		public Style DockRightStyle { get; set; }

		public override Style SelectStyle(object item, DependencyObject container)
		{
			C1DockTabControl tc = container as C1DockTabControl;
			if (tc == null) return DockTopStyle;

			if (tc.ShowHeader == true && tc.Dock == C1.WPF.Dock.Top) return DockTopStyle;
			if (tc.ShowHeader == true && tc.Dock == C1.WPF.Dock.Bottom) return DockBottomStyle;
			if (tc.ShowHeader == true && tc.Dock == C1.WPF.Dock.Left) return DockLeftStyle;
			if (tc.ShowHeader == true && tc.Dock == C1.WPF.Dock.Right) return DockRightStyle;

			return DockTopStyle;
		}
	}
}