# Blackboard

A C# tool for quickly making highly interactive systems

🌱 This project hasn't even reached beta yet. It is still very new and deep in development.

The end goal is to have a tool for setting up complex event handling.
When complete BlackBoard should be capable of:

- Handling enabling/disabling menus and buttons in response to the conditions of the application
  as well as allowing the application to react to buttons and settings.
  It should be able to help with language selection and unit settings too.
  This will make web and desktop applications easier to develop and maintain.
  If done correctly, translated into multiple languages, and adopted could replace parts of systems like react, specifically the state and event system.
- Handling game state for long lasting settings such as items collected, health, character names, conversation states, regions unlocked, etc.
  Blackboard should be able to persist some states to help games save or recover from crashes.
  Blackboard would probably not be fast enough to deal with character locations nor complex environmental states like voxel changes.
  This would hopefully be useful for RPG's and games like Firewatch or Myst.
  Since this implementation is in C#, it will be customized to work as package in Unity.
- Handling server health, state, status, and message conditions which need to trigger flags or send logs for analytics.

The interface should have some of the feel of working with a database where the consumer uses a language to define the internals of Blackboard.
If databases are for massive amounts of long term information, then a "provocateur", like Blackboard, should be used for internal state and reactions
to that state in real-time. No matter the language Blackboard is being translated to, it should have a consistent language to talk to it,
just like how different kinds of databases can be accessed with a consistent language such as SQL. Blackboard will have custom quicker ways
to access the input and output from it (depending on the implementation) than always using the Blackboard language. For example, this C#
implementation should have accessors, events, and delegates to change values and react to triggers and events. 

## Help Wanted

Any help is welcome. See Issues for features and problems that need to be worked on.
