# Operators

- [Home](./Laguage.md)
- [Order of Operations](#order-of-operations)
- [Actions](#actions)
  - [Assignment](#assignment)
  - [Define](#define)
  - [Provoke](#provoke)
  - [Conditional Provoke](#conditional-provoke)
- [Unary Operations](#unary-operations)
  - [Negate](#negate)
  - [Logical NOT](#logical-not)
  - [Bitwise NOT](#bitwise-not)
  - [Group](#group)
- [Binary Operations](#binary-operations)
  - [Boolean Algebra](#boolean-algebra)
    - [Logical OR](#logical-or)
    - [Logical XOR](#logical-xor)
    - [Logical AND](#logical-and)
    - [Bitwise OR](#bitwise-or)
    - [Bitwise XOR](#bitwise-xor)
    - [Bitwise AND](#bitwise-and)
    - [Right Shift](#right-shift)
    - [Left Shift](#left-shift)
  - [Algebra](#algebra)
    - [Addition](#addition)
    - [Subtraction](#subtraction)
    - [Multiplication](#multiplication)
    - [Division](#division)
    - [Modulo](#modulo)
    - [Exponential](#exponential)
  - [Explicit Cast](#explicit-cast)
  - [Accessor](#accessor)
  - [Comparisons](#comparisons)
    - [Equal](#equal)
    - [Not Equal](#not-equal)
    - [Greater Than](#greater-than)
    - [Less Than](#less-than)
    - [Greater Than Or Equal](#greater-than-or-equal)
    - [Less Than Or Equal](#less-than-or-equal)
- [Ternary Operations](#ternary-operations)
  - [Select](#select)
- [Method Call](#method-call)

## Order of Operations

Blackboard code is expressions which end with a `;`.
An expression is made up of several operations.
The following are in order of importance where the lowest order value will be performed first.
Any operation with the same order of importance is performed from left to right,
except for the [Assignment](#assignment), [Provoke](#provoke), and [Conditional Provoke](#conditional-provoke)
which performs right to left.

oder | symbol        | name
----:|:-------------:|:-----------
  1  | `()` (group)  | [Group](#group)
  1  | `.`           | [Accessor](#accessor)
  1  | `()` (call)   | [Method Call](#method-call)
  2  | `-` (unary)   | [Negate](#negate)
  2  | `!`           | [Logical NOT](#logical-not)
  2  | `~`           | [Bitwise NOT](#bitwise-not)
  2  | `()` (cast)   | [Explicit Cast](#explicit-cast)
  3  | `**`          | [Exponential](#exponential)
  4  | `*`           | [Multiplication](#multiplication)
  4  | `/`           | [Division](#division)
  4  | `%`           | [Modulo](#modulo)
  5  | `+` (binary)  | [Addition](#addition)
  5  | `-` (binary)  | [Subtraction](#subtraction)
  6  | `>>`          | [Right Shift](#right-shift)
  6  | `<<`          | [Left Shift](#left-shift)
  7  | `&`           | [Bitwise AND](#bitwise-and) 
  8  | `^`           | [Bitwise XOR](#bitwise-xor)
  9  | `\|`          | [Bitwise OR](#bitwise-or)
 10  | `>`           | [Greater Than](#greater-than)
 10  | `<`           | [Less Than](#less-than)
 10  | `>=`          | [Greater Than Or Equal](#greater-than-or-equal)
 10  | `<=`          | [Less Than Or Equal](#less-than-or-equal)
 11  | `==`          | [Equal](#equal)
 11  | `!=`          | [Not Equal](#not-equal)  
 12  | `&&`          | [Logical AND](#logical-and)
 13  | `^^`          | [Logical XOR](#logical-xor)
 14  | `\|\|`        | [Logical OR](#logical-or)
 15  | `?:`          | [Switch](#switch)
 16  | `=`           | [Assignment](#assignment)
 17  | `:=`          | [Define](#define)
 17  | `->` (unary)  | [Provoke](#provoke)
 17  | `->` (binary) | [Conditional Provoke](#conditional-provoke)

For more information see [Order of Operations](https://en.wikipedia.org/wiki/Order_of_operations).

## Action

All action operations are performed with a [formula](./formula.md#assignment).
This means that they will only run when the formula is performed.
Actions are typically performed right to left.

### Assignment

A single equal sign, `=`, (not to be confused with double equal sign for [Equal](#equal))
is an assignment of the variable on the left to the value on the right.
The variable must be assignable, meaning it may not be defined to a rule.
During assignment, if a cast is needed, then the value will be implicitly cast.
If assignments are chained the same value is applied to each with a cast, if needed, for each.

Examples: `a = b`, `a = b = c`

### Define

A colon equals symbol, `:=`, is used to define a rule for a variable.
The define will create the left hand side variable and prepare the nodes on the right hand
side so that anytime the right hand side value is changed, it will automatically update
the value of the left hand side variable.

Examples: `a := b`

### Provoke

An arrow symbol, `->`, can be used in a unary operation for unconditionally provoking a trigger.
This is equivalent to a [conditional provoke](#conditional-provoke) with the left hand side a
[`true`](#./literals.md#Booleans) literal, e.g. `true -> a`.

Examples: `-> a`, `-> a -> b`

### Conditional Provoke

An arrow symbol, `->`, can be used in a binary operation for conditional provoking a trigger.
If the left hand side evaluates to [`true`](#./literals.md#Booleans) then the trigger(s)
on the right hand side will be provoked.

Examples: `a -> b`, `a -> b -> c`

## Unary Operations

### Negate

The minus sign, `-`, can be used in a unary operation for negating a value.
The negate operator can be used on signed types, [`double`](./Types.md#double) and
[`int`](./Types.md#int), to turn a negative value to a positive and vice versa.

Examples: `-a`, `-(a + b)`

For more information see [Additive Inverse](https://en.wikipedia.org/wiki/Additive_inverse).
Implemented with the [`Neg`](../Core/Nodes/Inner/Neg.cs) node.

### Logical NOT

The exclamation sign, `!`, can be used in a unary operation for getting the opposite boolean value. 
The logical NOT can be only used on [`bool`](./Types.md#bool) types.

Examples: `!a`, `!(a & b)`

For more information see [Logical Negation](https://en.wikipedia.org/wiki/Negation).
Implemented with the [`Not`](../Core/Nodes/Inner/Not.cs) node.

### Bitwise NOT

The tilde symbol, `~`, can be used in a unary operation for getting the NOT of each bit in a number.
The bitwise NOT can be used on integer types, [`int`](./Types.md#int).

Examples: `~a`

For more information see [Bitwise operation: NOT](https://en.wikipedia.org/wiki/Bitwise_operation#NOT).
Implemented with the [`BitwiseNeg`](../Core/Nodes/Inner/BitwiseNeg.cs) node.

### Group

Parenthesis, `()`, around operations will group the operation to be performed those
operations internal to the parenthesis before operations outside of it.
Parenthesis are free, they take no memory and no compute time, they only enforce
that operations are performed in a specific order.

Examples: `(a)`, `a * (b + c)`

## Binary Operations

### Boolean Algebra

#### Logical OR

Two bar symbols, `||`, can be used in a binary operation for getting the Logical OR of two values.
The Logical OR can be used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.
For triggers, if any trigger is provoked, it will emit a provoked trigger.

Examples: `a || b`

For more information see [Logical Disjunction](https://en.wikipedia.org/wiki/Logical_disjunction).
Implemented with the [Or](../Core/Nodes/Inner/Or.cs) and [Any](../Core/Nodes/Inner/Any) nodes.

#### Logical XOR

Two cap symbols, `^^`, can be used in a binary operation for getting the Logical XOR of two values.
The Logical XOR can be used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.

Examples: `a ^^ b`, `a ^^ b ^^ c`

For more information see [Exclusive OR](https://en.wikipedia.org/wiki/Exclusive_or).
Implemented with the [Xor](../Core/Nodes/Inner/Xor.cs) and [XorTrigger](../Core/Nodes/Inner/XorTrigger) nodes.

#### Logical AND

Two ampersands, `&&`, can be used in a binary operation for getting the Logical AND of two values.
The Logical OR can be used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.

Examples: `a && b`

For more information see [Logical Conjunction](https://en.wikipedia.org/wiki/Logical_conjunction).
Implemented with the [And](../Core/Nodes/Inner/And.cs) and [All](../Core/Nodes/Inner/All) nodes.

#### Bitwise OR

A single bar, `|`, can be used in a binary operation for getting the Bitwise OR of two values.
The Bitwise OR can be used on integer types, [`int`](./types.md#int), and it can be used as a
Logical OR when used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.

Examples: `a | b`

For more information see [Bitwise operation: OR](https://en.wikipedia.org/wiki/Bitwise_operation#OR).
Implemented with the [`BitwiseOr`](../Core/Nodes/Inner/BitwiseOr.cs),
[Or](../Core/Nodes/Inner/And.cs) and [Any](../Core/Nodes/Inner/Any) nodes.

#### Bitwise XOR

A single cap, `^`, can be used in a binary operation for getting the Bitwise XOR of two values.
The Bitwise XOR can be used on integer types, [`int`](./types.md#int), and it can be used as a
Logical XOR when used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.

Examples: `a ^ b`

For more information see [Bitwise operation: XOR](https://en.wikipedia.org/wiki/Bitwise_operation#XOR).
Implemented with the [`BitwiseXor`](../Core/Nodes/Inner/BitwiseXor.cs),
[Xor](../Core/Nodes/Inner/Xor.cs) and [XorTrigger](../Core/Nodes/Inner/XorTrigger) nodes.

#### Bitwise AND

A single ampersand, `&`, can be used in a binary operation for getting the Bitwise AND of two values.
The Bitwise AND can be used on integer types, [`int`](./types.md#int), and it can be used as a
Logical AND when used on [`bool`](./Types.md#bool) and [`trigger`](./Types.md#trigger) types.

Examples: `a & b`

For more information see [Bitwise operation: AND](https://en.wikipedia.org/wiki/Bitwise_operation#AND).
Implemented with the [`BitwiseAnd`](../Core/Nodes/Inner/BitwiseAnd.cs),
[And](../Core/Nodes/Inner/And.cs) and [All](../Core/Nodes/Inner/All) nodes.

#### Right Shift

Two greater than symbols, `>>`, can be used in a binary operation for Right Shifting the left hand value
by the number of bits indicated in the right hand value. The right hand side may not be negative.
The Right Shift can be used on integer types, like [`int`](./types.md#int).

Examples: `a >> b`

For more information see [Logical Shift](https://en.wikipedia.org/wiki/Logical_shift).
Implemented with the [`RightShift`](../Core/Nodes/Inner/RightShift.cs) node.

#### Left Shift

Two less than symbols, `<<`, can be used in a binary operation for Left Shifting the left hand value
by the number of bits indicated in the right hand value. The right hand side may not be negative.
The Left Shift can be used on integer types, like [`int`](./types.md#int).

Examples: `a << b`

For more information see [Logical Shift](https://en.wikipedia.org/wiki/Logical_shift).
Implemented with the [`LeftShift`](../Core/Nodes/Inner/LeftShift.cs) node.

### Algebra

#### Addition

A single plus symbol, `+`, can be used in a binary operation to get the sum of the two values.
If either value is a [string](./Types.md#string) then this will perform a concatenation.

Examples: `a + b`

For more information see [Summation](https://en.wikipedia.org/wiki/Summation).
Implemented with the [`Sum`](../Core/Nodes/Inner/Sum.cs) node.

#### Subtraction

A single plus symbol, `-`, can be used in a binary operation to get the right hand value subtracted from the left hand value.

Examples: `a - b`

For more information see [Subtraction](https://en.wikipedia.org/wiki/Subtraction).
Implemented with the [`Sub`](../Core/Nodes/Inner/Sub.cs) node.

#### Multiplication

13  | `*`          | `a * b`     | The product of the left hand value and the right hand value.

Examples: `a * b`

For more information see [Multiplication](https://en.wikipedia.org/wiki/Multiplication).
Implemented with the [`Sub`](../Core/Nodes/Inner/Sub.cs) node.







#### Division

13  | `/`          | `a / b`     | The left hand value divided by the right hand value.

Examples: `a / b`

For more information see [Division](https://en.wikipedia.org/wiki/Division_(mathematics)).
Implemented with the [`Sub`](../Core/Nodes/Inner/Sub.cs) node.






#### Modulo

13  | `%`          |      | The modulo (remainder) of the left hand value by the right hand value.

Examples: `a % b`

For more information see [Modulo](https://en.wikipedia.org/wiki/Modulo_(mathematics)).
Implemented with the [`Sub`](../Core/Nodes/Inner/Sub.cs) node.







#### Exponential

Two astrix symbols, `**`, can be used in a binary operation to raise the left hand size by the power of the right hand side.
The exponentials can be used on floating point types, like [`double`](./types.md#double).

Examples: `a ** b`

For more information see [Exponentiation](https://en.wikipedia.org/wiki/Exponentiation).
Implemented with the [`BinaryDoubleMath`](../Core/Nodes/Inner/BinaryDoubleMath.cs) node configured for `Pow`.
 
### Explicit Cast

Parenthesis, `()`, containing a [type](./Types.md) name before a value will cast the right hand value into the given type in the parenthesis.
If the right hand type can be implicitly cast to the given type, it will be implicitly cast instead of explicitly cast.
If no cast is required then this cast will have no effect.

Examples: `(a)b`, `(int)a`

See [types](./Types.md) for the lists of implicit and explicit casts.
Implemented with the [`Implicit`](../Core/Nodes/Inner/Implicit.cs) and [`Explicit`](../Core/Nodes/Inner/Explicit.cs) nodes.

### Accessor

A dot, `.`, accesses the value in the left hand value specified by the right hand side.
The left side is the receiver for the right side.
This is known as a member access and can be used for fields to get values, namespaces, and functions.

Examples: `a.b`, `a.b.c`

### Comparisons

#### Equal

Two equal signs, `==`, determines if the left and right side have the same value.
One side must be able to be implicitly cast to the other type.

Examples: `a == b`

Implemented with the [`Equal`](../Core/Nodes/Inner/Equal.cs) node.

#### Not Equal

`!=`, determines if the left and right side do NOT have the same value.
One side must be able to be implicitly cast to the other type.

Examples: `a != b`

Implemented with the [`NotEqual`](../Core/Nodes/Inner/NotEqual.cs) node.

#### Greater Than

A single greater than signs, `>`, determines if the left and right side have the same value.
One side must be able to be implicitly cast to the other type.

Examples: `a > b`

Implemented with the [`GreaterThan`](../Core/Nodes/Inner/GreaterThan.cs) node.

#### Less Than

A single less than signs, `<`, determines if the left and right side have the same value.
One side must be able to be implicitly cast to the other type.

Examples: `a < b`

Implemented with the [`LessThan`](../Core/Nodes/Inner/LessThan.cs) node.

#### Greater Than Or Equal

`>=`, determines if the left side value is greater than or equal to the right hand side.
One side must be able to be implicitly cast to the other type.

Examples: `a >= b`

Implemented with the [`GreaterThanOrEqual`](../Core/Nodes/Inner/GreaterThanOrEqual.cs) node.

#### Less Than Or Equal

`<=`, determines if the left side value is less than or equal to the right hand side.
One side must be able to be implicitly cast to the other type.

Examples: `a <= b`

Implemented with the [`LessThanOrEqual`](../Core/Nodes/Inner/LessThanOrEqual.cs) node.

## Ternary Operations

### Select

The select ternary, `?:`, can be used to switch between two values based on a boolean value.
The left hand value must be a [`bool`](#./Types.md#bool) and the other two values can be any type,
however one must be able to be implicitly cast into the other's type.

Examples: `a ? b : c`

For more information see [Conditional Assignment](https://en.wikipedia.org/wiki/%3F:#Conditional_assignment).
Implemented with the [`SelectValue`](../Core/Nodes/Inner/SelectValue.cs)
and [`SelectTrigger`](../Core/Nodes/Inner/SelectTrigger.cs) nodes.

## Method Call

Parenthesis, `()`, following after an identifier is a method call with optional parameters within the parenthesis.
Parameters are comma separated. Methods with the same name are chosen based on the number and type of the parameters.

Examples: `f()`, `f(a)`, `f(a, b)`, `f(a, b, c)`
