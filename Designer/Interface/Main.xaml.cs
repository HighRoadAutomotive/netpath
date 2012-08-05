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
using System.Windows.Shapes;

namespace WCFArchitect.Interface
{
	public partial class Main : Window
	{
		//Message Box Properties
		public string MessageProject { get { return (string)GetValue(MessageProjectProperty); } set { SetValue(MessageProjectProperty, value); } }
		public static readonly DependencyProperty MessageProjectProperty = DependencyProperty.Register("MessageProject", typeof(string), typeof(Main));

		public string MessageCaption { get { return (string)GetValue(MessageCaptionProperty); } set { SetValue(MessageCaptionProperty, value); } }
		public static readonly DependencyProperty MessageCaptionProperty = DependencyProperty.Register("MessageCaption", typeof(string), typeof(Main));

		public string Message { get { return (string)GetValue(MessageProperty); } set { SetValue(MessageProperty, value); } }
		public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(Main));

		public ObservableCollection<Button> MessageActions { get { return (ObservableCollection<Button>)GetValue(MessageActionsProperty); } set { SetValue(MessageActionsProperty, value); } }
		public static readonly DependencyProperty MessageActionsProperty = DependencyProperty.Register("MessageActions", typeof(ObservableCollection<Button>), typeof(Main));

		public bool IsProcessingMessage { get; private set; }

		public Main()
		{
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0) Globals.WindowsLevel = Globals.WindowsVersion.WinVista;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1) Globals.WindowsLevel = Globals.WindowsVersion.WinSeven;
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2) Globals.WindowsLevel = Globals.WindowsVersion.WinEight;

			MessageActions = new ObservableCollection<Button>();

			InitializeComponent();
		}

		private void Main_SourceInitialized(object sender, EventArgs e)
		{
		}

		private void Main_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Maximized)
			{
				if (Globals.WindowsLevel == Globals.WindowsVersion.WinVista || Globals.WindowsLevel == Globals.WindowsVersion.WinSeven)
				{
					WindowBorder.Margin = new Thickness(5, 4, 5, 5);
				}
				else if (Globals.WindowsLevel == Globals.WindowsVersion.WinEight)
				{
					WindowBorder.Margin = new Thickness(8);
				}
				WindowBorder.BorderThickness = new Thickness(0);
				MaximizeWindow.Visibility = System.Windows.Visibility.Collapsed;
				RestoreWindow.Visibility = System.Windows.Visibility.Visible;
			}
			if (this.WindowState == System.Windows.WindowState.Normal)
			{
				WindowBorder.Margin = new Thickness(0);
				WindowBorder.BorderThickness = new Thickness(1);
				MaximizeWindow.Visibility = System.Windows.Visibility.Visible;
				RestoreWindow.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		private void Main_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
			}
		}

		public async void ProcessNextMessage()
		{
			if (Application.Current.Dispatcher.CheckAccess() == true)
				InternalProcessNextMessage();
			else
				await Application.Current.Dispatcher.InvokeAsync(() => InternalProcessNextMessage(), System.Windows.Threading.DispatcherPriority.Normal);
		}

		private void InternalProcessNextMessage()
		{
			if (IsProcessingMessage == true) return;
			IsProcessingMessage = true;
			MessageBox.Visibility = System.Windows.Visibility.Visible;
			MessageActions.Clear();

			if (Globals.Messages.Count > 0)
			{
				MessageBox next = null;
				if (Globals.Messages.TryDequeue(out next) == false)
				{
					IsProcessingMessage = false;
					MessageBox.Visibility = System.Windows.Visibility.Hidden;
					return;
				}

				if (next != null)
				{
					MessageProject = next.Origin.Name;
					MessageCaption = next.Caption;
					Message = next.Message;
					foreach (MessageAction a in next.Actions)
					{
						Button nb = new Button();
						nb.Content = a.Title;
						nb.Tag = a.Action;
						nb.Margin = new Thickness(5);
						nb.Click += MessageAction_Click;
						MessageActions.Add(nb);
					}
				}
			}
		}

		private void MessageAction_Click(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			Action a = b.Tag as Action;
			a.Invoke();

			IsProcessingMessage = false;
			MessageBox.Visibility = System.Windows.Visibility.Hidden;

			if (Globals.Messages.Count > 0)
				ProcessNextMessage();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Minimized;
		}

		private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Maximized;
		}

		private void RestoreWindow_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Normal;
		}

		private void SystemMenu_Click(object sender, RoutedEventArgs e)
		{
			SystemMenu.ContextMenu.PlacementTarget = SystemMenu;
			SystemMenu.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			SystemMenu.ContextMenu.IsOpen = true;
		}

		private void SystemMenuOpen_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuSave_Click(object sender, RoutedEventArgs e)
		{
			ScreenButtons.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void SystemMenuSaveAs_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuOptions_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuHelp_Click(object sender, RoutedEventArgs e)
		{
		}

		private void SystemMenuExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
