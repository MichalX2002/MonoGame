
namespace Microsoft.Xna.Framework
{
    public delegate void SenderEvent<TSender>(TSender sender);
    public delegate void DataEvent<TSender, TEventData>(TSender sender, TEventData data);
}
