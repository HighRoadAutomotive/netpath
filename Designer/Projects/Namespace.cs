using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{

	internal class Namespace : OpenableDocument
	{
		public bool IsProjectRoot { get; private set; }

		public Guid ID { get { return (Guid)GetValue(IDProperty); } set { SetValue(IDProperty, value); } }
		public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(Guid), typeof(Namespace));

		public string Name { get { return (string)GetValue(NameProperty); } set { if (Globals.IsLoading == false) Globals.ReplaceDataType(Name, Parent == null ? Owner.Namespace.Name : Parent.FullName, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @""), Parent == null ? Owner.Namespace.Name : Parent.FullName); SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, "")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Namespace));

		public string FullName { get { return GetNamespaceString(); } set { SetValue(FullNameProperty, value); } }
		public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register("FullName", typeof(string), typeof(Namespace));

		public string URI { get { return (string)GetValue(URIProperty); } set { SetValue(URIProperty, value); } }
		public static readonly DependencyProperty URIProperty = DependencyProperty.Register("URI", typeof(string), typeof(Namespace));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(Namespace));

		public Project Owner { get { return (Project)GetValue(OwnerProperty); } set { SetValue(OwnerProperty, value); } }
		public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(Project), typeof(Namespace));

		public ObservableCollection<Namespace> Children { get { return (ObservableCollection<Namespace>)GetValue(ChildrenProperty); } set { SetValue(ChildrenProperty, value); } }
		public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(ObservableCollection<Namespace>), typeof(Namespace));

		public ObservableCollection<Enum> Enums { get { return (ObservableCollection<Enum>)GetValue(EnumsProperty); } set { SetValue(EnumsProperty, value); } }
		public static readonly DependencyProperty EnumsProperty = DependencyProperty.Register("Enums", typeof(ObservableCollection<Enum>), typeof(Namespace));

		public ObservableCollection<Data> Data { get { return (ObservableCollection<Data>)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<Data>), typeof(Namespace));

		public ObservableCollection<Service> Services { get { return (ObservableCollection<Service>)GetValue(ServicesProperty); } set { SetValue(ServicesProperty, value); } }
		public static readonly DependencyProperty ServicesProperty = DependencyProperty.Register("Services", typeof(ObservableCollection<Service>), typeof(Namespace));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Namespace));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Namespace));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Namespace));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Namespace));

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Namespace));

		private Namespace() : base()
		{
			this.Parent = null;
			this.Children = new ObservableCollection<Namespace>();
			this.Enums = new ObservableCollection<Enum>();
			this.Data = new ObservableCollection<Data>();
			this.Services = new ObservableCollection<Service>();
			this.UserOpenedList = new List<UserOpened>();
			this.URI = "";
		}

		public Namespace(string Name, Namespace Parent, Project Owner) : base()
		{
			this.ID = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			this.Owner = Owner;
			this.Children = new ObservableCollection<Namespace>();
			this.Enums = new ObservableCollection<Enum>();
			this.Data = new ObservableCollection<Data>();
			this.Services = new ObservableCollection<Service>();
			this.UserOpenedList = new List<UserOpened>();
			this.URI = "";
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Namespace.IsSearchingProperty) return;
			if (e.Property == Namespace.IsSearchMatchProperty) return;
			if (e.Property == Namespace.IsFilteringProperty) return;
			if (e.Property == Namespace.IsFilterMatchProperty) return;
			if (e.Property == Namespace.IsTreeExpandedProperty) return;

			IsDirty = true;
		}

		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (Service T in Services)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Namespace T in Children)
					T.Search(Value);
			}
			else
			{
				foreach (Service T in Services)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Data T in Data)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Enum T in Enums)
				{
					T.IsSearching = false;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					T.Search(Value);
				}
				foreach (Namespace T in Children)
					T.Search(Value);
			}
		}

		public void Filter(bool FilterAll, bool FilterNamespaces, bool FilterServices, bool FilterData, bool FilterEnums)
		{
			foreach (Service T in Services)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterServices == true) T.IsFilterMatch = true;
			}
			foreach (Data T in Data)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterData == true) T.IsFilterMatch = true;
			}
			foreach (Enum T in Enums)
			{
				T.IsFiltering = !FilterAll;
				T.IsFilterMatch = false;
				if (FilterEnums == true) T.IsFilterMatch = true;
			}
			foreach (Namespace T in Children)
				T.Filter(FilterAll, FilterNamespaces, FilterServices, FilterData, FilterEnums);
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Namespace || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
							if (FullName != null && FullName != "") if (FullName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
							if (URI != null && URI != "") if (URI.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner, this));
							if (FullName != null && FullName != "") if (FullName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
							if (URI != null && URI != "") if (URI.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("URI", URI, Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner, this));
						if (FullName != null && FullName != "") if (Args.RegexSearch.IsMatch(FullName)) results.Add(new FindReplaceResult("Full Name", FullName, Owner, this));
						if (URI != null && URI != "") if (Args.RegexSearch.IsMatch(URI)) results.Add(new FindReplaceResult("URI", URI, Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = IsActive;
					IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (URI != null && URI != "") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (URI != null && URI != "") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (URI != null && URI != "") URI = Args.RegexSearch.Replace(URI, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (Service S in Services)
				results.AddRange(S.FindReplace(Args));

			foreach (Data S in Data)
				results.AddRange(S.FindReplace(Args));

			foreach (Enum S in Enums)
				results.AddRange(S.FindReplace(Args));

			foreach (Namespace S in Children)
				results.AddRange(S.FindReplace(Args));

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = IsActive;
				IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "URI") URI = Microsoft.VisualBasic.Strings.Replace(URI, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "URI") URI = Args.RegexSearch.Replace(URI, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}

		public void ChangeOwners(Projects.Project NewOwner)
		{
			foreach (Projects.Service S in Services)
			{
				foreach (Projects.Host H in Owner.Hosts)
				{
					if (H.Service == S)
					{
						Owner.Hosts.Remove(H);
						NewOwner.Hosts.Add(H);
						foreach (Projects.HostEndpoint HE in H.Endpoints) HE.Binding = HE.Binding.Copy(H.CodeName, NewOwner);
						break;
					}
				}
			}

			foreach (Namespace N in Children)
				N.ChangeOwners(NewOwner);
		}

		private string GetNamespaceString()
		{
			if (IsProjectRoot == false)
			{
				string ns = "";

				Namespace tp = Parent;
				if (tp == null) tp = Owner.Namespace;

				string tns = "";
				while (tp != null)
				{
					tns = tns.Insert(0, tp.Name + ".");
					tp = tp.Parent;
				}
				ns += tns;

				ns += Name;

				return ns;
			}
			else
			{
				return Name;
			}
		}

		public void UpdateFullNamespace()
		{
			//A hack to support the fact that we couldn't change the namespace layout and root is not actually the root of anything. This MUST be changed in the V2 file structure reorg. The Updater must handle this.
			if (IsProjectRoot == false)
			{
				FullName = GetNamespaceString();
				foreach (Namespace N in Children)
					N.UpdateFullNamespace();
			}
			else //This is the hack.
			{
				FullName = Name;
			}
		}

		public bool HasServices()
		{
			bool HN = false;

			//A hack to support the fact that we couldn't change the namespace layout and root is not actually the root of anything. This MUST be changed in the V2 file structure reorg. The Updater must handle this.
			if (IsProjectRoot == false)
			{
				foreach (Namespace N in Children)
					if (N.HasServices() == true) HN = true;

				if (Services.Count > 0) HN = true;
			}
			else // This is the hack.
			{
				if (Owner.Services.Count > 0) HN = true;
			}
			return HN;
		}

		public void SetProjectRoot()
		{
			IsProjectRoot = true;
		}

		public void OpenItems()
		{
			if (IsOpen == true)
				Globals.MainScreen.OpenProjectItem(this);

			foreach (Enum E in Enums)
			{
				if (E.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(E);

				//Set owners
				E.Parent = this;
				foreach (EnumElement EE in E.Elements)
					EE.Owner = E;
			}

			foreach (Data D in Data)
			{
				if (D.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(D);

				//Set owners
				D.Parent = this;
				foreach (DataElement DE in D.Elements)
					DE.Owner = D;
			}

			foreach (Service S in Services)
			{
				if (S.IsOpen == true)
					Globals.MainScreen.OpenProjectItem(S);

				//Set owners
				S.Parent = this;
				foreach (Operation O in S.Operations)
				{
					O.Owner = S;
					foreach (OperationParameter OP in O.Parameters)
						OP.Owner = S;
				}
				foreach (Property P in S.Properties)
					P.Owner = S;
			}

			foreach (Namespace N in Children)
				N.OpenItems();
		}

		public void CloseItems()
		{
			Globals.MainScreen.CloseProjectItem(this);

			foreach (Enum E in Enums)
			{
				if (E.IsOpen == true)
					Globals.MainScreen.CloseProjectItem(E);
			}

			foreach (Data D in Data)
			{
				if (D.IsOpen == true)
					Globals.MainScreen.CloseProjectItem(D);
			}

			foreach (Service S in Services)
			{
				if (S.IsOpen == true)
					Globals.MainScreen.CloseProjectItem(S);
			}

			foreach (Namespace N in Children)
				N.CloseItems();
		}

		public bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (URI == null || URI == "")
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1000", "The '" + Name + "' namespace in the '" + Owner.Name + "' project does not have a URI. Namespaces MUST have a URI.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(URI) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS1001", "The URI '" + URI + "' for the '" + Name + "' namespace in the '" + Owner.Name + "' project is not a valid URI. Any associated services and data may not function properly.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			foreach (Enum EE in Enums)
				if (EE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Data DE in Data)
				if (DE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Service SE in Services)
				if (SE.VerifyCode(Compiler) == false) NoErrors = false;

			foreach (Namespace TN in Children)
				if (TN.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public string GenerateServerCode30(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateServerCode30(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}

		public string GenerateServerCode35(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateServerCode35(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}

		public string GenerateServerCode40(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Enums.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tEnumerations");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Enum EE in Enums)
					Code.AppendLine(EE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateServerCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateServerCode40(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}

		public string GenerateClientCode30(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode30(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateClientCode30(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}

		public string GenerateClientCode35(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode35(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateClientCode35(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}

		public string GenerateClientCode40(bool GenerateAssemblyCode, string ProjectName)
		{
			StringBuilder Code = new StringBuilder();

			Code.AppendFormat("namespace {0}{1}", FullName, Environment.NewLine);
			Code.AppendLine("{");

			if (Data.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tData Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Data DE in Data)
					Code.AppendLine(DE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			if (Services.Count > 0)
			{
				Code.AppendLine("\t/**************************************************************************");
				Code.AppendLine("\t*\tService Contracts");
				Code.AppendLine("\t**************************************************************************/");
				Code.AppendLine();
				foreach (Service SE in Services)
					Code.AppendLine(SE.GenerateClientCode40(GenerateAssemblyCode, ProjectName));
				Code.AppendLine();
			}

			Code.AppendLine("}");

			foreach (Namespace TN in Children)
				Code.AppendLine(TN.GenerateClientCode40(GenerateAssemblyCode, ProjectName));

			return Code.ToString();
		}
	}
}
