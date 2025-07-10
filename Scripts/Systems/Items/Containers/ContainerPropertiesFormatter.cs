using System.Buffers;
using MemoryPack;

namespace Systems.Items
{
    public sealed class ContainerPropertiesFormatter : MemoryPackFormatter<ContainerProperties>
    {
        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref ContainerProperties value)
        {
            writer.WriteInt32(value.size);
            writer.WriteString(value.name);
            writer.WriteInt32(value.gridWidth);
            writer.WriteBoolean(value.scaleDownGridIfSmaller);
            writer.WriteInt32((int)value.type);
        }

        public override void Deserialize(ref MemoryPackReader reader, scoped ref ContainerProperties value)
        {
            if (!reader.TryReadObjectHeader(out var _))
            {
                value = default;
                return;
            }
            value.size = reader.ReadInt32();
            value.name = reader.ReadString();
            value.gridWidth = reader.ReadInt32();
            value.scaleDownGridIfSmaller = reader.ReadBoolean();
            value.type = (ContainerType)reader.ReadInt32();
        }
    }
}
