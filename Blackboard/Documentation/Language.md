# The Blackboard Language

- [Comments](#comments)
- [Literals](#literals)
  - [Booleans](#booleans)
  - [Integers](#integers)
    - [Binary](#binary)
    - [Octal](#octal)
    - [Decimal](#decimal)
    - [Hexadecimal](#hexadecimal)
  - [Floats](#floats)
  - [Strings](#strings)
- [Operators](#operators)
- [Types](#types)
  - [Bool](#bool)
  - [Int](#int)
  - [Double](#double)
  - [String](#string)
  - [Trigger](#trigger)

## Comments

Comments can be added anywhere in the code and have no effect on the way the code runs.
Comments are very important to describe why something exists in the code.

Single line comments start with `//` and continue until the end of the line.

```
// One line comment
```

Multiline comments start with `/*` and continue until `*/`. This comment must be closed before the end of the file.

```
/*
Multiline
comment
*/
```

### Comment Suggestions

Let the code explain _what_ is being done and your comments explain _why_ you would bother writing that code.

| Bad                                               | Good                                             |
|:--------------------------------------------------|:-------------------------------------------------|
| This function adds the prior value to the <br>    | Gets the Fibonacci value at the given iteration. |
| current value for the given number of iterations. |                                                  |

_Your future self will love your past self for leaving good comments_

## Literals

Literals are hard coded constant values.

### Booleans

The reserved words `true` and `false` are the literals for a true boolean value and a false boolean value respectively.

### Integers

Integers literals are currently only 32-bit whole numbers with a minimum value of -2,147,483,648 and a maximum value of 2,147,483,647 inclusively.
Integers may be specified as [Binary](#binary), [Octal](#octal), [Decimal](#decimal), or [Hexadecimal](#hexadecimal).

#### Binary

Binary number literals represent [base two numbers](https://en.wikipedia.org/wiki/Binary_number).
They are ones and zeros followed by a lowercase "b", `{0-1}+b`.
For example `0b`, `101b`, and `10110101b`.

#### Octal

Octal number literals represent [base eight numbers](https://en.wikipedia.org/wiki/Octal).
They are zero through seven digits followed by a lowercase "o", `{0-7}+o`. 
For example `0o`, `223o`, and `765o`.

#### Decimal

Decimal number literals represent [base ten numbers](https://en.wikipedia.org/wiki/Decimal).
They are zero through nine digits followed by an optional lowercase "d", `{0-9}+d?`. 
For example `0`, `0d`, `91`, `91d`, and `24601`.

#### Hexadecimal

Hexadecimal number literals represent [base sixteen numbers](https://en.wikipedia.org/wiki/Hexadecimal).
They are zero through nine or "a" through "f", case insensitive,
digits preceded by a zero and lowercase "x", `0x{0-9a-fA-F}+`. 
For example `0x0`, `0xFF`, `0xffbac` and `0xFF0012`

### Floats

Floating point number literals are [IEEE-754](https://en.wikipedia.org/wiki/IEEE_754)
64-bit finite real numbers with 1-bit sign, 53-bit mantissa and 11-bit exponent.
This is the same as the typical `double` type in most languages with the same ranges and precision.

The literals are decimal digits followed by either a decimal point (`.`) and fractional part, `{0-9}+\.{0-9}+`,
or an exponential, `{0-9}+(e|E)-?{0-9}+`.
If the literal has a fractional part then it may be followed by an exponential, `{0-9}+\.{0-9}+(e|E)-?{0-9}+`.
For example `0.0`, `3.14`, `1e10`, `1e-10`, `0.001e-2`, and `12345.0`

### Strings

String literals contain zero or more characters.
Strings can either wrapped in quotes (`'`) or double quotes (`"`).

```
"This is a double quoted string"
'This is a single quoted string'
```

Strings may have escape sequences to insert special characters

| Escape Sequence | Name            | Unicode | Comment |
|:---------------:|:----------------|:--------|:--------|
| `\\`            | backslash       | 0x005C  |         |
| `\'`            | single quote    | 0x0027  | Single quotes may be used in double quote strings without being escaped. |
| `\"`            | double quote    | 0x0022  | Double quotes may be used in single quote strings without being escaped. |
| `\0`            | null            | 0x0000  |         |
| `\a`            | alert           | 0x0007  |         |
| `\b`            | backspace       | 0x0008  |         |
| `\f`            | form feed       | 0x000C  |         |
| `\n`            | newline         | 0x000A  |         |
| `\r`            | carriage return | 0x000D  |         |
| `\t`            | horizontal tab  | 0x0009  |         |
| `\v`            | vertical tab    | 0x000B  |         |
| `\xHH`          | Unicode Escape (UTF-8)  | 0x00 - 0xFF | |
| `\uHHHH`        | Unicode Escape (UTF-16) | 0x0000 - 0xFFFF | |
| `\UHHHHHHHH`    | Unicode Escape (UTF-32) | 0x00000000 - 0xFFFFFFFF | |

## Operators

Blackboard code is expressions which end with a `;`.
An expression is made up of several operations.
The following are in order of importance where the lowest order value will be performed last.
Any operation with the same order of importance is per1formed from left to right,
except for the assignment which performs right to left.

oder | symbol       | example     | meaning
----:|:------------:|:-----------:|:------
  1  | `=`          | `a = b`     | Assignment of right hand value to the left hand variable.
  2  | `?:`         | `a ? b : c` | Ternary; If the left hand value is true then the middle value is returned otherwise the right hand value is returned.
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
 15  | `+` (unary)  | `+a`        | The value unmodified.
 15  | `!`          | `!a`        | Logical NOT of the value.
 15  | `~`          | `~a`        | Bit-wise NOT of the value.
 15  | `()` (cast)  | `(a) b`     | Casts the right hand value into the [type](#types) given in the parenthesis.
 16  | `()` (group) | `(a)`       | Performs any operations internal to the parenthesis before operations outside of it.
 16  | `.`          | `a.b`       | Accesses the value in the right hand value specified by the left hand side.
 16  | `()` (call)  | `a(b)`      | Calls the right hand method with the zero or more parameters in the parenthesis.

## Types

There are currently only five build in types.
In the current version no custom types can be created and there are not parameter types.

### Bool


### Int


### Double


### String



### Trigger





===================================================================================================================================
-----------------------------------------------------------------------------------------------------------------------------------


## Inputs

TODO:

```
in A = 9;
in int B = A + 5, C = 16;
```

```
A = 46;
B = C = 33;

A = 2.14; // Error: May not assign a double to an int
```

## Rule

TODO:

```
A := 12;
int B := 24;
rule C = 36;
rule int D = B + C;
```

## Constants

Constants are values which cannot be changed.

```
const float A = 4.56;
const int B = (int)A + 1;
const C = 3.14; // Implied type

A = 2; // Error: May not modify the value of a constant.
```

When a constant is defined, it works the same as when assigning an input,
any variable/trigger in the equation is used at the current value/state.
This means if the variable is changed, the constant will not automatically update.

```
in int A = 10;
const B = A + 2; // B gets assigned to 12
A = 24;          // B is still assigned to 12
```

Even though a constant is constant, it may still be deleted.
When a constant has been deleted, any rules using the constant will continue
to use the same value. Deleting a constant simply frees up the identifier
that was used for the constant to be redefined as any value, rule, etc.
Redefining the identifier will still have no effect on any rule which used
the constant prior to it being deleted.

```
const A = 8;
B := A - 1;  // B is assigned to 7
del A;       // B is still 7
in A = 9;    // B is still 7
```

Constants can be read out of BlackBoard like any other value, rule, etc.

```
const A = 8;
get A;
```

## Outputting Items

TODO:

```
get A, B, C;
get D = A + B, E = 12;
get float F = 13.4;
```

## Triggers

TODO:

```
in trigger A, B;
```

```
-> A;
C > 3 -> B;
```

```
D := on(C > 3);
E := D || (A && B);
```


## Namespace

TODO:
