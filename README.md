# 1314 ShadowBrook St.

## Table of Contents

- [1314 ShadowBrook St.](#1314-shadowbrook-st)
  - [Table of Contents](#table-of-contents)
  - [Game Mechanics](#game-mechanics)
    - [GameManager](#gamemanager)
      - [Overview](#overview)
    - [Checkpoint](#checkpoint)
      - [Example of Save and Load Checkpoint](#example-of-save-and-load-checkpoint)
      - [Adding a New Feature and Checkpoint Integration](#adding-a-new-feature-and-checkpoint-integration)
        - [1. Define the Feature in `Checkpoint` Class](#1-define-the-feature-in-checkpoint-class)
        - [2. Declare it in GameManager](#2-declare-it-in-gamemanager)
        - [3. Update it in GameManager](#3-update-it-in-gamemanager)
        - [4. Integrate with NewGame Method](#4-integrate-with-newgame-method)
    - [DoorController](#doorcontroller)
    - [GhostController](#ghostcontroller)
    - [SceneChangeInvokable](#scenechangeinvokable)
  - [Battle System](#battle-system)
    - [Overview](#overview-1)
    - [Initialization](#initialization)
    - [Player and Enemy Turns](#player-and-enemy-turns)
    - [Combat Actions](#combat-actions)
    - [Spell System](#spell-system)
    - [PlayerHealthBar](#playerhealthbar)

## Game Mechanics

### GameManager

#### Overview
- The `GameManager` script is a singleton class responsible for managing the game state, including player health, inventory, spells, checkpoints, and more.
- It handles the spawning of enemies, player health, coins, and provides methods for saving and loading game progress.
- The script also manages the game's spells, inventory, and scene transitions.

The `GameManager` class serves as a central controller for managing various game-related functionalities. It includes features such as:

- **Singleton Instance**: Ensures that there is only one instance of the `GameManager` throughout the game.

- **Spell Management**: Handles spells through the `SpellManager` and allows registration of spell events.

- **Input Management**: Controls player input through the `InputManager`.

- **Inventory Management**: Manages the player's inventory using the `InventoryManager`.

- **Scene and Checkpoint Handling**: Tracks the current scene, previous scene, and manages game checkpoints. It provides methods for saving and loading checkpoints.

- **Player Health and Coins**: Tracks player health and in-game currency (coins).

- **Door Interaction**: Allows the opening of doors and provides a method to check if a door can be opened.

- **Item Management**: Handles adding and removing items from the player's inventory.

- **Combat Scene Preparation**: Prepares for entering and returning from combat scenes by saving and updating relevant data.

- **Async Checkpoint Loading**: Implements asynchronous loading of checkpoints to avoid freezing the game during loading.

- **Default Spells**: Initializes default spells for the player.

- **New Game and Continue**: Provides methods for starting a new game or continuing from a saved checkpoint.

### Checkpoint

The `Checkpoint` class is a serializable class used to store and transfer the state of the game at a specific point, including:

- **Player Health**: Current health of the player.

- **Scene Spawns Dictionary**: Tracks the state of enemy spawns in each scene.

- **Play Door Sound Dictionary**: Keeps track of whether the door sound should be played in each scene.

- **Player Position Dictionary**: Stores the player's position in each scene.

- **Available Spells Dictionary**: Manages the availability of spells.

- **Scene Name, Coins, CanOpen (Door), Items**: Additional information about the current state.
#### Example of Save and Load Checkpoint

```csharp
// Save checkpoint
GameManager.Instance.SaveCheckpoint();

// Load checkpoint
GameManager.Instance.LoadCheckpoint();
```

#### Adding a New Feature and Checkpoint Integration

When introducing a new feature to the game that needs to be included in the checkpoint system, follow these steps to ensure proper saving and loading:

##### 1. Define the Feature in `Checkpoint` Class

In the `Checkpoint` class, ensure that the new feature is added to the constructor parameters. Here is an example:

```csharp
public class Checkpoint
{
    // Existing parameters...

    // New feature parameters
    public YourFeatureType YourFeature { get; private set; }

    public Checkpoint(
        // Existing parameters...
        YourFeatureType yourFeature,
        // Other parameters...
        )
    {
        // Existing assignments...

        // Assign the new feature
        this.YourFeature = yourFeature;
    }
}
```
##### 2. Declare it in GameManager
Make sure to declare it with `[SerializeField]`. For example:
```csharp
public class GameManager : MonoBehaviour
{
    // Existing parameters...
    [SerializeField] private int YourFeature;
```
##### 3. Update it in GameManager

In the GameManager script, add the loading and saving logic for the new feature in the LoadCheckpoint and SaveCheckpoint methods. Here's an example:

```csharp
public class GameManager
{
    private void SaveCheckpoint()
    {
        Checkpoint = new Checkpoint(
          // Existing code...

          //Save your feature
          YourFeatureValue;
        );
    }

    private void LoadCheckpoint()
    {
        // Existing loading logic...

        // Load the new feature
        YourFeatureValue = checkpoint.YourFeature;
    }

    // Other methods...
}
```
##### 4. Integrate with NewGame Method
If the new feature is relevant to the game's initialization, add the necessary code in the NewGame method. The following example considers the `newFeature` to be a function. If you have it as `int` or other types, initialize it with an interger (ex. 100), a string, etc.

```csharp
public class GameManager
{
    // Existing code...

    private void NewGame()
    {
      // Exisiting code
      Checkpoint = new(100, new(), new(), new(), CreateDefaultAvailableSpells(), scene, 0, false, new(), yourNewFeature());
    }

    // Other methods...
}
```


### DoorController

The `DoorController` class handles player interaction with doors. Key features include:

- **Collider and Bounds**: Utilizes Unity's collider system to detect player interaction. The `Bounds` property provides information about the collider's bounds.

- **Invokable Interface**: Implements the `Invokable` interface, allowing the door to be invoked.

- **Input-based Interaction**: Listens for player input and triggers the door invocation when specific conditions are met.

### GhostController

The `GhostController` class manages player control, movement, and interactions. It includes:

- **Dialogue Integration**: Supports dialogue UI and interactions.

- **Collider and Bounds**: Utilizes Unity's collider system for player bounds.

- **Jumping States**: Implements a state machine for jumping, including grounded, in-flight, and landing states.

- **Control Enable/Disable**: Provides methods to enable or disable player control.

- **Shop Manager Integration**: Interacts with the `ShopManager` for shop-related functionalities.

- **Animator Integration**: Utilizes Unity's Animator for character animations.

- **Rotation Handling**: Manages player rotation based on input.

- **HandleJump and FixedUpdate**: Controls player jumping and movement in the fixed update loop.

### SceneChangeInvokable

The `SceneChangeInvokable` class handles scene changes and door interactions. Key features include:

- **Animator for Transition**: Utilizes an animator for smooth scene transition effects.

- **Scene Change Coroutine**: Implements a coroutine for asynchronous scene changes.

- **Conditional Scene Change**: Allows scene changes based on specific conditions and flags.



## Battle System

### Overview

- **Battle State:** The system has various states such as `START`, `PLAYER_TURN`, `ENEMY_TURN`, `WON`, and `LOST`, representing different phases of the battle.

- **Combat Options:** Enumerated options like `Stun`, `Heal`, `Knife`, `Slam`, `Electrocute`, and `Firebolt` define possible actions during the battle.

- **Turn Actions:** The `TurnActions` class encapsulates information about a combat action, including the action type, wait time, and a corresponding function.

###  Initialization

- The battle is initialized through the `SetupBattle` coroutine, setting up initial conditions and triggering battle start listeners.
- The default spells are handled by selectionManager, once the player selects the four default spells, they'll be sent to the GameManager, and the battleSystem is able to take it out in `SetupSpells`.

###  Player and Enemy Turns

- The system manages player and enemy turns, allowing the player to choose from various combat options during their turn.

- Actions are processed asynchronously, and turn outcomes depend on the chosen actions, including dealing damage, applying spells, and handling special effects.

- The battle state transitions between `PLAYER_TURN`, `ENEMY_TURN`, and resolves with either a victory (`WON`) or defeat (`LOST`) state.

###  Combat Actions

- Various combat actions such as throwing a knife, casting spells like firebolt or lightning, and defensive moves like dodging or stunning are implemented.

- Combat actions are visualized through animations, sounds, and corresponding effects in the game environment.

###  Spell System

- The player's available spells are managed through the `SetupSpells` method, allowing the player to choose their abilities before the battle begins.

- The system registers spell event functions based on the selected spells, providing a flexible and extensible spell system.

###  PlayerHealthBar

The `PlayerHealthBar` script (it's in Player folder) manages the health of the player and enemies during combat.

- **Initialization:** The class initializes the health bar based on whether it represents the player or an enemy.

- **TakeDamage Method:** Handles damage calculation, updating health, and notifying the GameManager of changes. The method also updates the UI health bar.

- **HPManager Method:** A utility method to manage the player's health by calling the `TakeDamage` method.

- **AddCoins Method:** Adds coins to the player's inventory. (ignore this, it's here because we were too tired to move it somewhere else but we'll do that this semester hopefullt)
