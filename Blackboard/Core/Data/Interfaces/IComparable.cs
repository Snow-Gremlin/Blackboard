using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates a data type which is comparable.</summary>
    /// <typeparam name="T">The type of data to compare.</typeparam>
    public interface IComparable<T>: S.IComparable<T>, S.IEquatable<T>, IData
        where T : IData { }
}
