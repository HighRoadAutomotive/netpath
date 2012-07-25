using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WCFArchitect.Interface
{
	[ValueConversion(typeof(object), typeof(double))]
	public class ScrollbarWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)12.0 + System.Convert.ToDouble((string)parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(double))]
	public class ElementOrderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int v = (int)value;
			if (v == -1) return "";
			return v.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string v = (string)value;
			if (v == "" || v == null) return -1;
			try { return System.Convert.ToInt32(v); }
			catch { return -1; }
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class ProjectTypeImage32Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "pack://application:,,,/WCFArchitect;component/Icons/X32/Project.png";
			Type valueType = value.GetType();
			if (valueType == typeof(Projects.Project)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/Project.png";
			if (valueType == typeof(Projects.DependencyProject)) return "pack://application:,,,/WCFArchitect;component/Icons/X32/DependencyProject.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/X32/Project.png";
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
			else if (valueType == typeof(Projects.DependencyProject)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/DependencyProject.png";
			else if (valueType == typeof(Projects.Namespace)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Namespace.png";
			else if (valueType == typeof(Projects.Service)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Service.png";
			else if (valueType == typeof(Projects.Operation)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Operation.png";
			else if (valueType == typeof(Projects.OperationParameter)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			else if (valueType == typeof(Projects.Property)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			else if (valueType == typeof(Projects.Data)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Data.png";
			else if (valueType == typeof(Projects.DataElement)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			else if (valueType == typeof(Projects.Enum)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Enum.png";
			else if (valueType == typeof(Projects.EnumElement)) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png";
			return "pack://application:,,,/WCFArchitect;component/Icons/X16/Project.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(C1.WPF.Dock), typeof(int))]
	public class TabPositionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			C1.WPF.Dock lt = (C1.WPF.Dock)value;
			if (lt == C1.WPF.Dock.Top) return 0;
			if (lt == C1.WPF.Dock.Right) return 1;
			if (lt == C1.WPF.Dock.Left) return 2;
			if (lt == C1.WPF.Dock.Bottom) return 3;
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int lt = (int)value;
			if (lt == 0) return C1.WPF.Dock.Top;
			if (lt == 1) return C1.WPF.Dock.Right;
			if (lt == 2) return C1.WPF.Dock.Left;
			if (lt == 3) return C1.WPF.Dock.Bottom;
			return C1.WPF.Dock.Top;
		}
	}

	public class ProjectItemHeaderTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DependencyHeader { get; set; }
		public DataTemplate ProjectHeader { get; set; }
		public DataTemplate NamespaceHeader { get; set; }
		public DataTemplate ServiceHeader { get; set; }
		public DataTemplate DataHeader { get; set; }
		public DataTemplate EnumHeader { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			Type valueType = item.GetType();

			if (valueType == typeof(Projects.DependencyProject)) return DependencyHeader;
			if (valueType == typeof(Projects.Project)) return ProjectHeader;
			if (valueType == typeof(Projects.Namespace)) return NamespaceHeader;
			if (valueType == typeof(Projects.Service)) return ServiceHeader;
			if (valueType == typeof(Projects.Data)) return DataHeader;
			if (valueType == typeof(Projects.Enum)) return EnumHeader;
			return ProjectHeader;
		}
	}

	public class ProjectNavigatorTemplateSelector : DataTemplateSelector
	{
		public DataTemplate BindingsHeader { get; set; }
		public DataTemplate SecurityHeader { get; set; }
		public DataTemplate HostsHeader { get; set; }
		public DataTemplate NamespaceHeader { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			Type valueType = item.GetType();

			if (valueType == typeof(ObservableCollection<Projects.ServiceBinding>)) return BindingsHeader;
			if (valueType == typeof(ObservableCollection<Projects.BindingSecurity>)) return SecurityHeader;
			if (valueType == typeof(ObservableCollection<Projects.Host>)) return HostsHeader;
			return null;
		}
	}

	public class ProjectNavigatorMultiBinding : IMultiValueConverter
	{
		public static bool ViewAll = false;
		public static bool ViewNamespaces = false;
		public static bool ViewServices = false;
		public static bool ViewData = false;
		public static bool ViewEnums = false;

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			CompositeCollection results = new CompositeCollection();

			ViewAll = Globals.MainScreen.SolutionNavigatorShowAll;
			ViewNamespaces = Globals.MainScreen.SolutionNavigatorShowNamespaces;
			ViewServices = Globals.MainScreen.SolutionNavigatorShowServices;
			ViewData = Globals.MainScreen.SolutionNavigatorShowData;
			ViewEnums = Globals.MainScreen.SolutionNavigatorShowEnums;

			foreach (object v in values)
			{
				if (v != null)
				{
					Type valueType = v.GetType();

					if (valueType == typeof(ObservableCollection<Projects.Namespace>))
					{
						CollectionContainer cc = new CollectionContainer();
						cc.Collection = v as ObservableCollection<Projects.Namespace>;
						results.Add(cc);
					}
					else if (valueType == typeof(ObservableCollection<Projects.Service>))
					{
						if (ViewAll == true || ViewServices == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.Service>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.Operation>))
					{
						if (ViewAll == true || ViewServices == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.Operation>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.Property>))
					{
						if (ViewAll == true || ViewServices == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.Property>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.Data>))
					{
						if (ViewAll == true || ViewData == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.Data>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.DataElement>))
					{
						if (ViewAll == true || ViewData == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.DataElement>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.Enum>))
					{
						if (ViewAll == true || ViewEnums == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.Enum>;
							results.Add(cc);
						}
					}
					else if (valueType == typeof(ObservableCollection<Projects.EnumElement>))
					{
						if (ViewAll == true || ViewEnums == true)
						{
							CollectionContainer cc = new CollectionContainer();
							cc.Collection = v as ObservableCollection<Projects.EnumElement>;
							results.Add(cc);
						}
					}
					else
					{
						results.Add(v);
					}
				}
			}

			return results;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	[ValueConversion(typeof(Compiler.CompileMessageSeverity), typeof(string))]
	public class CompileMessageSeverityImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			Compiler.CompileMessageSeverity lt = (Compiler.CompileMessageSeverity)value;
			if (lt == Compiler.CompileMessageSeverity.Error) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Error.png";
			if (lt == Compiler.CompileMessageSeverity.Warning) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Warning.png";
			if (lt == Compiler.CompileMessageSeverity.Message) return "pack://application:,,,/WCFArchitect;component/Icons/X16/Message.png";
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
				Projects.Project T = value as Projects.Project;
				if (T.Name == "" || T.Name == null)
					return "Project Settings";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding T = value as Projects.ServiceBinding;
				if (T.Name == "" || T.Name == null)
					return "Service Binding";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity T = value as Projects.BindingSecurity;
				if (T.Name == "" || T.Name == null)
					return "Binding Security";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Host))
			{
				Projects.Host T = value as Projects.Host;
				if (T.Name == "" || T.Name == null)
					return "Host";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace T = value as Projects.Namespace;
				if (T.Name == "" || T.Name == null)
					return "Namespace";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Service))
			{
				Projects.Service T = value as Projects.Service;
				if (T.Name == "" || T.Name == null)
					return "Service";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Operation))
			{
				Projects.Operation T = value as Projects.Operation;
				if (T.Name == "" || T.Name == null)
					return "Operation";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter T = value as Projects.OperationParameter;
				if (T.Name == "" || T.Name == null)
					return "Operation Parameter";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Property))
			{
				Projects.Property T = value as Projects.Property;
				if (T.Name == "" || T.Name == null)
					return "Property";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Data))
			{
				Projects.Data T = value as Projects.Data;
				if (T.Name == "" || T.Name == null)
					return "Data";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.DataElement))
			{
				Projects.DataElement T = value as Projects.DataElement;
				if (T.Name == "" || T.Name == null)
					return "Data Value";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum T = value as Projects.Enum;
				if (T.Name == "" || T.Name == null)
					return "Enum";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.EnumElement))
			{
				Projects.EnumElement T = value as Projects.EnumElement;
				if (T.Name == "" || T.Name == null)
					return "Enum Value";
				else
					return T.Name;
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
			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				Projects.Project T = value as Projects.Project;
				if (T.Name == "" || T.Name == null)
					return "Project Settings";
				else
					return T.Name;
			}
			if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding T = value as Projects.ServiceBinding;
				if (T.Parent.Name == "" || T.Parent.Name == null)
					return "Service Binding";
				else
					return T.Parent.Name;
			}
			if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity T = value as Projects.BindingSecurity;
				if (T.Parent.Name == "" || T.Parent.Name == null)
					return "Binding Security";
				else
					return T.Parent.Name;
			}
			if (valueType == typeof(Projects.Host))
			{
				Projects.Host T = value as Projects.Host;
				if (T.Parent.Name == "" || T.Parent.Name == null)
					return "Host";
				else
					return T.Parent.Name;
			}
			if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace T = value as Projects.Namespace;
				if (T.Owner.Name == "" || T.Owner.Name == null)
					return "Namespace";
				else
					return T.Owner.Name;
			}
			if (valueType == typeof(Projects.Service))
			{
				Projects.Service T = value as Projects.Service;
				if (T.Parent.Owner.Name == "" || T.Parent.Owner.Name == null)
					return "Service";
				else
					return T.Parent.Owner.Name;
			}
			if (valueType == typeof(Projects.Data))
			{
				Projects.Data T = value as Projects.Data;
				if (T.Parent.Owner.Name == "" || T.Parent.Owner.Name == null)
					return "Data";
				else
					return T.Parent.Owner.Name;
			}
			if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum T = value as Projects.Enum;
				if (T.Parent.Owner.Name == "" || T.Parent.Owner.Name == null)
					return "Enum";
				else
					return T.Parent.Owner.Name;
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class SolutionNavigatorContextMenuVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool v = (bool)value;
			if (v == true)
			{
#if STANDARD
				return Visibility.Collapsed;
#else
				return Visibility.Visible;
#endif
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InterfaceVersionVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool v = (bool)value;
			if (v == true)
			{
#if STANDARD
				return Visibility.Collapsed;
#else
				return Visibility.Visible;
#endif
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BoolVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool v = (bool)value;
			if (v == false)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(Visibility))]
	public class ProjectTypeVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return Visibility.Visible;
			Type valueType = value.GetType();

			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				Projects.Project v = value as Projects.Project;
				if (v.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding v = value as Projects.ServiceBinding;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity v = value as Projects.BindingSecurity;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Host))
			{
				Projects.Host v = value as Projects.Host;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.HostEndpoint))
			{
				Projects.HostEndpoint v = value as Projects.HostEndpoint;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.HostBehavior))
			{
				Projects.HostBehavior v = value as Projects.HostBehavior;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace v = value as Projects.Namespace;
				if (v.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Service))
			{
				Projects.Service v = value as Projects.Service;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Operation))
			{
				Projects.Operation v = value as Projects.Operation;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter v = value as Projects.OperationParameter;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Property))
			{
				Projects.Property v = value as Projects.Property;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Data))
			{
				Projects.Data v = value as Projects.Data;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.DataElement))
			{
				Projects.DataElement v = value as Projects.DataElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum v = value as Projects.Enum;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else if (valueType == typeof(Projects.EnumElement))
			{
				Projects.EnumElement v = value as Projects.EnumElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return Visibility.Collapsed;
#else
					return Visibility.Visible;
#endif
				}
				return Visibility.Visible;
			}
			else
				return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(bool))]
	public class ProjectTypeEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return true;
			Type valueType = value.GetType();

			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				Projects.Project v = value as Projects.Project;
				if (v.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding v = value as Projects.ServiceBinding;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity v = value as Projects.BindingSecurity;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Host))
			{
				Projects.Host v = value as Projects.Host;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.HostEndpoint))
			{
				Projects.HostEndpoint v = value as Projects.HostEndpoint;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.HostBehavior))
			{
				Projects.HostBehavior v = value as Projects.HostBehavior;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace v = value as Projects.Namespace;
				if (v.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Service))
			{
				Projects.Service v = value as Projects.Service;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Operation))
			{
				Projects.Operation v = value as Projects.Operation;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter v = value as Projects.OperationParameter;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Property))
			{
				Projects.Property v = value as Projects.Property;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Data))
			{
				Projects.Data v = value as Projects.Data;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.DataElement))
			{
				Projects.DataElement v = value as Projects.DataElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum v = value as Projects.Enum;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else if (valueType == typeof(Projects.EnumElement))
			{
				Projects.EnumElement v = value as Projects.EnumElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return false;
#else
					return true;
#endif
				}
				return true;
			}
			else
				return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(object), typeof(bool))]
	public class ProjectTypeReadOnlyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return true;
			Type valueType = value.GetType();

			if (valueType == typeof(Projects.Project) || valueType == typeof(Projects.DependencyProject))
			{
				Projects.Project v = value as Projects.Project;
				if (v.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.ServiceBinding))
			{
				Projects.ServiceBinding v = value as Projects.ServiceBinding;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.BindingSecurity))
			{
				Projects.BindingSecurity v = value as Projects.BindingSecurity;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Host))
			{
				Projects.Host v = value as Projects.Host;
				if (v.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.HostEndpoint))
			{
				Projects.HostEndpoint v = value as Projects.HostEndpoint;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.HostBehavior))
			{
				Projects.HostBehavior v = value as Projects.HostBehavior;
				if (v.Parent.Parent.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Namespace))
			{
				Projects.Namespace v = value as Projects.Namespace;
				if (v.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Service))
			{
				Projects.Service v = value as Projects.Service;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Operation))
			{
				Projects.Operation v = value as Projects.Operation;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.OperationParameter))
			{
				Projects.OperationParameter v = value as Projects.OperationParameter;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Property))
			{
				Projects.Property v = value as Projects.Property;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Data))
			{
				Projects.Data v = value as Projects.Data;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.DataElement))
			{
				Projects.DataElement v = value as Projects.DataElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.Enum))
			{
				Projects.Enum v = value as Projects.Enum;
				if (v.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else if (valueType == typeof(Projects.EnumElement))
			{
				Projects.EnumElement v = value as Projects.EnumElement;
				if (v.Owner.Parent.Owner.IsDependencyProject == true)
				{
#if STANDARD
					return true;
#else
					return false;
#endif
				}
				return false;
			}
			else
				return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}