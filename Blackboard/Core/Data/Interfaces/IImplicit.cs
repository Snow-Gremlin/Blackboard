namespace Blackboard.Core.Data.Interfaces {

    /// <summary>This indicates this data can be implicity cast to the given data type.</summary>
    /// <typeparam name="Tin">This is the type to cast from.</typeparam>
    /// <typeparam name="Tout">This is the type to cast to, this should be the implementing type.</typeparam>
    public interface IImplicit<in Tin, out Tout>: ICast<Tin, Tout>
        where Tin  : IData
        where Tout : IData { }
}
