using UnityEngine;

public static class HawkDove
{
    public enum StrategyType
    {
        Hawk,
        Dove
    }
    
    // returns (self payoff, opponent payoff)
    // upgraded matrix with V = value of resource, C = cost of conflict
    public static (float self, float opp) Payoff(StrategyType s, StrategyType o, float V, float C)
    {
        return (s, o) switch
        {
            (StrategyType.Hawk, StrategyType.Hawk) => ((V - C) / 2f, (V - C) / 2f),
            (StrategyType.Hawk, StrategyType.Dove) => (V, 0),
            (StrategyType.Dove, StrategyType.Hawk) => (0, V),
            (StrategyType.Dove, StrategyType.Dove) => (V / 2f, V / 2f),
            _ => throw new System.ArgumentException("Invalid strategies")
        };
    }

    // "Evolutionarily Stable Strategy" mixed probability: 
    // play Hawk with p = V/C
    // If V >= C, always Hawk (pure Nash). If V < C, mixd strategy with some Doves to avoid costly conflict
    public static float HawkProbability(float V, float C)
    {
        return V >= C ? 1f : V / C;
    }

    public static StrategyType ChooseStrategy(Country self, Country opp, RelationState state, float V, float C)
    {
        float p = HawkProbability(V, C);

        /*
        i edned up using a 10% chance of calling to war each turn in allied state
        this method (ChooseStrategy) is only called during Natural state so a state modifier is not needed
        */

        // aggressive countries get up to +0.2 on top of whatever the game theory says, peaceful ones get almost nothing
        // 0.2 is basically a weight modifier
        p += self.GetAiPersonality(self.aiPersonality) * 0.2f;

        // global power check: if opponent is much stronger, back down
        float powerRatio = opp.militaryPower / Mathf.Max(self.militaryPower, 1f);
        if (powerRatio > 2f) p *= 0.3f;  // would be suicidal to attack since opponent is much stronger (militarypower wise)

        p = Mathf.Clamp01(p); // ensure probability is between 0 and 1

        return UnityEngine.Random.value < p ? StrategyType.Hawk : StrategyType.Dove; // if random value is less than p, choose Hawk, otherwise choose Dove
        // random value added to introduce some unpredictability and variation in strategies, simulating real-world decision making
    }
}