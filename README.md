AutoBoss v2[.5].1
========

Automatic boss spawner for terraria. Spawns personally selected bosses and minions in defined arenas

To do
============
* Re-add minion spawning



ChangeLog:
============
 *v1.1.3
----------

* Changed default layout to stop crash OnUpdate.

 *v1.1.4
----------

* Changed OnUpdate section so layout shouldn't matter.

 *v1.1.5
----------

* Updated to ApiVersion(1, 14); Now works on Terraria 1.2

 *v1.2
---------

* Should now be able to support infinite arenas. 
* No longer needs 'arena' as default region
* Cleaned up (some) messy code, may function better now

*v2[.5]- Major modifications
------------------------------

* Rename from BossSpawner2 to AutoBoss+
* Removed usage of OnUpdate; replaced with System.Timers
* Added more error notification when /boss reload fails
* [Untested] Added ability to create custom message colours for AutoBoss messages
* Added the ability to view configuration options. Briefly added ability to change configuration options in-game,
 but removed that function
* Added some [Note] tags to the config. [Note]s can be deleted.
* Added some code comments in the _Timer. Will potentially populate the rest with comments if I get bored
* Removed some stupid stuff and made it nicer
* Fixed failed lists
* [Untested] Fixed timer still counting while bosses are alive, and then spawning more (even if the battle is not finished)

*v2[.5].1
-----------

* Added a region check for bosses that triggers every 3 seconds: If they're not in any of the boss regions, it will
 teleport them back to the first boss region in the config list.
* In addition to this, if the bosses are teleported x times (config defined), the battle is ended and the bosses are killed.
* Added changelog to README.md, because why not?

*v2.7
-----------
* Cleaned up a LOT of messy code.
* Spawns etc now work correctly
* Basically everything works better

*v3
-----------
* Fixed stuff up a lot. Spawns now work, continuous works, various other things work. Only bit missing is minion spawns
