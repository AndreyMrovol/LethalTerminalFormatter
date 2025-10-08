# 0.2.31

- fixed cyclic dependency with Dawnlib and LGU

# 0.2.30

- fixed logger logging every single thing

# 0.2.29

- fixed an issue with `ItemWeights` displaying wrong cruiser prices (thanks: `narpeh`!)
- added a config for difficulty string length (thanks: `satoru_satou0121`, `bobixaboxa`!)

# 0.2.28

- fixed an issue with displaying correct store rotation when multiple nodes with the same name exists (thanks, `purpletheproto`!)
- fixed an issue with `Decorations` and `Upgrades` sections displaying empty (thanks, `barret_silver`!)

# 0.2.27

- fixed suits section displaying when no suits were available to buy (thanks: `barret_silver`, `purpletheproto`, `lunxara`!)
- fixed an issue with some items displaying wrong `Buy` and `BuyAfter` nodes (thanks, `narpeh`!)

# 0.2.26

- fixed an issue with `BuyableDecoration`s not being able to register (thanks, `readek`!)
- fixed `Bestiary` and `Storage` node not resetting correctly (thanks, `barret_silver`!)
- fixed `Store` node displaying empty section headers (thanks, `moroxide`!)

# 0.2.25

- Merged [PR #32](https://github.com/AndreyMrovol/LethalTerminalFormatter/pull/32) by [pacoito123](https://github.com/pacoito123):
  - fixed compatibility with [StoreRotationConfig](https://thunderstore.io/c/lethal-company/p/pacoito/StoreRotationConfig)
  - added suit section to the `Store` node
  - in-game changes to `Lines to Scroll` setting should now apply immediately
- bumped mrovlib version

# 0.2.24

- added bestiary and storage nodes (thanks, `barret_silver`)
- fixed an issue with store lagging the game (thanks: `lunxara`, `zaggy1024`)
- fixed an issue with `MapperRestore` mod messing store page (thanks: `narpeh`, `dopadream`)
- fixed an issue with `Store` node displaying decimal prices (thanks, `dopadream`)
- fixed warning spam during startup

# 0.2.23

- removed font size changes

# 0.2.22

- fixed an issue where moon numbers were not the same length (thanks, autumnis)
- added `ShowGroupDividerLines` option (thanks, testaccount666)

# 0.2.20

- cleaned up some code
- debug logger will now use MrovLib's settings
- set terminal's font size to not change in moons catalogue (sorry, darmuh)
- things will be correctly reset on lobby reload (thanks, bbapepsiman)
- reworked how the nodes are replaced (thanks: impulsivelass, explodingcore)
- fixed an issue with `Moons` node not displaying LLL's override info (thanks, explodingcore)
- fixed an issue with moon numbers not being aligned correctly (thanks, autumnis)

# 0.2.19

- fixed an issue with `specialNodes.ToArray()` returning null values (thanks, [darmuh](https://github.com/darmuh))
- fixed an issue with Company Cruiser appearing multiple times (thanks: s1ckboy, potteur)

# 0.2.18

- added support for v55's BuyableVehicles
- fixed an issue with `Store` node not separating upgrades into groups correctly

# 0.2.17

- bumped MrovLib dependency

# 0.2.16

- fixed Logger returning null values

# 0.2.15

- updated MrovLib dependency to 0.1.1
- logs are now toggle-able

# 0.2.14

- added a working scroll mechanic (thank you, pacoito123! )
- added optional decorations to the output
- added an option to display moon numbers
- implemented changes proposed in [#13](https://github.com/AndreyMrovol/LethalTerminalFormatter/issues/13) and [#21](https://github.com/AndreyMrovol/LethalTerminalFormatter/issues/21)
- fixed an issue with `new TerminalNode()` (thanks, diffoz)
- changed some logs

# 0.2.13

- fixed an issue with routes showing up multiple times after reloading lobby (thanks, monty!)

# 0.2.12

- fixed an issue with no-LLL moon list not using LQ values (thanks, monty!)

# 0.2.11

- fixed an issue with latest LateGameUpgrades compatibility patch

# 0.2.10

- changed the displaying format of LLl's `Moons` node to avoid leaving 1-item groups
- fixed an issue with displaying moon groups when there aren't many moons present
- added a support for LLL's `RouteLocked` option

# 0.2.9

- added a warning to be displayed when filtering by TAG

# 0.2.8

- added `RouteAfter` node: the success screen after routing
- changed `Route` node to use the same system as `RouteAfter`

# 0.2.7

- hopefully finally fixed all buynode-related issues
- LethalQuantities risk level is now displayed correctly ([#15](https://github.com/AndreyMrovol/LethalTerminalFormatter/issues/15))

# 0.2.6

- fixed a missing `BuyAfter` node (thanks, [mari0no1](https://github.com/Mari0no1))
- store's Decorations section is divided into groups (same as Items)

# 0.2.5

- fixed an issue with BuyableThing constructor crashing when price of modded item is 0 (thanks, [mari0no1](https://github.com/Mari0no1))

# 0.2.4

- fixed issues with `Buy` nodes not displaying correctly (or at all) (again) ([#12](https://github.com/AndreyMrovol/LethalTerminalFormatter/issues/12))
- fixed vanilla scrollbar being a variable mess (thank you, [Major-Scott](https://github.com/Major-Scott/TerminalPlus))

# 0.2.3

- fixed issues with `Buy` nodes not displaying correctly (or at all)

# 0.2.2

- fixed all issues with old LLL versions being used
- fixed issues with WeatherTweaks not displaying uncertain weather
- fixed issues with `simulate` not working

# 0.2.1

- i forgot that LLL is no longer needed

# 0.2.0

probably the biggest to date

- definitely fixed all issues with LLL 1.2.0
- added `Buy` node: buying items/unlockables/decorations
- added `BuyAfter` node: the success screen after buying
- added `CannotAfford` node: the failure screen after buying
- added the system for easier management of registered items and unlockables
- changed the way difficulty is displayed in the LLL node (thanks, [mari0no1](https://github.com/Mari0no1))
- added support for LGU's "Efficient Engines" upgrade: the correct prices will show up in moon catalogue

# 0.1.5

- fixed all issues with LLL 1.2.0

# 0.1.4

- added MrovLib as a dependency

# 0.1.3

- items disabled in LethalLib will not show up in store
- fixed WeatherTweaks integration: `None` weather will show up as ``

# 0.1.2

- added option to disable specific nodes

# 0.1.1

- WeatherTweaks support

# 0.1.0

- Initial v50 version
- completely reworked everything lmfao

# 0.0.16

- changed how `route` screen looks

# 0.0.15

- fixed issues with LGU store
- added option to shorten weather names

# 0.0.14

- fixed issue with LGU not displaying correct levels
- fixed issue with dividing by 0 in store page
- fixed issue with LethalRegeneration showing up when disabled
- fixed issue with `Sector-0` simulate command errors
- added some logging messages

# 0.0.13

- fixed issue with LGU not being present in the modlist (thanks, sfDesat!)

# 0.0.12

- LethalRegeneration support
- LateGameUpgrades store support
- No detailed scan config option
- changed the underlying library for creating tables to make my life easier

# 0.0.11

- fixed `moons` not displaying in groups

# 0.0.10

- changed `simulate` command
- changed `route` command
- fixed some things inside formatting tables
- changed `The Company` display on moons page
- fixed the issue with using `view monitor` in terminal
- last used filter/sort/preview settings are saved and loaded on game reload

# 0.0.8

- removed some extensive logging

# 0.0.7

- fix company buying % bug

# 0.0.6

- added vanilla moon list support
- changed `scan` command

# 0.0.5

- AdvancedCompany compatibility
- Company building buying % fixed
