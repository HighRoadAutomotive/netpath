﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using WCFArchitect.Projects;

namespace WCFArchitect.Interface.Data
{
	internal partial class TypeSelector : ContentControl
	{
		private Projects.Project IntProject { get; set; }
		public object Project { get { return GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(object), typeof(TypeSelector), new PropertyMetadata(null, ProjectChangedCallback));

		private static void ProjectChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeSelector;
			if (t == null) return;
			if (e.NewValue == null)
			{
				t.IntProject = null;
				return;
			}

			if (e.NewValue.GetType() == typeof(Projects.Project))
			{
				var v = e.NewValue as Projects.Project;
				if (v != null) t.IntProject = v;
			}
			if (e.NewValue.GetType() == typeof(Projects.Method) || e.NewValue.GetType() == typeof(Projects.Property))
			{
				var v = e.NewValue as Operation;
				if (v != null) t.IntProject = v.Owner.Parent.Owner;
			}
			if (e.NewValue.GetType() == typeof(MethodParameter))
			{
				var v = e.NewValue as MethodParameter;
				if (v != null) t.IntProject = v.Owner.Parent.Owner;
			}
			if (e.NewValue.GetType() == typeof(DataType))
			{
				var v = e.NewValue as DataType;
				if (v != null) t.IntProject = v.Parent.Owner;
			}
			if (e.NewValue.GetType() == typeof(Projects.Data))
			{
				var v = e.NewValue as Projects.Data;
				if (v != null) t.IntProject = v.Parent.Owner;
			}
			if (e.NewValue.GetType() == typeof(DataElement))
			{
				var v = e.NewValue as DataElement;
				if (v != null) t.IntProject = v.Owner.Parent.Owner;
			}
		}

		public DataType OpenType { get { return (DataType)GetValue(TypeProperty); } set { IsSettingType = true; SetValue(TypeProperty, value); IsSettingType = false; } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("OpenType", typeof(DataType), typeof(TypeSelector), new PropertyMetadata(null, OpenTypeChangedCallback));

		private static void OpenTypeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var t = o as TypeSelector;
			if(t == null) return;

			t.IsSettingType = true;
			var bindingExpression = t.TypeName.GetBindingExpression(TextBox.TextProperty);
			if (bindingExpression != null) bindingExpression.UpdateTarget();
			t.IsSettingType = false;
			t.HasResults = false;
		}

		public bool HasResults { get { return (bool)GetValue(HasResultsProperty); } set { SetValue(HasResultsProperty, value); } }
		public static readonly DependencyProperty HasResultsProperty = DependencyProperty.Register("HasResults", typeof(bool), typeof(TypeSelector), new PropertyMetadata(false));

		public ObservableCollection<DataType> Results { get { return (ObservableCollection<DataType>)GetValue(ResultsProperty); } set { SetValue(ResultsProperty, value); HasResults = value != null && value.Count != 0; } }
		public static readonly DependencyProperty ResultsProperty = DependencyProperty.Register("Results", typeof(ObservableCollection<DataType>), typeof(TypeSelector));

		public string LabelText { get { return (string)GetValue(LabelTextProperty); } set { SetValue(LabelTextProperty, value); } }
		public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(TypeSelector), new PropertyMetadata(""));

