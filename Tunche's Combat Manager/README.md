# Tunche's Combat Manager [CODE NOT PROVIDED]
![Tunche Hivemind](/Images/Hivemind.gif)

## HOW DOES IT WORK?
1. Each enemy is classified into two groups, Melee and Ranged, and the system handles those groups independently.
2. When an enemy is created, the enemy is asigned a target corresponding to the player with the highest score after evaluating which one hasthe least amount of enemies targetting them and also their proximity.
3. Each time an attack cycle is finished, the system evaluates all the enemies targetting each player based on: proximity, aggresion score, if it is on attack cooldown or not and the enemy difficulty (all of then are values determined by the design for each enemy).
4. The system ranks the enemies from the highest to lowest score and makes the top N enemies (in the production version of the game N=3) to attack sequentially.
5. When an enemy attack finishes, it enters a cooldown state that makes it unavailble to be selected for the next attack cycle rounds until the cooldown finishes.
6. If an enemy has not been selected for multiple attack cycles, it gets a bonus in his scoring promoting its attack in the next round.
7. If an enemy has multiple attacks, after each attack cycle that the enemy has not attacked it evaluates if it should switch attack patterns or maybe even change from a melee state to a ranged state
