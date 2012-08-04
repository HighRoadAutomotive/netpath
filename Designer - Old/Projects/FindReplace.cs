using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;

namespace WCFArchitect.Projects
{
	internal enum FindItems
	{
		Any,
		Project,
		Namespace,
		Service,
		Data,
		Enum,
	}

	internal enum FindLocations
	{
		EntireSolution,
		CurrentProject,
		OpenDocuments,
	}

	internal class FindReplaceInfo
	{
		// Find parameters
		public FindItems Items { get; protected set; }
		public FindLocations Location { get; protected set; }
		public string Search { get; protected set; }
		public bool MatchCase { get; protected set; }
		public bool UseRegex { get; protected set; }
		public Regex RegexSearch { get; protected set; }

		//Replace parameters
		public bool ReplaceAll { get; protected set; }
		public string Replace { get; protected set; }
		public bool IsDataType { get; protected set; }

		public FindReplaceInfo(FindItems Items, FindLocations Location, string Search, bool MatchCase, bool UseRegex, bool IsDataType = false)
		{
			this.Items = Items;
			this.Location = Location;
			this.Search = Search;
			this.MatchCase = MatchCase;
			this.UseRegex = UseRegex;
			if (UseRegex == true)
				if (MatchCase == true) RegexSearch = new Regex(this.Search, RegexOptions.None);
				else RegexSearch = new Regex(this.Search, RegexOptions.IgnoreCase);

			this.ReplaceAll = false;
			this.Replace = "";
			this.IsDataType = IsDataType;
		}

		public FindReplaceInfo(FindItems Items, FindLocations Location, string Search, bool MatchCase, bool UseRegex, string Replace, bool IsDataType = false)
		{
			this.Items = Items;
			this.Location = Location;
			this.Search = Search;
			this.MatchCase = MatchCase;
			this.UseRegex = UseRegex;
			if (UseRegex == true)
				if (MatchCase == true) RegexSearch = new Regex(this.Search, RegexOptions.None);
				else RegexSearch = new Regex(this.Search, RegexOptions.IgnoreCase);

			this.ReplaceAll = true;
			this.Replace = Replace;
			this.IsDataType = IsDataType;
		}
	}

	internal class FindReplaceResult : DependencyObject
	{
		public string Field { get { return (string)GetValue(FieldProperty); } set { SetValue(FieldProperty, value); } }
		public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(string), typeof(FindReplaceResult));

		public string Value { get { return (string)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(FindReplaceResult));

		public Project Project { get { return (Project)GetValue(ProjectProperty); } set { SetValue(ProjectProperty, value); } }
		public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register("Project", typeof(Project), typeof(FindReplaceResult));

		public object Item { get { return (object)GetValue(ItemProperty); } set { SetValue(ItemProperty, value); } }
		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(object), typeof(FindReplaceResult));

		public FindReplaceResult(string Field, string Value, Project Project, object Item)
		{
			this.Field = Field;
			this.Value = Value;
			this.Project = Project;
			this.Item = Item;
		}
	}
}