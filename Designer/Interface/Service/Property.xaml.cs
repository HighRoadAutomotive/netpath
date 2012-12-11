﻿using System;
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

namespace NETPath.Interface.Service
{
	internal partial class Property : Grid
	{
		public Projects.Property PropertyData { get { return (Projects.Property)GetValue(PropertyDataProperty); } set { SetValue(PropertyDataProperty, value); } }
		public static readonly DependencyProperty PropertyDataProperty = DependencyProperty.Register("PropertyData", typeof(Projects.Property), typeof(Property));

		public Projects.Project ServiceProject { get { return (Projects.Project)GetValue(ServiceProjectProperty); } set { SetValue(ServiceProjectProperty, value); } }
		public static readonly DependencyProperty ServiceProjectProperty = DependencyProperty.Register("ServiceProject", typeof(Projects.Project), typeof(Property));
		
		public Property()
		{
			InitializeComponent();

			DataContext = this;
		}

		public Property(Projects.Property Data)
		{
			PropertyData = Data;
			ServiceProject = Data.Owner.Parent.Owner;

			InitializeComponent();

			DataContext = this;
		}
	}
}