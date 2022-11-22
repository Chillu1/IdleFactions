# TODO:

* First choice design, dwarf/goblin
* Prestige prototyping, after first choice design
* Progression
  * Heat-Ocean phase
  * Partial mid-phase (skeleton-necro, human-nature-treant, w/e)
  * Meaningful early (choice) Upgrades
  * Harsher upgrades, 8x bigger costs, 2x + multiplier
  * Have upgrades cost population? So they're also better timing based and not only resource based.
* Multi-resource progressiom, ex. both light & lava check? List of upgrades (with upgrades) that check every interval for X needed resources
* Rates text being displayed on factions that care about it (generate it, live it, generate cost it)
* Tabs (Wiki, Achievements, All resource rates, Settings?)
* Buying 10%-25%-50%-Max population
* Resource needs tied to each other, by type%? (Ex. Skeleton farming upgrade consumes wildlife, but other gens aren't affected. While dark & magic affect everything 100%)
* Buying next most with 10-100 population?
* New upgrade just unlocked (change color on bottom faction tab)
  * On upgrade, update bottom faction buttons (new upgrades available)
  * On upgrade discovery, update bottom faction buttons (new upgrades available)
  * Red can't afford upgrade, blue normal, can.

## Tests

* UpgradeActionGeneralMultiplier
* Added tests
* Save Load States
  * Faction
  * Upgrades
  * Resources
  * Progression

## Ideal Product

* UI
  * Faction
    * Upgrade
      * Hover over for info panel
        * Name
        * Description
        * Costs
        * Effects
    * Population 1-100%
    * Art
    * Rates

## Misc

* Dynamic to string representation of resource value
* Capping population/auto toggle generation to a limit (ex. so we don't have 1000 skeletons draining all dark & magic)
* Update current UI unlocked upgrade if factionType is same as UI
* Achivements
  * Separate generated population & bought?
  * Bonus multipliers? Then we should have more meaningful achievements, not "100 X resource"
* Offline progress (max 24h/time resource?) 
* Snapshot based rates, every X seconds, store the resource amounts, then calculate the rate based on the difference between the current & previous snapshot. Lowering the used PC resources
* Mine defense-type "oracle", that gives you a hint on what to do next
* Change UI Color with faction?