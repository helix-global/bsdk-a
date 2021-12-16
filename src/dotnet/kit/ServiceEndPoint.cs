using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

public class ServiceEndPoint<I> : IDisposable
    {
    private IChannel channel;
    private readonly Boolean needreg = true;
    private readonly String  uri;
    private I o;

    public ServiceEndPoint(String name)
        {
        uri = $"tcp://localhost:50000/{name}";
        channel = new TcpChannel();
        foreach (var item in ChannelServices.RegisteredChannels) {
            if (item.ChannelName == channel.ChannelName)
                {
                needreg = false;
                }
            }
        if (needreg)
            {
            ChannelServices.RegisterChannel(channel, false);
            }
        }

    public I Channel { get {
        if (o == null) {
            o = (I)Activator.GetObject(typeof(I),uri);
            }
        return o;
        }}

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    void IDisposable.Dispose() {
        o = default;
        if (channel != null) {
            if (needreg) {
                ChannelServices.UnregisterChannel(channel);
                }
            channel = null;
            }
        }
    }