		public bool IsValid { get { return (bool)GetValue(IsValidProperty); } set { SetValue(IsValidProperty, value); } }
		public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool), typeof(TypeSelector), new PropertyMetadata(false));

		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TypeSelector));
		public event RoutedEventHandler Selected { add { AddHandler(SelectedEvent, value); } remove { RemoveHandler(SelectedEvent, value); } }

		public static readonly RoutedEvent ValidationChangedEvent = EventManager.RegisterRoutedEvent("ValidationChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TypeSelector));
		public event RoutedEventHandler ValidationChanged { add { AddHandler(ValidationChangedEvent, value); } remove { RemoveHandler(ValidationChangedEvent, value); } }

		private bool IsInitializing { get; set; }
		private bool IsSettingType { get; set; }

		private bool SelectCollectionType { get; set; }
		private bool SelectDictionaryKey { get; set; }
		private bool SelectDictionaryType { get; set; }
		
		public TypeSelector()
		{
			IsInitializing = true;
			InitializeComponent();
			BG.DataContext = this;
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);

			IsInitializing = false;
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			HasResults = false;
		}

		private void TypeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(IsInitializing)
			{
				IsInitializing = false;
				return;
			}
			if (IsSettingType) return;
			if (IntProject == null) return;
			if (TypeName.Text == "")
			{
				OpenType = null;
				HasResults = false;
				return;
			}

			SelectCollectionType = false;
			SelectDictionaryKey = false;
			SelectDictionaryType = false;

			string sstr = TypeName.Text;

			if (OpenType != null && (OpenType.TypeMode == DataTypeMode.Collection || OpenType.TypeMode == DataTypeMode.Dictionary))
			{
				if (OpenType.TypeMode == DataTypeMode.Collection)
				{
					int go = TypeName.Text.IndexOf("<", StringComparison.Ordinal);	//Generic Open Bracket
					int gc = TypeName.Text.IndexOf(">", StringComparison.Ordinal);	//Generic Close Bracket
					int cp = TypeName.SelectionStart;								//Caret Position

					if (go < 0 || gc < 0) return;

					//Ensure that there are no comma's as collections can only have one generic type.
					if(TypeName.Text.IndexOf(",", StringComparison.Ordinal) > -1)
					{
						TypeName.Text = TypeName.Text.Replace(",", "");
						return;
					}

					//Edit the current collection type
					if (cp <= go)
						Results = new ObservableCollection<DataType>(IntProject.SearchTypes(sstr = TypeName.Text.Substring(0, go).Replace("<", "").Replace(">", "").Trim()));

					//Edit the collection generic type
					if (cp > go && cp <= gc)
					{
						Results = new ObservableCollection<DataType>(IntProject.SearchTypes(sstr = TypeName.Text.Substring(go, gc - go).Replace("<", "").Replace(">", "").Trim(), false));		//Search for results without collections, the compiler does not support nested collections.
						SelectCollectionType = true;
					}
				}
				else if (OpenType.TypeMode == DataTypeMode.Dictionary)
				{
					int go = TypeName.Text.IndexOf("<", StringComparison.Ordinal);	//Generic Open Bracket
					int gs = TypeName.Text.IndexOf(",", StringComparison.Ordinal);	//Generic Separator
					int gc = TypeName.Text.IndexOf(">", StringComparison.Ordinal);	//Generic Close Bracket
					int cp = TypeName.SelectionStart;								//Caret Position

					if (go < 0 || gs < 0 || gc < 0) return;

					//Edit the current dictionary type
					if (cp <= go)
						Results = new ObservableCollection<DataType>(IntProject.SearchTypes(sstr = TypeName.Text.Substring(0, go).Replace("<", "").Replace(",", "").Replace(">", "").Trim()));

					//Edit the dictionary key type
					if (cp > go && cp <= gs)
					{
						Results = new ObservableCollection<DataType>(IntProject.SearchTypes(sstr = TypeName.Text.Substring(go, gs - go).Replace("<", "").Replace(",", "").Replace(">", "").Trim(), false));		//Search for results without collections, the compiler does not support nested collections.
						SelectDictionaryKey = true;
					}

					//Edit the dictionary value type
					if (cp > gs && cp <= gc)
					{
						Results = new ObservableCollection<DataType>(IntProject.SearchTypes(sstr = TypeName.Text.Substring(gs, gc - gs).Replace("<", "").Replace(",", "").Replace(">", "").Trim(), false));		//Search for results without collections, the compiler does not support nested collections.
						SelectDictionaryType = true;
					}
				}
			}
			else
				Results = new ObservableCollection<DataType>(IntProject.SearchTypes(TypeName.Text.Replace("[", "").Replace("]", "")));

			if (Results.Count >= 1)
			{
				foreach (DataType dt in Results.Where(dt => dt.Name.Equals(sstr, StringComparison.CurrentCultureIgnoreCase)))
				{
					TypesList.SelectedItem = dt;
					SelectType();
					return;
				}
			}

			TypesList.SelectedIndex = 0;

			//Handle arrays
			if (OpenType != null && ((TypeName.Text.Contains("[") || TypeName.Text.Contains("]")) && (OpenType.TypeMode == DataTypeMode.Collection || OpenType.TypeMode == DataTypeMode.Dictionary)))
				TypeName.Text = TypeName.Text.Replace("[", "").Replace("]", "");

			if (OpenType != null && TypeName.Text.EndsWith("[]") && OpenType.TypeMode != DataTypeMode.Array)
				OpenType = new DataType(DataTypeMode.Array) {CollectionGenericType = OpenType};

		}

		private void TypeName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SelectType();
			}
			if (Results != null && Results.Count > 1)
			{
				if (TypesList.SelectedIndex < 0) TypesList.SelectedIndex = 0;
				if (e.Key == Key.Up)
				{
					TypesList.SelectedIndex--;
					if (TypesList.SelectedIndex < 0) TypesList.SelectedIndex = Results.Count - 1;
				}
				if (e.Key == Key.Down)
				{
					TypesList.SelectedIndex++;
					if (TypesList.SelectedIndex >= Results.Count) TypesList.SelectedIndex = 0;
				}
			}
		}

		private void TypesList_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SelectType();
			}
		}

		private void TypesList_MouseUp(object sender, MouseButtonEventArgs e)
		{
			var lbi = Globals.GetVisualParent<ListBoxItem>(VisualTreeHelper.HitTest(TypesList, e.GetPosition(TypesList)).VisualHit);
			TypesList.SelectedItem = lbi.Content;
			SelectType();
		}

		private void SelectType()
		{
			int tnss = -1;

			if (TypesList.SelectedItem == null) return;
			var sdt = TypesList.SelectedItem as DataType;
			if (sdt == null) return;
			if (sdt.Parent != null && sdt.Parent.Owner != null && sdt.Parent.Owner.ID != IntProject.ID) sdt = sdt.Reference() ?? sdt;		//If the owner project of sdt is NOT the same as IntProject, create a reference link to it.
			IsSettingType = true;

			if (OpenType != null)
			{
				if ((SelectCollectionType == false && SelectDictionaryKey == false && SelectDictionaryType == false) && (OpenType.TypeMode == DataTypeMode.Collection || OpenType.TypeMode == DataTypeMode.Dictionary))
				{
					if (OpenType.TypeMode == DataTypeMode.Collection && sdt.TypeMode == DataTypeMode.Collection)
					{
						DataType tdt = OpenType.CollectionGenericType;
						OpenType = new DataType(sdt.Name, sdt.TypeMode) { CollectionGenericType = tdt };
					}
					if (OpenType.TypeMode == DataTypeMode.Collection && sdt.TypeMode == DataTypeMode.Dictionary)
					{
						DataType tdt = OpenType.CollectionGenericType;
						OpenType = new DataType(sdt.Name, sdt.TypeMode) { DictionaryKeyGenericType = new DataType(PrimitiveTypes.Int), DictionaryValueGenericType = tdt };
					}
					if (OpenType.TypeMode == DataTypeMode.Dictionary && sdt.TypeMode == DataTypeMode.Collection)
					{
						DataType tdt = OpenType.DictionaryValueGenericType;
						OpenType = new DataType(sdt.Name, sdt.TypeMode) { CollectionGenericType = tdt };
					}
					if (OpenType.TypeMode == DataTypeMode.Dictionary && sdt.TypeMode == DataTypeMode.Dictionary)
					{
						DataType tdtk = OpenType.DictionaryKeyGenericType;
						DataType tdtv = OpenType.DictionaryValueGenericType;
						OpenType = new DataType(sdt.Name, sdt.TypeMode) { DictionaryKeyGenericType = tdtk, DictionaryValueGenericType = tdtv };
					}
				}
				else if ((SelectCollectionType || SelectDictionaryKey || SelectDictionaryType) && (OpenType.TypeMode == DataTypeMode.Collection || OpenType.TypeMode == DataTypeMode.Dictionary))
				{
					if (SelectCollectionType)
						OpenType.CollectionGenericType = sdt;
					if (SelectDictionaryKey)
					{
						OpenType.DictionaryKeyGenericType = sdt;
						tnss = TypeName.Text.IndexOf(">", StringComparison.OrdinalIgnoreCase);
					}
					if (SelectDictionaryType)
						OpenType.DictionaryValueGenericType = sdt;
				}
				else
					OpenType = sdt;
			}
			else
			{
				if (sdt.TypeMode == DataTypeMode.Collection || sdt.TypeMode == DataTypeMode.Dictionary)
				{
					OpenType = new DataType(sdt.Name, sdt.TypeMode);

					if (sdt.TypeMode == DataTypeMode.Collection) tnss = TypeName.Text.IndexOf("<", StringComparison.OrdinalIgnoreCase) + 1;
					if (sdt.TypeMode == DataTypeMode.Dictionary) tnss = TypeName.Text.IndexOf("<", StringComparison.OrdinalIgnoreCase) + 1;
					if (OpenType.TypeMode == DataTypeMode.Dictionary && SelectDictionaryKey) tnss = TypeName.Text.IndexOf(">", StringComparison.OrdinalIgnoreCase);
				}
				else
					OpenType = sdt;
			}

			var bindingExpression = TypeName.GetBindingExpression(TextBox.TextProperty);
			if (bindingExpression != null) bindingExpression.UpdateTarget();

			TypeName.Focus();
			TypeName.SelectionStart = tnss == -1 ? OpenType.TypeName.Length : tnss;

			HasResults = false;
			SelectCollectionType = false;
			SelectDictionaryKey = false;
			SelectDictionaryType = false;
			IsSettingType = false;

			if (OpenType.TypeMode == DataTypeMode.Collection && OpenType.CollectionGenericType != null)
			{
				IsValid = true;
				RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
			}
			else if (OpenType.TypeMode == DataTypeMode.Dictionary && OpenType.DictionaryKeyGenericType != null && OpenType.DictionaryValueGenericType != null)
			{
				IsValid = true;
				RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
			}
			else if (OpenType.TypeMode != DataTypeMode.Collection && OpenType.TypeMode != DataTypeMode.Dictionary)
			{
				IsValid = true;
				RaiseEvent(new RoutedEventArgs(SelectedEvent, this));
			}
			else
				IsValid = false;

			RaiseEvent(new RoutedEventArgs(ValidationChangedEvent, this));
		}

		private void ContentControl_GotFocus(object sender, RoutedEventArgs e)
		{
			TypeName.Focus();
		}

		private void ContentControl_LostFocus(object sender, RoutedEventArgs e)
		{
			if (PART_Popup.IsMouseOver)
			{
				e.Handled = true;
				return;
			}
			HasResults = false;
		}
	}

	[ValueConversion(typeof(DataType), typeof(BitmapImage))]
	public class DataTypeModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var lt = (DataType)value;
			if (lt.TypeMode == DataTypeMode.Class || lt.TypeMode == DataTypeMode.Struct) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Data.png"));
			if (lt.TypeMode == DataTypeMode.Enum) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Enum.png"));
			if (lt.TypeMode == DataTypeMode.Primitive) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Property.png"));
			if (lt.TypeMode == DataTypeMode.Array) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Collection.png"));
			if (lt.IsExternalType && (lt.TypeMode == DataTypeMode.Collection || lt.TypeMode == DataTypeMode.Dictionary)) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/Collection.png"));
			if (lt.IsExternalType) return new BitmapImage(new Uri("pack://application:,,,/WCFArchitect;component/Icons/X16/External.png"));
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}