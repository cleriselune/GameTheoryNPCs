# Game theory in modeling the behavior of NPC in turn-based strategy game
[temporary ss i have to update UI]
<img width="1000" height="561" alt="image" src="https://github.com/user-attachments/assets/ffacbba3-dec9-46ca-b5cc-25267b7aafc5" />

### Features: 
#### ***Core features***
  - strategy selection based on probabilty of choosing hawk with p = V/C where V is possible resource gain and C is cost of injury
  - modifier on top of base probability based on the personality of AI (militarist, diplomatist, etc.)
  - hawk-dove game payoff matrix depending on chosen strategies by countries
<p align="center">
  <img width="500" height="500" alt="image" src="https://github.com/user-attachments/assets/59e2912e-39ff-446c-aada-8df86cda2fa2" />
  <img width="500" height="500" alt="image" src="https://github.com/user-attachments/assets/ef7879e0-3dc4-4adc-a706-95dd518d0a2b" />
</p>

  - the countries gain or lose 'treasury' property based on the score from the matrix
  - additional: power ratio check, weaker nations back down against much stronger ones (to not make suicidal choices)
  - applied relation 4 FSM states: Neutral, Allied, AtWar, PeaceDeal:

<p align="center">
  <img width="500" height="500" alt="image" src="https://github.com/user-attachments/assets/b61be2ed-fb36-47ad-a099-4c4e5554bdfd" />
  <img width="500" height="500" alt="image" src="https://github.com/user-attachments/assets/bb69c966-c2bd-4ba2-8700-427b9177b407" />
</p>


#### ***Diplomacy Mechanics***
  - ppinion system driving state transitions:
    - automatic alliance if opinion > 80
    - for two countries to go to war opinion has to be at least -50
  - truce timer post-war preventing immediate re-declaration (to limit the amount of wars)
  - alliance duration timer with expiry back to neutral state
  - war duration timer with two options:
    - expiration of war -> resolve to neutral state
    - peace offer:
      - accept -> resolve to normal
      - reject -> back to war

#### ***Country Properties***
  - militaryPower - affects injury cost C and power ratio check
  - economicStrength - affects resource value V
  - treasury - the resource the countries are fighting for
  - aiPersonality - modifier of aggresiveness

#### ***Simulation of Turns***
  - turn-based processing of all country pairs per turn
  - pair shuffling each turn to avoid first-mover bias
  - WorldState managing countries and relations and also getting the country list and country by id
  - UI with LineRenderer showing relation state by color
  - Process Turn button to click, it adds one year to the game (starting is 1444)

### Basic information

The “Assets/Scripts” directory contains all the scripts in one place.

No manually added resources are used at this time, the only one used is TextMesh Pro added by Unity automatically.

Not a finished game, only a prototype of implementing game theory in modeling the NPC behaviour :)



-------------------------------------------------------------------
