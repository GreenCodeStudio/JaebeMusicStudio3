// See https://aka.ms/new-console-template for more information

using Jacobi.Vst.Host.Interop;
using VstHost;

Console.WriteLine("Hello, World!");

var hostCmdStub = new HostCommandStub();
var ctx = VstPluginContext.Create("C:\\Program Files (x86)\\Common Files\\Steinberg\\VST2\\Voxengo\\Marvel GEQ.dll", hostCmdStub);

ctx.PropertyChanged += (s, e) =>
{
    Console.WriteLine("Property changed: " + e.PropertyName);
};
