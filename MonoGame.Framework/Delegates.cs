
namespace MonoGame.Framework
{
    public delegate void SimpleEventHandler<TSender>(TSender sender);
    public delegate void DataEventHandler<TSender, TData>(TSender sender, TData data);
}
