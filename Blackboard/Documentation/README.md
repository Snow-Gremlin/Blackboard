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

namespace N;
N.P := 19;

namespace M {
    A := 12;
    B := 4;
}
in int C = M.A + M.B;

namespace N {
    namespace M {
        C := 5;
    }
}

namespace N.M {
    C := 5;
}

?? Maybe?

in {
    string A;
    int B;
}

local {
    string A;
}

define {
    int B;
}

```
