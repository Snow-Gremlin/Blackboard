using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates a data type which is able to be equatable with another of the same type.</summary>
    /// <typeparam name="T">The type of data to check equality for.</typeparam>
    public interface IEquatable<T>: S.IEquatable<T>, IData
        where T : IData {}
}
