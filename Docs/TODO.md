# TODO:

* Fix resource classes
* Resource needs tied to each other, by type%? (Ex. Skeleton farming upgrade consumes wildlife, but other gens aren't affected. While dark & magic affect everything 100%)
* Separate generated population & bought?
* Unlockable faction upgrades, to unlock on X, or set resource amount

## Tests

* UpgradeActionGeneralMultiplier
* Added tests

## Ideal Product

* UI
  * Faction
    * Upgrade
      * Hover over for info panel
    * Population 1-100%
    * Art
    * Rates

## Misc

* Capping population/auto toggle generation to a limit (ex. so we don't have 1000 skeletons draining all dark & magic)
* Rates
* Buying 1-10-25-100-10%-25%-50%-Max population
* https://github.com/Razenpok/BreakInfinity.cs
* https://github.com/jtmueller/Collections.Pooled or ArrayPool, but seems that they mean pooled by pooled collections & not their items...
  * Do we even need any pooling? The objects we use frequently are all value types.