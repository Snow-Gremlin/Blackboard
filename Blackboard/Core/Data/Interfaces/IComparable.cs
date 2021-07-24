namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates a data type which is comparable.</summary>
    /// <typeparam name="T">The type of data to compare.</typeparam>
    public interface IComparable<in T>: System.IComparable<T>, IData
        where T : IData {
        // Empty
    }
}
