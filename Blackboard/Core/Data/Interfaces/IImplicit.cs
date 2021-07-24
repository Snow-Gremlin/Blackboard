namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates this data can be implicity cast to the given data type.</summary>
    /// <remarks>
    /// The awkward way this uses this data to cast another into a new instance of this data
    /// is because C# can't overload methods based on return type.
    /// </remarks>
    /// <typeparam name="Tin">This should be the type to cast from.</typeparam>
    /// <typeparam name="Tout">This should be the same type as this data.</typeparam>
    public interface IImplicit<in Tin, out Tout>: IData
        where Tin : IData
        where Tout : IData {

        /// <summary>This uses any Tout data to cast the Tin to a new Tout.</summary>
        /// <param name="value">The value to cast.</param>
        /// <returns>The casted output value.</returns>
        Tout CastFrom(Tin value);
    }
}
