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

* Updated the version number on the plugin itself... oops!

## 1.1.0

* The skull will now only kill its target player and phase through everyone else
* Added BepInEx config support
  * Config file will be created on game startup
  * The following properties can be configured:
    * Movement speed
    * Rotation speed
    * Whether the enemy can kill players other than its target
    * Spawn rarity on all vanilla moons
* Updated the plugin name to avoid conflicts with other mods