using Blackboard.Core.Extensions;
using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

/// <summary>A control for testing black board controls.</summary>
public partial class PushBoard : UserControl, IBlackBoardControl {

    /// <summary>The collection of all blackboard control children.</summary>
    private readonly IBlackBoardControl[] ctrls;

    /// <summary>Creates a new black board test control.</summary>
    public PushBoard() {
        this.InitializeComponent();
        this.ctrls = new IBlackBoardControl[] {
            this.trigger1, this.trigger2, this.trigger3, this.trigger4, this.trigger5,
            this.bool1,    this.bool2,    this.bool3,    this.bool4,    this.bool5,
            this.int1,     this.int2,     this.int3,     this.int4,     this.int5
        };
    }
    
    /// <summary>
    /// Connects this control to the given blackboard.
    /// Any previously connected blackboard should to be disconnected first.
    /// </summary>
    /// <param name="b">The blackboard to connect to.</param>
    public void Connect(Blackboard.Blackboard b) => this.ctrls.Foreach(ctrl => ctrl.Connect(b));
    
    /// <summary>Disconnects this control from a blackboard.</summary>
    /// <remarks>This has no effect if not connected.</remarks>
    public void Disconnect() => this.ctrls.Foreach(ctrl => ctrl.Disconnect());
}
