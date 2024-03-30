## 1.0.0

* Initial release

## 1.0.1

* Dropped direct dependency to BepInEx
* Updated repository URL

## 1.0.2

* Fixed terminal assets not being packed (thanks Hamunii!)

## 1.0.3

* Lowered the spawn chance from 100% to 66% while I look into adding a config

## 1.0.4

* Fixed wrong version number on the plugin

## 1.1.0

* The enemy will now only kill its target player and phase through everyone else
* Added BepInEx config support
    * Config file will be created on game startup
    * The following properties can be configured:
        * Movement speed
        * Rotation speed
        * Whether the enemy can kill players other than its target
        * Spawn rarity on all vanilla moons
* Updated the plugin name to avoid conflicts with other mods

# 1.2.0

* The enemy can spawn on modded moons now
* Added config key `ModdedRarity` to allow adjusting rarity on modded moons
    * It is currently not possible to adjust its rarity for specific modded moons, a solution is actively being worked on
* Renamed config key `ConfigCanOnlyKillTargetPlayer` to `CanOnlyCollideWithTargetPlayer` to clarify its purpose