namespace Blackboard.Core.Data.Interfaces;

/// <summary>Interface indicating that the data can be checked for equality.</summary>
/// <remarks>This is a required interface for most node to accept the implementing data type.</remarks>
/// <typeparam name="T">The type of the data implementing this interface.</typeparam>
public interface IEquatable<T> : IData, System.IEquatable<T>
    where T : struct, IData { }
