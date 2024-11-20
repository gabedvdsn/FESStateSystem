![fes_logo](https://github.com/user-attachments/assets/a26ba292-5a54-4000-ae70-a617f6fbafe4)

# FAR EMERALD DYNAMIC STATE SYSTEM
A fully abstract, scalable state system to represent actors in multiple states within multiple control contexts. Implemented almost entirely within scriptable objects, this system is modular and easily extendable. This project is built using native Unity and C#. 

### Breakdown of Example Material
For the purposes of this readme, I will be using the following example setup.
- The blue buttons trigger moderator transitions
- The white buttons trigger state transitions
- The yellow buttons activate conditional trigger runners
- The pink buttons retrieve actors under certain conditions
  
![image](https://github.com/user-attachments/assets/977a884b-86dd-48e6-814d-ad490ddc8140)

### ACTORS
Monobehaviours implementing the state system must implement the `StateActor` class. All functional implementation is complete and does not inherit any dependent implementation.

The `StateActor` class does, however, implement the Unity event functions `Awake()`, `Update()`, and `LateUpdate()`.

##### Inheritance
The `StateActor` class can be extended as you like. Other functionality within this system allows for type-specific retrieval of actors (see under **Actor Retrieval**).

![image](https://github.com/user-attachments/assets/97cf17f0-5e2a-4a1c-92ea-372e872d7e1c)

### GAMEPLAY STATE TAGS
The `GameplayStateTag` class is used to uniquely identify groups of actors, as well as state environments (see below). Every actor must be assigned a tag. It is important that every type of actor is assigned its own tag or that of its inherited type.

![image](https://github.com/user-attachments/assets/8c334423-8635-4772-b3c8-b54d31f45161)

### ENVIRONMENTS
The `StateEnvironment` class defines how actors are initialized using an initialization trigger (see under **Triggers**). Environments are used by the `GameplayStateManager` while setting up the scene environment.

![image](https://github.com/user-attachments/assets/8afb25ff-962f-482b-9b06-a13158d3fde4)

### GAMEPLAY STATE MANAGER
The `GameplayStateManager` class is a singleton manager class that handles initializing actors, managing subscribed actors, and performing trigger behaviors.

![image](https://github.com/user-attachments/assets/58f8c20c-0c7b-4140-a207-db55352265b7)

### STATE PRIORITY TAG
Priority tags are used to differentiate different levels (or contexts) of gameplay states and moderators.

![image](https://github.com/user-attachments/assets/dcf5e454-b7e9-42c8-aca3-56fcf75751ce)

### GAMEPLAY STATES
Gameplay states are the heart and soul of any state system. This implementation does not reinvent the wheel and defines the expected behaviors of any gameplay state. Active gameplay states are maintained within a state machine but are managed by state moderators. Gameplay states allow for both interruptions and natural conclusions.

While the gameplay state itself does not care about its priority level, the gameplay state is always defined by moderators (see below) within the context of a state priority tag. This differentiation allows the moderator to handle an actor's multiple states.

![image](https://github.com/user-attachments/assets/d7d1b047-b86c-4782-a9ea-731afd74dc36)
<details>
  <summary>Code Example</summary>
  
  ```
  public class PlantingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
  {
      public override AbstractGameplayState GenerateState(StateActor actor)
      {
          return new PlantingGameplayState(this, actor as PlayerStateActor);
      }
  }
  ```
  
  ```
public class PlantingGameplayState : AbstractPlayerGameplayState
{
        private float progress;
        private float plantSpeed = .5f;
        
        public PlantingGameplayState(AbstractGameplayStateScriptableObject gameplayState, PlayerStateActor actor) : base(gameplayState, actor)
        {
        }
        public override void Enter()
        {
            progress = 0f;
            UIManager.Instance.EnableProgressSlider("Planting");
            // Play planting animation
        }
        public override void LogicUpdate()
        {
            UIManager.Instance.SetProgressSliderValue(progress);
            progress += plantSpeed * Time.deltaTime;
            if (progress >= 1f)
            {
                Conclude();
            }
        }
        public override void PhysicsUpdate()
        {
            // Nothing needed
        }
        public override void Interrupt()
        {
            // Don't plant
        }
        public override void Conclude()
        {
            // Plant plant
            Player.Moderator.ReturnToInitial(StateData);
        }
        public override void Exit()
        {
            UIManager.Instance.DisableProgressSlider();
            // Exit planting animation
        }
}
  ```
</details>

##### GAMEPLAY STATE GROUPS
Gameplay state groups are simply collections of gameplay states. Many systems, such as moderators and conditional triggers, require state groups as opposed to individual states. This adds some overhead work while creating the state system, but the ease and additional quickness of working with state groups quickly overcome this. 

![image](https://github.com/user-attachments/assets/571ff573-3923-43a0-8c5f-37364b2a35ae)

##### INHERITANCE
Gameplay states are easily extended. Desired states are extended from the `AbstractGameplayStateScriptableObject` class and define their own `AbstractGameplayState` class, within which any state-specific implementation is defined.

### STATE MODERATORS
The `GameplayStateModerator` class is the true work-horse of the operation. Each `StateActor` holds a reference to its own moderator. Moderators represent contexts within the game that are specific to types of `StateActor`, e.g. PlayerWithinCutsceneModerator, CameraModerator, or DefaultEnemyModerator. In general, unique moderators are only needed to account for special circumstances. Similar to gameplay states, moderators have their own priority tag, which controls whether or not other moderators can be implemented (i.e. a cutscene is triggered and the PlayerWithinCutsceneModerator moderator is implemented).

Moderators allow for intimate access to the inner workings of the state system. Because the system is abstract in general, moderators handle the work of finding, storing, changing, and interrupting states, in addition to other helpful functionality.

Each moderator must define the states it allows the actor to transition between, in addition to the initial states it may impose on the actor when the moderator is implemented. 

Lastly, moderators can define a list of system change responders (see under **System Change Responders**). 

![image](https://github.com/user-attachments/assets/9d35d77a-ad52-434f-bf9f-bb59bd4ec9bf)

### TRIGGERS
Triggers are the most important aspect of the state system. Triggers are responsible for changing states, handling state-related conditionals, and initializing actors. Triggers can be configured to always produce the desired result. I will go into greater detail about the types of triggers below.

#### STATE TRIGGERS
State triggers are the #1 most important aspect of this whole system. These triggers activate transitions between states and moderators, as well as soft state resets. State triggers can be configured down to every detail to control exactly what happens during state and moderator transitions. 

![image](https://github.com/user-attachments/assets/ccd04835-2e7c-406e-9f8b-b0070e271e69)

#### CONDITIONAL TRIGGERS
Conditional triggers are used to discern the state-related circumstances of an actor. Conditional triggers are used natively by trigger runners (via `GameplayStateManager`) and by system change responders to handle actor retrieval. Conditionals are not confined to this purpose and can be used widely to handle state-dependent decisions.

![image](https://github.com/user-attachments/assets/b788efb5-ef06-4d4e-bf24-55fd223a3f59)

#### CONDITIONAL GROUP TRIGGERS
Conditional group triggers behave similarly to conditional triggers, except they hold groups of conditionals (either conditional triggers or other conditional group triggers).

![image](https://github.com/user-attachments/assets/74ef3c91-f398-430f-ae86-58c4cd3a6470)

The conditional group trigger shown in the example is used below in the example for **System Change Responders**.

#### INITIALIZATION TRIGGERS
Initialization triggers are defined in state environments to define how actors should be initialized.

![image](https://github.com/user-attachments/assets/c16c7bd0-a9f0-423d-a2fc-a9c3f311f9eb)

#### TRIGGER RUNNERS
The other aspect of triggers is trigger runners which are one of the few components of this system that are not scriptable objects. Runners are responsible for activating triggers. Runners use retrievals (under some conditional) to activate every type of trigger within various contexts, such as single actors, multiple actors, and all actors. Runners define default functionality for state actors, as well as type-specific functionality for actor-inherited types.

![image](https://github.com/user-attachments/assets/c81c1740-094b-45bb-8a24-5071e43c28f1)

### SYSTEM CHANGE RESPONDERS
System change responders are utilized by moderators to handle state and moderator transition-specific decisions using conditional triggers and actor retrievals. System change responders provide the affected actor, transitioned states, and applied conditions to support easy extendability.

![image](https://github.com/user-attachments/assets/d37f0fef-4550-4e6e-bc35-e62dd31e9a66)

### ACTOR RETRIEVAL
Actor retrievals are the means by which to find subscribed actors. Retrievals cannot access type-specific member variables (see more under **Limitations**).

![image](https://github.com/user-attachments/assets/5bb5f180-75d7-4c74-8660-949dff2778f8)

#### SYSTEM SPECIFIC RETRIEVAL
System specific retrievals are retrievals that find actors based on their moderators and/or states (active or otherwise). 

![image](https://github.com/user-attachments/assets/c7815738-9cfb-4121-b8aa-1062323151ac)

### LIMITATIONS
- The `GameplayStateManager` holds a reference to every subscribed actor. This can introduce some overhead if actors are held in multiple places.
- Actor retrieval can produce slow results if the conditions being applied to it require iteration across every subscribed actor.
  - This applies to trigger runners that perform retrievals, as well as activating possibly expensive conditional triggers.
- Actor retrievals cannot access type-specific member variables.
  - This behavior can be implemented using an attribute system such as a Unity implementation of Unreal Engine's Gameplay Ability System ([GAS](https://dev.epicgames.com/documentation/en-us/unreal-engine/gameplay-ability-system-for-unreal-engine)), where actor-related variables are bound to tag objects.
  - One Unity implementation of GAS (which I have used successfully in the past) is @sjai013's [unity-gameplay-ability-system](https://github.com/sjai013/unity-gameplay-ability-system).

### DEPENDENCIES
This system makes use of the SerializedDictionary asset by AYellowPaper (see [Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/serialized-dictionary-243052)).


