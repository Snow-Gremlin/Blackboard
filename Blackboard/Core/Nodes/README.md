# Nodes

The nodes are the internal parts of the Blackboard slate.
The nodes define a graph. The graph represents the commands which have
been defined and are running in Blackboard to update values and provokes.
Each node is an operation in the user defined equations. For example, if
two or more numbers are added together using a Sum node in the graph.

```mermaid
flowchart TD
subgraph Interfaces

IChild[/IChild/]
    click IChild href "/Blackboard/Core/Nodes/Interfaces/IChild.cs"
    INode --> IChild

ICoalescable[/ICoalescable/]
    click ICoalescable href "/Blackboard/Core/Nodes/Interfaces/ICoalescable.cs"
    IChild --> ICoalescable

IConstant[/IConstant/]
    click IConstant href "/Blackboard/Core/Nodes/Interfaces/IConstant.cs"
    INode --> IConstant

INode[/INode/]
    click INode href "/Blackboard/Core/Nodes/Interfaces/INode.cs"
```

`TODO: Need to automatically generate the node inheritance graph`
