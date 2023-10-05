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

        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1 }),
            "Inserting the given number of parents would cause there to be more than the maximum allowed count.",
            "[child: Child]",
            "[new parent count: 1]");
        
        TestParent testP2 = new("Two");
        Assert.IsFalse(pc.Replace(testP1, testP2));

        TestTools.CheckException(() => _ = pc.Remove(0),
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
            "[child: Child]",
            "[fixed count: 1]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        TestTools.CheckException(() => _ = pc[1],
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[fixed count: 1]",
            "[index: 1]");
        
        TestParent testP1 = new("One");
        pc[0] = testP1;
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual("One", pc.Parents.Select(p => p.TypeName).Join(", "));

        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1 }),
            "May not insert a parent into the fixed parent part.",
            "[child: Child(One)]",
            "[fixed count: 1]",
            "[index: 0]");
        
        TestParent testP2 = new("Two");
        Assert.IsFalse(pc.Replace(testP2, testP1));
        Assert.IsTrue(pc.Replace(testP1, testP2));
        Assert.AreEqual(testP2, pc[0]);

        TestTools.CheckException(() => _ = pc.Remove(0),
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
            "[child: Child]",
            "[fixed count: 3]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        Assert.IsNull(pc[1]);
        Assert.IsNull(pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child]",
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

        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1 }),
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

        TestTools.CheckException(() => _ = pc.Remove(0),
            "May not remove a parent from the fixed parent part.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[index: 0]",
            "[length: 1]");

        Assert.IsTrue(pc.Replace(testP4, null));
        Assert.IsNull(pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.IsNull(pc[2]);
        
        Assert.IsTrue(pc.Replace(null, testP4));
        Assert.AreEqual(testP4, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP4, pc[2]);

        TestTools.CheckException(() => pc.SetAll(new List<IParent> { testP1, testP4 }),
            "Incorrect number of parents in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[new parent count: 2]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent> { testP1, testP4, testP1, testP4 }),
            "Incorrect number of parents in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[new parent count: 4]");
        
        TestTools.CheckException(() => pc.SetAll(new List<IParent> { testP1, testP4, testP2 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[index: 1]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: Four]");
        
        TestTypedParent<int> testP5 = new("Five");
        pc.SetAll(new List<IParent> { testP1, testP5, testP1 });
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP5, pc[1]);
        Assert.AreEqual(testP1, pc[2]);
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

        Assert.AreEqual(0, pc.Parents.Count()); // null parent not returned
        System.Type[] types = pc.Types.ToArray();
        Assert.AreEqual(5, types.Length);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[0]);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[1]);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[2]);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[3]);
        Assert.AreEqual(typeof(TestTypedParent<bool>), types[4]);
        
        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[variable count: 0]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: -1]");
        TestTools.CheckException(() => _ = pc[0],
            "Index out of bounds of node's parents.",
            "[child: Child]",
            "[variable count: 0]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 0]");
        
        TestTypedParent<bool> testP1 = new("One");
        TestTypedParent<bool> testP2 = new("Two");
        TestTypedParent<int>  testP3 = new("Three");
        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1, testP2, testP3 }),
            "Incorrect type of a parent in the list of parents to insert into a node.",
            "[child: Child]",
            "[variable count: 0]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[insert index: 0]",
            "[new parent index: 2]",
            "[expected type: TestTypedParent<Boolean>]",
            "[new parent: Three]");
        Assert.AreEqual(0, pc.Parents.Count()); // no parents were set
        Assert.AreEqual(0, pc.Count);
        Assert.AreEqual(0, pc.VarCount);

        Assert.IsTrue(pc.Insert(0, new IParent[] { testP1, testP2, testP2 }));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP2, pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child(One, Two, Two)]",
            "[variable count: 3]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 3]");
        Assert.AreEqual("One, Two, Two", pc.Parents.Select(p => p.TypeName).Join(", "));
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.VarCount);

        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1, testP2, testP2 }),
            "Inserting the given number of parents would cause there to be more than the maximum allowed count.",
            "[child: Child(One, Two, Two)]",
            "[variable count: 3]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[new parent count: 3]");

        Assert.IsTrue(pc.Insert(0, new IParent[] { testP2, testP1 }));
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP1, pc[1]);
        Assert.AreEqual(testP1, pc[2]);
        Assert.AreEqual(testP2, pc[3]);
        Assert.AreEqual(testP2, pc[4]);
        TestTools.CheckException(() => _ = pc[5],
            "Index out of bounds of node's parents.",
            "[child: Child(Two, One, One, Two, Two)]",
            "[variable count: 5]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 5]");
        Assert.AreEqual("Two, One, One, Two, Two", pc.Parents.Select(p => p.TypeName).Join(", "));
        Assert.AreEqual(5, pc.Count);
        Assert.AreEqual(5, pc.VarCount);

        TestTypedParent<bool> testP4 = new("Four");
        Assert.IsFalse(pc.Replace(testP4, testP1));
        Assert.IsTrue(pc.Replace(testP1, testP4));
        Assert.IsFalse(pc.Replace(testP1, testP4));
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP4, pc[1]);
        Assert.AreEqual(testP4, pc[2]);
        Assert.AreEqual(testP2, pc[3]);
        Assert.AreEqual(testP2, pc[4]);
        pc[4] = testP1;

        Assert.IsTrue(pc.Remove(2, 2));
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP4, pc[1]);
        Assert.AreEqual(testP1, pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child(Two, Four, One)]",
            "[variable count: 3]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 3]");

        TestTools.CheckException(() => pc.Remove(2, 2),
            "Index, with length taken into account, is out of bounds of node's parents.",
            "[child: Child(Two, Four, One)]",
            "[variable count: 3]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 2]",
            "[length: 2]");

        TestTools.CheckException(() => pc.Remove(0, 3),
            "Removing the given number of parents would cause there to be fewer than the minimum allowed count.",
            "[child: Child(Two, Four, One)]",
            "[variable count: 3]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 0]",
            "[length: 3]");

        Assert.IsTrue(pc.Remove(0, 2));
        Assert.AreEqual(testP1, pc[0]);
        TestTools.CheckException(() => _ = pc[1],
            "Index out of bounds of node's parents.",
            "[child: Child(One)]",
            "[variable count: 1]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 1]");
        
        Assert.IsTrue(pc.Insert(1, new IParent[] { testP2 }));
        Assert.IsTrue(pc.Insert(1, new IParent[] { testP4 }));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP4, pc[1]);
        Assert.AreEqual(testP2, pc[2]);
        pc[0] = testP2;
        pc[1] = testP2;
        pc[2] = testP4;
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP4, pc[2]);

        pc[1] = null;
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP4, pc[1]);
        
        TestTools.CheckException(() => pc[0] = testP3,
            "Incorrect type of a parent being set to a node.",
            "[child: Child(Two, Four)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 0]",
            "[expected type: TestTypedParent<Boolean>]",
            "[new parent: Three]");
        
        Assert.IsTrue(pc.Insert(2, new IParent[] { testP4, testP1, testP4 }));
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP4, pc[1]);
        Assert.AreEqual(testP4, pc[2]);
        Assert.AreEqual(testP1, pc[3]);
        Assert.AreEqual(testP4, pc[4]);
        
        Assert.IsTrue(pc.Replace(testP4, null));
        Assert.AreEqual(testP2, pc[0]);
        Assert.AreEqual(testP1, pc[1]);

        TestTools.CheckException(() => pc.SetAll(new List<IParent>()),
            "The number of parents to set is not within the allowed maximum and minimum counts.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[new parent count: 0]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent>() { testP1, testP1, testP1, testP1, testP1, testP1 }),
            "The number of parents to set is not within the allowed maximum and minimum counts.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[new parent count: 6]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent>() { testP1, testP3, testP1 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 1]",
            "[expected type: TestTypedParent<Boolean>]",
            "[new parent: Three]");

        pc.SetAll(new List<IParent>() { testP1, testP2, testP1 });
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP1, pc[2]);
    }

    [TestMethod]
    public void MixedParents() {
        TestChild child = new("Child", 3);
        TestTypedParent<bool>? parent1 = null;
        TestTypedParent<int>?  parent2 = null;
        TestTypedParent<bool>? parent3 = null;
        List<TestTypedParent<int>> parents = new();
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(() => parent3, p => parent3 = p).
            With(parents, 1, 4);

        Assert.AreEqual(child, pc.Child);
        Assert.IsTrue(pc.HasFixed);
        Assert.IsTrue(pc.HasVariable);
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(4, pc.MinimumCount);
        Assert.AreEqual(7, pc.MaximumCount);

        // TODO: Finish
    }

    // TODO: Add tests which checks these removal of fixed and var parents cases specifically
    //       including when the parent is used multiple times and at least one instance is not removed.
    // TODO: Test replace, set, remove with illegitimate.
}
