# Literals

Literals are hard coded constant values.

- [Home](./Laguage.md)
- [Booleans](#booleans)
- [Integers](#integers)
  - [Binary](#binary)
  - [Octal](#octal)
  - [Decimal](#decimal)
  - [Hexadecimal](#hexadecimal)
- [Doubles](#doubles)
- [Strings](#strings)

## Booleans

[bool](./Types.md#bool) literals are the reserved words `true` and `false`
for a true boolean value and a false boolean value respectively.

## Integers

[int](./Types.md#int) literals are currently only 32-bit whole numbers with
a minimum value of -2,147,483,648 and a maximum value of 2,147,483,647 inclusively.
Integers may be specified as [Binary](#binary), [Octal](#octal), [Decimal](#decimal), or [Hexadecimal](#hexadecimal).

### Binary

Binary number literals represent [base two numbers](https://en.wikipedia.org/wiki/Binary_number).
They are ones and zeros followed by a lowercase "b", `{0-1}+b`.
For example `0b`, `101b`, and `10110101b`.

### Octal

Octal number literals represent [base eight numbers](https://en.wikipedia.org/wiki/Octal).
They are zero through seven digits followed by a lowercase "o", `{0-7}+o`. 
For example `0o`, `223o`, and `765o`.

### Decimal

Decimal number literals represent [base ten numbers](https://en.wikipedia.org/wiki/Decimal).
They are zero through nine digits followed by an optional lowercase "d", `{0-9}+d?`. 
For example `0`, `0d`, `91`, `91d`, and `24601`.

### Hexadecimal

Hexadecimal number literals represent [base sixteen numbers](https://en.wikipedia.org/wiki/Hexadecimal).
They are zero through nine or "a" through "f", case insensitive,
digits preceded by a zero and lowercase "x", `0x{0-9a-fA-F}+`. 
For example `0x0`, `0xFF`, `0xffbac` and `0xFF0012`.

## Doubles

The [double](./Types.md#double) literals are decimal digits followed by either a decimal point (`.`)
and fractional part, `{0-9}+\.{0-9}+`, or an exponential, `{0-9}+(e|E)-?{0-9}+`.
If the literal has a fractional part then it may be followed by an exponential, `{0-9}+\.{0-9}+(e|E)-?{0-9}+`.
For example `0.0`, `3.14`, `1e10`, `1e-10`, `0.001e-2`, and `12345.0`.

## Strings

[string](./Types.md#string) literals contain zero or more characters.
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
