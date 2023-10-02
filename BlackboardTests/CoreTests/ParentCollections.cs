using Blackboard.Core.Extensions;
using Blackboard.Core.Nodes.Collections;
using Blackboard.Core.Nodes.Interfaces;
using BlackboardTests.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace BlackboardTests.CoreTests;

[TestClass]
sealed public class ParentCollections {

    sealed private class TestChild : IChild {
        public TestChild(string name, int fixedCapacity = 0) {
            this.TypeName = name;
            this.Parents  = new(this, fixedCapacity);
        }

        public string TypeName { get; init; }
        public ParentCollection Parents { get; init; }
        public INode NewInstance() => new TestChild(this.TypeName);
    }

    private class TestParent : IParent {
        private readonly HashSet<IChild> childSet = new();
        public TestParent(string name) => this.TypeName = name;
        virtual public string TypeName { get; init; }
        public INode NewInstance() => new TestParent(this.TypeName);
        public IEnumerable<IChild> Children => this.childSet;
        public bool HasChild(IChild child) => this.childSet.Contains(child);
        
        public bool AddChildren(IEnumerable<IChild> children) {
            bool added = false;
            foreach (IChild child in children)
                added = this.childSet.Add(child) || added;
            return added;
        }

        public bool RemoveChildren(IEnumerable<IChild> children) {
            bool removed = false;
            foreach (IChild child in children)
                removed = this.childSet.Remove(child) || removed;
            return removed;
        }
    }

    private class TestTypedParent<T>: TestParent {
        public TestTypedParent(string name): base(name) { }
    }

