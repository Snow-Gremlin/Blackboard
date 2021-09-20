# TODO

## Language definition

- [x] Need to handle receivers in definitions
- [x] Add multiple assignment (e.g. `x=y=1`)
- [x] Add a way to read values out of a parse, get/out/return/yield
- [x] Need to make the parser not apply until later (using actions) per line
- [ ] Need to make the parser not apply several lines. This will require a special action which
      makes nodes not created yet from one line able to be referenced in a following line.
- [ ] Reduce literals (e.g. `12 + 3*2` is `18`)
- [ ] Add temp/local value which exists only in a single parse,
      used for assigning to not recalculate the same value
- [ ] Add string pad, substring, and format methods
- [ ] Add isNAN, isInf, etc methods

## Logic Types

- [ ] Make it possible to create counters, latches, and toggles of different types
- [ ] Add methods to modify the value of a counter, latch, and toggles during evaluation
      (e.g. `counterA.value = 10;`)
- [ ] Add methods to set the increment, decrement, and reset values of a counter.
      This will require the language to allow field nodes which specify how an assignment/define
      is to be set. It must be able to indicate if it can define or assign only.
      (e.g. `counterA.reset := 30;`)

## More Base Value Types

- [ ] Add uint
- [ ] Add int64
- [ ] Add uint64
- [ ] Add bytes
- [ ] Add float

## Node optimizations

- [ ] N-ary (i.e. sum, mul, xor, all, any, and, or, average, max, min) optimization
      occurs when the parent and child are the same N-ary type and both have only one child
  - For example: a sum of X and Y has a child which is a sum of the parent sum and Z,
    could be replaces with a sum of X, Y, and Z
  - If the node is communitive then the inputs can be sorted, however keep in mind
    things like string concatination which are not communitive but could be combined.
    The sorting of inputs is to help improve the speed of the "find existing" optimization
- [ ] Unary (i.e. not, negate) optimization to remove duplicates (e.g. `!!x` is just `x`)
- [ ] Logic reduction (e.g. `A&B | C&B` is `(A|C)& B`)
- [ ] Math equation reduction
- [ ] Find existing N-ary nodes to use instead of using another new node
      (e.g. `X := A + B + 3` and `Y := A + C + B` could share the `A + B` node.
      Using `z` as the `A + B` node, not a named variable, then `X := z + 3` and `Y := z + C`)
  - This is improved by sorting the inputs so it doesn't have to be combitory comparison
  - The largest match (consecutive from the front, if not communitive) should be found
- [ ] Fins other nodes to use instead of using another new node.
      For these the parents have to match one to one

## Persistance

- [ ] Come up with a way to specify which nodes need to be persisted (inputs, counters, latches, toggles, etc)
- [ ] Add persistends which also keeps track of deltas to recover if crashed before save

## Advanced Types

### Add Enums

#### Add integer backed Enums

- [ ] Enums contain one and only one value at a time. The values are custom and defined using ints.
      (e.g. `enum{1..10} S1;`, `enum{1, 3, 5} S2;`, `enum{apple, banana, orange} S3;`, and maybe even `enum{-1, 1..10, 20} S4;`)
- [ ] Enums using identifier values (e.g. `enum{cat, dog, hampster} Pet;`) work like a namespace
      which can be accessed using a receiver (e.g. `Pet.cat`).
- [ ] Enums using integers will define the type as an extension of an int type.
      The value can be implicity used as an integer.
      An enum of the same type can be assinged to an enum of the same type without being checked.
      An integer will have to be explicity cast to be assigned to an enum.
      On cast the value will checked if valid.
- [ ] Methods to help cast from an integer to an enum which can have an optional default value
      to assign on an invalid input and a trigger can be fired on invalid input.

#### Add identifier backed Enums

- [ ] Enums contain one and only one value at a time. The values are custom and use ints internally.
      (e.g. `enum{apple, banana, orange} S1;`)
- [ ] Enums using identifier values (e.g. `enum{cat, dog, hampster} Pet;`) work like a namespace
      which can be accessed using a receiver (e.g. `Pet.cat`).

### Add Sets

- [ ] Sets are unique collections which can be added to or removed from. Sets stay sourced.
      The set can be of type integers, doubles, bool, enum, etc
- [ ] Add set methods: max, min, average, sum, product, etc

### Add Lists

  - [ ] Lists are like sets but not sorted and allow more than one duplicate value in them.
  - [ ] Add list methods: push, pop, queue, dequeue, get index, set index, sort, max, min, average, sum, product, etc
  - [ ] Add methods to add/remove, enqueue/dequeue, push/pop, etc from lists or sets which are kicked off by triggers.
        These act almost like latches but are methods with a list/set, trigger, and value input

## Additional improvements

  - [ ] Allow for translations and large dictionaries
  - [ ] Add sublist/substring indexers: [x], [^x], [x..], [..y], [..^y], [x..y], [x..^y], [..]
  - [ ] List and set literals which can be used for initialization
        (e.g. `list a = [1, 1, 2, 3, 5];`, `set b = {1, 2, 3};`)
