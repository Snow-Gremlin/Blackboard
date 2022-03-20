# Nodes

The nodes are the internal parts of the Blackboard slate.
The nodes define a graph. The graph represents the commands which have
been defined and are running in Blackboard to update values and provokes.
Each node is an operation in the user defined equations. For example, if
two or more numbers are added together using a Sum node in the graph.

```mermaid
classDiagram

subgraph Interfaces
   IChild[/IChild/]
      IChild<--INode
   ICoalescable[/ICoalescable/]
      ICoalescable<--IChild
   IConstant[/IConstant/]
      IConstant<--INode
   IDataNode[/IDataNode/]
      IDataNode<--INode
   IEvaluable[/IEvaluable/]
      IEvaluable<--IParent
   IFieldReader[/IFieldReader/]
      IFieldReader<--INode
   IFieldWriter[/IFieldWriter/]
      IFieldWriter<--IFieldReader
   IFuncDef[/IFuncDef/]
      IFuncDef<--INode
   IFuncGroup[/IFuncGroup/]
      IFuncGroup<--INode
   IInput[/IInput/]
      IInput<--IOutput
   INaryChild[/INaryChild<T:IParent>/]
      INaryChild<--IChild
   INode[/INode/]
   IOutput[/IOutput/]
      IOutput<--INode
   IParent[/IParent/]
      IParent<--INode
   ITrigger[/ITrigger/]
      ITrigger<--INode
   ITriggerInput[/ITriggerInput/]
      ITriggerInput<--IInput
      ITriggerInput<--ITriggerParent
   ITriggerOutput[/ITriggerOutput/]
      ITriggerOutput<--IOutput
      ITriggerOutput<--ITrigger
   ITriggerParent[/ITriggerParent/]
      ITriggerParent<--ITrigger
      ITriggerParent<--IParent
   IValue[/IValue<T:IData>/]
      IValue<--IDataNode
   IValueInput[/IValueInput<T:IData>/]
      IValueInput<--IValueParent
      IValueInput<--IInput
   IValueOutput[/IValueOutput<T:IData>/]
      IValueOutput<--IValue
      IValueOutput<--IOutput
   IValueParent[/IValueParent<T:IData>/]
      IValueParent<--IValue
      IValueParent<--IParent
end

```
