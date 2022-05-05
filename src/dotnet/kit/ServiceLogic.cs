using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using BinaryStudio.Diagnostics.Logging;
using log4net;

namespace srv
    {
    public class ServiceLogic : IDisposable
        {
        private static readonly ILogger logger = new ServiceLogger(LogManager.GetLogger(nameof(ServiceLogic)));
        private IChannel channel;
        public Int32 Port { get; }
        public ServiceLogic(Int32 port)
            {
            Port = port;
            channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(CryptographicOperations), "CryptographicOperations", WellKnownObjectMode.SingleCall);
            }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            if (channel != null) {
                ChannelServices.UnregisterChannel(channel);
                channel = null;
                }
            }
        }
    }