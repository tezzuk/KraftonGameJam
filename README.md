# 📘 README

## 🎮 Game Overview

This is a **tower defense game** where enemies march toward your crystal. But the twist is that your towers are really fragile due to Crystal's force field and breaks after a few time of firing. But the crystal provides Threads of Time that can be attached to the tower to rewind their condition to working again.
Your objective is to strategically place towers, connect them to the crystal, and stop the enemies before they reduce the crystal’s health to zero.

The game features a **Build Phase** where you can place or reposition towers, and a **Combat Phase** where enemies attack in waves. Towers consume ammo/fire capacity and can only be reactivated by rewinding them using special Time Threads.

---

## 🎮 Controls

* **Left Mouse Button (LMB)**

  * Place a tower at the selected location.
  * Place a repositioned tower after picking it up.
  * Connect towers to the crystal (click the crystal first, then the tower).
* **Right Mouse Button (RMB)**

  * Pick up an already placed tower to reposition it.
* **Bottom Panel Buttons**

  * Choose which tower type (Turret, Mortar, Firethrower) to place.
* **R Key**

  * Rewind a tower to **reactivate it after ammo/fire runs out**.
  * Rewind chances are **limited per wave**.
* **Main Menu**

  * Access tutorial

---

## 🖥️ System Requirements

* **Operating System**: Windows (tested build).
* **CPU/GPU**: Any modern PC capable of running Unity-built games.
* **RAM**: 2 GB minimum (4 GB recommended).
* **Disk Space**: \~500 MB free space.

---

## ⚙️ Installation & Launch

1. **Download** the game folder.
2. **Unzip** the contents to your preferred location.
3. Run the provided **`.exe` file** inside the Build folder to start the game.
4. Using the Git URL " https://github.com/tezzuk/KraftonGameJam.git " you can get source code.

---

## 🏗️ Game Mechanics

### 🔹 Build Phase (45 seconds per wave)

* Place towers using the UI buttons.
* Reposition towers by **RMB (pick up)** and **LMB (place)**.
* Buy new towers with money earned from damaging enemies.
* Upgrade the already existing towers with money earned from damaging enemies.
* Connect towers to the **Crystal** (blue diamond).
* Crystal connections are limited, and the available number is displayed on-screen.

### 🔹 Crystal

* Acts as the **core defense**.
* Loses **10% health** each time an enemy passes through.
* Provides energy threads to towers.
* Threads are also used for **rewind**.

### 🔹 Towers

* **Turret**: Deals **-4 damage per hit**.
* **Mortar**: Deals **-7 area damage** to enemies it collides with.
* **Firethrower**: Deals **continuous -5 damage per second**.
* ⚠️ **Capacity Limit**:

  * Each tower type has limited ammo/fire.
  * Once capacity is used up, the tower becomes **inactive**.
  * You must use **Rewind (R key)** with a crystal thread to reactivate it.
  * Each wave has **limited rewind chances**.
* **Towers** can be **upgraded** by using coins to increase damage and other parameters.
### 🔹 Enemies

* Two types appear in waves:

  1. **Fast-moving enemies**.
  2. **Slow-moving enemies**.
* Both types spawn together in each wave.

### 🔹 Damage & Effects

* Damage greater than 1 displays a **floating damage number** above the enemy.
* Enemies take a **freeze effect** (turning blue) for a short time when hit hard.

### 🔹 Progression

* Four waves of enemies are currently implemented.
* Earn money by damaging enemies to purchase more towers.
* All game parameters (tower stats, wave timing, rewind chances, etc.) can be tuned in the **Thread Manager**, **Build Manager**, and **Game Manager** scripts.

---

## 🐞 Known Issues

* Rare bug where enemy colors may not reset correctly after freezing.
* Debug messages are printed to the **Console** for learning/testing.
* Some balance values (tower ammo, rewind chances, enemy HP) are placeholder and may require tweaking.

---

