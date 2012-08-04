using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace WCFArchitect.Projects
{
	internal enum ServiceBindingTextEncoding
	{
		ASCII = 1,
		UTF7 = 2,
		UTF8 = 0,
		Unicode = 3,
		UTF32 = 4
	}

	internal enum ServiceBindingTransactionProtocol
	{
		Default = 0,
		OleTransactions = 1,
		WSAtomicTransaction11 = 2,
		WSAtomicTransactionOctober2004 = 3
	}

	internal abstract class ServiceBinding : DependencyObject
	{
		//General Binding Information
		protected Guid id = Guid.Empty;
		public Guid ID { get { return id; } }

		public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ServiceBinding));

		public string CodeName { get { return (string)GetValue(CodeNameProperty); } set { SetValue(CodeNameProperty, value); } }
		public static readonly DependencyProperty CodeNameProperty = DependencyProperty.Register("CodeName", typeof(string), typeof(ServiceBinding));

		//Basic Binding settings
		public string Namespace { get { return (string)GetValue(NamespaceProperty); } set { SetValue(NamespaceProperty, value); } }
		public static readonly DependencyProperty NamespaceProperty = DependencyProperty.Register("Namespace", typeof(string), typeof(ServiceBinding));

		public string EndpointAddress { get { return (string)GetValue(EndpointAddressProperty); } set { SetValue(EndpointAddressProperty, value); } }
		public static readonly DependencyProperty EndpointAddressProperty = DependencyProperty.Register("EndpointAddress", typeof(string), typeof(ServiceBinding));

		public string ListenAddress { get { return (string)GetValue(ListenAddressProperty); } set { SetValue(ListenAddressProperty, value); } }
		public static readonly DependencyProperty ListenAddressProperty = DependencyProperty.Register("ListenAddress", typeof(string), typeof(ServiceBinding));

		public TimeSpan CloseTimeout { get { return (TimeSpan)GetValue(CloseTimeoutProperty); } set { SetValue(CloseTimeoutProperty, value); } }
		public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(TimeSpan), typeof(ServiceBinding));

		public TimeSpan OpenTimeout { get { return (TimeSpan)GetValue(OpenTimeoutProperty); } set { SetValue(OpenTimeoutProperty, value); } }
		public static readonly DependencyProperty OpenTimeoutProperty = DependencyProperty.Register("OpenTimeout", typeof(TimeSpan), typeof(ServiceBinding));

		public TimeSpan ReceiveTimeout { get { return (TimeSpan)GetValue(ReceiveTimeoutProperty); } set { SetValue(ReceiveTimeoutProperty, value); } }
		public static readonly DependencyProperty ReceiveTimeoutProperty = DependencyProperty.Register("ReceiveTimeout", typeof(TimeSpan), typeof(ServiceBinding));

		public TimeSpan SendTimeout { get { return (TimeSpan)GetValue(SendTimeoutProperty); } set { SetValue(SendTimeoutProperty, value); } }
		public static readonly DependencyProperty SendTimeoutProperty = DependencyProperty.Register("SendTimeout", typeof(TimeSpan), typeof(ServiceBinding));

		public Namespace Parent { get { return (Namespace)GetValue(ParentProperty); } set { SetValue(ParentProperty, value); } }
		public static readonly DependencyProperty ParentProperty = DependencyProperty.Register("Parent", typeof(Namespace), typeof(ServiceBinding));

		//Internal Use - Searching / Filtering
		public bool IsSearching { get { return (bool)GetValue(IsSearchingProperty); } set { SetValue(IsSearchingProperty, value); } }
		public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register("IsSearching", typeof(bool), typeof(ServiceBinding));

		public bool IsSearchMatch { get { return (bool)GetValue(IsSearchMatchProperty); } set { SetValue(IsSearchMatchProperty, value); } }
		public static readonly DependencyProperty IsSearchMatchProperty = DependencyProperty.Register("IsSearchMatch", typeof(bool), typeof(ServiceBinding));

		public bool IsFiltering { get { return false; } set { } }
		public bool IsFilterMatch { get { return false; } set { } }

		public bool IsTreeExpanded { get { return (bool)GetValue(IsTreeExpandedProperty); } set { SetValue(IsTreeExpandedProperty, value); } }
		public static readonly DependencyProperty IsTreeExpandedProperty = DependencyProperty.Register("IsTreeExpanded", typeof(bool), typeof(ServiceBinding), new UIPropertyMetadata(false));

		public ServiceBinding() { }

		public virtual bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (CodeName == "" || CodeName == null)
			{
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6000", "A binding in the '" + Parent.Name + "' project has a blank Code Name. A Code Name MUST be specified.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
				NoErrors = false;
			}
			else
				if (Helpers.RegExs.MatchCodeName.IsMatch(CodeName) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6001", "The binding '" + Name + "' in the '" + Parent.Name + "' project contains invalid characters in the Code Name.", WCFArchitect.Compiler.CompileMessageSeverity.Error, Parent, this, this.GetType()));
					NoErrors = false;
				}
			if (Namespace == "" || Namespace == null) { }
			else
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(Namespace) == false)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6002", "The Namespace '" + Namespace + "' for the '" + Name + "' binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

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
							if (Name != null && Name != "") if (Namespace.IndexOf(Args.Search, StringComparison.InvariantCultureIgnoreCase) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") if (Name.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
							if (CodeName != null && CodeName != "") if (CodeName.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
							if (Namespace != null && Namespace != "") if (Namespace.IndexOf(Args.Search) >= 0) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Name != null && Name != "") if (Args.RegexSearch.IsMatch(Name)) results.Add(new FindReplaceResult("Name", Name, Parent.Owner, this));
						if (CodeName != null && CodeName != "") if (Args.RegexSearch.IsMatch(CodeName)) results.Add(new FindReplaceResult("Code Name", CodeName, Parent.Owner, this));
						if (Namespace != null && Namespace != "") if (Args.RegexSearch.IsMatch(Namespace)) results.Add(new FindReplaceResult("Namespace", Namespace, Parent.Owner, this));
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
								if (Namespace != null && Namespace != "") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
							}
						}
						else
						{
							if (Args.IsDataType == false)
							{
								if (Name != null && Name != "") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (CodeName != null && CodeName != "") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
								if (Namespace != null && Namespace != "") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							}
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Name != null && Name != "") Name = Args.RegexSearch.Replace(Name, Args.Replace);
							if (CodeName != null && CodeName != "") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
							if (Namespace != null && Namespace != "") Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
						}
					}
					Parent.IsActive = ia;
				}
			}

			return results;
		}

		public void Replace(FindReplaceInfo Args, string Field)
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
							if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Text);
						}
					}
					else
					{
						if (Args.IsDataType == false)
						{
							if (Field == "Name") Name = Microsoft.VisualBasic.Strings.Replace(Name, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Code Name") CodeName = Microsoft.VisualBasic.Strings.Replace(CodeName, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
							if (Field == "Namespace") Namespace = Microsoft.VisualBasic.Strings.Replace(Namespace, Args.Search, Args.Replace, 1, -1, Microsoft.VisualBasic.CompareMethod.Binary);
						}
					}
				}
				else
				{
					if (Args.IsDataType == false)
					{
						if (Field == "Name") Name = Args.RegexSearch.Replace(Name, Args.Replace);
						if (Field == "Code Name") CodeName = Args.RegexSearch.Replace(CodeName, Args.Replace);
						if (Field == "Namespace") Namespace = Args.RegexSearch.Replace(Namespace, Args.Replace);
					}
				}
				Parent.IsActive = ia;
			}
		}

		public abstract ServiceBinding Copy(string HostName, Namespace Parent);

		public abstract string GenerateServerCode30(string ProjectName);
		public abstract string GenerateServerCode35(string ProjectName);
		public abstract string GenerateServerCode40(string ProjectName);
		public abstract string GenerateServerCode35Client(string ProjectName);
		public abstract string GenerateServerCode40Client(string ProjectName);

		public abstract string GenerateClientCode30(string ProjectName);
		public abstract string GenerateClientCode35(string ProjectName);
		public abstract string GenerateClientCode40(string ProjectName);
		public abstract string GenerateClientCode35Client(string ProjectName);
		public abstract string GenerateClientCode40Client(string ProjectName);
	}

	#region  - ServiceBindingBasicHTTP Class -

	internal class ServiceBindingBasicHTTP : ServiceBinding
	{
		public BindingSecurityBasicHTTP Security { get { return (BindingSecurityBasicHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityBasicHTTP), typeof(ServiceBindingBasicHTTP));
				
		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingBasicHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingBasicHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingBasicHTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingBasicHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingBasicHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingBasicHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingBasicHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingBasicHTTP));

		public ServiceBindingBasicHTTP() { }

		public ServiceBindingBasicHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.AllowCookies = false;
			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxBufferSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransferMode = System.ServiceModel.TransferMode.Buffered;
			this.UseDefaultWebProxy = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6003", "The Security for the '" + Name + "' Basic HTTP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6004", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' Basic HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingBasicHTTP BD = new ServiceBindingBasicHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityBasicHTTP)Security.Copy(HostName, Parent);

			BD.AllowCookies = AllowCookies;
			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxBufferSize = MaxBufferSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.TransferMode = TransferMode;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : BasicHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), MessageEncoding), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : BasicHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), MessageEncoding), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : BasicHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tBasicHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), MessageEncoding), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : BasicHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(BasicHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MessageEncoding = WSMessageEncoding.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.WSMessageEncoding), MessageEncoding), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWSHTTP Class -

	internal class ServiceBindingWSHTTP : ServiceBinding
	{
		public BindingSecurityWSHTTP Security { get { return (BindingSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSHTTP), typeof(ServiceBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWSHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWSHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWSHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWSHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWSHTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWSHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWSHTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWSHTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWSHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWSHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWSHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWSHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWSHTTP));

		public ServiceBindingWSHTTP() { }

		public ServiceBindingWSHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.AllowCookies = false;
			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.ReliableSessionEnabled = false;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransactionFlow = true;
			this.UseDefaultWebProxy = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6005", "The Security for the '" + Name + "' WS HTTP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6006", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' WS HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWSHTTP BD = new ServiceBindingWSHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWSHTTP)Security.Copy(HostName, Parent);

			BD.AllowCookies = AllowCookies;
			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.ReliableSessionEnabled = ReliableSessionEnabled;
			BD.ReliableSessionInactivityTimeout = ReliableSessionInactivityTimeout;
			BD.ReliableSessionsOrdered = ReliableSessionsOrdered;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;

			Parent.Bindings.Add(BD);

			return BD;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWS2007HTTP Class -

	internal class ServiceBindingWS2007HTTP : ServiceBinding
	{
		public BindingSecurityWSHTTP Security { get { return (BindingSecurityWSHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSHTTP), typeof(ServiceBindingWSHTTP));

		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007HTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007HTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWS2007HTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingWS2007HTTP() { }

		public ServiceBindingWS2007HTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.AllowCookies = false;
			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.ReliableSessionEnabled = false;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransactionFlow = true;
			this.UseDefaultWebProxy = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWS2007HTTP BD = new ServiceBindingWS2007HTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWSHTTP)Security.Copy(HostName, Parent);

			BD.AllowCookies = AllowCookies;
			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.ReliableSessionEnabled = ReliableSessionEnabled;
			BD.ReliableSessionInactivityTimeout = ReliableSessionInactivityTimeout;
			BD.ReliableSessionsOrdered = ReliableSessionsOrdered;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6007", "The Security for the '" + Name + "' WS 2007 HTTP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6008", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' WS 2007 HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007HttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007HttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007HttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007HttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWSDualHTTP Class -

	internal class ServiceBindingWSDualHTTP : ServiceBinding
	{
		public BindingSecurityWSDualHTTP Security { get { return (BindingSecurityWSDualHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSDualHTTP), typeof(ServiceBindingWSDualHTTP));

		public string ClientBaseAddress { get { return (string)GetValue(ClientBaseAddressProperty); } set { SetValue(ClientBaseAddressProperty, value); } }
		public static readonly DependencyProperty ClientBaseAddressProperty = DependencyProperty.Register("ClientBaseAddress", typeof(string), typeof(ServiceBindingWSDualHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007HTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007HTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWS2007HTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWS2007HTTP));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007HTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007HTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007HTTP));

		public ServiceBindingWSDualHTTP() { }

		public ServiceBindingWSDualHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransactionFlow = true;
			this.UseDefaultWebProxy = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6009", "The Security for the '" + Name + "' WS Dual Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6010", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' WS Dual HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWSDualHTTP BD = new ServiceBindingWSDualHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWSDualHTTP)Security.Copy(HostName, Parent);

			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.ReliableSessionInactivityTimeout = ReliableSessionInactivityTimeout;
			BD.ReliableSessionsOrdered = ReliableSessionsOrdered;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;
			BD.TransactionFlow = TransactionFlow;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSDualHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", ClientBaseAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSDualHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", ClientBaseAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSDualHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSDualHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", ClientBaseAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSDualHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ClientBaseAddress != "") Code.AppendFormat("\t\t\tthis.ClientBaseAddress = new Uri(\"{0}\");{1}", ClientBaseAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWSFederationHTTP Class -

	internal class ServiceBindingWSFederationHTTP : ServiceBinding
	{
		public BindingSecurityWSFederationHTTP Security { get { return (BindingSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSFederationHTTP), typeof(ServiceBindingWSFederationHTTP));
		
		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWSFederationHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWSFederationHTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWSFederationHTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWSFederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWSFederationHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWSFederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(ServiceBindingWSFederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(ServiceBindingWSFederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWSFederationHTTP));

		public ServiceBindingWSFederationHTTP() { }

		public ServiceBindingWSFederationHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.ReliableSessionEnabled = false;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransactionFlow = true;
			this.UseDefaultWebProxy = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6011", "The Security for the '" + Name + "' WS Federation HTTP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6012", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' WS Federation HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWSFederationHTTP BD = new ServiceBindingWSFederationHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWSFederationHTTP)Security.Copy(HostName, Parent);

			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.ReliableSessionEnabled = ReliableSessionEnabled;
			BD.ReliableSessionInactivityTimeout = ReliableSessionInactivityTimeout;
			BD.ReliableSessionsOrdered = ReliableSessionsOrdered;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;
			BD.PrivacyNoticeAt = PrivacyNoticeAt;
			BD.PrivacyNoticeVersion = PrivacyNoticeVersion;
			BD.TransactionFlow = TransactionFlow;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSFederationHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSFederationHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSFederationHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WSFederationHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWS2007FederationHTTP Class -

	internal class ServiceBindingWS2007FederationHTTP : ServiceBinding
	{
		public BindingSecurityWSFederationHTTP Security { get { return (BindingSecurityWSFederationHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWSFederationHTTP), typeof(ServiceBindingWS2007FederationHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWS2007FederationHTTP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingWS2007FederationHTTP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWS2007FederationHTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBinding));

		public System.ServiceModel.WSMessageEncoding MessageEncoding { get { return (System.ServiceModel.WSMessageEncoding)GetValue(MessageEncodingProperty); } set { SetValue(MessageEncodingProperty, value); } }
		public static readonly DependencyProperty MessageEncodingProperty = DependencyProperty.Register("MessageEncoding", typeof(System.ServiceModel.WSMessageEncoding), typeof(ServiceBindingWS2007FederationHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWS2007FederationHTTP));

		public ServiceBindingTextEncoding TextEncoding { get { return (ServiceBindingTextEncoding)GetValue(TextEncodingProperty); } set { SetValue(TextEncodingProperty, value); } }
		public static readonly DependencyProperty TextEncodingProperty = DependencyProperty.Register("TextEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWS2007FederationHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public string PrivacyNoticeAt { get { return (string)GetValue(PrivacyNoticeAtProperty); } set { SetValue(PrivacyNoticeAtProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeAtProperty = DependencyProperty.Register("PrivacyNoticeAt", typeof(string), typeof(ServiceBindingWS2007FederationHTTP));

		public int PrivacyNoticeVersion { get { return (int)GetValue(PrivacyNoticeVersionProperty); } set { SetValue(PrivacyNoticeVersionProperty, value); } }
		public static readonly DependencyProperty PrivacyNoticeVersionProperty = DependencyProperty.Register("PrivacyNoticeVersion", typeof(int), typeof(ServiceBindingWS2007FederationHTTP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingWS2007FederationHTTP));

		public ServiceBindingWS2007FederationHTTP() { }

		public ServiceBindingWS2007FederationHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.BypassProxyOnLocal = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MessageEncoding = System.ServiceModel.WSMessageEncoding.Text;
			this.ReliableSessionEnabled = false;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TextEncoding = ServiceBindingTextEncoding.UTF8;
			this.TransactionFlow = true;
			this.UseDefaultWebProxy = true;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6013", "The Security for the '" + Name + "' WS 2007 Fenderation HTTP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6014", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' WS 2007 Federation HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWS2007FederationHTTP BD = new ServiceBindingWS2007FederationHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWSFederationHTTP)Security.Copy(HostName, Parent);

			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.ReliableSessionEnabled = ReliableSessionEnabled;
			BD.ReliableSessionInactivityTimeout = ReliableSessionInactivityTimeout;
			BD.ReliableSessionsOrdered = ReliableSessionsOrdered;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.MessageEncoding = MessageEncoding;
			BD.ProxyAddress = ProxyAddress;
			BD.TextEncoding = TextEncoding;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;
			BD.PrivacyNoticeAt = PrivacyNoticeAt;
			BD.PrivacyNoticeVersion = PrivacyNoticeVersion;
			BD.TransactionFlow = TransactionFlow;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007FederationHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007FederationHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007FederationHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSFederationHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWSFederationHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WS2007FederationHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WSDualHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (PrivacyNoticeAt != "")
			{
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeAt = new Uri(\"{0}\");{1}", PrivacyNoticeAt, Environment.NewLine);
				Code.AppendFormat("\t\t\tthis.PrivacyNoticeVersion = {0};{1}", PrivacyNoticeVersion, Environment.NewLine);
			}
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TextEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), TextEncoding), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}");
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingTCP Class -

	internal class ServiceBindingTCP : ServiceBinding
	{
		public BindingSecurityTCP Security { get { return (BindingSecurityTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityTCP), typeof(ServiceBindingTCP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingTCP));

		public bool ReliableSessionEnabled { get { return (bool)GetValue(ReliableSessionEnabledProperty); } set { SetValue(ReliableSessionEnabledProperty, value); } }
		public static readonly DependencyProperty ReliableSessionEnabledProperty = DependencyProperty.Register("ReliableSessionEnabled", typeof(bool), typeof(ServiceBindingTCP));

		public TimeSpan ReliableSessionInactivityTimeout { get { return (TimeSpan)GetValue(ReliableSessionInactivityTimeoutProperty); } set { SetValue(ReliableSessionInactivityTimeoutProperty, value); } }
		public static readonly DependencyProperty ReliableSessionInactivityTimeoutProperty = DependencyProperty.Register("ReliableSessionInactivityTimeout", typeof(TimeSpan), typeof(ServiceBindingTCP));

		public bool ReliableSessionsOrdered { get { return (bool)GetValue(ReliableSessionsOrderedProperty); } set { SetValue(ReliableSessionsOrderedProperty, value); } }
		public static readonly DependencyProperty ReliableSessionsOrderedProperty = DependencyProperty.Register("ReliableSessionsOrdered", typeof(bool), typeof(ServiceBindingTCP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingTCP));

		public Prospective.Utilities.Types.Base2 MaxBufferSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingTCP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBinding));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingTCP));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingTCP));

		public int ListenBacklog { get { return (int)GetValue(ListenBacklogProperty); } set { SetValue(ListenBacklogProperty, value); } }
		public static readonly DependencyProperty ListenBacklogProperty = DependencyProperty.Register("ListenBacklog", typeof(int), typeof(ServiceBindingTCP));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(ServiceBindingTCP));

		public bool PortSharingEnabled { get { return (bool)GetValue(PortSharingEnabledProperty); } set { SetValue(PortSharingEnabledProperty, value); } }
		public static readonly DependencyProperty PortSharingEnabledProperty = DependencyProperty.Register("PortSharingEnabled", typeof(bool), typeof(ServiceBindingTCP));

		public ServiceBindingTransactionProtocol TransactionProtocol { get { return (ServiceBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(ServiceBindingTransactionProtocol), typeof(ServiceBindingTCP));

		public ServiceBindingTCP() { }

		public ServiceBindingTCP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.ListenBacklog = 10;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxBufferSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxConnections = 10;
			this.PortSharingEnabled = false;
			this.ReliableSessionEnabled = false;
			this.ReliableSessionInactivityTimeout = new TimeSpan(0, 10, 0);
			this.ReliableSessionsOrdered = false;
			this.TransactionFlow = true;
			this.TransactionProtocol = ServiceBindingTransactionProtocol.Default;
			this.TransferMode = System.ServiceModel.TransferMode.Buffered;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6015", "The Security for the '" + Name + "' TCP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingTCP BD = new ServiceBindingTCP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityTCP)Security.Copy(HostName, Parent);

			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxBufferSize = MaxBufferSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.TransferMode = TransferMode;
			BD.TransactionFlow = TransactionFlow;
			BD.ListenBacklog = ListenBacklog;
			BD.MaxConnections = MaxConnections;
			BD.PortSharingEnabled = PortSharingEnabled;
			BD.TransactionProtocol = TransactionProtocol;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetTcpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", ListenBacklog, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetTcpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", ListenBacklog, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetTcpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetTcpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", ListenBacklog, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetTcpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetTcpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ListenBacklog = {0};{1}", ListenBacklog, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.PortSharingEnabled = {0};{1}", PortSharingEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Enabled = {0};{1}", ReliableSessionEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.InactivityTimeout = new TimeSpan({0});{1}", ReliableSessionInactivityTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReliableSession.Ordered = {0};{1}", ReliableSessionsOrdered == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingNamedPipe Class -

	internal class ServiceBindingNamedPipe : ServiceBinding
	{
		public BindingSecurityNamedPipe Security { get { return (BindingSecurityNamedPipe)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityNamedPipe), typeof(ServiceBindingNamedPipe));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingNamedPipe));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingNamedPipe));

		public Prospective.Utilities.Types.Base2 MaxBufferSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingNamedPipe));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingNamedPipe));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingNamedPipe));

		public bool TransactionFlow { get { return (bool)GetValue(TransactionFlowProperty); } set { SetValue(TransactionFlowProperty, value); } }
		public static readonly DependencyProperty TransactionFlowProperty = DependencyProperty.Register("TransactionFlow", typeof(bool), typeof(ServiceBindingNamedPipe));

		public int MaxConnections { get { return (int)GetValue(MaxConnectionsProperty); } set { SetValue(MaxConnectionsProperty, value); } }
		public static readonly DependencyProperty MaxConnectionsProperty = DependencyProperty.Register("MaxConnections", typeof(int), typeof(ServiceBindingNamedPipe));

		public ServiceBindingTransactionProtocol TransactionProtocol { get { return (ServiceBindingTransactionProtocol)GetValue(TransactionProtocolProperty); } set { SetValue(TransactionProtocolProperty, value); } }
		public static readonly DependencyProperty TransactionProtocolProperty = DependencyProperty.Register("TransactionProtocol", typeof(ServiceBindingTransactionProtocol), typeof(ServiceBindingNamedPipe));

		public ServiceBindingNamedPipe() { }

		public ServiceBindingNamedPipe(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxBufferSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxConnections = 10;
			this.TransactionFlow = true;
			this.TransactionProtocol = ServiceBindingTransactionProtocol.Default;
			this.TransferMode = System.ServiceModel.TransferMode.Buffered;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6016", "The Security for the '" + Name + "' Named Pipe Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingNamedPipe BD = new ServiceBindingNamedPipe(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityNamedPipe)Security.Copy(HostName, Parent);

			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxBufferSize = MaxBufferSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.TransferMode = TransferMode;
			BD.TransactionFlow = TransactionFlow;
			BD.MaxConnections = MaxConnections;
			BD.TransactionProtocol = TransactionProtocol;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetNamedPipeBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetNamedPipeBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetNamedPipeBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetNamedPipeSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetNamedPipeBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetNamedPipeSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxConnections = {0};{1}", MaxConnections, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionFlow = {0};{1}", TransactionFlow == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransactionProtocol = TransactionProtocol.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTransactionProtocol), TransactionProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingMSMQ Class -

	internal class ServiceBindingMSMQ : ServiceBinding
	{
		public BindingSecurityMSMQ Security { get { return (BindingSecurityMSMQ)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityMSMQ), typeof(ServiceBindingMSMQ));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingMSMQ));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingMSMQ));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(ServiceBindingMSMQ));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(ServiceBindingMSMQ));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.QueueTransferProtocol QueueTransferProtocol { get { return (System.ServiceModel.QueueTransferProtocol)GetValue(QueueTransferProtocolProperty); } set { SetValue(QueueTransferProtocolProperty, value); } }
		public static readonly DependencyProperty QueueTransferProtocolProperty = DependencyProperty.Register("QueueTransferProtocol", typeof(System.ServiceModel.QueueTransferProtocol), typeof(ServiceBindingMSMQ));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(ServiceBindingMSMQ));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(ServiceBindingMSMQ));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(ServiceBindingMSMQ));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(ServiceBindingMSMQ));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(ServiceBindingMSMQ));

		public bool UseActiveDirectory { get { return (bool)GetValue(UseActiveDirectoryProperty); } set { SetValue(UseActiveDirectoryProperty, value); } }
		public static readonly DependencyProperty UseActiveDirectoryProperty = DependencyProperty.Register("UseActiveDirectory", typeof(bool), typeof(ServiceBindingMSMQ));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(ServiceBindingMSMQ));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(ServiceBindingMSMQ));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(ServiceBindingMSMQ));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(ServiceBindingMSMQ));

		public ServiceBindingMSMQ() { }

		public ServiceBindingMSMQ(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;		//This is the default if Durable is true
			this.Durable = true;			//Must be true if ExactlyOnce is true.
			this.ExactlyOnce = true;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxRetryCycles = 2;
			this.QueueTransferProtocol = System.ServiceModel.QueueTransferProtocol.Native;
			this.ReceiveContextEnabled = true;
			this.ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Reject;
			this.ReceiveRetryCount = 5;
			this.RetryCycleDelay = new TimeSpan(0, 10, 0);
			this.TimeToLive = new TimeSpan(1, 0, 0, 0);
			this.UseActiveDirectory = false;
			this.UseMSMQTracing = false;
			this.UseSourceJournal = false;
			this.ValidityDuration = TimeSpan.Zero;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6017", "The Security for the '" + Name + "' MSMQ Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingMSMQ BD = new ServiceBindingMSMQ(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityMSMQ)Security.Copy(HostName, Parent);

			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.CustomDeadLetterQueue = CustomDeadLetterQueue;
			BD.DeadLetterQueue = DeadLetterQueue;
			BD.ExactlyOnce = ExactlyOnce;
			BD.MaxRetryCycles = MaxRetryCycles;
			BD.QueueTransferProtocol = QueueTransferProtocol;
			BD.ReceiveContextEnabled = ReceiveContextEnabled;
			BD.ReceiveErrorHandling = ReceiveErrorHandling;
			BD.ReceiveRetryCount = ReceiveRetryCount;
			BD.RetryCycleDelay = RetryCycleDelay;
			BD.TimeToLive = TimeToLive;
			BD.UseActiveDirectory = UseActiveDirectory;
			BD.UseMSMQTracing = UseMSMQTracing;
			BD.UseSourceJournal = UseSourceJournal;
			BD.ValidityDuration = ValidityDuration;
			BD.Durable = Durable;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetMsmqBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), QueueTransferProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetMsmqBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), QueueTransferProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetMsmqBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tNetMsmqSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), QueueTransferProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetMsmqBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(NetMsmqSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.QueueTransferProtocol = QueueTransferProtocol.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.QueueTransferProtocol), QueueTransferProtocol), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseActiveDirectory = {0};{1}", UseActiveDirectory == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingPeerTCP Class -

	internal class ServiceBindingPeerTCP : ServiceBinding
	{
		public BindingSecurityPeerTCP Security { get { return (BindingSecurityPeerTCP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityPeerTCP), typeof(ServiceBindingPeerTCP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingPeerTCP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingPeerTCP));

		public string ListenIPAddress { get { return (string)GetValue(ListenIPAddressProperty); } set { SetValue(ListenIPAddressProperty, value); } }
		public static readonly DependencyProperty ListenIPAddressProperty = DependencyProperty.Register("ListenIPAddress", typeof(string), typeof(ServiceBindingPeerTCP));
		
		public int Port { get { return (int)GetValue(PortProperty); } set { SetValue(PortProperty, value); } }
		public static readonly DependencyProperty PortProperty = DependencyProperty.Register("Port", typeof(int), typeof(ServiceBindingPeerTCP));

		public ServiceBindingPeerTCP() { }

		public ServiceBindingPeerTCP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.ListenIPAddress = "";
			this.Port = 31337;		
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6018", "The Security for the '" + Name + "' Peer TCP Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ListenIPAddress != "" && ListenIPAddress != null)
				if (Helpers.RegExs.MatchIPv4.IsMatch(ListenIPAddress) == false && Helpers.RegExs.MatchIPv6.IsMatch(ListenIPAddress) == false)
				{
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6019", "The Listen IP Address for the '" + Name + "' Peer TCP Binding in the '" + Parent.Name + "' project is not valid.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
					NoErrors = true;
				}

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingPeerTCP BD = new ServiceBindingPeerTCP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityPeerTCP)Security.Copy(HostName, Parent);

			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.ListenIPAddress = ListenIPAddress;
			BD.Port = Port;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetPeerTcpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", ListenIPAddress, Environment.NewLine); }
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Port = {0};{1}", Port, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetPeerTcpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", ListenIPAddress, Environment.NewLine); }
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Port = {0};{1}", Port, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\this.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetPeerTcpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tPeerSecuritySettings sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", ListenIPAddress, Environment.NewLine); }
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Port = {0};{1}", Port, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : NetPeerTcpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(PeerSecuritySettings CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			if (ListenIPAddress == "") { Code.AppendLine("\t\t\tthis.ListenIPAddress = null;"); } else { Code.AppendFormat("\t\t\tthis.ListenIPAddress = IPAddress.Parse(\"{0}\");{1}", ListenIPAddress, Environment.NewLine); }
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Port = {0};{1}", Port, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\this.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

	#region - ServiceBindingWebHTTP Class -

	internal class ServiceBindingWebHTTP : ServiceBinding
	{
		public BindingSecurityWebHTTP Security { get { return (BindingSecurityWebHTTP)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityWebHTTP), typeof(ServiceBindingWebHTTP));
				
		public bool AllowCookies { get { return (bool)GetValue(AllowCookiesProperty); } set { SetValue(AllowCookiesProperty, value); } }
		public static readonly DependencyProperty AllowCookiesProperty = DependencyProperty.Register("AllowCookies", typeof(bool), typeof(ServiceBindingWebHTTP));

		public bool BypassProxyOnLocal { get { return (bool)GetValue(BypassProxyOnLocalProperty); } set { SetValue(BypassProxyOnLocalProperty, value); } }
		public static readonly DependencyProperty BypassProxyOnLocalProperty = DependencyProperty.Register("BypassProxyOnLocal", typeof(bool), typeof(ServiceBindingWebHTTP));

		public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode { get { return (System.ServiceModel.HostNameComparisonMode)GetValue(HostNameComparisonModeProperty); } set { SetValue(HostNameComparisonModeProperty, value); } }
		public static readonly DependencyProperty HostNameComparisonModeProperty = DependencyProperty.Register("HostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), typeof(ServiceBindingWebHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferPoolSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferPoolSizeProperty); } set { SetValue(MaxBufferPoolSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferPoolSizeProperty = DependencyProperty.Register("MaxBufferPoolSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWebHTTP));

		public Prospective.Utilities.Types.Base2 MaxBufferSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxBufferSizeProperty); } set { SetValue(MaxBufferSizeProperty, value); } }
		public static readonly DependencyProperty MaxBufferSizeProperty = DependencyProperty.Register("MaxBufferSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWebHTTP));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingWebHTTP));

		public string ProxyAddress { get { return (string)GetValue(ProxyAddressProperty); } set { SetValue(ProxyAddressProperty, value); } }
		public static readonly DependencyProperty ProxyAddressProperty = DependencyProperty.Register("ProxyAddress", typeof(string), typeof(ServiceBindingWebHTTP));

		public System.ServiceModel.TransferMode TransferMode { get { return (System.ServiceModel.TransferMode)GetValue(TransferModeProperty); } set { SetValue(TransferModeProperty, value); } }
		public static readonly DependencyProperty TransferModeProperty = DependencyProperty.Register("TransferMode", typeof(System.ServiceModel.TransferMode), typeof(ServiceBindingWebHTTP));

		public bool UseDefaultWebProxy { get { return (bool)GetValue(UseDefaultWebProxyProperty); } set { SetValue(UseDefaultWebProxyProperty, value); } }
		public static readonly DependencyProperty UseDefaultWebProxyProperty = DependencyProperty.Register("UseDefaultWebProxy", typeof(bool), typeof(ServiceBindingWebHTTP));

		public bool CrossDomainScriptAccessEnabled { get { return (bool)GetValue(CrossDomainScriptAccessEnabledProperty); } set { SetValue(CrossDomainScriptAccessEnabledProperty, value); } }
		public static readonly DependencyProperty CrossDomainScriptAccessEnabledProperty = DependencyProperty.Register("CrossDomainScriptAccessEnabled", typeof(bool), typeof(ServiceBindingWebHTTP));

		public ServiceBindingTextEncoding WriteEncoding { get { return (ServiceBindingTextEncoding)GetValue(WriteEncodingProperty); } set { SetValue(WriteEncodingProperty, value); } }
		public static readonly DependencyProperty WriteEncodingProperty = DependencyProperty.Register("WriteEncoding", typeof(ServiceBindingTextEncoding), typeof(ServiceBindingWebHTTP));

		public ServiceBindingWebHTTP() { }

		public ServiceBindingWebHTTP(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.AllowCookies = false;
			this.BypassProxyOnLocal = false;
			this.CrossDomainScriptAccessEnabled = false;
			this.HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard;
			this.MaxBufferPoolSize = new Prospective.Utilities.Types.Base2(524288M);
			this.MaxBufferSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.TransferMode = System.ServiceModel.TransferMode.Buffered;
			this.UseDefaultWebProxy = true;
			this.WriteEncoding = ServiceBindingTextEncoding.UTF8;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6020", "The Security for the '" + Name + "' MSMQ Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));
			if (ProxyAddress != "" && ProxyAddress != null)
				if (Helpers.RegExs.MatchHTTPURI.IsMatch(ProxyAddress) == false)
					Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6021", "The Proxy Address '" + ProxyAddress + "' for the '" + Name + "' Basic HTTP Binding in the '" + Parent.Name + "' project is not a valid URI.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingWebHTTP BD = new ServiceBindingWebHTTP(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityWebHTTP)Security.Copy(HostName, Parent);

			BD.BypassProxyOnLocal = BypassProxyOnLocal;
			BD.HostNameComparisonMode = HostNameComparisonMode;
			BD.MaxBufferPoolSize = MaxBufferPoolSize;
			BD.MaxBufferSize = MaxBufferSize;
			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.ProxyAddress = ProxyAddress;
			BD.TransferMode = TransferMode;
			BD.UseDefaultWebProxy = UseDefaultWebProxy;
			BD.CrossDomainScriptAccessEnabled = CrossDomainScriptAccessEnabled;
			BD.WriteEncoding = WriteEncoding;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return "";
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WebHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), WriteEncoding), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WebHttpBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), WriteEncoding), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\this.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return "";
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return "";
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WebHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tWebHttpSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), WriteEncoding), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : WebHttpBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(WebHttpSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.AllowCookies = {0};{1}", AllowCookies == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.BypassProxyOnLocal = {0};{1}", BypassProxyOnLocal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.CrossDomainScriptAccessEnabled = {0};{1}", CrossDomainScriptAccessEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.HostNameComparisonMode = HostNameComparisonMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.HostNameComparisonMode), HostNameComparisonMode), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferPoolSize = {0};{1}", MaxBufferPoolSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxBufferSize = {0};{1}", Convert.ToInt32(MaxBufferSize.BytesNormalized), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.ProxyAddress = new Uri(\"{0}\");{1}", ProxyAddress, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TransferMode = TransferMode.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.TransferMode), TransferMode), Environment.NewLine);
			if (ProxyAddress != "" && UseDefaultWebProxy == false) Code.AppendFormat("\t\t\tthis.UseDefaultWebProxy = false;{0}", Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.WriteEncoding = System.Text.Encoding.{0};{1}", System.Enum.GetName(typeof(ServiceBindingTextEncoding), WriteEncoding), Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\this.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return "";
		}
	}

	#endregion 

	#region - ServiceBindingMSMQIntegration Class -

	internal class ServiceBindingMSMQIntegration : ServiceBinding
	{
		public BindingSecurityMSMQIntegration Security { get { return (BindingSecurityMSMQIntegration)GetValue(SecurityProperty); } set { SetValue(SecurityProperty, value); } }
		public static readonly DependencyProperty SecurityProperty = DependencyProperty.Register("Security", typeof(BindingSecurityMSMQIntegration), typeof(ServiceBindingMSMQIntegration));

		public Prospective.Utilities.Types.Base2 MaxReceivedMessageSize { get { return (Prospective.Utilities.Types.Base2)GetValue(MaxReceivedMessageSizeProperty); } set { SetValue(MaxReceivedMessageSizeProperty, value); } }
		public static readonly DependencyProperty MaxReceivedMessageSizeProperty = DependencyProperty.Register("MaxReceivedMessageSize", typeof(Prospective.Utilities.Types.Base2), typeof(ServiceBindingMSMQIntegration));

		public string CustomDeadLetterQueue { get { return (string)GetValue(CustomDeadLetterQueueProperty); } set { SetValue(CustomDeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty CustomDeadLetterQueueProperty = DependencyProperty.Register("CustomDeadLetterQueue", typeof(string), typeof(ServiceBindingMSMQIntegration));

		public System.ServiceModel.DeadLetterQueue DeadLetterQueue { get { return (System.ServiceModel.DeadLetterQueue)GetValue(DeadLetterQueueProperty); } set { SetValue(DeadLetterQueueProperty, value); } }
		public static readonly DependencyProperty DeadLetterQueueProperty = DependencyProperty.Register("DeadLetterQueue", typeof(System.ServiceModel.DeadLetterQueue), typeof(ServiceBindingMSMQIntegration));

		public bool ExactlyOnce { get { return (bool)GetValue(ExactlyOnceProperty); } set { SetValue(ExactlyOnceProperty, value); } }
		public static readonly DependencyProperty ExactlyOnceProperty = DependencyProperty.Register("ExactlyOnce", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public int MaxRetryCycles { get { return (int)GetValue(MaxRetryCyclesProperty); } set { SetValue(MaxRetryCyclesProperty, value); } }
		public static readonly DependencyProperty MaxRetryCyclesProperty = DependencyProperty.Register("MaxRetryCycles", typeof(int), typeof(ServiceBindingMSMQIntegration));

		public bool ReceiveContextEnabled { get { return (bool)GetValue(ReceiveContextEnabledProperty); } set { SetValue(ReceiveContextEnabledProperty, value); } }
		public static readonly DependencyProperty ReceiveContextEnabledProperty = DependencyProperty.Register("ReceiveContextEnabled", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling { get { return (System.ServiceModel.ReceiveErrorHandling)GetValue(ReceiveErrorHandlingProperty); } set { SetValue(ReceiveErrorHandlingProperty, value); } }
		public static readonly DependencyProperty ReceiveErrorHandlingProperty = DependencyProperty.Register("ReceiveErrorHandling", typeof(System.ServiceModel.ReceiveErrorHandling), typeof(ServiceBindingMSMQIntegration));

		public int ReceiveRetryCount { get { return (int)GetValue(ReceiveRetryCountProperty); } set { SetValue(ReceiveRetryCountProperty, value); } }
		public static readonly DependencyProperty ReceiveRetryCountProperty = DependencyProperty.Register("ReceiveRetryCount", typeof(int), typeof(ServiceBindingMSMQIntegration));

		public TimeSpan RetryCycleDelay { get { return (TimeSpan)GetValue(RetryCycleDelayProperty); } set { SetValue(RetryCycleDelayProperty, value); } }
		public static readonly DependencyProperty RetryCycleDelayProperty = DependencyProperty.Register("RetryCycleDelay", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration));

		public TimeSpan TimeToLive { get { return (TimeSpan)GetValue(TimeToLiveProperty); } set { SetValue(TimeToLiveProperty, value); } }
		public static readonly DependencyProperty TimeToLiveProperty = DependencyProperty.Register("TimeToLive", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration));

		public bool UseMSMQTracing { get { return (bool)GetValue(UseMSMQTracingProperty); } set { SetValue(UseMSMQTracingProperty, value); } }
		public static readonly DependencyProperty UseMSMQTracingProperty = DependencyProperty.Register("UseMSMQTracing", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public bool UseSourceJournal { get { return (bool)GetValue(UseSourceJournalProperty); } set { SetValue(UseSourceJournalProperty, value); } }
		public static readonly DependencyProperty UseSourceJournalProperty = DependencyProperty.Register("UseSourceJournal", typeof(bool), typeof(ServiceBindingMSMQIntegration));

		public TimeSpan ValidityDuration { get { return (TimeSpan)GetValue(ValidityDurationProperty); } set { SetValue(ValidityDurationProperty, value); } }
		public static readonly DependencyProperty ValidityDurationProperty = DependencyProperty.Register("ValidityDuration", typeof(TimeSpan), typeof(ServiceBindingMSMQIntegration));

		public bool Durable { get { return (bool)GetValue(DurableProperty); } set { SetValue(DurableProperty, value); } }
		public static readonly DependencyProperty DurableProperty = DependencyProperty.Register("Durable", typeof(bool), typeof(ServiceBinding));

		public System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat SerializationFormat { get { return (System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat)GetValue(SerializationFormatProperty); } set { SetValue(SerializationFormatProperty, value); } }
		public static readonly DependencyProperty SerializationFormatProperty = DependencyProperty.Register("SerializationFormat", typeof(System.ServiceModel.MsmqIntegration.MsmqMessageSerializationFormat), typeof(ServiceBindingMSMQIntegration));

		public ServiceBindingMSMQIntegration() { }

		public ServiceBindingMSMQIntegration(string Name, Namespace Parent)
		{
			this.id = Guid.NewGuid();
			this.Name = Name;
			this.Parent = Parent;
			System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\W+");
			this.CodeName = r.Replace(Name, @"");

			this.CloseTimeout = new TimeSpan(0, 1, 0);
			this.OpenTimeout = new TimeSpan(0, 1, 0);
			this.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.SendTimeout = new TimeSpan(0, 1, 0);

			this.DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;		//This is the default if Durable is true
			this.Durable = true;			//Must be true if ExactlyOnce is true.
			this.ExactlyOnce = true;
			this.MaxReceivedMessageSize = new Prospective.Utilities.Types.Base2(65536M);
			this.MaxRetryCycles = 2;
			this.ReceiveContextEnabled = true;
			this.ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Reject;
			this.ReceiveRetryCount = 5;
			this.RetryCycleDelay = new TimeSpan(0, 10, 0);
			this.TimeToLive = new TimeSpan(1, 0, 0, 0);
			this.UseMSMQTracing = false;
			this.UseSourceJournal = false;
			this.ValidityDuration = TimeSpan.Zero;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ServiceBinding.IsSearchingProperty) return;
			if (e.Property == ServiceBinding.IsSearchMatchProperty) return;
			if (e.Property == ServiceBinding.IsTreeExpandedProperty) return;

			if (Parent != null)
				Parent.IsDirty = true;
		}

		public override bool VerifyCode(Compiler.Compiler Compiler)
		{
			bool NoErrors = true;

			if (base.VerifyCode(Compiler) == false) NoErrors = false;

			if (Security == null)
				Compiler.AddMessage(new WCFArchitect.Compiler.CompileMessage("GS6021", "The Security for the '" + Name + "' MSMQ Binding in the '" + Parent.Name + "' project is not set. The default values will be used. This may result in data being transmitted over insecure connections.", WCFArchitect.Compiler.CompileMessageSeverity.Warning, Parent, this, this.GetType()));

			return NoErrors;
		}

		public override ServiceBinding Copy(string HostName, Namespace Parent)
		{
			if (Parent == this.Parent) return this;

			ServiceBindingMSMQIntegration BD = new ServiceBindingMSMQIntegration(Name + HostName, Parent);
			BD.Namespace = Namespace;
			BD.EndpointAddress = EndpointAddress;
			BD.ListenAddress = ListenAddress;
			BD.CloseTimeout = CloseTimeout;
			BD.OpenTimeout = OpenTimeout;
			BD.ReceiveTimeout = ReceiveTimeout;
			BD.SendTimeout = SendTimeout;
			BD.Security = (BindingSecurityMSMQIntegration)Security.Copy(HostName, Parent);

			BD.MaxReceivedMessageSize = MaxReceivedMessageSize;
			BD.CustomDeadLetterQueue = CustomDeadLetterQueue;
			BD.DeadLetterQueue = DeadLetterQueue;
			BD.ExactlyOnce = ExactlyOnce;
			BD.MaxRetryCycles = MaxRetryCycles;
			BD.ReceiveContextEnabled = ReceiveContextEnabled;
			BD.ReceiveErrorHandling = ReceiveErrorHandling;
			BD.ReceiveRetryCount = ReceiveRetryCount;
			BD.RetryCycleDelay = RetryCycleDelay;
			BD.TimeToLive = TimeToLive;
			BD.UseMSMQTracing = UseMSMQTracing;
			BD.UseSourceJournal = UseSourceJournal;
			BD.ValidityDuration = ValidityDuration;
			BD.Durable = Durable;
			BD.SerializationFormat = SerializationFormat;

			Parent.Bindings.Add(BD);

			return BD;
		}

		public override string GenerateServerCode30(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding{2}", Parent.Owner.ServerPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateServerCode35Client(string ProjectName)
		{
			return GenerateServerCode35(ProjectName);
		}

		public override string GenerateServerCode40Client(string ProjectName)
		{
			return GenerateServerCode40(ProjectName);
		}

		public override string GenerateClientCode30(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode35(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tSystem.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity sec = this.Security;");
			Code.AppendLine("\t\t\tsec = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\t{0}.Security.Set{1}Security(this.Security);{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode40(string ProjectName)
		{
			StringBuilder Code = new StringBuilder();
			Code.AppendFormat("\t{0} class {1} : System.ServiceModel.MsmqIntegration.MsmqIntegrationBinding{2}", Parent.Owner.ClientPublicClasses == true ? "public" : "internal", CodeName, Environment.NewLine);
			Code.AppendLine("\t{");
			Code.AppendFormat("\t\tpublic {0}(){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendFormat("\t\tpublic {0}(System.ServiceModel.MsmqIntegration.MsmqIntegrationSecurity CustomSecurity, System.Xml.XmlDictionaryReaderQuotas ReaderQuotas){1}", CodeName, Environment.NewLine);
			Code.AppendLine("\t\t{");
			Code.AppendLine("\t\t\tSetDefaults();");
			Code.AppendLine("\t\t\tthis.Security = CustomSecurity;");
			Code.AppendLine("\t\t\tthis.ReaderQuotas = ReaderQuotas;");
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t\tprivate void SetDefaults()");
			Code.AppendLine("\t\t{");
			// Generic Binding code.
			Code.AppendFormat("\t\t\tthis.CloseTimeout = new TimeSpan({0});{1}", CloseTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Name = \"{0}\";{1}", Name, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Namespace = \"{0}\";{1}", Namespace, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.OpenTimeout = new TimeSpan({0});{1}", OpenTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveTimeout = new TimeSpan({0});{1}", ReceiveTimeout.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.SendTimeout = new TimeSpan({0});{1}", SendTimeout.Ticks, Environment.NewLine);
			// Binding Specific code.
			Code.AppendFormat("\t\t\tthis.CustomerDeadLetterQueue = new Uri(\"{0}\");{1}", CustomDeadLetterQueue, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.DeadLetterQueue = DeadLetterQueue.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.DeadLetterQueue), DeadLetterQueue), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.Durable = {0};{1}", Durable == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ExactlyOnce = {0};{1}", ExactlyOnce == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxReceivedMessageSize = {0};{1}", MaxReceivedMessageSize.BytesNormalized, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.MaxRetryCycles = {0};{1}", MaxRetryCycles, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveContextEnabled = {0};{1}", ReceiveContextEnabled == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveErrorHandling = ReceiveErrorHandling.{0};{1}", System.Enum.GetName(typeof(System.ServiceModel.ReceiveErrorHandling), ReceiveErrorHandling), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.ReceiveRetryCount = {0};{1}", ReceiveRetryCount, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.RetryCycleDelay = new TimeSpan({0});{1}", RetryCycleDelay.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.TimeToLive = new TimeSpan({0});{1}", TimeToLive.Ticks, Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseMSMQTracing = {0};{1}", UseMSMQTracing == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			Code.AppendFormat("\t\t\tthis.UseSourceJournal = {0};{1}", UseSourceJournal == true ? Boolean.TrueString.ToLower() : Boolean.FalseString.ToLower(), Environment.NewLine);
			if (ValidityDuration != TimeSpan.Zero) Code.AppendFormat("\t\t\tthis.ValidityDuration = new TimeSpan({0});{1}", ValidityDuration.Ticks, Environment.NewLine);
			if (Security != null) Code.AppendFormat("\t\t\tthis.Security = {0}.Security.Create{1}Security();{2}", Parent.FullName, Security.CodeName, Environment.NewLine);
			Code.AppendLine("\t\t}");
			Code.AppendLine("\t}");
			return Code.ToString();
		}

		public override string GenerateClientCode35Client(string ProjectName)
		{
			return GenerateClientCode35(ProjectName);
		}

		public override string GenerateClientCode40Client(string ProjectName)
		{
			return GenerateClientCode40(ProjectName);
		}
	}

	#endregion

}