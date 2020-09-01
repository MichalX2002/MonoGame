﻿
namespace MonoGame.Framework
{
    /// <summary>
    /// Represents the method that will handle an event that has no event data.
    /// </summary>
    /// <typeparam name="TSender">The type of the event source.</typeparam>
    /// <param name="sender">The source of the event.</param>
    public delegate void Event<TSender>(TSender sender);

    /// <summary>
    /// Represents the method that will handle an event when the event provides data.
    /// </summary>
    /// <typeparam name="TSender">The type of the event source.</typeparam>
    /// <typeparam name="TData">The type of the event data generated by the event.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="data">An object that contains the event data.</param>
    public delegate void DataEvent<TSender, TData>(TSender sender, TData data);
}