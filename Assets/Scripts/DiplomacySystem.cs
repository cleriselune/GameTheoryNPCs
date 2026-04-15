using UnityEngine;

public class DiplomacySystem : MonoBehaviour
{
    public WorldState world;

    // called by TurnManager each turn for every pair of countries
    public void ProcessPair(Relation relation)
    {
        relation.Tick(); // update timers and handle state-specific logic

        // proccess behaviour based on he state^ and give them payoffs and opions etcc
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
            case RelationState.PeaceDeal: // notyt implemented yet
                ProcessPeaceDeal(relation);
                break;
        }
    }

    void ProcessNeutral(Relation relation)
    {
        if (relation.truceTurnsRemaining > 0) {
            relation.truceTurnsRemaining -= 1;
            return; // still in truce, skip normal processing
        }

        Country countryA = world.GetCountry(relation.idA);
        Country countryB = world.GetCountry(relation.idB);

        float V = ResourceGain(countryA, countryB);
        float C = InjuryCost(countryA, countryB);

        var strategyA = HawkDove.ChooseStrategy(countryA, countryB, relation.state, V, C);
        var strategyB = HawkDove.ChooseStrategy(countryB, countryA, relation.state, V, C);

        if (relation.opinion > 50f) { // if opinion is high enough, form alliance regardless of strategies
            relation.ModifyOpinion(+10f);
            relation.Fire(RelationEvent.OpinionHighEnough);
        }
        if (strategyA == HawkDove.StrategyType.Hawk && strategyB == HawkDove.StrategyType.Dove) {
            relation.ModifyOpinion(-10f); // playing Hawk against a Dove makes you look aggressiveeeee soo decreasing opinion

            var (payoffA, payoffB) = HawkDove.Payoff(strategyA, strategyB, V, C);

            // apply payoffs: hawk gets the resource value, dove gets nothing but also pays no cost
            countryA.treasury += payoffA;
            countryB.treasury += payoffB;
            Debug.Log(countryA.countryName + " played Hawk against " + countryB.countryName + " gaining " + payoffA + " and country2 played Dove gaining " + payoffB + "; decreasing opinion to " + relation.opinion);
            
        }
        if (strategyB == HawkDove.StrategyType.Hawk && strategyA == HawkDove.StrategyType.Dove) { 
            relation.ModifyOpinion(-10f); 

            var (payoffB, payoffA) = HawkDove.Payoff(strategyB, strategyA, V, C);
            countryB.treasury += payoffB;
            countryA.treasury += payoffA;

            Debug.Log(countryB.countryName + " played Hawk against " + countryA.countryName + " gaining " + payoffB + " and country1 played Dove gaining " + payoffA + "; decreasing opinion to " + relation.opinion);
        }
        if (strategyA == HawkDove.StrategyType.Dove && strategyB == HawkDove.StrategyType.Dove) {
            relation.ModifyOpinion(+10f);
            relation.Fire(RelationEvent.BothPlayedDove); // if both played Dove, form alliance
        }
        if (strategyA == HawkDove.StrategyType.Hawk && strategyB == HawkDove.StrategyType.Hawk) {
            if (relation.opinion < -50f) { // only escalate if already hostile

                var (payoffA, payoffB) = HawkDove.Payoff(strategyA, strategyB, V, C);
                // apply payoffs: both sides pay the cost of conflict and split the resource value
                // V - C / 2
                countryA.treasury += payoffA;
                countryB.treasury += payoffB;
                Debug.Log("WAR: " + countryA.countryName + " vs " + countryB.countryName + " gaining payoffA: " + payoffA + " and payoffB: " + payoffB + "; opinion" + relation.opinion);
                relation.Fire(RelationEvent.BothPlayedHawk);
            }
        }

    }

    void ProcessAllied(Relation relation)
    {
        Country countryA = world.GetCountry(relation.idA);
        Country countryB = world.GetCountry(relation.idB);

        var (payoffA, payoffB) = HawkDove.Payoff(HawkDove.StrategyType.Dove, HawkDove.StrategyType.Dove, ResourceGain(countryA, countryB), InjuryCost(countryA, countryB));

        countryA.treasury += payoffA;
        countryB.treasury += payoffB; // v / 2 value with no costttt
        Debug.Log(countryA.countryName + " and " + countryB.countryName + " are allied, both gaining " + payoffA + " and " + payoffB);

        relation.ModifyOpinion(+5f); // opinion strengthens over time in alliance

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

        // each turn is a hawkhawkk engagement (both committed to war)
        var (payoffA, payoffB) = HawkDove.Payoff(HawkDove.StrategyType.Hawk, HawkDove.StrategyType.Hawk, ResourceGain(countryA, countryB), InjuryCost(countryA, countryB));
        countryA.treasury += payoffA;
        countryB.treasury += payoffB;
        // damage = negative payoff when C > V

        // preventing nevative military power by setting a minimum threshold

        // random chance of offering peace each turn, maybe based on war duration or losses?
        if (Random.value < 0.1f) {
            relation.Fire(RelationEvent.PeaceOffered); // for not leaving this
        }
    }

    void ProcessPeaceDeal(Relation relation)
    {
        // chance of accepting or rejecting the peace deal

        if (Random.value < 0.5f) {
            relation.Fire(RelationEvent.PeaceAccepted);
        } else {
            relation.Fire(RelationEvent.PeaceRejected);
        }
    }

    // === UTILITY CALCULATIONS ===

    // V
    // from HawkDove matrix
    float ResourceGain(Country a, Country b) 
        => (a.economicStrength + b.economicStrength) / 2f;

    // C
    float InjuryCost(Country a, Country b) 
        => (a.militaryPower + b.militaryPower) / 2f;

}
