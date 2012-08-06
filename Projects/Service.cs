﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;

namespace WCFArchitect.Projects
{
	[DataContract()]
	public class Service : DataType
	{
		[DataMember()] public ObservableCollection<Operation> Operations { get { return (ObservableCollection<Operation>)GetValue(OperationsProperty); } set { SetValue(OperationsProperty, value); } }
		public static readonly DependencyProperty OperationsProperty = DependencyProperty.Register("Operations", typeof(ObservableCollection<Operation>), typeof(Service));

		[DataMember()] public bool IsCallback { get { return (bool)GetValue(IsCallbackProperty); } set { SetValue(IsCallbackProperty, value); } }
		public static readonly DependencyProperty IsCallbackProperty = DependencyProperty.Register("IsCallback", typeof(bool), typeof(Service));

		[DataMember()] public Service Callback { get { return (Service)GetValue(CallbackProperty); } set { SetValue(CallbackProperty, value); } }
		public static readonly DependencyProperty CallbackProperty = DependencyProperty.Register("Callback", typeof(Service), typeof(Service));

		[DataMember()] public System.Net.Security.ProtectionLevel ProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(Service));

		[DataMember()] public System.ServiceModel.SessionMode SessionMode { get { return (System.ServiceModel.SessionMode)GetValue(SessionModeProperty); } set { SetValue(SessionModeProperty, value); } }
		public static readonly DependencyProperty SessionModeProperty = DependencyProperty.Register("SessionMode", typeof(System.ServiceModel.SessionMode), typeof(Service));

