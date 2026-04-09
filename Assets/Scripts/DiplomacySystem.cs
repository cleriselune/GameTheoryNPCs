using UnityEngine;

public class DiplomacySystem : MonoBehaviour
{
    public WorldState world;

    // called by TurnManager each turn for every pair of countries
    public void ProcessPair(Relation relation)
    {
        relation.Tick(); // update timers and handle expirations
        switch (relation.state)
        {
            case RelationState.Neutral:
                ProcessNeutral(relation);
                break;
            case RelationState.Allied:
                ProcessAllied(relation);
                break;
            case RelationState.AtWar:
                ProcessAtWar(relation);
                break;
            // case RelationState.PeaceDeal: // notyt implemented yet
            //     ProcessPeaceDeal(relation);
            //     break;
        }
    }

    void ProcessNeutral(Relation relation)
    {
        Country countryA = world.GetCountry(relation.idA);
        Country countryB = world.GetCountry(relation.idB);

        float V = ResourceCost(countryA, countryB);
        float C = AverageInjuryCost(countryA, countryB);

        var strategyA = HawkDove.ChooseStrategy(countryA, countryB, relation.state, V, C);
        var strategyB = HawkDove.ChooseStrategy(countryB, countryA, relation.state, V, C);

        var (payA, payB) = HawkDove.Payoff(strategyA, strategyB, V, C);

        // here some kind of paymentt system would go, for now just opinion shifts and state changes based on behavior

        // opinion shifts from behavior
        if (strategyA == HawkDove.StrategyType.Hawk) {
            relation.ModifyOpinion(-15f);
        }
        if (strategyB == HawkDove.StrategyType.Hawk) { 
            relation.ModifyOpinion(-15f); 
        }
        if (strategyA == HawkDove.StrategyType.Dove && strategyB == HawkDove.StrategyType.Dove) {
            relation.ModifyOpinion(+10f);
            // alliance formed if opinion is high enough
            if (relation.opinion >= Relation.ALLY_THRESHOLD) {
                relation.Fire(RelationEvent.OpinionHighEnough);
            }
        }
        if (strategyA == HawkDove.StrategyType.Hawk && strategyB == HawkDove.StrategyType.Hawk) {
            relation.Fire(RelationEvent.BothPlayedHawk);
        }

    }

    void ProcessAllied(Relation relation)
    {
        Country countryA = world.GetCountry(relation.idA);
        Country countryB = world.GetCountry(relation.idB);

        // some kinda payment for alliance

        relation.ModifyOpinion(+2f); // opinion strengthens over time in alliance

        //random change of ally calling to war each turn, based on some factors like threat level,, maybe random events later on
        if (Random.value < 0.1f) {
            relation.Fire(RelationEvent.AllyCalledToWar);
        }

    }
    
    void ProcessAtWar(Relation relation)
    {
        Country countryA = world.GetCountry(relation.idA);
        Country countryB = world.GetCountry(relation.idB);

        // some kinda payment for being at war, maybe based on battles or just a flat cost

        relation.ModifyOpinion(-5f); // opinion worsens over time in war

        // Each turn is a Hawk-Hawk engagement (both committed to war)
        float V = 20f;  // high value for war since both sides are fully committed
        float C = AverageInjuryCost(countryA, countryB);
        var (dmgA, dmgB) = HawkDove.Payoff(HawkDove.StrategyType.Hawk, HawkDove.StrategyType.Hawk, V, C);

        // damage = negative payoff when C > V
        countryA.militaryPower += dmgA;
        countryB.militaryPower += dmgB;

        // random chance of offering peace each turn, maybe based on war duration or losses?
        if (Random.value < 0.1f) {
            //relation.Fire(RelationEvent.PeaceOffered); // for not leaving this
        }
    }

    // === UTILITY CALCULATIONS ===

    // V
    float ResourceCost(Country a, Country b) 
        => (a.economicStrength + b.economicStrength) / 2f;

    // C
    float AverageInjuryCost(Country a, Country b) 
        => (a.militaryPower + b.militaryPower) / 2f;

}
