using RestForge.Projects;
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

namespace RestForge.Designer
{
	public partial class Main : Window
	{
		public ObservableCollection<BuildMessageXaml> GeneratorMessages { get { return (ObservableCollection<BuildMessageXaml>)GetValue(GeneratorMessagesProperty); } set { SetValue(GeneratorMessagesProperty, value); } }
		public static readonly DependencyProperty GeneratorMessagesProperty = DependencyProperty.Register("GeneratorMessages", typeof(ObservableCollection<BuildMessageXaml>), typeof(Main));


		public Main()
		{
			GeneratorMessages = new ObservableCollection<BuildMessageXaml>();
			InitializeComponent();
		}
	}

	public class BuildMessageXaml : DependencyObject
	{
		public BuildMessageSeverity Severity { get { return (BuildMessageSeverity)GetValue(SeverityProperty); } protected set { SetValue(SeverityPropertyKey, value); } }
		private static readonly DependencyPropertyKey SeverityPropertyKey = DependencyProperty.RegisterReadOnly("Severity", typeof(BuildMessageSeverity), typeof(BuildMessageXaml), new PropertyMetadata(BuildMessageSeverity.Info));
		public static readonly DependencyProperty SeverityProperty = SeverityPropertyKey.DependencyProperty;

		public string Code { get { return (string)GetValue(CodeProperty); } protected set { SetValue(CodePropertyKey, value); } }
		private static readonly DependencyPropertyKey CodePropertyKey = DependencyProperty.RegisterReadOnly("Code", typeof(string), typeof(BuildMessageXaml), new PropertyMetadata(""));
		public static readonly DependencyProperty CodeProperty = CodePropertyKey.DependencyProperty;

		public string Description { get { return (string)GetValue(DescriptionProperty); } protected set { SetValue(DescriptionPropertyKey, value); } }
		private static readonly DependencyPropertyKey DescriptionPropertyKey = DependencyProperty.RegisterReadOnly("Description", typeof(string), typeof(BuildMessageXaml), new PropertyMetadata(""));
		public static readonly DependencyProperty DescriptionProperty = DescriptionPropertyKey.DependencyProperty;

		public IProject Owner { get { return (IProject)GetValue(OwnerProperty); } protected set { SetValue(OwnerPropertyKey, value); } }
		private static readonly DependencyPropertyKey OwnerPropertyKey = DependencyProperty.RegisterReadOnly("Owner", typeof(IProject), typeof(BuildMessageXaml), new PropertyMetadata(null));
		public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;

		public string ErrorObject { get { return (string)GetValue(ErrorObjectProperty); } protected set { SetValue(ErrorObjectPropertyKey, value); } }
		private static readonly DependencyPropertyKey ErrorObjectPropertyKey = DependencyProperty.RegisterReadOnly("ErrorObject", typeof(string), typeof(BuildMessageXaml), new PropertyMetadata(""));
		public static readonly DependencyProperty ErrorObjectProperty = ErrorObjectPropertyKey.DependencyProperty;

		public BuildMessageXaml(BuildMessage message)
		{
			Severity = message.Severity;
			Code = message.Code;
			Description = message.Description;
			Owner = message.Owner;
			ErrorObject = message.ErrorObject;
		}
	}

	public class MessageSeverityStyleConverter : IValueConverter
	{
		public Style ErrorStyle { get; set; }

		public Style WarningStyle { get; set; }

		public Style InfoStyle { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var t = (BuildMessageSeverity) value;
			if (t == BuildMessageSeverity.Error) return ErrorStyle;
			if (t == BuildMessageSeverity.Warn) return WarningStyle;
			return InfoStyle;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
