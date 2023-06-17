using Blackboard.Core.Data.Caps;
using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Functions;
using Blackboard.Core.Nodes.Inner;
using Blackboard.Core.Nodes.Outer;
using Blackboard.Core.Types;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using S = System;

namespace BlackboardTests.CoreTests;

[TestClass]
public class Benchmarks {

    /// <summary>Runs and measures a test on the given FromType implementation.</summary>
    /// <param name="title">The title to show for this implementation.</param>
    /// <param name="testHandle">The FromType implementation to test and measure.</param>
    /// <returns>The average milliseconds per </returns>
    static private double measureFromTypeImplementation(string title, S.Func<S.Type, Type> testHandle) {
        Dictionary<S.Type, Type> testTypes = new() {
                { typeof(And),                   Type.Bool },
                { typeof(InputValue<Bool>),      Type.Bool },
                { typeof(InputValue<Double>),    Type.Double },
                { typeof(InputValue<String>),    Type.String },
                { typeof(Counter<Int>),          Type.CounterInt },
                { typeof(Counter<Double>),       Type.CounterDouble },
                { typeof(Toggler),               Type.Toggler },
                { typeof(Latch<String>),         Type.LatchString },
                { typeof(OutputTrigger),         Type.Trigger },
                { typeof(Namespace),             Type.Namespace },
                { typeof(FuncGroup),             Type.FuncGroup },
                { typeof(Function<Sum<Double>>), Type.FuncDef },
                { typeof(string),                null },
            };

        S.Action testAction = () => {
            foreach (KeyValuePair<S.Type, Type> types in testTypes) {
                Type result = testHandle(types.Key);

                // Check types are equal simply instead of using assert to reduce test overhead.
                if (result != types.Value)
                    Assert.Fail("FromType({0}) returned {1} but expected {2}",
                            types.Key, result?.ToString() ?? "null", types.Value?.ToString() ?? "null");
            }
        };
        return testAction.Measure(title: title, divisor: testTypes.Count);
    }

    /// <summary>This is a benchmark comparison of the old FromType implementation to the new implementation.</summary>
    [TestMethod]
    public void FromTypeImplementations() {
        double a = measureFromTypeImplementation("Scan All Types In Reverse", Type.AllTypes.FirstAssignable);
        double b = measureFromTypeImplementation("Follow Inheritance (Current Implementation)", Type.FromType);

        Assert.IsTrue(a > b, "The current implementation ("+b+") is faster than other ("+a+")");
        S.Console.WriteLine("Current implementation is {0:P} the time of the other.", b/a);
    }
}
