
namespace MonoGame.Framework
{
    public delegate void SenderDelegate<TSender>(TSender sender);
    public delegate void MessageDelegate<TSender, TData>(TSender sender, TData data);
}
