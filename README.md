<h1 align="center"> GAME THEORY IN MODELING THE BEHAVIOUR OF NPC'S IN A TURN-BASED STRATEGY GAME </h1>

[ss of in-game screen, countries, the lines are relation between countries]
          
          white - neutral
          red - at war
          green - allied 
          
<img width="1280" height="719" alt="image" src="https://github.com/user-attachments/assets/a179afde-a8a0-4945-aba1-3046d898ae40" />

### Video Of Usage [with old visualization, will update]

https://github.com/user-attachments/assets/bda94416-00c8-4e10-93b6-083a09a90650

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

### Game Loop 
What scripts call to make it all work:

<pre>
  DiplomacyUI.cs:
  └── Start()
      └── BuildLines() -> creates intial lines based on relations between countries
  └── Update() -> updates line positions with colors when they change with each turn
  └── OnProcessTurnButton()
      └── gameManager.ProcessTurn()
  
  GameManager.cs:
  └── Start()
      └── InitRelations() -> iterates through countries and initializes relations between them
  └── ProcessTurn() -> simulates a turn (year) added
      └── diplomacy.ProcessPair(relation) -> for each relation process the pair

  DiplomacySystem.cs:
  └── ProcessPair(relation)
      └── relation.Tick() ->updates timers (like war and alliance duration) and specific logic
      └── switch (relation.state) -> based on the state choose process
          └── ProcessNeutral(relation)
          └── ProcessAllied(relation)
          └── ProcessAtWar(relation)
          └── ProcessPeaceDeal(relation)
  
  Process methods do stuff like:
    - choose HawkDove strategy based on calculated resource and cost
    - apply payoff matrix from HawkDove.Payoff (it's added to treasury of a country)
    - modifies opinion based on chosen strategy
    - fires RelationEvent (fe. RelationEvent.BothPlayedHawk which calls to war etc.)
  
  Relation.cs:
  └── constructor
  └── ModifyOpinion(amount)
  └── ResetOpinion -> to 0
  └── Fire(relationEvent, world)
      └── Evaluate(relationEvent) -> FSM states flows, it's visible on ss above
  └── Tick()
      └── TickNeutral() -> does nothing
      └── TickAllied() -> counts alliance turns, if alliance is longer than threshold it resets and fires and event ending the alliance (expiry)
      └── TickAtWar() -> counts war turns, if war longer than than threshold, resets opinion and fires relation event of war ending, also sets truce turns to 4 to stop soon escalation
      └── TickPeaceDeal() -> after a peace deal sets truce turns to 5
    
  RelationState.cs:
  └── enum RelationState : Neutral, Allied, AtWar, PeaceDeal
  └── enum RelationEvent : all possible event to happen (conditions of changing between fsm states)
    
    
  HawkDove.cs:
  └── enum StrategyType:
      └── Hawk
      └── Dove
  └── Payoff(StrategyType s, StrategyType o, V, C) -> returns (payoff s, payoff o), uses hawk dove matrix to determine the payoff for each country of the two
      └── HawkProbability(V, C) -> how probable it is for a country to choose hawk based on V / C (resource / cost)
      └── ChooseStrategy(country a, country b, relationstate, V, C) -> uses probability^, modifies it through ai personality, checks power ratio and using random value determines the strategy type
    
  WorldState.cs:
    list of countries
    list of relations between countries
  └── Awake()
      └── CreateCountries() -> fills the list with pre created countries
  └── GetCountry(id)
  └── GetCountries()
  └── GetRelation(idA, idB)
  └── GetRelations()
    
  Country.cs:
  └── enum AiPersonality : Democratic, Balanced, Militaristic
  └── GetAiPersonality(aiPersonality) -> returns a float modifier to agressiveness based on the personality chosen when creating country object
  └── constructor
    
  
</pre>


### Basic information

The “Assets/Scripts” directory contains all the scripts in one place.

No manually added resources are used at this time, the only one used is TextMesh Pro added by Unity automatically.

Not a finished game, only a prototype of implementing game theory in modeling the NPC behaviour :)



-------------------------------------------------------------------
