//using Entitas.Unity;
//
//public class PropertiesCreate : IPerformanceTest {
//    const int n = 100;
//
//
//    public void Before() {
//    }
//
//    public void Run() {
//        for (int i = 0; i < n; i++) {
//            new Properties(input);
//        }
//    }
//
//    const string input = @"quests.task.description.tut_001 = Tap on the BUILD button
//quests.task.hint.tut_001 = Welcome to Doge!\nPlease follow the tutorial closely.
//quests.task.description.tut_002 = Build 1 HOUSE
//quests.task.hint.tut_002 = Tap on the House Button and place the building next to a canal.
//quests.task.description.tut_003 = Tap on the HOUSE
//quests.task.hint.tut_003 = You can tap buildings to learn what they produce
//quests.task.description.tut_004 = Tap anywhere to close the menu
//quests.task.hint.tut_004 = The House produces 1 worker and 5 coins every 3 minutes.
//quests.task.general.building.build = Build {amount}x {type}.
//quests.task.general.building.upgrade = Upgrade {type} to level {value}.
//quests.task.general.building.build.hint = You'll find the buildings in the shop. Tap the shop button to open it.
//quests.task.general.building.upgrade.hint = Tap on a building to open the building menu. Then press the upgrade button.
//quests.task.general.resource.production = Have {amount} of {type} in your inventory.
//quests.task.general.resource.production.hint = Produce resources in your refineries and on islands.
//quests.task.general.resource.crafting = Craft {amount} of {type}.
//quests.task.general.resource.crafting.hint = Tap on the crafting building to open the crafting menu.
//quests.task.general.resource.trading = Trade {amount}x {type} with a trading city.
//quests.task.general.resource.trading.hint = Tap on a trading city in the world view and hit the trade icon.
//quests.task.general.unlock_sector = Explore sector {value}.
//quests.task.general.unlock_sector.hint = Tap on a locked sector in the world view.
//quests.task.general.colonize_island = Tap on a unclaimed island that produces {type}.
//quests.task.general.colonize_island.hint = If you don't see a unclaimed islands, you'll have to explore more sectors.
//quests.task.general.select = Select the {type}.
//quests.task.general.select.hint = Tap on the {type} to select it.
//quests.task.general.show_overlay = Open the {type} ({value}).
//quests.task.general.show_overlay.hint = Find out which button you have to press to open the popup.
//quests.task.general.show_game_view = Switch to {type} view. 
//quests.task.general.show_game_view.hint = Tap on the button in the lower right corner.
//quests.task.general.research_tech = Research technology {type}
//quests.task.general.research_tech.hint = Technology can be research in Academy or Arsenal. 
//quests.task.general.capacity = Increase Capacty of resource {type} to {value}.
//quests.task.general.capacity.hint = Capacity can be increased through upgrading houses, town hall or storage.
//quests.task.general.upgrade = Upgrade {type} to level {value}.
//quests.task.general.upgrade.hint = Select the {type} and press the upgrade button.
//quests.task.general.show_hud_element = Make HUD element {type} appear on your screen.
//quests.task.general.show_hud_element.hint = Find out which button you have to press.
//quests.task.description.tutorial_01 = Tap on the Build button
//quests.task.description.tutorial_02 = Build one house
//quests.task.description.tutorial_03 = Select a house
//quests.task.description.tutorial_04 = Deselect the house
//quests.task.description.tutorial_05 = Build a Townhall
//quests.task.description.tutorial_06 = Build a Shipyard
//quests.task.description.tutorial_07 = Select the Shipyard
//quests.task.description.tutorial_08 = Tap on the small shop button
//quests.task.description.tutorial_09 = Build a Sloop
//quests.task.description.tutorial_10 = Tap on World button
//quests.task.description.tutorial_11 = Unlock first sector
//quests.task.description.tutorial_12 = Tap on Quests button
//quests.task.description.conquerLumberIsland = Attack Lumber island
//quests.task.description.startTownhallUpgrade = Start Townhall upgrade
//quests.task.description.speedupTownhall = Tap speed up button
//quests.task.description.ownSawmill = Build a Sawmill
//quests.task.description.ownHouse = Build second house
//quests.task.description.ownAcademy = Build an Academy
//quests.task.description.researchWarfare = Research Warfare
//quests.task.description.ownBarracks = Build a Barracks
//quests.task.description.craftPike = Craft a pike.
//quests.task.description.upgradeLumberIsland = Upgrade lumber island
//quests.task.description.craftPikeman = Train a pikeman
//quests.task.description.ownWheatIsland = Attack wheat island
//quests.task.description.exploreSector_1_0 = Unlock sector 1,0
//quests.task.description.exploreSector_2_0 = Unlock sector 2,0
//quests.task.description.ownHousing = Provide housing for 50 workers
//quests.task.description.ownBakery = Build a Bakery
//quests.task.description.ownBrewery = Build a Brewery
//quests.task.description.tradeBread = Buy 10 bread
//quests.task.description.checkLumberProduction = Check how much lumber you own
//quests.task.description.haveLumberProduction = Produce 50 more lumber
//quests.task.description.buildDocks = Build a Docks
//quests.task.description.completeTradeDeal = Complete a trader deal
//quests.task.description.ownHousing_2 = Provide housing for 60 workers
//quests.task.description.haveWheatProduction = Produce 30 more wheat
//quests.task.description.ownStorage = Build a Storage
//quests.task.description.ownIronOre = Attack iron ore island
//quests.task.description.ownCoker = Build a Cocker
//quests.task.description.haveIronProduction = Produce 5 more iron
//quests.task.description.ownFortress = Build a Fortress
//quests.task.description.ownVault = Build a Vault
//quests.task.description.craftSword = Craft a short sword.
//quests.task.description.ownArsenal = Build an Arsenal
//quests.task.description.ownToolmaker = Build a Toolmaker
//quests.task.description.ownShield = Craft a shield.
//quests.task.description.ownSwordsman = Train a swordsman
//quests.task.description.own3IronOreIslands = Own 3 iron ore islands
//quests.task.description.ownFlaxIsland = Attack flax island
//quests.task.description.hireExpert = Hire an Expert
//quests.task.description.branch4_04 = Produce one Damasks
//quests.task.description.branch4_05 = Build a Weaver
//quests.task.description.branch4_06 = Craft 5 Bowstrings
//quests.task.description.branch4_07 = Craft 5 Bows
//quests.task.description.branch4_08 = Train 5 Archers
//quests.task.description.branch4_09 = Own 3 flax islands
//quests.task.description.branch4_10 = Attack clay island
//quests.task.description.branch4_11 = Bred 5 Horses
//quests.task.description.branch4_12 = Train 5 Cavaliry
//quests.task.description.branch4_13 = Attack cattle island
//quests.task.description.branch4_14 = Produce 10 more meat
//quests.task.description.branch4_15 = Attack stone island
//quests.task.description.branch4_16 = Attack sulphur island
//quests.task.description.branch4_17 = Attack lead island
//quests.task.hint.tutorial_01 = Welcome to Doge! Please follow the tutorial closely.
//quests.task.hint.tutorial_02 = Tap on the House button and place the building next to a canal.
//quests.task.hint.tutorial_03 = You can tap buildings to learn what they produce.
//quests.task.hint.tutorial_04 = The House generates Coins and provides Housing for your population.
//quests.task.hint.tutorial_05 = We need the Townhall to build more buildings. In shop switch to Civic category.
//quests.task.hint.tutorial_06 = We need the Shipyard to build a ship. Shipyard is also a Civic building
//quests.task.hint.tutorial_08 = We need ships to explore the map and conquer islands.
//quests.task.hint.tutorial_09 = Wait until the Sloop is constructed.
//quests.task.hint.tutorial_10 = We need to find a Lumber source to produce planks, which we need to develop our island.
//quests.task.hint.tutorial_11 = Tap on the sector [0, 0] to explore it.
//quests.task.hint.tutorial_12 = Quests will guide you from now on...
//quests.task.hint.conquerLumberIsland = Attack the Lumber island by tapping on it, tapping on the swords icon and selecting 5 Pikeman before confirming.
//quests.task.hint.startTownhallUpgrade = Our town is growing! We need to increase our building limit.
//quests.task.hint.speedupTownhall = You can use gems to speed up the upgrade.
//quests.task.hint.ownSawmill = The Sawmill turns Lumber into Planks for building.
//quests.task.hint.ownHouse = Our growing empire needs more Housing. Houses increase the population limit.
//quests.task.hint.ownAcademy = You can research civilian technologies at the academy.
//quests.task.hint.researchWarfare = Select academy. And Tap on the shop button.
//quests.task.hint.craftPike = Pikes are needed to train Pikemen. Craft them at the Carpenter.
//quests.task.hint.upgradeLumberIsland = We can upgrade islands to increase their production.
//quests.task.hint.craftPikeman = Pikemen are trained at the Barracks.
//quests.task.hint.ownWheatIsland = Pirates hold a nearby Wheat island: let's attack them!
//quests.task.hint.ownHousing = Upgrade or build more Houses to increase it. Tap on the workers icon to check it.
//quests.task.hint.ownBakery = Bread is used for troops.
//quests.task.hint.ownBrewery = Beer is used for colonies and exploration.
//quests.task.hint.tradeBread = We need bread to train troops. The trading city is south of our town.
//quests.task.hint.checkLumberProduction = Tap on the inventory button in the top right, open the 'Raw' submenu and tap the Lumber icon.
//quests.task.hint.haveLumberProduction = Upgrade or conquer more lumber islands to increase it.
//quests.task.hint.completeTradeDeal = Tap on the ship next to the docks to see the deal.
//quests.task.hint.ownHousing_2 = Upgrade or build more Houses to increase it. Tap on the workers icon to check it.
//quests.task.hint.ownStorage = The Storage building increases the amount of resources you can store.
//quests.task.hint.ownCoker = The Coker produces Coal from Lumber.
//quests.task.hint.haveIronProduction = Build the Iron Smelter to produce Iron.
//quests.task.hint.ownFortress = The Fortress is used to build fortifications against other players' attacks.
//quests.task.hint.ownVault = The Vault protects a percentage of your resources from other players' attacks.
//quests.task.hint.craftSword = Short swords are needed to train Swordsmen. Craft them at the Blacksmith.
//quests.task.hint.ownArsenal = The Arsenal is used to research military and naval technologies.
//quests.task.hint.ownToolmaker = Build the Toolmaker to produce tool.
//quests.task.hint.ownShield = Upgrade the Carpenter to be able to craft Shields.
//quests.task.hint.ownSwordsman = Swordsmen are a strong infantry unit.
//quests.task.hint.ownTavern = The Tavern generates Culture, and boosts nearby refineries.
//quests.task.hint.hireExpert = Tap the Townhall to hire the expert for a limited period of time. The more you hire experts, the better they become!
//quests.task.hint.branch4_04 = Damasks are produced by the Damask Maker.
//quests.task.hint.branch4_06 = You can craft it at the Weaver.
//quests.task.hint.branch4_07 = You can craft it at the Carpenter.
//quests.task.hint.branch4_08 = Archers are ranged units strong against traps.
//quests.task.hint.branch4_11 = Horses are bred at the Stable.
//quests.task.hint.branch4_12 = Cavalry is strong against towers.
//quests.task.hint.branch4_14 = Meat is used to train troops.
//";
//}
//
