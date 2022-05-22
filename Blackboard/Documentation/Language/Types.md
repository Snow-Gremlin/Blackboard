# Types

There are currently only five main build-in types which are normally specified in definitions and casts:
[`bool`](#bool), [`int`](#int), [`double`](#double), [`string`](#string), and [`trigger`](#trigger).

There are also has several built-in types which are specified in specific ways or not yet definable:
[`namespace`](#namespace), [function-group](#function-group), and [function-def](#function-def).

There are three build-in complex types which can be created via methods:
[`counter`](#counter), [`toggler`](#toggler), and [`latch`](#latch).

In the current version no custom types can be created.

- [Home](./Laguage.md)
- [node](#node)
- [Base Types](#base-types)
  - [bool](#bool)
  - [int](#int)
  - [double](#double)
  - [string](#string)
  - [trigger](#trigger)
- [namespace](#namespace)
- [function-group](#function-group)
- [function-def](#function-def)
- [Compound Types](#compound-types)
  - [counter](#counter)
  - [toggler](#toggler)
  - [latch](#latch)

### node

The Blackboard is built out of nodes. Nodes is the root type of all other types.

### bool

The `bool` type is a boolean value.
This can be assigned to [boolean literals](#booleans).
`bool` is returned from equality tests, such as `<`, `>=`, and `=`.
They can use logical operations, such as `!`, `&`, and `|`.
`bool` implicitly casts to [`trigger`](#trigger) and [`string`](#string).

```
in a = true;
bool b := false;
c := a | !b;
bool d = a == b;
```

### int

The `int` type is an 32-bit signed integer.
This can be assigned to [integer literals](#integers).
`int` implicitly casts to [`double`](#double) and [`string`](#string).

```
in int a = 42;
b := 32;
c := a + b * 2;
```

### double

The `double` type is a floating point 64-bit finite real numbers with 1-bit sign, 53-bit mantissa,
and 11-bit exponent, as defined by [IEEE-754](https://en.wikipedia.org/wiki/IEEE_754).
This can be assigned to [double literals](#doubles).
`double` implicitly casts to [`string`](#string) and explicitly casts to [`int`](#int).

### string

The `string` type is an immutable list of characters which are one byte each and using UTF-8 encoding.
This can be assigned to [string literals](#strings).

### trigger

The `trigger` type is a signal indicator. Imagine this as a wire which a pulse can be sent down.
The trigger type is like an instantaneous boolean which can only be momentarily
(for the duration of a blackboard update) be set to true.
This is designed to indicate when events occur, such as a button is clicked.

When a trigger emits a pulse it is called "provoked".

### namespace

The `namespace` type is a set of variables and constants stored by an identifier name.

```
namespace A {
    namespace B {
        in C = 10;
    }
    B.C = 12;
}
A.B.C = 14;
```

### function-group

The `function-group` is a collection of [function definitions](#function-def) by the same name.
When a function is called, it is the group which is called and the arguments determine
which function definition, if any, will be called.
Function groups are used by built-in methods.
Currently there is no way to define function groups within the Blackboard language.

### function-def

The `function-def` is a single definition for a single function with a specific signature.
Function definitions are predefined for a number of various usages.
Currently there is no way to define function definitions within the Blackboard language.
`function-def` can be implicitly cast to [`function-group`](#function-group).

### counter

The `counter` is a value which can be incremented and decremented by specific step sizes
whenever a [`trigger`](#trigger) is provoked.

### toggler

The `toggler` is a boolean value which can be toggled on and off.
This can be imagined as a switch which can be flipped whenever a [`trigger`](#trigger) is provoked.

### latch

The `latch` is a value which is set to a new value whenever a [`trigger`](#trigger) is provoked.
The value it is set to is another variable which can change as needed and the latch will only
take on that value when its trigger is provoked.
