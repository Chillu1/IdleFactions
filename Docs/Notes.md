# Notes

## Temp Notes

Resource classes:
Base resource                                           , type, value
Stored resource, stored resources in controller         , type, value,                        add, remove
Need resource                                           , type, value, baseValue, multiplier, add, remove
Cost resource                                           , type, value

So resource needs to store a reference to what Needs Type its linked & Resource Type or the specific upgrade action somehow?
So we get all multipliers of base, but also return a pooled dict of new resources & their multipliers.

Base dependent on base.
New dependent on new & baseLive (not gen?).

Create Base LiveCosts & GenerateCosts, that every Generate

* Resource needs tied to each other, by type%? (Ex. Skeleton farming upgrade consumes wildlife, but other gens aren't affected. While dark & magic affect everything 100%)
Ex. Skeleton:
  properties.SetGenerate(new[]
  {
  new ResourceCost(ResourceType.Bones, 1d) Depends 100% on Dark & Magic (all base LiveCost & GenCost), and nothing else
  });
  properties.SetCreateCost(new[]
  {
  new ResourceCost(ResourceType.Dark, 10d),
  new ResourceCost(ResourceType.Magic, 20d)
  });
  properties.SetLiveCost(new[]
  {
  new ResourceCost(ResourceType.Dark, 0.1d),
  new ResourceCost(ResourceType.Magic, 0.2d)
  });

  new Upgrade("Add food generation", new ResourceCost(ResourceType.Magic, 500d),
  upgradeActions: new UpgradeActionNewResource(ResourceNeedsType.Generate, ResourceType.Food, 0.5)), Would Depend 100% on Dark & Magic, and 100% on wildlife

Prox progression:
1 Essence Gen
1 Light or dark?
Light upgrades, 10 light = 2x light prod, 10 dark = 2x light prod
Dark upgrades, 10 dark = 2x dark prod, 10 light = 2x dark prod

1000 light, 1000 dark = unlock lava

lava needs 0.5 light to live


Early game:

Light
Essence
ark
Lava
Heat
Water,


Mid game:

Nature Plants
Mages
Warlocks
Necro
Skeletons
Demons
Golem
Monsters/Fantasy?..
Goblins
Ogres
Dwarfs
Elves


Nature Wildlife
Treant


Late game?:

Human
Drowner

Light = Active play resource?
Dark = Passive play resource?

Time resource, how to balance for afk time being 2 strong? Less time resource production the more we have it? Or a capped resource amount, 24h?

TimeFaction? Creating a special resource, time?
Extra resource "time", that get's generated when game is offline/afk?

Faction UI:
* Buttons
  * Buy population
  * Upgrades
  * Toggle generation
* Info
  * Population
  * Needs
    * Generation
    * GenerationCost
    * LiveCost
    * CreateCost
  * Rates
    * GenerationRate
    * LiveRate

## Notes
Homm like factions/cities, with building choices.
Factions interact with each other, using each other resources.


| Faction          | Generates       | NeededToCreate                                     | NeedsToLive   | NeedsToGenerate      |
|------------------|-----------------|----------------------------------------------------|---------------|----------------------|
| Divinity/Creator | Light           |                                                    |               |                      |
| Necro            | Skeletons       | Human, Energy, Dark, Skeleton?                     | Food, Mana    | Dark                 |
| Mage             | Mana            | Human, Energy, Light, Skeleton/Human?              | Food, ?       | Energy               |
| Human            | Food, Wood      | Plants, Light, Water, Skeleton?, Lifeforce/Energy? | Food, Water   | Plants               |
| Gobling/Ogre/w/e | Food, Stone     | Plants, Dark, Water, Skeleton?                     | Food, Water   |                      |
| Ocean            | Water           | Light                                              | Light         |                      |
| Warlock faction  | Magic/mana      | Human, ?                                           | Food, ?       |                      |
| Demons/Fire      | Heat/fire/light |                                                    | Light?        |                      |
| Nature           | Plants, Food-   |                                                    | Water, Light? | Light, Water         |
| Treant           | Wood            | Light, Plants, Water                               | Light, Water  | Water, Light         |
| Nature2?         | Wildlife        |                                                    | Light, Water  | Light, Water, Plants |
| Dwarf?           | Gold/treasures  |                                                    | Food          |                      |
| Void?Elders?	    | Dark            |                                                    |               | Light                |
| Evil?            | Souls/bodies    |                                                    | Dark          | Human                |
| Creation         | Energy/Essence  |                                                    |               |                      |
| Golem            | Stone           |                                                    | Human?,       |                      |
| Drowner          | Bodies          |                                                    | Water         | Human                |


First you only have base factions
Then to ex. create humans, you need skeletons, food, wildlife, wood, etc

Light/Darkness generation is probably very limited.
Makes it so player has to player around this limitation (& the limitation doesn't feel bad).

Light:
Nature, Treant, Human, Mage, Warlock, Drowner

Food is needed by:
Human,

Water is needed by:
Treants, Nature, Human

Skeletons are needed by:
Human, Drowner?, Warlock

Mana is needed by:
Warlock

## Ideas
Each faction shares some upgrades, raw production, automatic, etc.
And each faction has a unique set of buildings.

Choose "good" or "evil" start.
Makes it so you have a very basic income of light/dark.

Worlds?
Starting world has X factions, ex. need X skeletons to get to another world? Kinda meh?