using System;
using System.Collections.ObjectModel;
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

namespace WCFArchitect.Interface.Project.Host
{
	internal partial class NewBehavior : Window
	{
		private class NewBehaviorItem
		{
			public string ImageSource { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public Type BehaviorType { get; set; }

			public NewBehaviorItem(string ImageSource, string Name, string Description, Type BehaviorType)
			{
				this.ImageSource = ImageSource;
				this.Name = Name;
				this.Description = Description;
				this.BehaviorType = BehaviorType;
			}
		}
		private ObservableCollection<NewBehaviorItem> ItemsList;

		public Projects.Host SelectedHost { get; set; }

		public NewBehavior(Projects.Host SelectedHost)
		{
			this.SelectedHost = SelectedHost;

			InitializeComponent();

			ItemsList = new ObservableCollection<NewBehaviorItem>();
			BehaviorItems.ItemsSource = ItemsList;

			ItemsList.Add(new NewBehaviorItem("pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorDebug.png", "Debug Behavior", "Defines a Debug behavior for the Service Host.", typeof(Projects.HostDebugBehavior)));
			ItemsList.Add(new NewBehaviorItem("pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorMetadata.png", "Metadata Behavior", "Defines Metadata for the Service Host.", typeof(Projects.HostMetadataBehavior)));
			ItemsList.Add(new NewBehaviorItem("pack://application:,,,/WCFArchitect;component/Icons/X32/BehaviorThrottling.png", "Throttling Behavior", "Defines a Throttling behavior for the Service Host.", typeof(Projects.HostThrottlingBehavior)));
		}

		private void Add_Click(object sender, RoutedEventArgs e)
		{
			if (BehaviorName.Text == "")
			{
				Prospective.Controls.MessageBox.Show("You must enter a name for the new behavior!", "Error Creating the Behavior", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				BehaviorName.Focus();
				return;
			}

			if (((NewBehaviorItem)BehaviorItems.SelectedItem).BehaviorType == typeof(Projects.HostDebugBehavior)) SelectedHost.Behaviors.Add(new Projects.HostDebugBehavior(BehaviorName.Text, SelectedHost));
			if (((NewBehaviorItem)BehaviorItems.SelectedItem).BehaviorType == typeof(Projects.HostMetadataBehavior)) SelectedHost.Behaviors.Add(new Projects.HostMetadataBehavior(BehaviorName.Text, SelectedHost));
			if (((NewBehaviorItem)BehaviorItems.SelectedItem).BehaviorType == typeof(Projects.HostThrottlingBehavior)) SelectedHost.Behaviors.Add(new Projects.HostThrottlingBehavior(BehaviorName.Text, SelectedHost));

			this.DialogResult = true;
			this.Close();
		}

		private void CloseWindow_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void BehaviorItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NewBehaviorItem t = (NewBehaviorItem)BehaviorItems.SelectedItem;
			BehaviorType.Text = t.Name;
			Description.Text = t.Description;
		}
	}
}