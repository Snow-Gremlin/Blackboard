﻿using Blackboard.Core.Data.Caps;
using Blackboard.Core.Data.Interfaces;
using Blackboard.Core.Nodes.Bases;
using Blackboard.Core.Nodes.Interfaces;
using System.Collections.Generic;

namespace Blackboard.Core.Nodes.Inner;

/// <summary>Gets the average of all the inputs.</summary>
sealed internal class Average : NaryValue<Double, Double> {

    /// <summary>This is a factory function for creating new instances of this node easily.</summary>
    static public readonly IFuncDef Factory = CreateFactory((values) => new Average(values));

    /// <summary>Creates an average value node.</summary>
    public Average() { }

    /// <summary>Creates an average value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Average(params IValueParent<Double>[] parents) : base(parents) { }

    /// <summary>Creates an average value node.</summary>
    /// <param name="parents">The initial set of parents to use.</param>
    public Average(IEnumerable<IValueParent<Double>> parents) : base(parents) { }

    /// <summary>Creates a new instance of this node with no parents but similar configuration.</summary>
    /// <returns>The new instance of this node.</returns>
    public override INode NewInstance() => new Average();

    /// <summary>This is the type name of the node.</summary>
    public override string TypeName => nameof(Average);

    /// <summary>Gets the average of all the parent's values.</summary>
    /// <param name="values">The parents to average together.</param>
    /// <returns>The average of all the given values.</returns>
    protected override Double OnEval(IEnumerable<Double> values) {
        int count = 0;
        double sum = 0.0;
        foreach (Double value in values) {
            count++;
            sum += value.Value;
        }
        return new(count <= 0 ? 0.0 : sum/count);
    }

    /// <summary>
    /// The identity element for the node which is a constant
    /// to use when coalescing the node for optimization.
    /// </summary>
    public override IConstant Identity => Double.Zero.ToLiteral();

    /// <summary>
    /// Indicates that parents of the same type as this node may be removed and
    /// all of the parent's parents will be inserted at the same location as the parent node was.
    /// </summary>
    public override bool ParentIncorporate => false;

    /// <summary>Indicates that the parents may be reduced to the smallest set.</summary>
    public override bool ParentReducible => false;
}
