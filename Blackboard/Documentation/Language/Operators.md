# Operators

- [Home](./Laguage.md)
- [Order of Operations](#order-of-operations)
- [Unary](#unary)
  - [Negate](#negate)
  - [Logical NOT](#logical-not)
  - [Bit-wise NOT](#bit-wise-not)
  - [Group](#group)
- [Binary](#binary)
  - [Comparisons](#comparisons)
- [Ternary](#ternary)
  - [Switch](#switch)
- [Variable](#variable)

## Order of Operations

Blackboard code is expressions which end with a `;`.
An expression is made up of several operations.
The following are in order of importance where the lowest order value will be performed last.
Any operation with the same order of importance is per1formed from left to right,
except for the assignment which performs right to left.

oder | symbol       | example     | meaning
----:|:------------:|:-----------:|:------
  1  | `=`          | `a = b`     | Assignment of right hand value to the left hand variable.
  2  | `?:`         | `a ? b : c` | Ternary; If the left hand value ([bool](./Types.md#bool)) is true then the middle value is returned otherwise the right hand value is returned.
  3  | `\|\|`       | `a \|\| b`  | Logical OR of the left hand value and right hand value.
  4  | `^^`         | `a ^^ b`    | Logical XOR of the left hand value and right hand value.
  5  | `&&`         | `a && b`    | Logical AND of the left hand value and right hand value.
  6  | `==`         | `a == b`    | True if the left hand value and right hand value are equal, false otherwise.
  6  | `!=`         | `a != b`    | True if the left hand value and right hand value are not equal, false otherwise.
  7  | `>`          | `a > b`     | True if the left hand value is greater than the right hand value, false otherwise.
  7  | `<`          | `a < b`     | True if the left hand value is less than the right hand value, false otherwise.
  7  | `>=`         | `a >= b`    | True if the left hand value is greater than or equal to the right hand value, false otherwise.
  7  | `<=`         | `a <= b`    | True if the left hand value is less than or equal to the right hand value, false otherwise.
  8  | `\|`         | `a \| b`    | Bit-wise OR of the left hand value and right hand value.
  9  | `^`          | `a ^ b`     | Bit-wise XOR of the left hand value and right hand value.
 10  | `&`          | `a & b`     | Bit-wise AND of the left hand value and right hand value.
 11  | `>>`         | `a >> b`    | Bit-wise shifts the left hand value to the right by the number of bit in the right hand value.
 11  | `<<`         | `a << b`    | Bit-wise shifts the left hand value to the left by the number of bit in the right hand value.
 12  | `+` (binary) | `a + b`     | The sum of the left hand value and the right hand value.
 12  | `-` (binary) | `a - b`     | The left hand value after the right hand value has been subtracted from it.
 13  | `*`          | `a * b`     | The product of the left hand value and the right hand value.
 13  | `/`          | `a / b`     | The left hand value divided by the right hand value.
 13  | `%`          | `a % b`     | The modulo (remainder) of the left hand value by the right hand value.
 14  | `**`         | `a ** b`    | The left hand value raised to the power of the right hand vale.
 15  | `-` (unary)  | `-a`        | Negates the value.
 15  | `!`          | `!a`        | Logical NOT of the value.
 15  | `~`          | `~a`        | Bit-wise NOT of the value.
 15  | `()` (cast)  | `(a) b`     | Casts the right hand value into the [type](./Types.md) given in the parenthesis.
 16  | `()` (group) | `(a)`       | Performs any operations internal to the parenthesis before operations outside of it.
 16  | `.`          | `a.b`       | Accesses the value in the right hand value specified by the left hand side. The left side is the receiver for the right side.
 16  | `()` (call)  | `a(b)`      | Calls the right hand method with the zero or more parameters in the parenthesis.

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

16  | `()` (group) | `(a)`       | Performs any operations internal to the parenthesis before operations outside of it.


No node is created for a group. Parenthesis are free, they take 

## Binary

1  | `=`          | `a = b`     | Assignment of right hand value to the left hand variable.

3  | `\|\|`       | `a \|\| b`  | Logical OR of the left hand value and right hand value.
4  | `^^`         | `a ^^ b`    | Logical XOR of the left hand value and right hand value.
5  | `&&`         | `a && b`    | Logical AND of the left hand value and right hand value.

  
8  | `\|`         | `a \| b`    | Bit-wise OR of the left hand value and right hand value.
9  | `^`          | `a ^ b`     | Bit-wise XOR of the left hand value and right hand value.
10  | `&`          | `a & b`     | Bit-wise AND of the left hand value and right hand value.
 
11  | `>>`         | `a >> b`    | Bit-wise shifts the left hand value to the right by the number of bit in the right hand value.
11  | `<<`         | `a << b`    | Bit-wise shifts the left hand value to the left by the number of bit in the right hand value.
12  | `+` (binary) | `a + b`     | The sum of the left hand value and the right hand value.
12  | `-` (binary) | `a - b`     | The left hand value after the right hand value has been subtracted from it.
13  | `*`          | `a * b`     | The product of the left hand value and the right hand value.
13  | `/`          | `a / b`     | The left hand value divided by the right hand value.
13  | `%`          | `a % b`     | The modulo (remainder) of the left hand value by the right hand value.
14  | `**`         | `a ** b`    | The left hand value raised to the power of the right hand vale.
 
15  | `()` (cast)  | `(a) b`     | Casts the right hand value into the [type](#types) given in the parenthesis.
16  | `.`          | `a.b`       | Accesses the value in the right hand value specified by the left hand side. The left side is the receiver for the right side.

### Comparisons

6  | `==`         | `a == b`    | True if the left hand value and right hand value are equal, false otherwise.
6  | `!=`         | `a != b`    | True if the left hand value and right hand value are not equal, false otherwise.
7  | `>`          | `a > b`     | True if the left hand value is greater than the right hand value, false otherwise.
7  | `<`          | `a < b`     | True if the left hand value is less than the right hand value, false otherwise.
7  | `>=`         | `a >= b`    | True if the left hand value is greater than or equal to the right hand value, false otherwise.
7  | `<=`         | `a <= b`    | True if the left hand value is less than or equal to the right hand value, false otherwise.

## Ternary


### Switch

`?:`
`a ? b : c`
Ternary; If the left hand value ([bool](#bool)) is true then the middle value is returned otherwise the right hand value is returned.

## Variable

16  | `()` (call)  | `a(b)`      | Calls the right hand method with the zero or more parameters in the parenthesis.
