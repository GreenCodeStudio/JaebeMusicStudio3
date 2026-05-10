using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace VstHost;

public class HostCommandStub:IVstHostCommandStub
{
    public IVstPluginContext PluginContext { get; set; }
    public IVstHostCommands20 Commands { get; }
}