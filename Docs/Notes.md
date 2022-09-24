# Notes

## Temp Notes

1e6 pop
4h population decay. 14400s
0.0144% pop decay per second?

/// <summary>
///		Sum of n ^ 1.2 for n = 0 to n
/// </summary>
public static double GetScaling12Formula(int n)
{
const double fifth = 0.0000583333;
const double fourth = 0.00175;
const double third = 0.0305416667;
const double second = 0.57525;
//const double first = 0.3819;

	return fifth * Math.Pow(n, 5) - fourth * Math.Pow(n, 4) + third * Math.Pow(n, 3) + second * Math.Pow(n, 2);
}

/// <summary>
///		Sum of n ^ 1.05 for n = 0 to n
/// </summary>
public static double GetScaling105Formula(int n)
{
const double fifth = 0.000015;
const double fourth = 0.0004166667;
const double third = 0.006325;
const double second = 0.5205666667;
//const double first = 0.47031;

	return fifth * Math.Pow(n, 5) - fourth * Math.Pow(n, 4) + third * Math.Pow(n, 3) + second * Math.Pow(n, 2);
}

Exponential Sum scales 2 hard. Have to use exponential only for specific factions?

Early game. Resource 1-1e9 light. Mid game. 1e9-1e50, Late game. 1e50-1e100?
            Pop      1-1e3,                 1e3-1e16,


n^2 no sum
1 pop = 1 cost
10 pop = 100
100 pop = 10000
100000 pop = 1e10
1e6 pop = 1e12
1e9 pop = 1e18

^1.05
1 pop = 1
10 pop = 56
100 pop = 119863
1e6 pop = 15e24

^1.2
1 pop = 1
10 pop = 76
100 pop = 444627
1e6 pop = 60e24

^1.5, scales a bit too much?
1 pop = 1 cost
10 pop = 140
100 pop = 1242965
1000 pop = 156838971753
1e6 pop = 161e24
1e9 pop = 161e39

Hover over panel
* Title
* Description/Effects
* Costs

Achievements:
* Let there be light, first light production
* Losing all population (to 1)

Rates:
Per second/5, keep changing (adding/removing) a dict of resources.
After X seconds, display the rate, & clear the dicts.

10%: Get CreateCost. GetNeededResouces, get their amounts / 10. Get the lowest of those. Buy that many.  

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