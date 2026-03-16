using UnityEngine;

public class Country
{
    public string countryName;
    public float militaryPower;
    public float economicStrength;
    public float aggressionLevel; // 0-1 scale, where 1 is most aggressive
    // could change it to structure depending on country type (e.g., democracy, dictatorship, etc.)

    // Country constructor
    public Country(string countryName, float militaryPower, float economicStrength, float aggressionLevel)
    {
        this.countryName = countryName;
        this.militaryPower = militaryPower;
        this.economicStrength = economicStrength;
        this.aggressionLevel = aggressionLevel;
    }

    // Method to choose a strategy based on aggression level
    public StrategyType ChooseStrategy()
    {
        float rand = Random.value;

        if (rand < aggressionLevel) {
            return StrategyType.Hawk;
        }
        else {
            return StrategyType.Dove;
        }

    }

    // Method to decide action against another country based on chosen strategy
    public ActionType DecideAction(Country other)
    {
        StrategyType myStrategy = ChooseStrategy();

        if (myStrategy == StrategyType.Hawk) {
            return ActionType.Attack;
        }
        else {
            return ActionType.Ally;
        }
    }

    // Method to execute the chosen action against another country
    public void ExecuteAction(ActionType action, Country other)
    {
        switch (action) {
            case ActionType.Attack:
                // Simple combat resolution: attacker loses 20% of defender's military power
                float loss = other.militaryPower * 0.2f;
                militaryPower -= loss;
                Debug.Log(countryName + " attacked " + other.countryName + " and lost " + loss + " troops");
                break;
            case ActionType.Ally:
                // Forming an alliance increases economic strength for both countries
                economicStrength += 1;
                Debug.Log(countryName + " formed alliance with " + other.countryName);
                break;
            case ActionType.None:
                // No action taken
                break;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
