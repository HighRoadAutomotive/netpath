using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace WCFArchitect.Projects
{

	internal enum BindingSecurityAlgorithmSuite
	{
		Default,
		Basic128,
		Basic128Rsa15,
		Basic128Sha256,
		Basic128Sha256Rsa15,
		Basic192,
		Basic192Rsa15,
		Basic192Sha256,
		Basic192Sha256Rsa15,
		Basic256,
		Basic256Rsa15,
		Basic256Sha256,
		Basic256Sha256Rsa15,
		TripleDes,
		TripleDesRsa15,
		TripleDesSha256,
		TripleDesSha256Rsa15
	}

	internal abstract class BindingSecurity : DependencyObject
	{
		protected Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		public string  Name { get { return (string )GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string ), typeof(BindingSecurity));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, value); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(BindingSecurity));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(BindingSecurity));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(BindingSecurity));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(BindingSecurity));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(BindingSecurity), new UIPropertyMetadata(false));

		public BindingSecurity() { }


		public virtual bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (CodeName == "" || CodeName == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS7000", "A binding security element in the '" + Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS7001", "The binding security element '" + Name + "' in the '" + Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}

			return NoErrors;
		}

		public List<FindReplaceResult> FindReplace(FindReplaceInfo Args)
		{
			List<FindReplaceResult> results = new List<FindReplaceResult>();

			if (Args.Items == FindItems.Project || Args.Items == FindItems.Any)
			{
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
					}
				}

				if (Args.ReplaceAll == true)
				{
					bool ia = Parent.IsActive;
					Parent.IsActive = true;
					if (Args.UseRegex == false)
					{
						if (Args.MatchCase == false)
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						}
					}
					Parent.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace (FindReplaceInfo Args, string Field)
		{
			if (Args.ReplaceAll == true)
			{
				bool ia = Parent.IsActive;
				Parent.IsActive = true;
				if (Args.UseRegex == false)
				{
					if (Args.MatchCase == false)
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
					}
				}
				Parent.IsActive = ia;
			}
		}

		public abstract BindingSecurity Copy(string HostName, Namespace Parent);

		public abstract string GenerateServerCode30();
		public abstract string GenerateServerCode35();
		public abstract string GenerateServerCode40();
		public abstract string GenerateServerCode35Client();
		public abstract string GenerateServerCode40Client();

		public abstract string GenerateClientCode30();
		public abstract string GenerateClientCode35();
		public abstract string GenerateClientCode40();
		public abstract string GenerateClientCode35Client();
		public abstract string GenerateClientCode40Client();
	}

	#region - BindingSecurityBasicHTTP Class -

	internal class BindingSecurityBasicHTTP : BindingSecurity
	{
		public System.ServiceModel.BasicHttpSecurityMode Mode { get { return (System.ServiceModel.BasicHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.BasicHttpSecurityMode), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.BasicHttpMessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.BasicHttpMessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.BasicHttpMessageCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityBasicHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityBasicHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityBasicHTTP));

		public BindingSecurityBasicHTTP() { }

		public BindingSecurityBasicHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.BasicHttpSecurityMode.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityBasicHTTP BD = new BindingSecurityBasicHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(BasicHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.BasicHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.BasicHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static BasicHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\tBasicHttpSecurity sec = new BasicHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.BasicHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.BasicHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(BasicHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.BasicHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.BasicHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static BasicHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\tBasicHttpSecurity sec = new BasicHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = BasicHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.BasicHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.BasicHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityWSHTTP Class -

	internal class BindingSecurityWSHTTP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode )GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityWSHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSHTTP));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(BindingSecurityWSHTTP));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityWSHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityWSHTTP));

		public BindingSecurityWSHTTP() { }

		public BindingSecurityWSHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256;
			this.MessageClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
			this.MessageEstablishSecurityContext = true;
			this.MessageNegotiateServiceCredential = true;
			this.TransportClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
			this.TransportProxyCredentialType = System.ServiceModel.HttpProxyCredentialType.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSHTTP BD = new BindingSecurityWSHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageEstablishSecurityContext = MessageEstablishSecurityContext;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = new WSHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = new WSHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityWSDualHTTP Class -

	internal class BindingSecurityWSDualHTTP : BindingSecurity
	{
		public System.ServiceModel.WSDualHttpSecurityMode Mode { get { return (System.ServiceModel.WSDualHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSDualHttpSecurityMode), typeof(BindingSecurityWSDualHTTP));
		
		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSHTTP));

		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSHTTP));

		public BindingSecurityWSDualHTTP() { }

		public BindingSecurityWSDualHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WSDualHttpSecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256;
			this.MessageClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
			this.MessageNegotiateServiceCredential = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSDualHTTP BD = new BindingSecurityWSDualHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSDualHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSDualHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSDualHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSDualHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = new WSDualHttpSecurity();");
			Code.AppendFormat("\t\t\tthis.Mode = WSDualHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSDualHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tthis.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSDualHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSDualHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSDualHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSDualHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = new WSDualHttpSecurity();");
			Code.AppendFormat("\t\t\tthis.Mode = WSDualHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSDualHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSDualHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tthis.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tthis.Message.ClientCredentialType = BasicHttpMessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.BasicHttpMessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tthis.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityWSFederationHTTP Class -

	internal class BindingSecurityWSFederationHTTP : BindingSecurity
	{
		public System.ServiceModel.WSFederationHttpSecurityMode Mode { get { return (System.ServiceModel.WSFederationHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WSFederationHttpSecurityMode), typeof(BindingSecurityWSFederationHTTP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityWSFederationHTTP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityWSFederationHTTP));

		public bool MessageEstablishSecurityContext { get { return (bool)GetValue(MessageEstablishSecurityContextProperty); } set { SetValue(MessageEstablishSecurityContextProperty, value); } }
		public static readonly DependencyProperty MessageEstablishSecurityContextProperty = DependencyProperty.Register("MessageEstablishSecurityContext", typeof(bool), typeof(BindingSecurityWSFederationHTTP));

		public System.IdentityModel.Tokens.SecurityKeyType MessageIssuedKeyType { get { return (System.IdentityModel.Tokens.SecurityKeyType)GetValue(MessageIssuedKeyTypeProperty); } set { SetValue(MessageIssuedKeyTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedKeyTypeProperty = DependencyProperty.Register("MessageIssuedKeyType", typeof(System.IdentityModel.Tokens.SecurityKeyType), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuedTokenType { get { return (string)GetValue(MessageIssuedTokenTypeProperty); } set { SetValue(MessageIssuedTokenTypeProperty, value); } }
		public static readonly DependencyProperty MessageIssuedTokenTypeProperty = DependencyProperty.Register("MessageIssuedTokenType", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuerAddress { get { return (string)GetValue(MessageIssuerAddressProperty); } set { SetValue(MessageIssuerAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerAddressProperty = DependencyProperty.Register("MessageIssuerAddress", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public string MessageIssuerMetadataAddress { get { return (string)GetValue(MessageIssuerMetadataAddressProperty); } set { SetValue(MessageIssuerMetadataAddressProperty, value); } }
		public static readonly DependencyProperty MessageIssuerMetadataAddressProperty = DependencyProperty.Register("MessageIssuerMetadataAddress", typeof(string), typeof(BindingSecurityWSFederationHTTP));
		
		public bool MessageNegotiateServiceCredential { get { return (bool)GetValue(MessageNegotiateServiceCredentialProperty); } set { SetValue(MessageNegotiateServiceCredentialProperty, value); } }
		public static readonly DependencyProperty MessageNegotiateServiceCredentialProperty = DependencyProperty.Register("MessageNegotiateServiceCredential", typeof(bool), typeof(BindingSecurityWSFederationHTTP));

		public BindingSecurityWSFederationHTTP() { }

		public BindingSecurityWSFederationHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WSFederationHttpSecurityMode.Message;
			this.MessageAlgorithmSuite = BindingSecurityAlgorithmSuite.Basic256Rsa15;
			this.MessageEstablishSecurityContext = true;
			this.MessageIssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.SymmetricKey;
			this.MessageNegotiateServiceCredential = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWSFederationHTTP BD = new BindingSecurityWSFederationHTTP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.MessageEstablishSecurityContext = MessageEstablishSecurityContext;
			BD.MessageIssuedKeyType = MessageIssuedKeyType;
			BD.MessageIssuedTokenType = MessageIssuedTokenType;
			BD.MessageIssuerAddress = MessageIssuerAddress;
			BD.MessageIssuerMetadataAddress = MessageIssuerMetadataAddress;
			BD.MessageNegotiateServiceCredential = MessageNegotiateServiceCredential;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSFederationHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSFederationHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSFederationHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), MessageIssuedKeyType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedTokenType = \"{0}\";", MessageIssuedTokenType);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerAddress = new EndpointAddress(\"{0}\");", MessageIssuerAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");", MessageIssuerMetadataAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSFederationHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = new WSFederationHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WSFederationHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSFederationHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), MessageIssuedKeyType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedTokenType = \"{0}\";", MessageIssuedTokenType);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerAddress = new EndpointAddress(\"{0}\");", MessageIssuerAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");", MessageIssuerMetadataAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WSFederationHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WSFederationHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSFederationHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), MessageIssuedKeyType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedTokenType = \"{0}\";", MessageIssuedTokenType);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerAddress = new EndpointAddress(\"{0}\");", MessageIssuerAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");", MessageIssuerMetadataAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static WSFederationHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = new WSFederationHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WSFederationHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WSFederationHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WSFederationHttpSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.EstablishSecurityContext = {0};", MessageEstablishSecurityContext == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedKeyType = System.IdentityModel.Tokens.SecurityKeyType.{0};", System.Enum.GetName(typeof(System.IdentityModel.Tokens.SecurityKeyType), MessageIssuedKeyType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuedTokenType = \"{0}\";", MessageIssuedTokenType);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerAddress = new EndpointAddress(\"{0}\");", MessageIssuerAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.IssuerMetadataAddress = new EndpointAddress(\"{0}\");", MessageIssuerMetadataAddress);
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.NegotiateServiceCredential = {0};", MessageNegotiateServiceCredential == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower());
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityTCP Class -

	internal class BindingSecurityTCP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityTCP));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityTCP));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityTCP));

		public System.ServiceModel.TcpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.TcpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.TcpClientCredentialType), typeof(BindingSecurityTCP));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityTCP));
		
		public BindingSecurityTCP() {}

		public BindingSecurityTCP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Transport;
			this.TransportClientCredentialType = System.ServiceModel.TcpClientCredentialType.Windows;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityTCP BD = new BindingSecurityTCP(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProtectionLevel = TransportProtectionLevel;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(NetTcpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();

			//Generate the basic CreateSecurity function.
			Code.AppendFormat("\t\tpublic static NetTcpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = new NetTcpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			//Generate the CreateSecurity function that supports ExtendedProtectionPolicy.
			Code.AppendFormat("\t\tpublic static NetTcpSecurity Create{0}Security(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = new NetTcpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(NetTcpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();

			//Generate the basic CreateSecurity function.
			Code.AppendFormat("\t\tpublic static NetTcpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = new NetTcpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			//Generate the CreateSecurity function that supports ExtendedProtectionPolicy.
			Code.AppendFormat("\t\tpublic static NetTcpSecurity Create{0}Security(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = new NetTcpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = SecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.SecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.SecurityMode.TransportWithMessageCredential)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = TcpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.TcpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityNamedPipe Class -

	internal class BindingSecurityNamedPipe : BindingSecurity
	{
		public System.ServiceModel.NetNamedPipeSecurityMode Mode { get { return (System.ServiceModel.NetNamedPipeSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetNamedPipeSecurityMode), typeof(BindingSecurityNamedPipe));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityNamedPipe));

		public BindingSecurityNamedPipe() { }

		public BindingSecurityNamedPipe(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.NetNamedPipeSecurityMode.Transport;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityNamedPipe BD = new BindingSecurityNamedPipe(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportProtectionLevel = TransportProtectionLevel;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode30();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetNamedPipeSecurity Create{0}Security(NetNamedPipeSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetNamedPipeSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetNamedPipeSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetNamedPipeSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = new NetNamedPipeSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetNamedPipeSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetNamedPipeSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode30();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetNamedPipeSecurity Create{0}Security(NetNamedPipeSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetNamedPipeSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetNamedPipeSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetNamedPipeSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = new NetNamedPipeSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetNamedPipeSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetNamedPipeSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetNamedPipeSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ProtectionLevel = ProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityMSMQ Class -

	internal class BindingSecurityMSMQ : BindingSecurity
	{
		public System.ServiceModel.NetMsmqSecurityMode Mode { get { return (System.ServiceModel.NetMsmqSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.NetMsmqSecurityMode), typeof(BindingSecurityMSMQ));

		public BindingSecurityAlgorithmSuite MessageAlgorithmSuite { get { return (BindingSecurityAlgorithmSuite)GetValue(MessageAlgorithmSuiteProperty); } set { SetValue(MessageAlgorithmSuiteProperty, value); } }
		public static DependencyProperty MessageAlgorithmSuiteProperty = DependencyProperty.Register("MessageAlgorithmSuite", typeof(BindingSecurityAlgorithmSuite), typeof(BindingSecurityMSMQ));

		public System.ServiceModel.MessageCredentialType MessageClientCredentialType { get { return (System.ServiceModel.MessageCredentialType)GetValue(MessageClientCredentialTypeProperty); } set { SetValue(MessageClientCredentialTypeProperty, value); } }
		public static DependencyProperty MessageClientCredentialTypeProperty = DependencyProperty.Register("MessageClientCredentialType", typeof(System.ServiceModel.MessageCredentialType), typeof(BindingSecurityMSMQ));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(BindingSecurityMSMQ));
		
		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(BindingSecurityMSMQ));
		
		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityMSMQ));
		
		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(BindingSecurityMSMQ));

		public BindingSecurityMSMQ() { }

		public BindingSecurityMSMQ(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.NetMsmqSecurityMode.Transport;
			this.TransportAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
			this.TransportEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.Aes;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			this.TransportSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha512;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityMSMQ BD = new BindingSecurityMSMQ(Name + HostName, Parent);
			BD.MessageAlgorithmSuite = MessageAlgorithmSuite;
			BD.MessageClientCredentialType = MessageClientCredentialType;
			BD.TransportAuthenticationMode = TransportAuthenticationMode;
			BD.TransportEncryptionAlgorithm = TransportEncryptionAlgorithm;
			BD.TransportProtectionLevel = TransportProtectionLevel;
			BD.TransportSecureHashAlgorithm = TransportSecureHashAlgorithm;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode30();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(NetMsmqSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetMsmqSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Both)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetMsmqSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = new NetMsmqSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetMsmqSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Both)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode30();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(NetMsmqSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetMsmqSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Both)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static NetMsmqSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = new NetMsmqSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.NetMsmqSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.NetMsmqSecurityMode.Message)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			else if (Mode == System.ServiceModel.NetMsmqSecurityMode.Both)
			{
				Code.AppendFormat("\t\t\tsec.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.{0};", System.Enum.GetName(typeof(BindingSecurityAlgorithmSuite), MessageAlgorithmSuite));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Message.ClientCredentialType = MessageCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.MessageCredentialType), MessageClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityPeerTCP Class -

	internal class BindingSecurityPeerTCP : BindingSecurity
	{
		public System.ServiceModel.SecurityMode Mode { get { return (System.ServiceModel.SecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.SecurityMode), typeof(BindingSecurityPeerTCP));

		public System.ServiceModel.PeerTransportCredentialType TransportClientCredentialType { get { return (System.ServiceModel.PeerTransportCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.PeerTransportCredentialType), typeof(BindingSecurityPeerTCP));

		public BindingSecurityPeerTCP() { }

		public BindingSecurityPeerTCP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.SecurityMode.Transport;
			this.TransportClientCredentialType = System.ServiceModel.PeerTransportCredentialType.Password;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityPeerTCP BD = new BindingSecurityPeerTCP(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(PeerSecuritySettings sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.CredentialType = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), TransportClientCredentialTypeProperty));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static PeerSecuritySettings Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = new PeerSecuritySettings();");
			Code.AppendFormat("\t\t\tsec.Mode = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.CredentialType = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), TransportClientCredentialTypeProperty));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(PeerSecuritySettings sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.CredentialType = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), TransportClientCredentialTypeProperty));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static PeerSecuritySettings Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = new PeerSecuritySettings();");
			Code.AppendFormat("\t\t\tsec.Mode = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.SecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.CredentialType = PeerTransportCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.PeerTransportCredentialType), TransportClientCredentialTypeProperty));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}

	#endregion

	#region - BindingSecurityWebHTTP Class -

	internal class BindingSecurityWebHTTP : BindingSecurity
	{
		public System.ServiceModel.WebHttpSecurityMode Mode { get { return (System.ServiceModel.WebHttpSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.WebHttpSecurityMode), typeof(BindingSecurityWebHTTP));

		public System.ServiceModel.HttpClientCredentialType TransportClientCredentialType { get { return (System.ServiceModel.HttpClientCredentialType)GetValue(TransportClientCredentialTypeProperty); } set { SetValue(TransportClientCredentialTypeProperty, value); } }
		public static DependencyProperty TransportClientCredentialTypeProperty = DependencyProperty.Register("TransportClientCredentialType", typeof(System.ServiceModel.HttpClientCredentialType), typeof(BindingSecurityWebHTTP));

		public System.ServiceModel.HttpProxyCredentialType TransportProxyCredentialType { get { return (System.ServiceModel.HttpProxyCredentialType)GetValue(TransportProxyCredentialTypeProperty); } set { SetValue(TransportProxyCredentialTypeProperty, value); } }
		public static DependencyProperty TransportProxyCredentialTypeProperty = DependencyProperty.Register("TransportProxyCredentialType", typeof(System.ServiceModel.HttpProxyCredentialType), typeof(BindingSecurityWebHTTP));

		public string TransportRealm { get { return (string)GetValue(TransportRealmProperty); } set { SetValue(TransportRealmProperty, value); } }
		public static readonly DependencyProperty TransportRealmProperty = DependencyProperty.Register("TransportRealm", typeof(string), typeof(BindingSecurityWebHTTP));

		public BindingSecurityWebHTTP() { }

		public BindingSecurityWebHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.WebHttpSecurityMode.None;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityWebHTTP BD = new BindingSecurityWebHTTP(Name + HostName, Parent);
			BD.Mode = Mode;
			BD.TransportClientCredentialType = TransportClientCredentialType;
			BD.TransportProxyCredentialType = TransportProxyCredentialType;
			BD.TransportRealm = TransportRealm;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return "";
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WebHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();

			//Generate the basic CreateSecurity function.
			Code.AppendFormat("\t\tpublic static WebHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = new WebHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			//Generate the CreateSecurity function that supports ExtendedPrivacyPolicy
			Code.AppendFormat("\t\tpublic static WebHttpSecurity Create{0}Security(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = new WebHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return "";
		}

		public override string GenerateClientCode30()
		{
			return "";
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(WebHttpSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();

			//Generate the basic CreateSecurity function.
			Code.AppendFormat("\t\tpublic static WebHttpSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = new WebHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			//Generate the CreateSecurity function that supports ExtendedPrivacyPolicy
			Code.AppendFormat("\t\tpublic static WebHttpSecurity Create{0}Security(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy Policy)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = new WebHttpSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = WebHttpSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.WebHttpSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.WebHttpSecurityMode.Transport)
			{
				Code.AppendLine("\t\t\tsec.Transport.ExtendedProtectionPolicy = Policy;");
				Code.AppendFormat("\t\t\tsec.Transport.ClientCredentialType = HttpClientCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpClientCredentialType), TransportClientCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.ProxyCredentialType = HttpProxyCredentialType.{0};", System.Enum.GetName(typeof(System.ServiceModel.HttpProxyCredentialType), TransportProxyCredentialType));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.Realm = \"{0}\";", TransportRealm);
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");

			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return "";
		}
	}

	#endregion

	#region - BindingSecurityMSMQIntegration Class -

	internal class BindingSecurityMSMQIntegration : BindingSecurity
	{
		public System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode Mode { get { return (System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqAuthenticationMode TransportAuthenticationMode { get { return (System.ServiceModel.MsmqAuthenticationMode)GetValue(TransportAuthenticationModeProperty); } set { SetValue(TransportAuthenticationModeProperty, value); } }
		public static readonly DependencyProperty TransportAuthenticationModeProperty = DependencyProperty.Register("TransportAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqEncryptionAlgorithm TransportEncryptionAlgorithm { get { return (System.ServiceModel.MsmqEncryptionAlgorithm)GetValue(TransportEncryptionAlgorithmProperty); } set { SetValue(TransportEncryptionAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportEncryptionAlgorithmProperty = DependencyProperty.Register("TransportEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), typeof(BindingSecurityMSMQIntegration));

		public System.Net.Security.ProtectionLevel TransportProtectionLevel { get { return (System.Net.Security.ProtectionLevel)GetValue(TransportProtectionLevelProperty); } set { SetValue(TransportProtectionLevelProperty, value); } }
		public static DependencyProperty TransportProtectionLevelProperty = DependencyProperty.Register("TransportProtectionLevel", typeof(System.Net.Security.ProtectionLevel), typeof(BindingSecurityMSMQIntegration));

		public System.ServiceModel.MsmqSecureHashAlgorithm TransportSecureHashAlgorithm { get { return (System.ServiceModel.MsmqSecureHashAlgorithm)GetValue(TransportSecureHashAlgorithmProperty); } set { SetValue(TransportSecureHashAlgorithmProperty, value); } }
		public static readonly DependencyProperty TransportSecureHashAlgorithmProperty = DependencyProperty.Register("TransportSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), typeof(BindingSecurityMSMQIntegration));

		public BindingSecurityMSMQIntegration() { }

		public BindingSecurityMSMQIntegration(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");
			this.Parent = Parent;

			this.Mode = System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport;
			this.TransportAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
			this.TransportEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.Aes;
			this.TransportProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
			this.TransportSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha512;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BindingSecurity.IsSearchingProperty) return;
			if (e.Property == BindingSecurity.IsSearchMatchProperty) return;
			if (e.Property == BindingSecurity.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			return NoErrors;
		}

		public override BindingSecurity Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			BindingSecurityMSMQIntegration BD = new BindingSecurityMSMQIntegration(Name + HostName, Parent);
			BD.TransportAuthenticationMode = TransportAuthenticationMode;
			BD.TransportEncryptionAlgorithm = TransportEncryptionAlgorithm;
			BD.TransportProtectionLevel = TransportProtectionLevel;
			BD.TransportSecureHashAlgorithm = TransportSecureHashAlgorithm;
			BD.Mode = Mode;

			Parent.Security.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = new System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client()
		{
			return GenerateServerCode35();
		}

		public override string GenerateServerCode40Client()
		{
			return GenerateServerCode40();
		}

		public override string GenerateClientCode30()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode35()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static void Set{0}Security(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec)", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40()
		{
			StringBuilder Code = new System.Text.StringBuilder();
			Code.AppendFormat("\t\tpublic static System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity Create{0}Security()", CodeName);
			Code.AppendLine();
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = new System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity();");
			Code.AppendFormat("\t\t\tsec.Mode = NetMsmqSecurityMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode), Mode));
			Code.AppendLine();
			if (Mode == System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurityMode.Transport)
			{
				Code.AppendFormat("\t\t\tsec.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqAuthenticationMode), TransportAuthenticationMode));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqEncryptionAlgorithm = MsmqEncryptionAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqEncryptionAlgorithm), TransportEncryptionAlgorithm));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqProtectionLevel = MsmqProtectionLevel.{0};", System.Enum.GetName(typeof(System.Net.Security.ProtectionLevel), TransportProtectionLevel));
				Code.AppendLine();
				Code.AppendFormat("\t\t\tsec.Transport.MsmqSecureHashAlgorithm = MsmqSecureHashAlgorithm.{0};", System.Enum.GetName(typeof(System.ServiceModel.MsmqSecureHashAlgorithm), TransportSecureHashAlgorithm));
				Code.AppendLine();
			}
			Code.AppendLine("\t\t\treturn sec;");
			Code.AppendLine("\t\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client()
		{
			return GenerateClientCode35();
		}

		public override string GenerateClientCode40Client()
		{
			return GenerateClientCode40();
		}
	}
	#endregion
}