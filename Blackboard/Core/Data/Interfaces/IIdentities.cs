namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates that this Blackboard data type can get basic arithmetic identities.</summary>
    /// <typeparam name="T">The type of the data implementing this interface.</typeparam>
    public interface IIdentities<T>: IData
        where T : IData {

        /// <summary>Gets this additive identity, which is typically zero.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        /// <returns>The identity data value.</returns>
        T Zero();

        /// <summary>Gets this multiplicative identity, which is typically one.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        /// <returns>The identity data value.</returns>
        T One();

        /// <summary>Gets the minimum value for this data type.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        /// <returns>The minimum data value.</returns>
        T MinValue();

        /// <summary>Gets the maximum value for this data type.</summary>
        /// <remarks>The current value is not used when getting this identity.</remarks>
        /// <returns>The maximum data value.</returns>
        T MaxValue();
    }
}
