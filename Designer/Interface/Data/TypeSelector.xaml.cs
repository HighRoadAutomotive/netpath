using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WCFArchitect.Interface.Data
{
	public partial class TypeSelector : ContentControl
	{
		public Projects.Project Project { get { return (Projects.Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Projects.Project), typeof(TypeSelector));

		public Projects.DataType Type { get { return (Projects.DataType)GetValue(TypeProperty); } set { IsSettingType = true; SetValue(TypeProperty, value); IsSettingType = false; } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Projects.DataType), typeof(TypeSelector));

		public bool HasResults { get { return (bool)GetValue(HasResultsProperty); } set { SetValue(HasResultsProperty, value); } }
		public static readonly DependencyProperty HasResultsProperty = DependencyProperty.Register("HasResults", typeof(bool), typeof(TypeSelector), new PropertyMetadata(false));

		public ObservableCollection<Projects.DataType> Results { get { return (ObservableCollection<Projects.DataType>)GetValue(ResultsProperty); } set { SetValue(ResultsProperty, value); if (value.Count == 0) { HasResults = false; } else { HasResults = true; } } }
		public static readonly DependencyProperty ResultsProperty = DependencyProperty.Register("Results", typeof(ObservableCollection<Projects.DataType>), typeof(TypeSelector));

		private bool IsSettingType { get; set; }

		private bool SelectCollectionType { get; set; }
		private bool SelectDictionaryKey { get; set; }
		private bool SelectDictionaryType { get; set; }

		public TypeSelector()
		{
			InitializeComponent();
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			HasResults = false;
		}

		private void TypeName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsSettingType == true) return;
			if (Project == null) return;
			if (TypeName.Text == "") return;

			SelectCollectionType = false;
			SelectDictionaryKey = false;
			SelectDictionaryType = false;


			if (Type != null && (Type.TypeMode == Projects.DataTypeMode.Collection || Type.TypeMode == Projects.DataTypeMode.Dictionary))
			{
				if (Type.TypeMode == Projects.DataTypeMode.Collection)
				{
					if (TypeName.SelectionStart <= TypeName.Text.IndexOf("<"))
					{
						Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text.Substring(0, TypeName.Text.IndexOf("<")).Replace("<", "").Replace(">", "")));
					}
					if (TypeName.SelectionStart > TypeName.Text.IndexOf("<") && TypeName.SelectionStart <= TypeName.Text.IndexOf(">"))
					{
						Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text.Substring(TypeName.Text.IndexOf("<"), TypeName.Text.IndexOf(">")).Replace("<", "").Replace(">", "")));
						SelectCollectionType = true;
					}
				}
				else if (Type.TypeMode == Projects.DataTypeMode.Dictionary)
				{
					if (TypeName.SelectionStart <= TypeName.Text.IndexOf("<"))
					{
						Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text.Substring(0, TypeName.Text.IndexOf("<")).Replace("<", "").Replace(",", "").Replace(">", "")));
					}
					if (TypeName.SelectionStart > TypeName.Text.IndexOf("<") && TypeName.SelectionStart <= TypeName.Text.IndexOf(","))
					{
						Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text.Substring(TypeName.Text.IndexOf("<"), TypeName.Text.IndexOf(",")).Replace("<", "").Replace(",", "").Replace(">", "")));
						SelectDictionaryKey = true;
					}
					if (TypeName.SelectionStart > TypeName.Text.IndexOf(",") && TypeName.SelectionStart <= TypeName.Text.IndexOf(">"))
					{
						Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text.Substring(TypeName.Text.IndexOf(","), TypeName.Text.IndexOf(">")).Replace("<", "").Replace(",", "").Replace(">", "")));
						SelectDictionaryType = true;
					}
				}
			}
			else
				Results = new ObservableCollection<Projects.DataType>(Project.SearchTypes(TypeName.Text));

			//Handle arrays
			if ((TypeName.Text.Contains("[") == true || TypeName.Text.Contains("]") == true) && (Type.TypeMode == Projects.DataTypeMode.Collection || Type.TypeMode == Projects.DataTypeMode.Dictionary))
				TypeName.Text = TypeName.Text.Replace("[", "").Replace("]", "");

			if (TypeName.Text.Contains("[]") == true)
			{
				Projects.DataType t = new Projects.DataType(Projects.DataTypeMode.Array);
				t.CollectionGenericType = Type;
				Type = t;
			}
		}

		private void TypeName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				TypesList.SelectedIndex = 0;
				TypesList_KeyUp(sender, e);
			}
		}

		private void TypesList_KeyUp(object sender, KeyEventArgs e)
		{
			if (TypesList.SelectedItem == null) return;
			Projects.DataType sdt = TypesList.SelectedItem as Projects.DataType;
			if (sdt == null) return;

			if (e.Key == Key.Enter)
			{
				if (Type != null)
				{
					if ((SelectCollectionType == false && SelectDictionaryKey == false && SelectDictionaryType == false) && (Type.TypeMode == Projects.DataTypeMode.Collection || Type.TypeMode == Projects.DataTypeMode.Dictionary))
					{
						if (Type.TypeMode == Projects.DataTypeMode.Collection && sdt.TypeMode == Projects.DataTypeMode.Collection)
						{
							Projects.DataType ndt = new Projects.DataType(sdt.Name, Projects.DataTypeMode.Collection);
							ndt.CollectionGenericType = Type.CollectionGenericType;
							Type = ndt;
						}
						if (Type.TypeMode == Projects.DataTypeMode.Collection && sdt.TypeMode == Projects.DataTypeMode.Dictionary)
						{
							Projects.DataType ndt = new Projects.DataType(sdt.Name, Projects.DataTypeMode.Collection);
							ndt.DictionaryKeyGenericType = new Projects.DataType(Projects.PrimitiveTypes.Int);
							ndt.DictionaryValueGenericType = Type.CollectionGenericType;
							Type = ndt;
						}
						if (Type.TypeMode == Projects.DataTypeMode.Dictionary && sdt.TypeMode == Projects.DataTypeMode.Collection)
						{
							Projects.DataType ndt = new Projects.DataType(sdt.Name, Projects.DataTypeMode.Collection);
							ndt.CollectionGenericType = Type.DictionaryValueGenericType;
							Type = ndt;
						}
						if (Type.TypeMode == Projects.DataTypeMode.Dictionary && sdt.TypeMode == Projects.DataTypeMode.Dictionary)
						{
							Projects.DataType ndt = new Projects.DataType(sdt.Name, Projects.DataTypeMode.Collection);
							ndt.DictionaryKeyGenericType = Type.DictionaryKeyGenericType;
							ndt.DictionaryValueGenericType = Type.DictionaryValueGenericType;
							Type = ndt;
						}
					}
					else if ((SelectCollectionType == true || SelectDictionaryKey == true || SelectDictionaryType == true) && (Type.TypeMode == Projects.DataTypeMode.Collection || Type.TypeMode == Projects.DataTypeMode.Dictionary))
					{
						if (SelectCollectionType == true)
							Type.CollectionGenericType = sdt;
						if (SelectDictionaryKey == true)
							Type.DictionaryKeyGenericType = sdt;
						if (SelectDictionaryType == true)
							Type.DictionaryValueGenericType = sdt;
					}
					else
						Type = sdt;
				}
				else
				{
					if (sdt.TypeMode == Projects.DataTypeMode.Collection || sdt.TypeMode == Projects.DataTypeMode.Dictionary)
						Type = new Projects.DataType(sdt.Name, sdt.TypeMode);
					else
						Type = sdt;
				}
			}

			SelectCollectionType = false;
			SelectDictionaryKey = false;
			SelectDictionaryType = false;
		}
	}
}