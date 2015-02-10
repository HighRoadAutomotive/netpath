using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace NETPath.Projects.WebApi
{
	public class WebApiProject : Project
	{
		public WebApiNamespace Namespace { get { return (WebApiNamespace)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(WebApiNamespace), typeof(Project));

		public WebApiProject()
		{
		}

		public WebApiProject(string Name, string Path) : base(Name, Path)
		{
			Namespace = new WebApiNamespace(Helpers.RegExs.ReplaceSpaces.Replace(Name, "."), null, this) { URI = "http://tempuri.org/" };

			//Default Using Namespaces
			UsingNamespaces.AddRange(new []
								  {
									  new ProjectUsingNamespace("Microsoft.Owin.Hosting", true, true, true, true),
									  new ProjectUsingNamespace("System.Net.Http", true, true, true, true),
									  new ProjectUsingNamespace("System.Net.Http.Headers", true, true, true, true),
									  new ProjectUsingNamespace("System.Net.Http.Formatting", true, true, true, true),
									  new ProjectUsingNamespace("System.ServiceModel.Description", true, true, true, true),
									  new ProjectUsingNamespace("System.Web.Http", true, true, true, true),
									  new ProjectUsingNamespace("Owin", true, true, true, true),
								  });
			UsingNamespaces.Sort(a => a.Namespace, StringComparer.OrdinalIgnoreCase);
		}

		public override void SetSelected(DataType Type, Namespace nameSpace = null)
		{
			if (Type == null) return;
			var tt = Type.GetType();
			var ns = nameSpace as WebApiNamespace ?? Namespace;

			if (tt == typeof(Enum))
				foreach (var t in ns.Enums)
					t.IsSelected = false;

			if (tt == typeof(WebApiData))
				foreach (var t in ns.Data)
					t.IsSelected = false;

			if (tt == typeof(WebApiService))
				foreach (var t in ns.Services)
					t.IsSelected = false;

			foreach (var tn in ns.Children)
				SetSelected(Type, tn);

			Type.IsSelected = true;
		}

		public override List<DataType> SearchTypes(string Search, bool IncludeCollections = true, bool IncludeVoid = false, bool DataOnly = false, bool IncludeInheritable = false)
		{
			if (string.IsNullOrEmpty(Search)) return new List<DataType>();

			var results = new List<DataType>();

			if (IncludeInheritable) results.AddRange(from a in InheritableTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);

			if (DataOnly == false)
			{
				//Decide whether or not we want to include collections and dictionaries in the results.
				if (IncludeCollections) results.AddRange(from a in DefaultTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
				else results.AddRange(from a in DefaultTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 && a.TypeMode != DataTypeMode.Collection && a.TypeMode != DataTypeMode.Dictionary select a);
				//Search Externally defined types
				results.AddRange(from a in ExternalTypes where a.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0 select a);
			}
			results.AddRange(Namespace.SearchTypes(Search));

			if (IncludeInheritable) results.RemoveAll(a => a.GetType() == typeof(Enum));

			if (IncludeVoid && VoidType.Name.IndexOf(Search, StringComparison.CurrentCultureIgnoreCase) >= 0) results.Add(VoidType);

			return results;
		}
	}
}