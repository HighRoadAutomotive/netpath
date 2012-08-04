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

namespace WCFArchitect.Interface.Enum
{
	internal partial class EnumElement : Grid
	{
		public Projects.EnumElement Data { get; set; }
		private Projects.EnumDataType DataType { get; set; }

		private Action<EnumElement, int> FocusUpAction;
		private Action<EnumElement, int> FocusDownAction;
		private Action<EnumElement> DeleteAction;

		public EnumElement(Projects.EnumElement Data, Projects.EnumDataType DataType, Action<EnumElement, int> FocusUp, Action<EnumElement, int> FocusDown, Action<EnumElement> Delete)
		{
			this.Data = Data;
			this.DataType = DataType;
			this.FocusUpAction = FocusUp;
			this.FocusDownAction = FocusDown;
			this.DeleteAction = Delete;

			InitializeComponent();
		}

		private void ElementName_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, 0);
			if (e.Key == Key.Down) FocusDownAction(this, 0);
		}

		private void ElementName_GotFocus(object sender, RoutedEventArgs e)
		{
			ValueName.Background = Brushes.White;
		}

		private void ElementName_LostFocus(object sender, RoutedEventArgs e)
		{
			ValueName.Background = Brushes.Transparent;
		}

		private void Value_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, 1);
			if (e.Key == Key.Down) FocusDownAction(this, 1);
		}

		private void Value_GotFocus(object sender, RoutedEventArgs e)
		{
			Value.Background = Brushes.White;
		}

		private void Value_LostFocus(object sender, RoutedEventArgs e)
		{
			Value.Background = Brushes.Transparent;
		}

		private void Value_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;

			if (DataType == Projects.EnumDataType.Int)
				try { Convert.ToInt32(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.SByte)
				try { Convert.ToSByte(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Byte)
				try { Convert.ToByte(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.UShort)
				try { Convert.ToUInt16(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Short)
				try { Convert.ToInt16(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.UInt)
				try { Convert.ToUInt32(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Long)
				try { Convert.ToInt64(Value.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.ULong)
				try { Convert.ToUInt64(Value.Text); }
				catch { e.IsValid = false; }
		}

		private void ContractValue_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up) FocusUpAction(this, 2);
			if (e.Key == Key.Down) FocusDownAction(this, 2);
		}

		private void ContractValue_GotFocus(object sender, RoutedEventArgs e)
		{
			ContractValue.Background = Brushes.White;
		}

		private void ContractValue_LostFocus(object sender, RoutedEventArgs e)
		{
			ContractValue.Background = Brushes.Transparent;
		}

		private void ContractValue_Validate(object sender, Prospective.Controls.ValidateEventArgs e)
		{
			e.IsValid = true;

		if (DataType == Projects.EnumDataType.Int)
				try { Convert.ToInt32(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.SByte)
				try { Convert.ToSByte(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Byte)
				try { Convert.ToByte(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.UShort)
				try { Convert.ToUInt16(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Short)
				try { Convert.ToInt16(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.UInt)
				try { Convert.ToUInt32(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.Long)
				try { Convert.ToInt64(ContractValue.Text); }
				catch { e.IsValid = false; }
			else if (DataType == Projects.EnumDataType.ULong)
				try { Convert.ToUInt64(ContractValue.Text); }
				catch { e.IsValid = false; }
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteAction(this);
		}
	}
}