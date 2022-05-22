# The Blackboard Language

- [Comments](#comments)
- [Inputs](#inputs)
- [Rules](#rules)
- [Constants](#constants)
- [Outputting Items](#outputting-items)
- [Triggers](#triggers)
- [Namespace](#namespace)

- [Operators](./Operators.md)
- [Literals](./Literals.md)
- [Types](./Types.md)

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

## Inputs

TODO: Finish and rework

```
in A = 9;
in int B = A + 5, C = 16;
```

```
A = 46;
B = C = 33;

A = 2.14; // Error: May not assign a double to an int
```

## Rules

TODO: Finish and rework

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

TODO: Finish and rework

```
get A, B, C;
get D = A + B, E = 12;
get float F = 13.4;
```

## Triggers

TODO: Finish and rework

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

TODO: Start
