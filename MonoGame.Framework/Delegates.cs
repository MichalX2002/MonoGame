
namespace MonoGame.Framework
{
    public delegate void MessageHandler<TSender>(TSender sender);
    public delegate void DataMessageHandler<TSender, TData>(TSender sender, TData data);
}
