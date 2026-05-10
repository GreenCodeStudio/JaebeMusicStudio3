using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;

namespace JaebeMusicStudio3.VstHost;

public class HostCommandStub:IVstHostCommandStub
{
    public IVstPluginContext PluginContext { get; set; }
    public IVstHostCommands20 Commands { get; }
}