using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RestForge.Modules
{
	public abstract class Document : ContentControl
	{
		public Guid ID { get; private set; }

		public string Title { get { return (string)GetValue(TitleProperty); } protected set { SetValue(TitlePropertyKey, value); } }
		private static readonly DependencyPropertyKey TitlePropertyKey = DependencyProperty.RegisterReadOnly("Title", typeof(string), typeof(Document), new PropertyMetadata(""));
		public static readonly DependencyProperty TitleProperty = TitlePropertyKey.DependencyProperty;

		public PathGeometry Icon { get { return (PathGeometry)GetValue(IconProperty); } protected set { SetValue(IconPropertyKey, value); } }
		private static readonly DependencyPropertyKey IconPropertyKey = DependencyProperty.RegisterReadOnly("Icon", typeof(PathGeometry), typeof(Document), new PropertyMetadata(null));
		public static readonly DependencyProperty IconProperty = IconPropertyKey.DependencyProperty;

		public bool CanClose { get { return (bool)GetValue(CanCloseProperty); } protected set { SetValue(CanClosePropertyKey, value); } }
		private static readonly DependencyPropertyKey CanClosePropertyKey = DependencyProperty.RegisterReadOnly("CanClose", typeof(bool), typeof(Document), new PropertyMetadata(true));
		public static readonly DependencyProperty CanCloseProperty = CanClosePropertyKey.DependencyProperty;

		public bool CanPin { get { return (bool)GetValue(CanPinProperty); } protected set { SetValue(CanPinPropertyKey, value); } }
		private static readonly DependencyPropertyKey CanPinPropertyKey = DependencyProperty.RegisterReadOnly("CanPin", typeof(bool), typeof(Document), new PropertyMetadata(false));
		public static readonly DependencyProperty CanPinProperty = CanPinPropertyKey.DependencyProperty;

		private object _content = null;			//This is used to store the visual tree when not displayed.

		public void EnableContentRendering()
		{
			this.Content = _content;
			_content = null;
		}

		public void DisableContentRendering()
		{
			_content = this.Content;
			this.Content = null;
		}
	}
}
