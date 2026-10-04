# Rolling Kitchen

A first-person cooking game built in **Unity 6.6** (URP, Input System, UI Toolkit). You
run a food truck: customers walk up to the counter and order burgers and fries, and you
fetch ingredients from the fridge, slice them on the cutting board, grill and deep-fry
them, stack them into dishes on the customer's tray and serve before their patience runs
out.

**▶ [Gameplay demo](https://idk.com)**

<!-- TODO: replace the demo link and add a screenshot/GIF of gameplay -->
<!-- ![gameplay](docs/gameplay.gif) -->

## Features

- **Recipe dependency graph** - ingredients and their processes (slice, grill, deep-fry)
  are authored as ScriptableObjects; the game derives a graph from them and uses it to
  estimate each order's prep time and auto-derive customer patience
- **Food stacking** - pick up any ingredient and drop it onto another to build a burger;
  stacks merge in either direction, keep a valid bottom-to-top order and reject
  conflicting combinations
- **Cooking inside a stack** - put a stack on the griddle and only the members that can
  grill (patty, cheese) cook, each replaced in place when done
- **Mouse-driven cutting minigames** - circular sweeps around a bun and line-by-line
  slicing of tomatoes, onions and potatoes, both on one shared base class
- **Three game modes from one loop** - a campaign of levels that unlock new ingredients,
  an endless mode with a parametric difficulty ramp, and a step-based tutorial
- **UI Toolkit** interface: main menu, HUD with reputation bar and order tickets, fridge
  picker, pause and game-over screens

## How it works

The player looks at things and presses one key. Everything in the kitchen - ingredients,
stacks, the fridge, cutting board, griddle, deep fryer, trays and customers - implements
the same `IInteractable` interface, and `InteractionController` raycasts from the camera
to find whichever one is in focus.

The service loop is event-driven:

```
CustomerScheduler ── spawns ──▶ Customer walks to counter ──▶ OrderFactory builds Order
        ▲                                                            │
        │                                     patience = prep time estimate × multiplier
        │                                                            ▼
        │                       player cooks, stacks and places food on the slot's Tray
        │                                                            │
        │                                 OrderManager.TryServe (dish matching on tray)
        │                                                            ▼
        └──── Served / OrderTimedOut ──▶ ReputationController ──▶ ServiceController
                                                                 (win / lose / game over UI)
```

`ServiceConfig` (a ScriptableObject that survives the scene change) carries the selected
mode and level from the main menu into the service scene; every system then reads the
active `LevelData` through it.

---

## Technical highlights

### Recipe dependency graph

**Problem.** Every order needs a patience timer. Hand-tuning one per dish breaks the
moment a dish gains a topping or a recipe changes, and a burger with sliced tomato,
grilled patty and fries takes far longer to make than a plain one. The timer should come
from what the dish actually requires.

**Approach.** Each `IngredientData` asset lists its `IngredientProcess`es: a type
(`Slice`, `Grill`, `DeepFry`), a duration, the resulting ingredient prefabs, and
optionally a cutting minigame. A bun slices into a bottom and top bun; a raw patty grills
into a cooked patty; a potato slices into raw fries which deep-fry into fries.

`IngredientProcessGraph` loads the ingredient catalog once at startup and inverts it into
a *result → producers* index, so any ingredient can be traced back to its raw source.
On top of that index:

- **`ExpectedDuration(ingredients)`** walks each required ingredient back through its
  producers, propagating *demand*: one bun slice yields both a bottom and a top bun, so
  needing both still costs one slice, while needing two sliced tomatoes costs two tomato
  slices. Each process is counted once and multiplied by how many times it must run.
- **Process durations come from the minigame itself** when one is attached:
  `CircleCuttingMinigame` derives its time from the expected sweep speed,
  `LineCuttingMinigame` from line count × cut distance / expected input speed. Tuning a
  minigame automatically retunes every order that uses it.
- **`GetProducers`** is also used by `LevelData` to collect every ingredient a level
  needs *including its raw sources*, which is what the fridge is limited to.

`DishData` sums its required ingredients, `Order` sums its dishes, and
`CustomerScheduler` sets each customer's patience to
`order.ExpectedDuration × level.WaitTimeMultiplier`.

**Result.** Adding a new ingredient or recipe is pure data authoring - no timer to tune,
no code to touch. Randomly topped dishes get proportionally more time automatically.

### Food stacking system

**Problem.** A burger is built by dropping items onto each other in any order the player
likes: patty onto bun, bun onto patty in hand, a whole half-built burger onto a box.
Stacks must stay physically sensible (bun at the bottom, top bun on top, no two patties'
worth of buns) regardless of which side the player is holding.

**Approach.** Each `PickableData` declares a `StackingRole`, an ordered enum:
`Container < Base < Topping < Top`. An `IngredientStack` component is added on the bottom
item and owns an ordered list of members.

- **Order checking.** `FindInsertionIndex` searches, top-down, for a position where the
  incoming group can be spliced into the host so that roles never decrease from bottom
  to top. A topping can go between bun and top bun; a box can only go underneath.
- **Bidirectional merging.** `Merge` tries inserting the held group into the target, and
  if that fails, the target into the held group - so holding a bun and clicking a patty
  works the same as holding the patty and clicking the bun. If an insert lands at the
  bottom, the stack is **re-rooted** onto the new bottom item and, if the player was
  holding part of it, the hand is re-pointed at the new stack.
- **Conflict detection.** `HasRoleConflict` rejects any merge that would give one stack
  more than one container, base or top.
- **In-place cooking.** `ReplaceRoot` / `Replace` swap a member for its process results
  while keeping its position in the stack. The griddle and deep fryer advance cooking
  progress per member, so a patty with cheese on top cooks both, and fries can be scooped
  out of the fryer by clicking it with an empty box.

Members get randomized yaw rotations, at least 45° apart from their neighbours, so a
stack reads as hand-built rather than perfectly aligned.

`OrderManager` matches tray contents to orders as multisets of `PickableData`, so stack
order and duplicate dishes in one order are handled correctly.

### Decoupled interactions

**Problem.** A first-person cooking game has many different "use" targets, each with its
own rules. If the player controller knows about griddles and fryers, every new station
means editing the player.

**Approach.** One interface:

```
IInteractable
 ├── CanInteract(in InteractionContext ctx)
 ├── Interact(in InteractionContext ctx)
 └── GetInteractionPrompt(in InteractionContext ctx)
```

`InteractionContext` is a `readonly struct` carrying the held item plus the hand's
actions as delegates - `PickUp`, `Remove` (destroy), `Release` (let go without
destroying). Stations decide for themselves what to accept and what happens to the hand;
`InteractionController` only raycasts, manages focus events for the HUD prompt, and
builds the context. It never inspects what it is pointing at.

**Result.** The cutting board, griddle, deep fryer, fridge, bin, trays, ingredient crates
and customers all plug in as separate components. The tutorial reuses the same stations
and only *observes* them to decide when a step is complete.

### Mouse-driven cutting minigames

**Problem.** Slicing should feel physical - drag a knife through the food - but different
ingredients need different motions, and each one must report a realistic duration to the
recipe graph.

**Approach.** `CuttingMinigame` is an abstract base that owns the lifecycle (begin,
per-frame tick, complete/cancel callback) and the shared input primitive: the mouse ray
is intersected with a plane through the ingredient facing the camera, giving a 3D cursor
on the cutting board. When an ingredient is placed, the cutting board switches to a
point-of-interest camera and spawns the minigame prefab referenced by the ingredient's
`Slice` process.

- **`CircleCuttingMinigame`** (bun) - the knife orbits the ingredient at its bounding
  radius. The cursor's angle around the centre is computed in camera space with `atan2`;
  the knife follows at a capped angular speed, only counts motion in one direction, and
  per-frame progress is clamped so the player has to actually sweep a full 360°.
- **`LineCuttingMinigame`** (tomato, onion, potato) - a set of cut lines interpolated
  between two authored positions. Holding the mouse and moving horizontally accumulates
  cut distance along the current line; reaching the threshold completes the line and
  moves the knife to the next.

Leaving the camera (Escape) cancels the minigame cleanly through the same base class.

### Three game modes from one data-driven loop

**Problem.** Campaign, endless and tutorial play differently, but duplicating the
service loop per mode would triple the code to maintain.

**Approach.** All three run the same scene and systems. `LevelData` assets describe a
level - order count, max dishes per order, spawn intervals, patience multiplier,
reputation gain/loss, the dishes and allowed toppings, and the ingredient it unlocks -
and modes differ only in which level they read and a few switches:

- **Campaign** - levels in order; finishing all orders saves progress and shows the
  ingredient unlocked by the next level.
- **Endless** - starts from the first level's parameters and ramps them with
  `EndlessDifficulty.ForServedOrders`: a pure function that interpolates order size,
  spawn gaps, patience and reputation gain toward hard limits over a configurable number
  of served orders. Orders never run out; the score is the served count, saved as a high
  score.
- **Tutorial** - a fixed order with every topping, no patience timer, and
  `TutorialController` running a list of steps. Each step is a hint plus either a timeout
  or a completion predicate that inspects the hand, trays, griddle, fryer and cutting
  board ("cooked patty and melted cheese are on the griddle").

---

## Controls

| Action | Input |
| --- | --- |
| Move | WASD / arrow keys |
| Look | Mouse |
| Interact (pick up, place, stack, use station, serve) | E |
| Slice | Hold left mouse and drag (circular sweep or side-to-side) |
| Leave cutting board / close menu / pause | Escape |

## Running the project

1. Clone the repository.
2. Open it with **Unity 6000.6.0f1** (Unity 6.6).
3. Import the third-party assets below into `Assets/ThirdParty/`, then re-link the
   pickable data (see the following sections).
4. Open `Assets/Scenes/MainMenuScene.unity` and press **Play**.

### Third-Party Assets

These assets are not included in the repository. Obtain them from their original sources
and import them through Unity before opening `ServiceScene`.

| Asset                                  | Creator / Publisher | Source                                                                                                               |
| -------------------------------------- | ------------------- | -------------------------------------------------------------------------------------------------------------------- |
| CubexCube - Free City Pack I           | Cube x Cube         | [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/urban/cubexcube-free-city-pack-i-199815)   |
| Pandazole - Kitchen Food low poly pack | Pandazole           | [Unity Asset Store](https://assetstore.unity.com/packages/3d/props/food/pandazole-kitchen-food-low-poly-pack-204525) |
| Free Pack - Stick Man                  | PolyOne Studio      | [Unity Asset Store](https://assetstore.unity.com/packages/3d/characters/free-pack-stick-man-389802)                  |
| Toony Kitchen & Ingredients Model FREE | Sigun Studio        | [Unity Asset Store](https://assetstore.unity.com/packages/3d/props/toony-kitchen-ingredients-model-free-301805)      |
| Low-Poly Park                          | Thunderent          | [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/urban/low-poly-park-61922)                 |

### Icon Images

The UI icons are rendered from the third-party prefabs and committed alongside the data
that uses them: ingredient icons in `Assets/Features/Ingredients/<Ingredient>/Icons/`,
container icons in `Assets/Features/Container/<Container>/`. To add or re-render icons
after importing the third-party assets:

1. Open any scene in the Unity Editor.
2. Run **Tools → Generate Prefab Images…**.
3. Set **Output Folder** to the target folder (e.g. `Assets/Features/Ingredients/Tomato/Icons`),
   add the prefabs to render and press **Generate**.

Each prefab is rendered to a transparent 512×512 PNG named after the prefab
(perspective camera, Unity-preview-style 30°/30° angle). Camera angle and resolution can
be changed via the constants at the top of `Assets/Editor/PrefabImageRenderer.cs`.

### Pickable Data References

The pickable data assets in `Assets/Features/Ingredients/*/Data/` (one folder per
ingredient) and `Assets/Features/Container/*/` reference prefabs from the third-party
assets. Those prefabs are not included in the repository, so the references are broken
after cloning. After importing the third-party assets, open each data asset and re-assign
its **Prefab** (and **Icon**, where broken) reference.

## Project structure

```
Assets/
├── Core/                # Cursor lock, point-of-interest camera
├── Editor/              # Prefab → PNG icon renderer
├── Features/
│   ├── Ingredients/     # IngredientData, IngredientProcessGraph, catalog, per-ingredient assets
│   ├── Container/       # Burger and fries box data, icon generator
│   ├── Pickup/          # Pickable, Ingredient, IngredientStack, PickableData (stacking roles)
│   ├── Interaction/     # IInteractable, InteractionContext, InteractionController,
│   │                    # CuttingPlate, Griddle, Deepfryer, Fridge, Tray, Bin
│   ├── Minigame/        # CuttingMinigame, CircleCuttingMinigame, LineCuttingMinigame
│   ├── Dish/            # DishData, Order, OrderFactory, OrderManager
│   ├── Customer/        # CustomerScheduler, CustomerController
│   ├── Service/         # ServiceController, ServiceConfig, LevelData, EndlessDifficulty
│   ├── Reputation/      # ReputationController
│   ├── Tutorial/        # TutorialController
│   ├── Player/          # Movement, look, held item
│   ├── Audio/           # SFX routing
│   └── UI/              # UI Toolkit controllers + UXML/USS (menu, HUD, tickets, pause, game over)
└── Scenes/              # MainMenuScene, ServiceScene
```

## Known limitations / future work

- The prep-time estimate assumes one process runs at a time; it doesn't account for
  grilling and frying in parallel
- Third-party art is not redistributable, so the project needs manual asset import
  before it runs
