﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17020
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: System.Runtime.Serialization.ContractNamespaceAttribute("http://tempuri.org/WCFArchitect/Samples/HelloWCF/", ClrNamespace="WCFArchitect.Samples.HelloWCF")]

namespace WCFArchitect.Samples.HelloWCF
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
    public partial class User : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string AddressField;
        
        private string CityField;
        
        private string FirstNameField;
        
        private System.Guid IDField;
        
        private string LastNameField;
        
        private string StateField;
        
        private WCFArchitect.Samples.HelloWCF.FavoriteColor UserColorField;
        
        private string ZipField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Address
        {
            get
            {
                return this.AddressField;
            }
            set
            {
                this.AddressField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string City
        {
            get
            {
                return this.CityField;
            }
            set
            {
                this.CityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName
        {
            get
            {
                return this.FirstNameField;
            }
            set
            {
                this.FirstNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ID
        {
            get
            {
                return this.IDField;
            }
            set
            {
                this.IDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName
        {
            get
            {
                return this.LastNameField;
            }
            set
            {
                this.LastNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string State
        {
            get
            {
                return this.StateField;
            }
            set
            {
                this.StateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WCFArchitect.Samples.HelloWCF.FavoriteColor UserColor
        {
            get
            {
                return this.UserColorField;
            }
            set
            {
                this.UserColorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Zip
        {
            get
            {
                return this.ZipField;
            }
            set
            {
                this.ZipField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FavoriteColor", Namespace="http://tempuri.org/WCFArchitect/Samples/HelloWCF/")]
    public enum FavoriteColor : int
    {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Red = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Green = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Blue = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://tempuri.org/WCFArchitect/Samples/HelloWCF/", ConfigurationName="WCFArchitect.Samples.HelloWCF.Users", CallbackContract=typeof(WCFArchitect.Samples.HelloWCF.UsersCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface Users
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/GetUserInfo", ReplyAction="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/GetUserInfoResponse")]
        WCFArchitect.Samples.HelloWCF.User GetUserInfo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/SetUserInfo", ReplyAction="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/SetUserInfoResponse")]
        void SetUserInfo(WCFArchitect.Samples.HelloWCF.User UserInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/Connect", ReplyAction="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/ConnectResponse")]
        System.Guid Connect();
        
        [System.ServiceModel.OperationContractAttribute(IsTerminating=true, Action="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/Disconnect", ReplyAction="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/DisconnectResponse")]
        void Disconnect(System.Guid UserID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface UsersCallback
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/UserInfoUpdated", ReplyAction="http://tempuri.org/WCFArchitect/Samples/HelloWCF/Users/UserInfoUpdatedResponse")]
        void UserInfoUpdated(WCFArchitect.Samples.HelloWCF.User NewUserInfo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface UsersChannel : WCFArchitect.Samples.HelloWCF.Users, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UsersClient : System.ServiceModel.DuplexClientBase<WCFArchitect.Samples.HelloWCF.Users>, WCFArchitect.Samples.HelloWCF.Users
    {
        
        public UsersClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance)
        {
        }
        
        public UsersClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName)
        {
        }
        
        public UsersClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }
        
        public UsersClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }
        
        public UsersClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress)
        {
        }
        
        public WCFArchitect.Samples.HelloWCF.User GetUserInfo()
        {
            return base.Channel.GetUserInfo();
        }
        
        public void SetUserInfo(WCFArchitect.Samples.HelloWCF.User UserInfo)
        {
            base.Channel.SetUserInfo(UserInfo);
        }
        
        public System.Guid Connect()
        {
            return base.Channel.Connect();
        }
        
        public void Disconnect(System.Guid UserID)
        {
            base.Channel.Disconnect(UserID);
        }
    }
}

