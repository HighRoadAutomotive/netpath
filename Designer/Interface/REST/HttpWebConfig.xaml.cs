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

namespace NETPath.Interface.Rest
{
	internal partial class HttpWebConfig : Grid
	{
		public Projects.RestHTTPWebConfiguration HttpConfig { get { return (Projects.RestHTTPWebConfiguration)GetValue(HttpConfigProperty); } set { SetValue(HttpConfigProperty, value); } }
		public static readonly DependencyProperty HttpConfigProperty = DependencyProperty.Register("HttpConfig", typeof(Projects.RestHTTPWebConfiguration), typeof(HttpWebConfig));

		public HttpWebConfig(Projects.RestHTTPWebConfiguration HttpConfig)
		{
			InitializeComponent();

			this.HttpConfig = HttpConfig;
			DataContext = this.HttpConfig;
		}
	}
}