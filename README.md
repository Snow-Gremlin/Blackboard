# Blackboard

A C# tool for quickly making highly interactive systems

🌱 This project hasn't even reached beta yet. It is still very new and deep in developement.

The end goal is to have a tool for setting up complex event handling.
When complete BlackBoard should be capable of:

- Handling enabling/disabling menus and buttons in response to the conditions of the application
  as well as allowing the application to react to buttons and settings.
  It should be able to help with language seleciton and unit settings too.
  This will make wed and desktop applications easier to develope and maintain.
  If done correctly, translated into multiple languages, and adopted could replace parts of systems like react, specifically the state and event system.
- Handling game state for long lasting settings such as items collected, health, character names, conversation states, regions unlocked, etc.
  Blackboard should be able to persist some states to help games save or recover from crashes.
  Blackboard would probably not be fast enought to deal with character locations nor complex enviromental states like voxel changes.
  This would hopefully be useful for RPG's and games like Firewatch or Myst.
  Since this implementation is in C#, it will be customized to work as package in Unity.
- Handling server health and message conditions which need to trigger flags or send logs for analytics.

The interface should have some of the feel of working with a database where the consumer uses a language to define the internals of Blackboard.
If databases are for massive amounts of long term information, then a "provocateur", like Blackboard, should be used for internal state and reactions
to that state in real-time. No matter the language Blackboard is being translated to, it should have a consistent language to talk to it,
just like how different kinds of databases can be accessed with a consistent language such as SQL. Blackboard will have custom quicker ways
to access the input and output or it (depending on the implementation) than always using the Blackboard language. For example, this C#
implementaiton should have accessors, events, and delegates to changet values and react to tiggers and events. 

## Help Wanted

Any help is welcome. This project needs help with:

- Need to update this readme, add an image, some diagrams, etc to make it catchy and quickly get people interested trying out Blackboard.
- Need to add Github files like `CONTRIBUTING.md` and `PULL_REQUEST_TEMPLATE`
- Need to add User Documentation
- Need to add Unit-tests and functional tests
- Need to flesh out the [Design Documentation](https://github.com/Grant-Nelson/BlackboardCSharp/blob/main/Blackboard/Documentation/Design.md)
- Need to grammar check and spell check code comments

Currenty the parser is being finished. After that the API for consuming Blackboard will be started.
