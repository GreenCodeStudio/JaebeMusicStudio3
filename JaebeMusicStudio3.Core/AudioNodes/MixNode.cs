using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

// public class MixNode : IAudioNode
// {
//     public IEnumerable<NodeInputDefinition> Inputs =>new  List<NodeInputDefinition>()
//     {
//         new  NodeInputDefinition()
//         {
//             Name="input",
//             Node=this,
//             
//         }
//     }
//     public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
//     {
//         new NodeOutputDefinition()
//         {
//             Name = "main",
//             Node = this,
//             Type = NodeConnectionType.SingleChannelAudio
//         }
//     };
//     public Task<Dictionary<string, object>> Render(RenderingChunk chunk)
//     {
//         throw new NotImplementedException();
//     }
// }