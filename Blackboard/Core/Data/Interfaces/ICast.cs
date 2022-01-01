namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates this data can be cast to the given data type.</summary>
    /// <remarks>
    /// The awkward way this code uses this data to cast another into a new instance
    /// of this data is because C# can't overload methods based on return type.
    /// </remarks>
    /// <typeparam name="Tin">This is the type to cast from.</typeparam>
    /// <typeparam name="Tout">This is the type to cast to, this should be the implementing type.</typeparam>
    public interface ICast<in Tin, out Tout>: IData
        where Tin  : IData
        where Tout : IData {

        /// <summary>Cast from the given input into the data type implementing this interface.</summary>
        /// <param name="value">The value to cast.</param>
        /// <returns>The casted output value.</returns>
        Tout CastFrom(Tin value);
    }
}
