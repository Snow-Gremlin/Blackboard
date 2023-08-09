using Blackboard.Core.Extensions;
using Blackboard.Core.Inspect;
using System.Collections.Generic;
using System.Linq;

namespace Blackboard.Parser.Builder;

/// <summary>The stack of values which are currently being worked on during a parse.</summary>
sealed internal class BuilderStack<T> {
    private readonly string usage;
    private readonly LinkedList<T> stack;

    /// <summary>Creates a new stack.</summary>
    /// <param name="usage">The short usage of this stack used for logging.</param>
    internal BuilderStack(string usage) {
        this.usage = usage;
        this.stack = new();
    }

    /// <summary>Removes all the values from this stack.</summary>
    public void Clear() => this.stack.Clear();

    /// <summary>Pushes a value onto the stack.</summary>
    /// <param name="value">The value to push.</param>
    public void Push(T value) => this.stack.AddLast(value);

    /// <summary>Peeks the value off the top of the stack without removing it.</summary>
    /// <returns>The value that is on the top of the stack.</returns>
    public T Peek() {
        LinkedListNode<T> last = this.stack.Last ??
            throw new Message("May not peek in an empty map.");
        return last.Value;
    }

    /// <summary>Pops off a value is on the top of the stack.</summary>
    /// <returns>The value which was on top of the stack.</returns>
    public T Pop() {
        T node = this.Peek();
        this.stack.RemoveLast();
        return node;
    }

    /// <summary>Pops one or more values off the stack.</summary>
    /// <param name="count">The number of values to pop.</param>
    /// <returns>The popped values in the order oldest to newest.</returns>
    public T[] Pop(int count) {
        T[] items = new T[count];
        for (int i = count-1; i >= 0; i--) {
            items[i] = this.Peek();
            this.stack.RemoveLast();
        }
        return items;
    }

    /// <summary>Gets the human readable string of the current stack.</summary>
    /// <returns>The human readable string.</returns>
    public override string ToString() => this.ToString("", false);

    /// <summary>Gets the human readable string of the current stack.</summary>
    /// <param name="indent">The indent to apply to all but the first line being returned.</param>
    /// <param name="inline">Indicates if the output should be one line or multiple lines.</param>
    /// <returns>The human readable string.</returns>
    public string ToString(string indent, bool inline) =>
        this.stack.Count <= 0 ? "[]" :
        inline ? "[" + this.stack.Join(", ") + "]" :
        "[\n" + indent + this.stack.Strings().Indent(indent).Join(",\n" + indent) + "]";
}
