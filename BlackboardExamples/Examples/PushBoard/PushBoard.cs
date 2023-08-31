using BlackboardExamples.Controls;

namespace BlackboardExamples.Examples.PushBoard;

public partial class PushBoard : UserControl {




    public PushBoard() {
        this.InitializeComponent();

        this.setupPushBoard(null);
    }

    private void setupPushBoard(Blackboard.Blackboard? b) {
        this.setupTriggers(b);
        this.setupBools(b);


    }

    private void setupTriggers(Blackboard.Blackboard? b) {
        connect(this.trigger1, b);
        connect(this.trigger2, b);
        connect(this.trigger3, b);
        connect(this.trigger4, b);
        connect(this.trigger5, b);
    }

    private void setupBools(Blackboard.Blackboard? b) {
        connect(this.bool1, b);
        connect(this.bool2, b);
        connect(this.bool3, b);
        connect(this.bool4, b);
        connect(this.bool5, b);
    }



    static private void connect(IBlackBoardControl ctrl, Blackboard.Blackboard? b) {
        if (b is not null) ctrl.Connect(b);
        else ctrl.Disconnect();
    }
}
