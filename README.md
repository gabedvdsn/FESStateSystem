![fes_logo](https://github.com/user-attachments/assets/a26ba292-5a54-4000-ae70-a617f6fbafe4)

# FAR EMERALD DYNAMIC STATE SYSTEM
A fully abstract, scalable state system to represent actors in multiple states within multiple control contexts. Implemented almost entirely within scriptable objects, this system is modular and easily extendable. This project is built using native Unity and C#. 

## How To Use
### Creating States
Creating states is simplified by using the **State Creator** tool under `Tools/State Creator`. By using this tool, the user can avoid the repetitive task of creating scripts and writing the necessary boilerplate code. Let's walk through using this tool.

#### First Steps
Let's start by creating a new state for the player, called **Eating**. The player would enter this state when they are actively eating. Input the name of the state.

![Screenshot 2024-11-25 225654](https://github.com/user-attachments/assets/c923265e-9974-4404-835b-9fb6a8d4d14e)

#### Assigning Inheritance
Next, let's make it inherit from the `AbstractPlayerGameplayStateScriptableObject`. There are additional options to control the naming of the scripts. Without assigning an inherited script, the state will automatically inherit from `AbstractGameplayStateScriptableObject`.

![Screenshot 2024-11-25 225720](https://github.com/user-attachments/assets/3d5dfb33-f1c4-423a-baac-cabdc53ec921)

#### Actor Targets
By assigning an inherited script, its meaningful name is derived and assigned in the `Actor Target` field. This can be changed manually.

![Screenshot 2024-11-25 225840](https://github.com/user-attachments/assets/3d3b9262-fd0c-45c5-b018-457d24a1dc83)

And that is it! Hit `Create State` and the script will automatically be created at the desired path. 

#### Abstract States
Let's take a quick look at creating an abstract class. For this demo, instead of creating a state meant for the player actor, let's create a new subset of gameplay states for a camera actor.

![Screenshot 2024-11-25 225908](https://github.com/user-attachments/assets/265e950a-811f-4a2c-9830-ffdbf907292b)

#### Next Steps
Moving forward, you will have to write any further code within the script itself.

### Creating Retrievals
Creating retrievals is easy using the **Retrieval Creator** tool, found under `Tools/Retrieval Creator`. Just like the **State Creator** tool, this allows the user to bypass the repetitive re-writing of necessary boilerplate code. In general, besides the use cases mentioned under **Limitations**, there won't be many reasons to create new retrievals, but the option is nice to have.

#### First Steps
Let's create a new retrieval, called **InView**. This retrieval would find the actors that are in view. Input the name of the retrieval.

![Screenshot 2024-11-26 142443](https://github.com/user-attachments/assets/96e67b7b-47d0-49de-9ce1-5e1799fbb3a9)

#### External Sources
Many retrievals require the use of a `State Actor` object, and therefore implement a secondary retrieval. To utilize a `State Actor` object in the retrieval, check the box labeled `Use External Source`. For the purposes of this retrieval, we want to utilize this functionality. The source `State Actor` will serve as the origin from which other we will decide if other actors are *in view*.

![Screenshot 2024-11-26 142455](https://github.com/user-attachments/assets/07836c60-fd50-4ff1-9b83-b6437ef21830)

#### External Target
Targets refer to a `StateIdentifierTagScriptableObject`, or a list of `StateIdentifierTagScriptableObject`s, depending on the toggle selected (you may only choose one or the other). 

![Screenshot 2024-11-26 142455](https://github.com/user-attachments/assets/9061c3de-e8bc-4621-82c8-7c2114f9fc70)

For the purposes of this retrieval, we want to collect a single target `StateIdentifierTagScriptableObject` to use. This way, we can iterate over each `State Actor` under that identifier tag to compare which ones are *in view*. Alternatively, we could collect a list of `StateIdentifierTagScriptableObject` and iterate over many collections of `State Actor`s.

![Screenshot 2024-11-26 142526](https://github.com/user-attachments/assets/3e043cea-07b1-4101-9e32-a3f44542f403)

#### Next Steps
After filling out the form, click **Create Retrieval**, and the script will be created at the desired path. To implement the functionality of the retrieval, open the script and write the necessary code in the `RetrieveActor(out StateActor actor)` and `RetrieveManyActors(int count, out List<StateActor> actors)` methods. The `TryRetrieveActor<T>(out T actor)` and `TryRetrieveManyActors<T>(int count, out List<T> actors)` methods can also be overridden from the base class. An example of this is shown in the `SystemSpecificStateActorRetrievalScriptableObject` script, which is an included authored script. In that case, it allows for the `Source` field to be left as `null`.

## Descriptions
In this section, I will describe the purpose of each component of the state system. 

### Breakdown of Example Material
For the purposes of this readme, I will be using the following example setup.
- The blue buttons trigger moderator transitions
- The white buttons trigger state transitions
- The yellow buttons activate conditional trigger runners
- The pink buttons retrieve actors under certain conditions
  
![image](https://github.com/user-attachments/assets/977a884b-86dd-48e6-814d-ad490ddc8140)

---

### Actors
The `StateActor` component represents an actor that behaves according to a state system. Subclassing the `StateActor` class is acceptable but functionally unnecessary; all functional implementation is complete and subclasses do not inherit any dependent implementation. The responsibility of the `StateActor` component is not to mechanically control the state system, rather just to contain it. 

Objects should define their own behavior outside of the `StateActor` class (e.g. PlayerController, EnemyController) and delegate state-related behavior to individual gameplay states that directly reference the external behavior component. Every state holds a reference to the `StateActor` component (stored as `State`) which it is attached to. When states undergo initialization (which occurs once immediately after the state is created), the state can derive pertinent components from the state component using `State.GetComponent<T>()` or similar.

##### Actor Inheritance
Although stated as functionally unnecessary previously, `StateActor` class can be extended as you like. Other functionality within this system allows for type-specific retrieval of subclassed actors (see under **Actor Retrieval**).

![image](https://github.com/user-attachments/assets/ef794b36-5f2c-4ec3-9cac-9cc719b327cb)

---

### State Identifier Tags
The `StateIdentifierTagScriptableObject` class is used to uniquely identify groups of actors, as well as state environments (see below). Every actor must be assigned a tag. It is important that every type of actor is assigned its own tag or that of its inherited type.

![image](https://github.com/user-attachments/assets/8c334423-8635-4772-b3c8-b54d31f45161)

---

### State Environments
The `StateEnvironment` class defines how actors are initialized using an initialization trigger (see under **Triggers**). Environments are used by the `GameplayStateManager` while setting up the scene environment.

![image](https://github.com/user-attachments/assets/8afb25ff-962f-482b-9b06-a13158d3fde4)

---

### Gameplay State Manager
The `GameplayStateManager` class is a singleton manager class that handles initializing actors, managing subscribed actors, and performing trigger behaviors.

![image](https://github.com/user-attachments/assets/58f8c20c-0c7b-4140-a207-db55352265b7)

---

### State Priority Tags
Priority tags are used to differentiate different levels (or contexts) of gameplay states and moderators. State actors can only be in one state at a time per defined priority level.

![image](https://github.com/user-attachments/assets/dcf5e454-b7e9-42c8-aca3-56fcf75751ce)

---

### Gameplay States
Gameplay states are the heart and soul of any state system. This implementation does not reinvent the wheel and defines the expected behaviors of any gameplay state. Active gameplay states are maintained within a state machine but are managed by state moderators. Gameplay states allow for both interruptions and natural conclusions.

While the gameplay state itself does not care about its priority level, the gameplay state is always defined by moderators (see below) within the context of a state priority tag. This differentiation allows the moderator to handle an actor's multiple states.

![image](https://github.com/user-attachments/assets/d7d1b047-b86c-4782-a9ea-731afd74dc36)
<details>
  <summary>Code Example</summary>

  ```
  public abstract class AbstractPlayerGameplayStateScriptableObject : AbstractGameplayStateScriptableObject
  {
      public abstract class AbstractPlayerGameplayState : AbstractGameplayState
      {
          public PlayerController Player;
          
          public override void Initialize()
          {
              Player = State.GetComponent<PlayerController>();
          }
          public AbstractPlayerGameplayState(AbstractGameplayStateScriptableObject stateData, StateActor actor) : base(stateData, actor)
          {
              
          }
      }
  }

  ```
  
  ```
  public class PlantingGameplayStateScriptableObject : AbstractPlayerGameplayStateScriptableObject
  {
      public override AbstractGameplayState GenerateState(StateActor actor)
      {
          return new PlantingGameplayState(this, actor);
      }
  }
  ```
  
  ```
public class PlantingGameplayState : AbstractPlayerGameplayState
  {
      private float progress;
      private float plantSpeed = .5f;
      
      public PlantingGameplayState(AbstractGameplayStateScriptableObject stateDate, StateActor actor) : base(stateDate, actor)
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

---

##### Gameplay State Groups

Gameplay state groups are simply collections of gameplay states. Many systems, such as moderators and conditional triggers, require state groups as opposed to individual states. This adds some overhead work while creating the state system, but the ease and additional quickness of working with state groups quickly overcome this. 

![image](https://github.com/user-attachments/assets/571ff573-3923-43a0-8c5f-37364b2a35ae)

##### Gameplay State Inheritance
Gameplay states are easily extended. Desired states are extended from the `AbstractGameplayStateScriptableObject` class and define their own `AbstractGameplayState` class, within which any state-specific implementation is defined. Please see above in the **How To Use** section to learn more about easily creating states.

---

### State Moderators
The `GameplayStateModerator` class is the true work-horse of the operation. Each `StateActor` holds a reference to its own moderator. Moderators represent contexts within the game that are specific to types of `StateActor`, e.g. PlayerWithinCutsceneModerator, CameraModerator, or DefaultEnemyModerator. In general, unique moderators are only needed to account for special circumstances. Similar to gameplay states, moderators have their own priority tag, which controls whether or not other moderators can be implemented (i.e. a cutscene is triggered and the PlayerWithinCutsceneModerator moderator is implemented).

Moderators allow for intimate access to the inner workings of the state system. Because the system is abstract in general, moderators handle the work of finding, storing, changing, and interrupting states, in addition to other helpful functionality.

Each moderator must define the states it allows the actor to transition between, in addition to the initial states it may impose on the actor when the moderator is implemented. 

Lastly, moderators can define a list of system change responders (see under **System Change Responders**). 

![image](https://github.com/user-attachments/assets/9d35d77a-ad52-434f-bf9f-bb59bd4ec9bf)

---

### Triggers
Triggers are the most important aspect of the state system. Triggers are responsible for changing states, handling state-related conditionals, and initializing actors. Triggers can be configured to always produce the desired result. I will go into greater detail about the types of triggers below.

#### State Triggers
State triggers are the #1 most important aspect of this whole system. These triggers activate transitions between states and moderators, as well as soft state resets. State triggers can be configured down to every detail to control exactly what happens during state and moderator transitions. 

![image](https://github.com/user-attachments/assets/ccd04835-2e7c-406e-9f8b-b0070e271e69)

---

#### Conditional Triggers
Conditional triggers are used to discern the state-related circumstances of an actor. Conditional triggers are used natively by trigger runners (via `GameplayStateManager`) and by system change responders to handle actor retrieval. Conditionals are not confined to this purpose and can be used widely to handle state-dependent decisions.

![image](https://github.com/user-attachments/assets/574c7ba4-3530-4106-8188-315966a8f221)

---

###### Conditional Trigger Groups
Conditional group triggers behave similarly to conditional triggers, except they hold groups of conditionals (either conditional triggers or other conditional group triggers).

![image](https://github.com/user-attachments/assets/74ef3c91-f398-430f-ae86-58c4cd3a6470)

The conditional group trigger shown in the example is used below in the example for **System Change Responders**.

---

#### Initialization Triggers
Initialization triggers are utilized in `StateEnvironment` objects and define the `StateModerator` and, optionally, any overriding states a `StateActor` should be initialized to.

![image](https://github.com/user-attachments/assets/c16c7bd0-a9f0-423d-a2fc-a9c3f311f9eb)

---

#### Trigger Runners
The other aspect of triggers is trigger runners which are one of the few components of this system that are not scriptable objects. Runners are responsible for activating triggers. Runners use retrievals (under some conditional) to activate every type of trigger within various contexts, such as single actors, multiple actors, and all actors. Runners define default functionality for state actors, as well as type-specific functionality for actor-inherited types.

![image](https://github.com/user-attachments/assets/c81c1740-094b-45bb-8a24-5071e43c28f1)

---

### System Change Responders
System change responders are utilized by moderators to handle state and moderator transition-specific decisions using conditional triggers and actor retrievals. System change responders provide the affected actor, transitioned states, and applied conditions to support easy extendability. Responders are useful for implementing modular state-based feedback and behavior that can make a direct impact in-game. The main use case for responders is in recognizing very specific state and moderator transitions, as opposed to implementing more complex decision-making behavior within the `Enter()` or `Exit()` methods of gameplay states (which can still be appropriate).

![image](https://github.com/user-attachments/assets/d37f0fef-4550-4e6e-bc35-e62dd31e9a66)

If either `StateConditionalTrigger` is left as `null`, it will always activate to `True`. Some responders will never activate based on the conditionals. For example, detecting a change FROM a state in *priority 0* TO a state in *priority 1* could never activate the responder. Likewise, detecting a change FROM one state TO that same state could never activate the trigger, unless the `StateTrigger` which activated the transition has `ReEnterSameStates` set to `True`.

---

### Actor Retrieval
Actor retrievals are the means by which to find subscribed actors. Retrievals cannot access type-specific member variables (see more under **Limitations**).

![image](https://github.com/user-attachments/assets/5bb5f180-75d7-4c74-8660-949dff2778f8)

---

#### System Specific Retrieval
System specific retrievals are retrievals that find actors based on their moderators and/or states (active or otherwise). 

![image](https://github.com/user-attachments/assets/c7815738-9cfb-4121-b8aa-1062323151ac)

---

## Other

### Future Work
- Add stated-related actions & state action permissibility screening
  - Actions extendable to manufactured contexts
- Add state logical continuation for state conclusion and interruption
- Robust example

##### Future Implementations
- Attribute-based system (e.g. GAS; see under **Limitations**).

### Limitations
- The `GameplayStateManager` holds a reference to every subscribed actor. This can introduce some overhead if actors are held in multiple places.
- Actor retrieval can produce slow results if the conditions being applied to it require iteration across every subscribed actor.
  - This applies to trigger runners that perform retrievals, as well as activating possibly expensive conditional triggers.
- `StateConditionalTrigger` and `SystemChangeResponse` scripts cannot access type-specific member variables.
  - The easiest solution would be to subclass these classes to align with specific behaviors (e.g. `PlayerStateConditional` and `PlayerSystemChangeResponse`) where you can ensure type safety.
  - This type of sensitive access can also be implemented using an attribute system such as a Unity implementation of Unreal Engine's Gameplay Ability System ([GAS](https://dev.epicgames.com/documentation/en-us/unreal-engine/gameplay-ability-system-for-unreal-engine)), where many actor-related variables are bound to tag objects.
    - One Unity implementation of GAS (which I have used successfully in the past) is @sjai013's [unity-gameplay-ability-system](https://github.com/sjai013/unity-gameplay-ability-system).
- `RetrieveStateActor` suffer a similar limitation

### Dependencies
This system makes use of the SerializedDictionary asset by AYellowPaper (see [Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/serialized-dictionary-243052)).


