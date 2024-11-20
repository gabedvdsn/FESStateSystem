# FAR EMERALD DYNAMIC STATE SYSTEM
A fully abstract, scalable state system to represent actors in multiple states within multiple control contexts. Implemented almost entirely within scriptable objects, this system is modular and easily extendable. This project is built using native Unity and C#. 

![fes_logo](https://github.com/user-attachments/assets/a26ba292-5a54-4000-ae70-a617f6fbafe4)

### ACTORS
Monobehaviours implementing the state system must implement the `StateActor` class. All functional implementation is complete and does not inherit any dependent implementation.

The `StateActor` class does, however, implement the Unity event functions `Awake()`, `Update()`, and `LateUpdate()`.

##### Inheritance
The `StateActor` class can be extended as you like. Other functionality within this system allows for type-specific retrieval of actors (see under **Actor Retrieval**).

### GAMEPLAY STATE TAGS
The `GameplayStateTag` class is used to uniquely identify groups of actors. Every actor must be assigned a tag. It is important that every type of actor is assigned its own tag or that of its inherited type.

### ENVIRONMENTS
The `StateEnvironment` class defines how actors are initialized using an initialization trigger (see under **Triggers**). Environments are used by the `GameplayStateManager` while setting up the scene environment.

### GAMEPLAY STATE MANAGER
The `GameplayStateManager` class is a singleton manager class that handles initializing actors, managing subscribed actors, and performing trigger behaviors.

### STATE PRIORITY TAG
Priority tags are used to differentiate different levels (or contexts) of gameplay states and moderators.

### GAMEPLAY STATES
Gameplay states are the heart and soul of any state system. This implementation does not reinvent the wheel and defines the expected behaviors of any gameplay state. Active gameplay states are maintained within a state machine but are managed by state moderators. Gameplay states allow for both interruptions and natural conclusions.

While the gameplay state itself does not care about its priority level, the gameplay state is always defined by moderators (see below) within the context of a state priority tag. This differentiation allows the moderator to handle an actor's multiple states.

##### GAMEPLAY STATE GROUPS
Gameplay state groups are simply collections of gameplay states. Many systems, such as moderators and conditional triggers, require state groups as opposed to individual states. This adds some overhead work while creating the state system, but the ease and additional quickness of working with state groups quickly overcome this. 

##### INHERITANCE
Gameplay states are easily extended. Desired states are extended from the `AbstractGameplayStateScriptableObject` class and define their own `AbstractGameplayState` class, within which any state-specific implementation is defined.

### STATE MODERATORS
The `GameplayStateModerator` class is the true work-horse of the operation. Each `StateActor` holds a reference to its own moderator. Moderators represent contexts within the game that are specific to types of `StateActor`, e.g. PlayerWithinCutsceneModerator, CameraModerator, or DefaultEnemyModerator. In general, unique moderators are only needed to account for special circumstances. Similar to gameplay states, moderators have their own priority tag, which controls whether or not other moderators can be implemented (i.e. a cutscene is triggered and the PlayerWithinCutsceneModerator moderator is implemented).

Moderators allow for intimate access to the inner workings of the state system. Because the system is abstract in general, moderators handle the work of finding, storing, changing, and interrupting states, in addition to other helpful functionality.

Each moderator must define the states it allows the actor to transition between, in addition to the initial states it may impose on the actor when the moderator is implemented. 

Lastly, moderators can define a list of system change responders (see under **System Change Responders**). 

### TRIGGERS
Triggers are the most important aspect of the state system. Triggers are responsible for changing states, handling state-related conditionals, and initializing actors. Triggers can be configured to always produce the desired result. I will go into greater detail about the types of triggers below.

#### STATE TRIGGERS
State triggers are the #1 most important aspect of this whole system. These triggers activate transitions between states and moderators, as well as soft state resets. State triggers can be configured down to every detail to control exactly what happens during state and moderator transitions. 

#### CONDITIONAL TRIGGERS
Conditional triggers are used to discern the state-related circumstances of an actor. Conditional triggers are used natively by trigger runners (via `GameplayStateManager`) and by system change responders to handle actor retrieval. Conditionals are not confined to this purpose and can be used widely to handle state-dependent decisions.

#### INITIALIZATION TRIGGERS
Initialization triggers are defined in state environments to define how actors should be initialized.

#### TRIGGER RUNNERS
The other aspect of triggers is trigger runners which are one of the few components of this system that are not scriptable objects. Runners are responsible for activating triggers. Runners use retrievals (under some conditional) to activate every type of trigger within various contexts, such as single actors, multiple actors, and all actors. Runners define default functionality for state actors, as well as type-specific functionality for actor-inherited types.

### SYSTEM CHANGE RESPONDERS
System change responders are utilized by moderators to handle state and moderator transition-specific decisions using conditional triggers and actor retrievals. System change responders provide the affected actor, transitioned states, and applied conditions to support easy extendability.

### ACTOR RETRIEVAL
Actor retrievals are the means by which to find subscribed actors. Retrievals cannot access type-specific member variables (see more under **Limitations**).

#### SYSTEM SPECIFIC RETRIEVAL
System specific retrievals are retrievals that find actors based on their moderators and/or states (active or otherwise). 

### LIMITATIONS
- The `GameplayStateManager` holds a reference to every subscribed actor. This can introduce some overhead if actors are held in multiple places.
- Actor retrieval can produce slow results if the conditions being applied to it require iteration across every subscribed actor.
  - This applies to trigger runners that perform retrievals, as well as activating possibly expensive conditional triggers.
- Actor retrievals cannot access type-specific member variables.
  - This behavior can be implemented using an attribute system such as GAS (Gameplay Ability System), where actor-related variables are bound to tag objects. 

### DEPENDENCIES
This system makes use of the SerializedDictionary asset by AYellowPaper (see [Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/serialized-dictionary-243052)).