    [TestMethod]
    public void NoParents() {
        TestChild child = new("Child");
        ParentCollection pc = child.Parents;

        Assert.AreEqual(child, pc.Child);
        Assert.IsFalse(pc.HasFixed);
        Assert.IsFalse(pc.HasVariable);
        Assert.AreEqual(0, pc.Count);
        Assert.AreEqual(0, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(0, pc.MinimumCount);
        Assert.AreEqual(0, pc.MaximumCount);
        Assert.AreEqual(0, pc.Parents.Count());
        Assert.AreEqual(0, pc.Types.Count());

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[index: -1]");
        TestTools.CheckException(() => _ = pc[0],
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[index: 0]");
        
        TestParent testP1 = new("One");
        TestTools.CheckException(() => pc[0] = testP1,
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[index: 0]");

        TestTools.CheckException(() => _ =pc.Insert(0, new IParent[] { testP1 }),
            "Inserting the given number of parents would cause there to be more than the maximum allowed count.",
            "[child: Child]",
            "[new parent count: 1]");
        
        TestParent testP2 = new("Two");
        Assert.IsFalse(pc.Replace(testP1, testP2));

        TestTools.CheckException(() => _ =pc.Remove(0),
            "Index, with length taken into account, is out of bounds of node's parents.",
            "[child: Child]",
            "[index: 0]",
            "[length: 1]");
    }

    [TestMethod]
    public void OneFixedParents() {
        TestChild child = new("Child");
        TestParent? parent1 = null;
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p);
        
        Assert.AreEqual(child, pc.Child);
        Assert.IsTrue(pc.HasFixed);
        Assert.IsFalse(pc.HasVariable);
        Assert.AreEqual(1, pc.Count);
        Assert.AreEqual(1, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(1, pc.MinimumCount);
        Assert.AreEqual(1, pc.MaximumCount);

        Assert.AreEqual(0, pc.Parents.Count()); // null parent not returned
        System.Type[] types = pc.Types.ToArray();
        Assert.AreEqual(1, types.Length);
        Assert.AreEqual(typeof(TestParent), types[0]);

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child()]",
            "[fixed count: 1]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        TestTools.CheckException(() => _ = pc[1],
            "Index out of bounds of node's parents.",
            "[child: Child()]",
            "[fixed count: 1]",
            "[index: 1]");
        
        TestParent testP1 = new("One");
        pc[0] = testP1;
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual("One", pc.Parents.Select(p => p.TypeName).Join(", "));

        TestTools.CheckException(() => _ =pc.Insert(0, new IParent[] { testP1 }),
            "May not insert a parent into the fixed parent part.",
            "[child: Child(One)]",
            "[fixed count: 1]",
            "[index: 0]");
        
        TestParent testP2 = new("Two");
        Assert.IsFalse(pc.Replace(testP2, testP1));
        Assert.IsTrue(pc.Replace(testP1, testP2));
        Assert.AreEqual(testP2, pc[0]);

        TestTools.CheckException(() => _ =pc.Remove(0),
            "May not remove a parent from the fixed parent part.",
            "[child: Child(Two)]",
            "[fixed count: 1]",
            "[index: 0]",
            "[length: 1]");
    }

    [TestMethod]
    public void ThreeFixedParents() {
        TestChild child = new("Child");
        TestTypedParent<bool>? parent1 = null;
        TestTypedParent<int>?  parent2 = null;
        TestTypedParent<bool>? parent3 = null;
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(() => parent3, p => parent3 = p);
        
        Assert.AreEqual(child, pc.Child);
        Assert.IsTrue(pc.HasFixed);
        Assert.IsFalse(pc.HasVariable);
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(3, pc.MinimumCount);
        Assert.AreEqual(3, pc.MaximumCount);

        Assert.AreEqual(0, pc.Parents.Count()); // null parent not returned
        System.Type[] types = pc.Types.ToArray();
        Assert.AreEqual(3, types.Length);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[0]);
        Assert.AreEqual(typeof(TestTypedParent<int>),  types[1]);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[2]);

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child()]",
            "[fixed count: 3]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        Assert.IsNull(pc[1]);
        Assert.IsNull(pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child()]",
            "[fixed count: 3]",
            "[index: 3]");
        
        TestTypedParent<bool> testP1 = new("One");
        TestTypedParent<int>  testP2 = new("Two");
        TestTypedParent<bool> testP3 = new("Three");
        pc[0] = testP1;
        pc[1] = testP2;
        pc[2] = testP3;
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        Assert.AreEqual("One, Two, Three", pc.Parents.Select(p => p.TypeName).Join(", "));

        TestTools.CheckException(() => pc[1] = testP3,
            "Incorrect type of a parent being set to a node.",
            "[child: Child(One, Two, Three)]",
            "[fixed count: 3]",
            "[index: 1]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: Three]");

        TestTools.CheckException(() => _ =pc.Insert(0, new IParent[] { testP1 }),
            "May not insert a parent into the fixed parent part.",
            "[child: Child(One, Two, Three)]",
            "[fixed count: 3]",
            "[index: 0]");
        
        TestTypedParent<bool> testP4 = new("Four");
        Assert.IsFalse(pc.Replace(testP4, testP1));
        TestTools.CheckException(() => pc.Replace(testP2, testP4),
            "Unable to replace old parent with new parent in fixed parent part.",
            "[child: Child(One, Two, Three)]",
            "[fixed count: 3]",
            "[index: 1]",
            "[old parent: Two]",
            "[new parent: Four]",
            "[target type: TestTypedParent<Int32>]");

        Assert.IsTrue(pc.Replace(testP1, testP3));
        Assert.AreEqual(testP3, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        
        Assert.IsTrue(pc.Replace(testP3, testP4));
        Assert.AreEqual(testP4, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP4, pc[2]);

        TestTools.CheckException(() => _ =pc.Remove(0),
            "May not remove a parent from the fixed parent part.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[index: 0]",
            "[length: 1]");
    }

    [TestMethod]
    public void OnlyVariableParents() {
        TestChild child = new("Child");
        List<TestTypedParent<bool>> parents = new();
        ParentCollection pc = child.Parents.
            With(parents, 1, 5);

        Assert.AreEqual(child, pc.Child);
        Assert.IsFalse(pc.HasFixed);
        Assert.IsTrue(pc.HasVariable);
        Assert.AreEqual(0, pc.Count);
        Assert.AreEqual(0, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(1, pc.MinimumCount);
        Assert.AreEqual(5, pc.MaximumCount);


        // TODO: Implement
    }

    [TestMethod]
    public void MixedParents() {
        // TODO: Add tests which checks these removal of fixed and var parents cases specifically
        //       including when the parent is used multiple times and at least one instance is not removed.
        // TODO: Test replace, set, remove with illegitimate.
    }
}
