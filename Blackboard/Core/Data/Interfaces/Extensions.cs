using Blackboard.Core.Data.Caps;
using Blackboard.Core.Nodes.Outer;

namespace Blackboard.Core.Data.Interfaces;

/// <summary>Extensions to add to data specific methods.</summary>
static internal class Extensions {

    /// <summary>Determines if this data can be cast implicitly, cast explicitly, or is already an inherited type.</summary>
    /// <typeparam name="T">The type of the data to cast to.</typeparam>
    /// <param name="value">The value to check if possible to cast.</param>
    /// <returns>True if possible to cast, false otherwise.</returns>
    static public bool CanCastTo<T>(this IData value)
        where T : IData =>
        value switch {
            T      => true,
            Bool   => default(T) is ICast<Bool,   T>,
            Float  => default(T) is ICast<Float,  T>,
            Double => default(T) is ICast<Double, T>,
            Int    => default(T) is ICast<Int,    T>,
            Uint   => default(T) is ICast<Uint,   T>,
            String => default(T) is ICast<String, T>,
            _      => throw new BlackboardException("Unexpected input type in cast").
                            With("Input", value)
        };

    /// <summary>Determines if this data can be cast implicitly or is already an inherited type.</summary>
    /// <typeparam name="T">The type of the data to implicitly cast to.</typeparam>
    /// <param name="value">The value to check if possible to implicitly cast.</param>
    /// <returns>True if possible to implicitly cast, false otherwise.</returns>
    static public bool CanImplicitCastTo<T>(this IData value)
        where T : IData =>
        value switch {
            T      => true,
            Bool   => default(T) is IImplicit<Bool,   T>,
            Float  => default(T) is IImplicit<Float,  T>,
            Double => default(T) is IImplicit<Double, T>,
            Int    => default(T) is IImplicit<Int,    T>,
            Uint   => default(T) is IImplicit<Uint,   T>,
            String => default(T) is IImplicit<String, T>,
            _      => throw new BlackboardException("Unexpected input type in implicit cast").
                            With("Input", value)
        };

    /// <summary>Determines if this data can be cast explicitly or is already an inherited type.</summary>
    /// <typeparam name="T">The type of the data to explicitly cast to.</typeparam>
    /// <param name="value">The value to check if possible to explicitly cast.</param>
    /// <returns>True if possible to explicitly cast, false otherwise.</returns>
    static public bool CanExplicitCastTo<T>(this IData value)
        where T : IData =>
        value switch {
            T      => true,
            Bool   => default(T) is IExplicit<Bool,   T>,
            Float  => default(T) is IExplicit<Float,  T>,
            Double => default(T) is IExplicit<Double, T>,
            Int    => default(T) is IExplicit<Int,    T>,
            Uint   => default(T) is IExplicit<Uint,   T>,
            String => default(T) is IExplicit<String, T>,
            _      => throw new BlackboardException("Unexpected input type in explicit cast").
                            With("Input", value)
        };

    /// <summary>Performs a cast of the given input type into the given output type using he given cast type.</summary>
    /// <remarks>This will throw an exception if unable to cast.</remarks>
    /// <typeparam name="Tin">The input type to cast from.</typeparam>
    /// <typeparam name="Tout">The output type to cast to.</typeparam>
    /// <typeparam name="TCast">The specific type of cast to try to use.</typeparam>
    /// <param name="value">The value to try to cast.</param>
    /// <returns>The input value cast into the output type.</returns>
    static private Tout castTo<Tin, Tout, TCast>(this Tin value)
        where Tin   : IData
        where Tout  : IData
        where TCast : ICast<Tin, Tout> =>
        default(Tout) is ICast<Tin, Tout> cast ? cast.CastFrom(value) :
            throw new BlackboardException("Unable to cast from the given input into the given output.").
                With("Input",     value).
                With("In Type",   typeof(Tin)).
                With("Out Type",  typeof(Tout)).
                With("Cast Type", typeof(TCast));

    /// <summary>Performs an cast from the given data value into the given type.</summary>
    /// <remarks>This will throw an exception if unable to cast.</remarks>
    /// <typeparam name="T">The type to cast the value into.</typeparam>
    /// <param name="value">The value to cast into that type.</param>
    /// <returns>The new data value in the given type.</returns>
    static public T CastTo<T>(this IData value) where T : IData =>
        value switch {
            T      v  => v,
            Bool   vb => castTo<Bool,   T, ICast<Bool,   T>>(vb),
            Float  vf => castTo<Float,  T, ICast<Float,  T>>(vf),
            Double vd => castTo<Double, T, ICast<Double, T>>(vd),
            Int    vi => castTo<Int,    T, ICast<Int,    T>>(vi),
            Uint   vu => castTo<Uint,   T, ICast<Uint,   T>>(vu),
            String vs => castTo<String, T, ICast<String, T>>(vs),
            _         => throw new BlackboardException("Unexpected input type in cast").
                            With("Input", value)
        };

    /// <summary>Performs an implicit cast from the given data value into the given type.</summary>
    /// <remarks>This will throw an exception if unable to implicit cast.</remarks>
    /// <typeparam name="T">The type to implicit cast the value into.</typeparam>
    /// <param name="value">The value to implicit cast into that type.</param>
    /// <returns>The new data value in the given type.</returns>
    static public T ImplicitCastTo<T>(this IData value) where T : IData =>
        value switch {
            T      v  => v,
            Bool   vb => castTo<Bool,   T, IImplicit<Bool,   T>>(vb),
            Float  vf => castTo<Float,  T, IImplicit<Float,  T>>(vf),
            Double vd => castTo<Double, T, IImplicit<Double, T>>(vd),
            Int    vi => castTo<Int,    T, IImplicit<Int,    T>>(vi),
            Uint   vu => castTo<Uint,   T, IImplicit<Uint,   T>>(vu),
            String vs => castTo<String, T, IImplicit<String, T>>(vs),
            _         => throw new BlackboardException("Unexpected input type in implicit cast").
                            With("Input", value)
        };

    /// <summary>Performs an explicit cast from the given data value into the given type.</summary>
    /// <remarks>This will throw an exception if unable to explicit cast.</remarks>
    /// <typeparam name="T">The type to explicit cast the value into.</typeparam>
    /// <param name="value">The value to explicit cast into that type.</param>
    /// <returns>The new data value in the given type.</returns>
    static public T ExplicitCastTo<T>(this IData value) where T : IData =>
        value switch {
            T      v  => v,
            Bool   vb => castTo<Bool,   T, IExplicit<Bool,   T>>(vb),
            Float  vf => castTo<Float,  T, IExplicit<Float,  T>>(vf),
            Double vd => castTo<Double, T, IExplicit<Double, T>>(vd),
            Int    vi => castTo<Int,    T, IExplicit<Int,    T>>(vi),
            Uint   vu => castTo<Uint,   T, IExplicit<Uint,   T>>(vu),
            String vs => castTo<String, T, IExplicit<String, T>>(vs),
            _         => throw new BlackboardException("Unexpected input type in explicit cast").
                            With("Input", value)
        };

    /// <summary>Creates a literal for the value of this data.</summary>
    /// <typeparam name="T">The type of data to create a literal with.</typeparam>
    /// <param name="value">The value to create a literal with.</param>
    /// <returns>The literal for the current data value.</returns>
    static public Literal<T> ToLiteral<T>(this T value) where T : struct, IEquatable<T> => new(value);
}
