using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Options
{
	internal class License : DependencyObject
	{
		public string Key { get { return (string)GetValue(KeyProperty); } set { SetValue(KeyProperty, value); } }
		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(string), typeof(License), new UIPropertyMetadata("0000000000000000000000000"));

		public string Authorization { get { return (string)GetValue(AuthorizationProperty); } set { SetValue(AuthorizationProperty, value); } }
		public static readonly DependencyProperty AuthorizationProperty = DependencyProperty.Register("Authorization", typeof(string), typeof(License));
		
		//Timebomb Variables
		public DateTime LastNISTCheck { get; set; }
		public bool LastNISTCheckOK { get; set; }
   }
}