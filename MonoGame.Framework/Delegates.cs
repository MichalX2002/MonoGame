
namespace Microsoft.Xna.Framework
{
    public delegate void SenderDelegate<TSender>(TSender sender);
    public delegate void EventDelegate<TSender, TEventData>(TSender sender, TEventData data);
}
