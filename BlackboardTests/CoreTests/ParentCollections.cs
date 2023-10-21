using Blackboard.Core.Inspect;
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
        public override string ToString() => Stringifier.Simple(this);
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
        Assert.IsFalse(pc.HasParents);
        Assert.AreEqual(0, pc.Count);
        Assert.AreEqual(0, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(0, pc.MinimumCount);
        Assert.AreEqual(0, pc.MaximumCount);
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
        Assert.IsTrue(pc.HasParents);
        Assert.AreEqual(1, pc.Count);
        Assert.AreEqual(1, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(1, pc.MinimumCount);
        Assert.AreEqual(1, pc.MaximumCount);
        TestTools.ValuesAreEqual(pc.Types,
            typeof(TestParent));

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child(null)]",
            "[fixed count: 1]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        TestTools.CheckException(() => _ = pc[1],
            "Index out of bounds of node's parents.",
            "[child: Child(null)]",
            "[fixed count: 1]",
            "[index: 1]");
        
        TestParent testP1 = new("One");
        pc[0] = testP1;
        Assert.AreEqual(testP1, pc[0]);

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
        Assert.IsTrue(pc.HasParents);
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(3, pc.MinimumCount);
        Assert.AreEqual(3, pc.MaximumCount);
        TestTools.ValuesAreEqual(pc.Types,
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<int>),
            typeof(TestTypedParent<bool>));

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child(null, null, null)]",
            "[fixed count: 3]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        Assert.IsNull(pc[1]);
        Assert.IsNull(pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child(null, null, null)]",
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
        TestTools.ValuesAreEqual(pc, testP1, testP2, testP3);

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

        TestTools.CheckException(() => pc.SetAll(new List<IParent?> { testP1, testP4 }),
            "Incorrect number of parents in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[new parent count: 2]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent?> { testP1, testP4, testP1, testP4 }),
            "Incorrect number of parents in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[new parent count: 4]");
        
        TestTools.CheckException(() => pc.SetAll(new List<IParent?> { testP1, testP4, testP2 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(Four, Two, Four)]",
            "[fixed count: 3]",
            "[index: 1]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: Four]");
        
        TestTypedParent<int> testP5 = new("Five");
        Assert.IsTrue(pc.SetAll(new List<IParent?> { testP1, testP5, testP1 }));
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
        Assert.IsFalse(pc.HasParents);
        Assert.AreEqual(0, pc.Count);
        Assert.AreEqual(0, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(1, pc.MinimumCount);
        Assert.AreEqual(5, pc.MaximumCount);
        TestTools.ValuesAreEqual(pc.Types,
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<bool>));
        
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
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.VarCount);
        Assert.IsTrue(pc.HasParents);

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
        TestTools.ValuesAreEqual(pc, testP2, testP4, testP4, testP2, testP2);

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

        TestTools.CheckException(() => pc.SetAll(new List<IParent?>()),
            "The number of parents to set is less than the minimum allowed count.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[new parent count: 0]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP1, testP1, testP1, testP1, testP1 }),
            "The number of parents to set is more than the maximum allowed count.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[new parent count: 6]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP3, testP1 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(Two, One)]",
            "[variable count: 2]",
            "[minimum count: 1]",
            "[maximum count: 5]",
            "[index: 1]",
            "[expected type: TestTypedParent<Boolean>]",
            "[new parent: Three]");

        Assert.IsTrue(pc.SetAll(new List<IParent?>() { testP1, testP2, testP1 }));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP1, pc[2]);
        TestTools.ValuesAreEqual(pc, testP1, testP2, testP1);
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
        Assert.IsTrue(pc.HasParents);
        Assert.AreEqual(3, pc.Count);
        Assert.AreEqual(3, pc.FixedCount);
        Assert.AreEqual(0, pc.VarCount);
        Assert.AreEqual(4, pc.MinimumCount);
        Assert.AreEqual(7, pc.MaximumCount);
        TestTools.ValuesAreEqual(pc.Types,
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<int>),
            typeof(TestTypedParent<bool>),
            typeof(TestTypedParent<int>),
            typeof(TestTypedParent<int>),
            typeof(TestTypedParent<int>),
            typeof(TestTypedParent<int>));

        TestTools.CheckException(() => _ = pc[-1],
            "Index out of bounds of node's parents.",
            "[child: Child(null, null, null, null)]",
            "[variable count: 0]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 3]",
            "[index: -1]");
        Assert.IsNull(pc[0]);
        Assert.IsNull(pc[1]);
        Assert.IsNull(pc[2]);
        TestTools.CheckException(() => _ = pc[3],
            "Index out of bounds of node's parents.",
            "[child: Child(null, null, null, null)]",
            "[variable count: 0]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 3]",
            "[index: 3]");

        TestTypedParent<bool> testP1 = new("One");
        TestTypedParent<int>  testP2 = new("Two");
        TestTypedParent<bool> testP3 = new("Three");
        TestTypedParent<int>  testP4 = new("Four");
        TestTools.CheckException(() => _ = pc.Insert(0, new IParent[] { testP1, testP2, testP4 }),
            "May not insert a parent into the fixed parent part.",
            "[child: Child(null, null, null, null)]",
            "[variable count: 0]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 3]",
            "[index: 0]");
        TestTools.CheckException(() => _ = pc.Insert(3, new IParent[] { testP1, testP2, testP4 }),
            "Incorrect type of a parent in the list of parents to insert into a node.",
            "[child: Child(null, null, null, null)]",
            "[variable count: 0]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 3]",
            "[insert index: 3]",
            "[new parent index: 0]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: One]");
        Assert.IsTrue(pc.Insert(3, new IParent[] { testP2, testP2, testP4 }));
        
        TestTools.CheckException(() => pc[0] = testP2,
            "Incorrect type of a parent being set to a node.",
            "[child: Child(null, null, null, Two, Two, Four)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 0]",
            "[expected type: TestTypedParent<Boolean>]",
            "[new parent: Two]");

        pc[0] = testP1;
        pc[1] = testP2;
        pc[2] = testP3;
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        Assert.AreEqual(testP2, pc[3]);
        Assert.AreEqual(testP2, pc[4]);
        Assert.AreEqual(testP4, pc[5]);
        TestTools.ValuesAreEqual(pc,testP1, testP2, testP3, testP2, testP2, testP4);
        
        TestTools.CheckException(() => _ = pc.Remove(0, 3),
            "May not remove a parent from the fixed parent part.",
            "[child: Child(One, Two, Three, Two, Two, Four)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 0]",
            "[length: 3]");

        TestTools.CheckException(() => _ = pc.Remove(3, 3),
            "Removing the given number of parents would cause there to be fewer than the minimum allowed count.",
            "[child: Child(One, Two, Three, Two, Two, Four)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 3]",
            "[length: 3]");

        Assert.IsTrue(pc.Remove(3, 2));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        Assert.AreEqual(testP4, pc[3]);
        
        TestTypedParent<int> testP5 = new("Five");
        Assert.IsTrue(pc.Insert(4, new IParent[] { testP5 }));
        Assert.AreEqual(testP4, pc[3]);
        Assert.AreEqual(testP5, pc[4]);
        Assert.IsTrue(pc.Insert(3, new IParent[] { testP5 }));
        Assert.AreEqual(testP5, pc[3]);
        Assert.AreEqual(testP4, pc[4]);
        Assert.AreEqual(testP5, pc[5]);
        
        TestTools.CheckException(() => _ = pc.Replace(testP1, testP5),
            "Unable to replace old parent with new parent in fixed parent part.",
            "[child: Child(One, Two, Three, Five, Four, Five)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 0]",
            "[old parent: One]",
            "[new parent: Five]",
            "[target type: TestTypedParent<Boolean>]");
        
        pc[1] = testP5;
        Assert.IsTrue(pc.Replace(testP5, testP2));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        Assert.AreEqual(testP2, pc[3]);
        Assert.AreEqual(testP4, pc[4]);
        Assert.AreEqual(testP2, pc[5]);
        Assert.IsFalse(pc.Replace(testP5, testP2));
        
        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP3, testP1, testP2, testP2, testP2 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(One, Two, Three, Two, Four, Two)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 1]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: Three]");
        
        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP2, testP1, testP2, testP1, testP2 }),
            "Incorrect type of a parent in the list of parents to set to a node.",
            "[child: Child(One, Two, Three, Two, Four, Two)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[index: 4]",
            "[expected type: TestTypedParent<Int32>]",
            "[new parent: One]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP2, testP1 }),
            "The number of parents to set is less than the minimum allowed count.",
            "[child: Child(One, Two, Three, Two, Four, Two)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[new parent count: 3]");

        TestTools.CheckException(() => pc.SetAll(new List<IParent?>() { testP1, testP2, testP1, testP2, testP2, testP2, testP2, testP2 }),
            "The number of parents to set is more than the maximum allowed count.",
            "[child: Child(One, Two, Three, Two, Four, Two)]",
            "[variable count: 3]",
            "[minimum count: 4]",
            "[maximum count: 7]",
            "[fixed count: 3]",
            "[total count: 6]",
            "[new parent count: 8]");

        Assert.IsTrue(pc.SetAll(new List<IParent?>() { testP1, testP2, testP3, testP4, testP5 }));
        Assert.AreEqual(testP1, pc[0]);
        Assert.AreEqual(testP2, pc[1]);
        Assert.AreEqual(testP3, pc[2]);
        Assert.AreEqual(testP4, pc[3]);
        Assert.AreEqual(testP5, pc[4]);
        TestTools.ValuesAreEqual(pc, testP1, testP2, testP3, testP4, testP5);
    }

    [TestMethod]
    public void AddingAndRemovingChildrenWithIndexer() {
        TestChild child = new("Child", 2);
        TestParent testP1 = new("One");
        TestParent testP2 = new("Two");
        TestParent testP3 = new("Three");
        TestParent testP4 = new("Four");
        testP1.AddChildren(child);
        testP3.AddChildren(child);

        TestParent? parent1 = testP1;
        TestParent? parent2 = testP2;
        List<TestParent> parents = new() { testP3, testP4 };
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(parents);

        Assert.IsTrue(child.Illegitimate());
        child.CheckLegitParents("[█][ ][█][ ]");
        
        TestParent testP5 = new("Five");
        Assert.IsTrue(testP1.HasChild(child));
        pc[0] = testP5;
        child.CheckLegitParents("[█][ ][█][ ]");
        Assert.IsFalse(testP1.HasChild(child));
        Assert.IsTrue(testP5.HasChild(child));

        TestParent testP6 = new("Six");
        Assert.IsFalse(testP2.HasChild(child));
        pc[1] = testP6;
        child.CheckLegitParents("[█][ ][█][ ]");
        Assert.IsFalse(testP2.HasChild(child));
        Assert.IsFalse(testP6.HasChild(child));

        TestParent testP7 = new("Seven");
        Assert.IsTrue(testP3.HasChild(child));
        pc[2] = testP7;
        child.CheckLegitParents("[█][ ][█][ ]");
        Assert.IsFalse(testP3.HasChild(child));
        Assert.IsTrue(testP7.HasChild(child));

        TestParent testP8 = new("Eight");
        Assert.IsFalse(testP4.HasChild(child));
        pc[3] = testP8;
        child.CheckLegitParents("[█][ ][█][ ]");
        Assert.IsFalse(testP4.HasChild(child));
        Assert.IsFalse(testP8.HasChild(child));
        child.CheckString("Child(Five, Six, Seven, Eight)");

        pc[1] = testP5;
        child.CheckLegitParents("[█][█][█][ ]");
        Assert.IsTrue(testP5.HasChild(child));
        Assert.IsFalse(testP6.HasChild(child));
        
        pc[1] = testP6;
        child.CheckLegitParents("[█][█][█][ ]");
        Assert.IsTrue(testP5.HasChild(child));
        Assert.IsTrue(testP6.HasChild(child));
        child.CheckString("Child(Five, Six, Seven, Eight)");
        Assert.IsTrue(child.Illegitimate());

        pc[2] = testP8;
        child.CheckLegitParents("[█][█][█][█]");
        Assert.IsTrue(testP8.HasChild(child));
        Assert.IsFalse(testP7.HasChild(child));
        child.CheckString("Child(Five, Six, Eight, Eight)"); 
        Assert.IsFalse(child.Illegitimate());

        pc[0] = null;
        child.CheckLegitParents("[ ][█][█][█]");
        Assert.IsFalse(testP5.HasChild(child));
        child.CheckString("Child(null, Six, Eight, Eight)");
        Assert.IsFalse(child.Illegitimate());

        pc[0] = testP1;
        child.CheckLegitParents("[ ][█][█][█]");
        Assert.IsFalse(testP5.HasChild(child));
        child.CheckString("Child(One, Six, Eight, Eight)");
        Assert.IsTrue(child.Illegitimate());
    }

    [TestMethod]
    public void AddingAndRemovingChildrenWithInsertAndRemove() {
        TestChild child1 = new("Child", 2);
        TestParent testP1 = new("One");
        TestParent testP2 = new("Two");
        TestParent testP3 = new("Three");
        TestParent testP4 = new("Four");
        testP1.AddChildren(child1);
        testP3.AddChildren(child1);

        TestParent? parent1 = testP1;
        TestParent? parent2 = testP2;
        List<TestParent> parents = new() { testP3, testP4 };
        ParentCollection pc1 = child1.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(parents);
        
        Assert.IsTrue(child1.Illegitimate());
        child1.CheckLegitParents("[█][ ][█][ ]");

        Assert.IsTrue(pc1.Insert(2, new IParent[] { testP1, testP2, testP1 }));
        child1.CheckLegitParents("[█][ ][█][ ][█][█][ ]");
        child1.CheckString("Child(One, Two, One, Two, One, Three, Four)");

        Assert.IsTrue(pc1.Remove(2, 2));
        child1.CheckLegitParents("[█][ ][█][█][ ]");
        child1.CheckString("Child(One, Two, One, Three, Four)");
        
        pc1[0] = null;
        child1.CheckLegitParents("[ ][ ][█][█][ ]");
        child1.CheckString("Child(null, Two, One, Three, Four)");
        Assert.IsTrue(testP1.Children.Contains(child1));
        Assert.IsTrue(testP3.Children.Contains(child1));

        Assert.IsTrue(pc1.Remove(2, 1));
        child1.CheckLegitParents("[ ][ ][█][ ]");
        child1.CheckString("Child(null, Two, Three, Four)");
        Assert.IsFalse(testP1.Children.Contains(child1));
        Assert.IsTrue(testP3.Children.Contains(child1));

        Assert.IsTrue(pc1.Remove(2, 1));
        child1.CheckLegitParents("[ ][ ][ ]");
        child1.CheckString("Child(null, Two, Four)");
        Assert.IsFalse(testP1.Children.Contains(child1));
        Assert.IsFalse(testP3.Children.Contains(child1));
        
        Assert.IsTrue(pc1.Insert(2, new IParent[] { testP1, testP3 }));
        child1.CheckLegitParents("[ ][ ][ ][ ][ ]");
        child1.CheckString("Child(null, Two, One, Three, Four)");

        TestChild child2 = new("Child", 2);
        TestParent testP5 = new("Five");
        TestParent testP6 = new("Six");
        testP5.AddChildren(child2);
        testP6.AddChildren(child2);

        TestParent? parent3 = testP5;
        TestParent? parent4 = testP6;
        ParentCollection pc2 = child2.Parents.
            With(() => parent3, p => parent3 = p).
            With(() => parent4, p => parent4 = p);
        
        child2.CheckLegitParents("[█][█]");
        child2.CheckString("Child(Five, Six)");

        TestChild child3 = new("Child", 2);
        TestParent testP7 = new("Seven");
        TestParent testP8 = new("Eight");
        testP7.AddChildren(child3);
        testP8.AddChildren(child3);

        TestParent? parent5 = testP7;
        TestParent? parent6 = testP8;
        ParentCollection pc3 = child3.Parents.
            With(() => parent5, p => parent5 = p).
            With(() => parent6, p => parent6 = p);

        child3.CheckLegitParents("[█][█]");
        child3.CheckString("Child(Seven, Eight)");

        Assert.IsTrue(pc1.Insert(2, new IParent[] { testP5, testP6, testP7, testP8 }, child2));
        child1.CheckLegitParents("[ ][ ][█][█][ ][ ][ ][ ][ ]");
        child1.CheckString("Child(null, Two, Five, Six, Seven, Eight, One, Three, Four)");
        child2.CheckLegitParents("[ ][ ]");
        child2.CheckString("Child(Five, Six)");
        child3.CheckLegitParents("[█][█]");
        child3.CheckString("Child(Seven, Eight)");
    }

    [TestMethod]
    public void AddingAndRemovingChildrenWithReplace() {
        TestChild child = new("Child", 2);
        TestParent testP1 = new("One");
        TestParent testP2 = new("Two");
        TestParent testP3 = new("Three");
        TestParent testP4 = new("Four");
        testP1.AddChildren(child);
        testP3.AddChildren(child);

        TestParent? parent1 = testP1;
        TestParent? parent2 = testP2;
        List<TestParent> parents = new() { testP3, testP4 };
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(parents);

        Assert.IsTrue(child.Illegitimate());
        child.CheckLegitParents("[█][ ][█][ ]");
        
        TestParent testP5 = new("Five");
        Assert.IsTrue(pc.Replace(testP1, testP5));
        child.CheckLegitParents("[█][ ][█][ ]");
        Assert.IsFalse(testP1.Children.Contains(child));
        Assert.IsTrue(testP5.Children.Contains(child));
        child.CheckString("Child(Five, Two, Three, Four)");

        Assert.IsTrue(pc.Replace(testP5, null));
        child.CheckLegitParents("[ ][ ][█][ ]");
        Assert.IsFalse(testP5.Children.Contains(child));
        child.CheckString("Child(null, Two, Three, Four)");
        
        Assert.IsTrue(pc.Replace(null, testP5));
        child.CheckLegitParents("[ ][ ][█][ ]");
        Assert.IsFalse(testP5.Children.Contains(child));
        child.CheckString("Child(Five, Two, Three, Four)");

        Assert.IsTrue(pc.Replace(testP5, testP3));
        child.CheckLegitParents("[█][ ][█][ ]");
        child.CheckString("Child(Three, Two, Three, Four)");
        
        Assert.IsTrue(pc.Replace(testP3, null));
        child.CheckLegitParents("[ ][ ][ ]");
        Assert.IsFalse(testP3.Children.Contains(child));
        child.CheckString("Child(null, Two, Four)");
    }

    [TestMethod]
    public void AddingAndRemovingChildrenWithSetAll() {
        TestChild child = new("Child", 2);
        TestParent testP1 = new("One");
        TestParent testP2 = new("Two");
        TestParent testP3 = new("Three");
        TestParent testP4 = new("Four");
        testP1.AddChildren(child);
        testP3.AddChildren(child);

        TestParent? parent1 = testP1;
        TestParent? parent2 = testP2;
        List<TestParent> parents = new() { testP3, testP4 };
        ParentCollection pc = child.Parents.
            With(() => parent1, p => parent1 = p).
            With(() => parent2, p => parent2 = p).
            With(parents);

        Assert.IsTrue(child.Illegitimate());
        child.CheckLegitParents("[█][ ][█][ ]");

        TestParent testP5 = new("Five");
        Assert.IsTrue(pc.SetAll(new() { testP5, testP1, testP2, testP4 }));
        child.CheckLegitParents("[ ][█][ ][ ]");
        Assert.IsTrue(testP1.Children.Contains(child));
        Assert.IsFalse(testP2.Children.Contains(child));
        Assert.IsFalse(testP3.Children.Contains(child));
        Assert.IsFalse(testP4.Children.Contains(child));
        Assert.IsFalse(testP5.Children.Contains(child));
        child.CheckString("Child(Five, One, Two, Four)");

        Assert.IsTrue(pc.SetAll(new() { testP1, testP1, testP1, testP1 }));
        child.CheckLegitParents("[█][█][█][█]");
        child.CheckString("Child(One, One, One, One)");
        Assert.IsFalse(child.Illegitimate());
        
        Assert.IsTrue(pc.SetAll(new() { testP4, testP3, testP2, testP1 }));
        child.CheckLegitParents("[█][█][█][█]");
        Assert.IsTrue(testP1.Children.Contains(child));
        Assert.IsTrue(testP2.Children.Contains(child));
        Assert.IsTrue(testP3.Children.Contains(child));
        Assert.IsTrue(testP4.Children.Contains(child));
        Assert.IsFalse(testP5.Children.Contains(child));
        child.CheckString("Child(Four, Three, Two, One)");
    }
}