		[DataMember()] public string ConfigurationName { get { return (string)GetValue(ConfigurationNameProperty); } set { SetValue(ConfigurationNameProperty, value); } }
		public static readonly DependencyProperty ConfigurationNameProperty = DependencyProperty.Register("ConfigurationName", typeof(string), typeof(Service));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Service));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Service));

		public bool IsFiltering { get { return (bool)GetValue(IsFilteringProperty); } set { SetValue(IsFilteringProperty, value); } }
		public static readonly DependencyProperty IsFilteringProperty = DependencyProperty.Register("IsFiltering", typeof(bool), typeof(Service));

		public bool IsFilterMatch { get { return (bool)GetValue(IsFilterMatchProperty); } set { SetValue(IsFilterMatchProperty, value); } }
		public static readonly DependencyProperty IsFilterMatchProperty = DependencyProperty.Register("IsFilterMatch", typeof(bool), typeof(Service));

		[DataMember()] public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(Service));

		public Service() : base()
		{
		}

		public Service(string Name, Namespace Parent) : base()
		{
			this.IsOpen = false;
			this.IsCallback = false;
			this.Operations = new ObservableCollection<Operation>();
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.ConfigurationName = "";
			this.Parent = Parent;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Service.IsSearchingProperty) return;
			if (e.Property == Service.IsSearchMatchProperty) return;
			if (e.Property == Service.IsFilteringProperty) return;
			if (e.Property == Service.IsFilterMatchProperty) return;
			if (e.Property == Service.IsTreeExpandedProperty) return;
		
			IsDirty = true;
		}
	
		public void Search(string Value)
		{
			if (Value != "")
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
				foreach (Property T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = T.Name.Contains(Value);
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
				foreach (Property T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public void SearchOperations(string Value)
		{
			if (Value != "")
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Operation T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public void SearchProperties(string Value)
		{
			if (Value != "")
			{
				foreach (Property T in Operations)
				{
					T.IsSearching = true;
					T.IsSearchMatch = true;
					if (T.Name != "" && T.Name.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) < 0) T.IsSearchMatch = false;
					if (IsSearchMatch == false) IsSearchMatch = T.IsSearchMatch;
				}
			}
			else
			{
				foreach (Property T in Operations)
				{
					T.IsSearching = false;
					T.IsSearchMatch = false;
				}
			}
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (ContractType.Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") if (Args.RegexSearch.IsMatch(ContractType.Name)) results.Add(new FindReplaceResult("Contract Name", ContractType.Name, Parent.Owner, this));
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
								if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (HasContractType == true) if (ContractType.Name != null && ContractType.Name != "") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
						}
					}
					IsActive = ia;
				}
			}

			foreach (Operation O in Operations)
				results.AddRange(O.FindReplace(Args));

			foreach (Property P in Operations)
				results.AddRange(P.FindReplace(Args));

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
							if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Microsoft.VisualBasic.Strings.Replace(ContractType.Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (HasContractType == true) if (Field == "Contract Name") ContractType.Name = Args.RegexSearch.Replace(ContractType.Name, Args.Replace);
					}
				}
				IsActive = ia;
			}
		}
	}

	[DataContract()]
	public abstract class Operation : DependencyObject
	{
		[DataMember()] protected Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		[DataMember()] public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(Operation));

		[DataMember()] public string ContractName { get { return (string)GetValue(ContractNameProperty); } set { SetValue(ContractNameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty ContractNameProperty = DependencyProperty.Register("ContractName", typeof(string), typeof(Operation));

		[DataMember()] public bool IsOneWay { get { return (bool)GetValue(IsOneWayProperty); } set { SetValue(IsOneWayProperty, value); } }
		public static readonly DependencyProperty IsOneWayProperty = DependencyProperty.Register("IsOneWay", typeof(bool), typeof(Operation));

		[DataMember()] public System.Net.Security.ProtectionLevel ProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(ProtectionLevelProperty); } set { SetValue(ProtectionLevelProperty, value); } }
		public static readonly DependencyProperty ProtectionLevelProperty = DependencyProperty.Register("ProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(Operation));

		[DataMember()] public DataType ReturnType { get { return (DataType)GetValue(ReturnTypeProperty); } set { SetValue(ReturnTypeProperty, value); } }
		public static readonly DependencyProperty ReturnTypeProperty = DependencyProperty.Register("ReturnType", typeof(DataType), typeof(Operation));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(Operation));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(Operation));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }
		public bool IsTreeExpanded { get { return false; } set { } }
		[DataMember()] public Service Owner { get; set; }

		public Operation() { }

		public Operation(string Name, Service Owner)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.ReturnType = new DataType(PrimitiveTypes.Void);
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.Owner = Owner;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
	
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Operation.IsSearchingProperty) return;
			if (e.Property == Operation.IsSearchMatchProperty) return;
			
			if (Owner != null)
				Owner.IsDirty = true;
		}

		public virtual List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this)); ;
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
							if (ContractName != null && ContractName != "") if (ContractName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						if (ContractName != null && ContractName != "") if (Args.RegexSearch.IsMatch(ContractName)) results.Add(new FindReplaceResult("Contract Name", ContractName, Owner.Parent.Owner, this)); ;
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (ContractName != null && ContractName != "") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary); ;
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (ContractName != null && ContractName != "") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
						}
					}
					Owner.IsActive = ia;
				}
			}

			return results;
		}

		public virtual void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Owner.IsActive;
				Owner.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Contract Name") ContractName = Microsoft.VisualBasic.Strings.Replace(ContractName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Contract Name") ContractName = Args.RegexSearch.Replace(ContractName, Args.Replace);
					}
				}
				Owner.IsActive = ia;
			}
		}
	}

	[DataContract()]
	public abstract class Method : Operation
	{
		[DataMember()] public bool UseAsyncPattern { get { return (bool)GetValue(UseAsyncPatternProperty); } set { SetValue(UseAsyncPatternProperty, value); } }
		public static readonly DependencyProperty UseAsyncPatternProperty = DependencyProperty.Register("UseAsyncPattern", typeof(bool), typeof(Method));

		[DataMember()] public bool UseAwaitPattern { get { return (bool)GetValue(UseAwaitPatternProperty); } set { SetValue(UseAwaitPatternProperty, value); } }
		public static readonly DependencyProperty UseAwaitPatternProperty = DependencyProperty.Register("UseAwaitPattern", typeof(bool), typeof(Method));

		[DataMember()] public bool IsInitiating { get { return (bool)GetValue(IsInitiatingProperty); } set { SetValue(IsInitiatingProperty, value); } }
		public static readonly DependencyProperty IsInitiatingProperty = DependencyProperty.Register("IsInitiating", typeof(bool), typeof(Method));

		[DataMember()] public bool IsTerminating { get { return (bool)GetValue(IsTerminatingProperty); } set { SetValue(IsTerminatingProperty, value); } }
		public static readonly DependencyProperty IsTerminatingProperty = DependencyProperty.Register("IsTerminating", typeof(bool), typeof(Method));

		[DataMember()] public ObservableCollection<MethodParameter> Parameters { get { return (ObservableCollection<MethodParameter>)GetValue(ParametersProperty); } set { SetValue(ParametersProperty, value); } }
		public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ObservableCollection<MethodParameter>), typeof(Method));

		public Method() : base() { }

		public Method(string Name, Service Owner) : base(Name, Owner)
		{
			this.Parameters = new ObservableCollection<MethodParameter>();
			this.ReturnType = new DataType(PrimitiveTypes.Void);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Operation.IsSearchingProperty) return;
			if (e.Property == Operation.IsSearchMatchProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}

		public override List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();
			results.AddRange(base.FindReplace(Args));
			foreach (MethodParameter mp in Parameters)
				results.AddRange(mp.FindReplace(Args));
			return results;
		}

		public override void Replace(FindReplaceInfo Args, string Field)
		{
			foreach (MethodParameter mp in Parameters)
				mp.Replace(Args, Field);
		}
	}

	[DataContract()]
	public class Property : Operation
	{
		[DataMember()] public bool IsReadOnly { get { return (bool)GetValue(IsReadOnlyProperty); } set { SetValue(IsReadOnlyProperty, value); } }
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Property));

		public Property() : base() { }

		public Property(string Name, Service Owner) : base(Name, Owner)
		{
			this.ReturnType = new DataType(PrimitiveTypes.String);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (e.Property == Operation.IsSearchingProperty) return;
			if (e.Property == Operation.IsSearchMatchProperty) return;

			if (Owner != null)
				Owner.IsDirty = true;
		}
	}

	[DataContract()]
	public class MethodParameter : DependencyObject
	{
		[DataMember()] private Guid id;
		public Guid ID { get { return id; } }

		[DataMember()] public DataType Type { get { return (DataType)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(DataType), typeof(MethodParameter));

		[DataMember()] public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, Helpers.RegExs.ReplaceSpaces.Replace(value == null ? "" : value, @"")); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(MethodParameter));

		[DataMember()] public bool IsHidden { get { return (bool)GetValue(IsHiddenProperty); } set { SetValue(IsHiddenProperty, value); } }
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(MethodParameter));

		//Internal Use
		public Service Owner { get; set; }

		public MethodParameter ()
		{
			this.id = Guid.NewGuid();
			this.Type = new DataType(PrimitiveTypes.String);
			IsHidden = false;
		}

		public MethodParameter(DataType Type, string Name, Service Owner)
		{
			this.id = Guid.NewGuid();
			this.Type = Type;
			this.Name = Name;
			IsHidden = false;
			this.Owner = Owner;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Service || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Owner.Parent.Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Owner.IsActive;
					Owner.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace); 
						}
					}
					Owner.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Owner.IsActive;
				Owner.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
					}
				}
				Owner.IsActive = ia;
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == OpenableDocument.IsDirtyProperty) return;
			if (Owner != null)
				Owner.IsDirty = true;
		}
	}
}