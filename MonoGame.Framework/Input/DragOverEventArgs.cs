
namespace MonoGame.Framework.Input
{
    public readonly struct DragOverEventArgs
    {
        public Point Position { get; }
        public DraggedObjectType ObjectType { get; }

        public DragOverEventArgs(Point position, DraggedObjectType objectType)
        {
            Position = position;
            ObjectType = objectType;
        }
    }
}
