﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HostLiveTaskScheduler.HostLiveService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HostLiveService.IHostLive")]
    public interface IHostLive {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHostLive/Start", ReplyAction="http://tempuri.org/IHostLive/StartResponse")]
        bool Start();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IHostLive/Start", ReplyAction="http://tempuri.org/IHostLive/StartResponse")]
        System.Threading.Tasks.Task<bool> StartAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHostLiveChannel : HostLiveTaskScheduler.HostLiveService.IHostLive, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class HostLiveClient : System.ServiceModel.ClientBase<HostLiveTaskScheduler.HostLiveService.IHostLive>, HostLiveTaskScheduler.HostLiveService.IHostLive {
        
        public HostLiveClient() {
        }
        
        public HostLiveClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public HostLiveClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public HostLiveClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public HostLiveClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool Start() {
            return base.Channel.Start();
        }
        
        public System.Threading.Tasks.Task<bool> StartAsync() {
            return base.Channel.StartAsync();
        }
    }
}
