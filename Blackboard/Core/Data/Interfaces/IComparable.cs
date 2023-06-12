using S = System;

namespace Blackboard.Core.Data.Interfaces {

    /// <summary>Interface indicating that the data is comparable.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IComparable<T>: IData, IEquatable<T>, S.IComparable<T>
        where T : struct, IData { }
}
