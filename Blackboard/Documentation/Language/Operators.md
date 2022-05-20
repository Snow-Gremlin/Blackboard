# Operators

- [Home](./Laguage.md)
- [Order of Operations](#order-of-operations)
- [Unary](#unary)
  - [Negate](#negate)
  - [Logical NOT](#logical-not)
  - [Bit-wise NOT](#bit-wise-not)
  - [Group](#group)
- [Binary](#binary)
  - [Assignment](#assignment)
  - [Logical OR](#logical-or)
  - [Logical XOR](#logical-xor)
  - [Logical AND](#logical-and)
  - [Bit-wise OR](#bit-wise-or)
  - [Bit-wise XOR](#bit-wise-xor)
  - [Bit-wise AND](#bit-wise-and)
  - [Right Shift](#right-shift)
  - [Left Shift](#left-shift)
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
- [Ternary](#ternary)
  - [Switch](#switch)
- [Variable](#variable)

## Order of Operations

Blackboard code is expressions which end with a `;`.
An expression is made up of several operations.
The following are in order of importance where the lowest order value will be performed last.
Any operation with the same order of importance is per1formed from left to right,
except for the assignment which performs right to left.

oder | symbol       | name
----:|:------------:|:-----------
  1  | `=`          | [Assignment](#assignment)
  2  | `?:`         | [Switch](#switch)
  3  | `\|\|`       | [Logical OR](#logical-or)
  4  | `^^`         | [Logical XOR](#logical-xor)
  5  | `&&`         | [Logical AND](#logical-and)
  6  | `==`         | [Equal](#equal)
  6  | `!=`         | [Not Equal](#not-equal)
  7  | `>`          | [Greater Than](#greater-than)
  7  | `<`          | [Less Than](#less-than)
  7  | `>=`         | [Greater Than Or Equal](#greater-than-or-equal)
  7  | `<=`         | [Less Than Or Equal](#less-than-or-equal)
  8  | `\|`         | [Bit-wise OR](#bit-wise-or)
  9  | `^`          | [Bit-wise XOR](#bit-wise-xor)
 10  | `&`          | [Bit-wise AND](#bit-wise-and)
 11  | `>>`         | [Right Shift](#right-shift)
 11  | `<<`         | [Left Shift](#left-shift)
 12  | `+` (binary) | [Addition](#addition)
 12  | `-` (binary) | [Subtraction](#subtraction)
 13  | `*`          | [Multiplication](#multiplication)
 13  | `/`          | [Division](#division)
 13  | `%`          | [Modulo](#modulo)
 14  | `**`         | [Exponential](#exponential)
 15  | `-` (unary)  | 
 15  | `!`          | 
 15  | `~`          | 
 15  | `()` (cast)  | 
 16  | `()` (group) | 
 16  | `.`          | 
 16  | `()` (call)  | 

## Unary

### Negate

The minus sign, `-`, can be used in a unary operation for negating a value.
The negate operator can be used on signed types, [`double`](./Types.md#double) and
[`int`](./Types.md#int), to turn a negative value to a positive and vice versa.

Examples: `-a`, `-(a + b)`

Implemented with the [`Neg node`](../Core/Nodes/Inner/Neg.cs)

### Logical NOT

The exclamation sign, `!`, can be used in a unary operation for getting the opposite boolean value. 
The logical NOT can be only used on [`bool`](./Types.md#bool) types.

Examples: `!a`, `!(a & b)`

Implemented with the [`Not node`](../Core/Nodes/Inner/Not.cs)

### Bit-wise NOT

The tilde symbol, `~`, can be used in a unary operation for getting the NOT of each bit in a number.
The bit-wise NOT can be used on integer types, [`int`](./Types.md#int).

Example: `~a`

Implemented with the [`BitwiseNeg node`](../Core/Nodes/Inner/BitwiseNeg.cs)

### Group

Parenthesis, `()`, around operations will group the operation to be performed those
operations internal to the parenthesis before operations outside of it.
Parenthesis are free, they take no memory and no compute time, they only enforce
that operations are performed in a specific order.

Examples: `(a)`, `a * (b + c)`

No node is created for a group.

## Binary

### Assignment

A single equal sign, `=`, 

1  | `=`          | `a = b`     | Assignment of right hand value to the left hand variable.

### Logical OR

3  | `\|\|`       | `a \|\| b`  | Logical OR of the left hand value and right hand value.

### Logical XOR

4  | `^^`         | `a ^^ b`    | Logical XOR of the left hand value and right hand value.

### Logical AND

5  | `&&`         | `a && b`    | Logical AND of the left hand value and right hand value.

### Bit-wise OR

8  | `\|`         | `a \| b`    | Bit-wise OR of the left hand value and right hand value.

### Bit-wise XOR

9  | `^`          | `a ^ b`     | Bit-wise XOR of the left hand value and right hand value.

### Bit-wise AND

10  | `&`          | `a & b`     | Bit-wise AND of the left hand value and right hand value.

### Right Shift

11  | `>>`         | `a >> b`    | Bit-wise shifts the left hand value to the right by the number of bit in the right hand value.

### Left Shift

11  | `<<`         | `a << b`    | Bit-wise shifts the left hand value to the left by the number of bit in the right hand value.

### Addition

12  | `+` (binary) | `a + b`     | The sum of the left hand value and the right hand value.

### Subtraction

12  | `-` (binary) | `a - b`     | The left hand value after the right hand value has been subtracted from it.

### Multiplication

13  | `*`          | `a * b`     | The product of the left hand value and the right hand value.

### Division

13  | `/`          | `a / b`     | The left hand value divided by the right hand value.

### Modulo

13  | `%`          | `a % b`     | The modulo (remainder) of the left hand value by the right hand value.

### Exponential

14  | `**`         | `a ** b`    | The left hand value raised to the power of the right hand vale.
 
### Explicit Cast

15  | `()` (cast)  | `(a) b`     | Casts the right hand value into the [type](#types) given in the parenthesis.

### Accessor

16  | `.`          | `a.b`       | Accesses the value in the right hand value specified by the left hand side. The left side is the receiver for the right side.

### Comparisons

#### Equal

6  | `==`         | `a == b`    | True if the left hand value and right hand value are equal, false otherwise.

#### Not Equal

6  | `!=`         | `a != b`    | True if the left hand value and right hand value are not equal, false otherwise.

#### Greater Than

7  | `>`          | `a > b`     | True if the left hand value is greater than the right hand value, false otherwise.

#### Less Than

7  | `<`          | `a < b`     | True if the left hand value is less than the right hand value, false otherwise.

#### Greater Than Or Equal

7  | `>=`         | `a >= b`    | True if the left hand value is greater than or equal to the right hand value, false otherwise.

#### Less Than Or Equal

7  | `<=`         | `a <= b`    | True if the left hand value is less than or equal to the right hand value, false otherwise.

## Ternary


### Switch

`?:`
`a ? b : c`
Ternary; If the left hand value ([bool](#bool)) is true then the middle value is returned otherwise the right hand value is returned.

## Variable

16  | `()` (call)  | `a(b)`      | Calls the right hand method with the zero or more parameters in the parenthesis.
