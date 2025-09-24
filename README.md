So The build Phase is the Time when you can place the towers just use the buttons given on the bottom to place them also u can reposition already place by clicking right mouse button to pick and then left mouse to place. 
To make a connection first click on the Crystal(Yellow Diamond) and then click on the desired tower
number of connection available can be seen on screen for turrent, mortor, firethrower.
crystal have some health which reduced by 10% whenever an enemy enter the 
everything will be output in the console to better understand 
Iitially i have set the 1 turret and 1 mortar 1 firethrower limit to be placed
turrent will give -2 damage on every hit. mortor will give -7 damage on each enemy who ever get collide, firethrower will give continuous damage of -2 per sec
the build time is set to 15seconds for each wave of enemy
in bulid time you can buy new turrent , mortor, firethrower using the money.
money can be earned by damaging enemies.
if the damage is considerable i.e. greater than 1 than it will pop up on the head of enemy and also enemy will freez for a little time (freezing effect shown by blue color of enemy)
every parameter can be tweaked through thread manager , build manager , game manager
system: window 
two wave of enemies will come
installation steps: unzip the folder and install using the exe file.
enemy: two type of enemied 1. fast moving, 2. slow moving. in both wave they come in combine
tutorial and instrutions can be seen at main menu.




## 🎮 Game Overview

This is a tower defense game where enemies march toward your crystal.
Your objective is to strategically place towers, connect them to the crystal, and stop the enemies before they reduce the crystal’s health to zero.

The game features a Build Phase where you can place or reposition towers, and a **Combat Phase** where enemies attack in waves. Towers consume ammo/fire capacity and can only be reactivated by rewinding them using special Time Threads.

## 🎮 Controls

* Left Mouse Button (LMB)

  * Place a tower at the selected location.
  * Place a repositioned tower after picking it up.
  * Connect towers to the crystal (click the crystal first, then the tower).
* Right Mouse Button (RMB)

  * Pick up an already placed tower to reposition it.
* Bottom Panel Button

  * Choose which tower type (Turret, Mortar, Firethrower) to place.
* R Key

  * Rewind a tower to reactivate it after ammo/fire runs out.
  * Rewind chances are limited per wave.
* Main Menu

  * Access tutorial and instructions.

## 🖥️ System Requirements

* Operating System: Windows (tested build).
* CPU/GPU: Any modern PC capable of running Unity-built games.
* RAM: 2 GB minimum (4 GB recommended).
* Disk Space: \~500 MB free space.

## ⚙️ Installation & Launch

1. Download the game folder.
2. Unzip the contents to your preferred location.
3. Run the provided `.exe` file to start the game.
4. Using the Git URL " https://github.com/tezzuk/KraftonGameJam.git " you can get source code.


## 🏗️ Game Mechanics

🔹 Build Phase (15 seconds per wave)

* Place towers using the UI buttons.
* Reposition towers by RMB (pick up) and LMB (place).
* Buy new towers with money earned from damaging enemies.
* Connect towers to the Crystal (yellow diamond).
* Crystal connections are limited, and the available number is displayed on-screen.

🔹 Crystal

* Acts as the core defense.
* Loses 10% health each time an enemy passes through.
* Provides energy threads to towers.
* Threads are also used for rewind.

🔹 Towers

* Turret: Deals -2 damage per hit.
* Mortar: Deals -7 area damage to enemies it collides with.
* Firethrower: Deals continuous -2 damage per second.
* ⚠️ Capacity Limit:

  * Each tower type has limited ammo/fire.
  * Once capacity is used up, the tower becomes inactive.
  * You must use Rewind (R key) with a crystal thread to reactivate it.
  * Each wave has limited rewind chances.

🔹 Enemies

* Two types appear in waves:

  1. Fast-moving enemies.
  2. Slow-moving enemies.
* Both types spawn together in each wave.

🔹 Damage & Effects

* Damage greater than 1 displays a floating damage number above the enemy.
* Enemies take a freeze effect (turning blue) for a short time when hit hard.

🔹 Progression

* Two waves of enemies are currently implemented.
* Earn money by damaging enemies to purchase more towers.
* All game parameters (tower stats, wave timing, rewind chances, etc.) can be tuned in the Thread Manager, Build Manager, and Game Manager scripts.

## 🐞 Known Issues

* Rare bug where enemy colors may not reset correctly after freezing.
* Debug messages are printed to the Console for learning/testing.
* Some balance values (tower ammo, rewind chances, enemy HP) are placeholder and may require tweaking.
