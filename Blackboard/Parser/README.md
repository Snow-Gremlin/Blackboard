# Blackboard Parser

## Language Notes

```
// Comment
in int A;
in bool B;
in double C;
in string D;
in trigger T;

out X = A + B * C + 4;

T1 = onTrue(X > 4); // T1 = on(X > 4);
T2 = onFalse(X > 4);
T3 = onChange(X);
T4 = onOnlyOne(T1, T2, T3);
T5 = onAny(T1, T2, T3); // T5 := T1 | T2 | T2;
T6 = onAll(T1, T2, T3); // T6 := T1 & T2 & T3;

Y = max(X, 10);
Z = int(3.15); // Z = truncate(3.15);
out Z;
```

## Todo

- Need to handle receivers in definition.
- Add temp/local value which exists only in a single parse, used for assigning to not recalculate the same value
- Add multiple assignment (x=y=1)
- Add a way to read values out of a parse, get/out/return/yield
- Need to make it not apply until later
- Need to add uint, int64, uint64, and bytes
- Need to add counters, latch, and triggers
- Add Log and Ln

## Ideas

- Concatinate, Format, etc for strings
- Allow for translations and large dictionaries
- *Enum*: One and only one value at a time. The values are custom and defined using ints.
  - `enum{1..10} S1;`
  - `enum{1, 2, 3} S2;`
  - `enum{apple, banana, orange} S3;`
- *Sets*: Unique collection which can be added to or removed from. Sets stay sourced.
  Add and remove are kicked off by triggers. The set can be of type integers, doubles, bool, or enum.
  Additionally they may have max, min, average, sum, product, etc.
- *List*: Are like sets but not sorted and allow more than one duplicate.
  They have push, pop, queue, dequeue, get index, set index, sort, max, min, average, sum, product, etc.
