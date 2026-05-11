using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public WorldState world;
    public DiplomacySystem diplomacy;
    
    int currentYear = 1444; // starting year
    // public int simTurns = 50; // how many years to simulate, can be set in inspector; not used, yet?
    // int wars = 0; previously used for logging, might use it later
    // int alliances = 0;

    void Start()
    {
        InitRelations();
        // Simulate();
    }

    void InitRelations()
    {
        var c = world.GetCountries();
        for (int i = 0; i < c.Count; i++)
        {
            for (int j = i + 1; j < c.Count; j++)
            {
                if (c[i] == c[j]) continue; // skip same country, handled in Relation constructor but just in case
                world.GetRelation(i, j); // this will create a relation if it doesn't exist
                Debug.Log("Initialized relation between " + c[i].countryName + " and " + c[j].countryName + " with initial opinion " + world.GetRelation(i, j).opinion);
            }
        }
        Debug.Log("Initialized " + world.GetCountries().Count + " countries and " + world.GetCountries().Count * (world.GetCountries().Count - 1) / 2 + " relations.");
    

    }

    public void ProcessTurn()
    {
        // process every pair of countries through the diplomacy system

        // shuffle to avoid first-mover bias
        List<Relation> shuffled = new List<Relation>(world.GetRelations());
        Shuffle(shuffled);
        // process each relation
        foreach (var relation in shuffled) {
            diplomacy.ProcessPair(relation);
        }

        // advancing time
        currentYear++;
        Debug.Log($"=== Year {currentYear} complete ===");
        diplomacy.RecordLogs(currentYear);
    }

    void Shuffle<T>(List<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

  /*  void Simulate()
    {
        for (int year = 0; year < years; year++)
        {
            Debug.Log("YEAR: " + year);

            int warsThisYear = 0;
            int alliancesThisYear = 0;

            // Randomly select two countries to interact
            Country A = countries[Random.Range(0, countries.Count)];
            Country B = countries[Random.Range(0, countries.Count)];

            // Ensure A and B are not the same country
            if (A == B) {
                continue;
            }

            HawkDove.StrategyType strategyA = A.ChooseStrategy();
            HawkDove.StrategyType strategyB = B.ChooseStrategy();

            var (payA, payB) = HawkDove.Payoff(strategyA, strategyB, 20f, 20f); // V = economic strength, C = fixed cost of conflict

            // Update economic strength based on payoffs
            A.economicStrength += payA;
            B.economicStrength += payB;

            Debug.Log(A.countryName + " strategy: " + strategyA);
            Debug.Log(B.countryName + " strategy: " + strategyB);

            ActionType actionA = A.DecideAction(B);

            if(actionA == ActionType.Attack)
            {
                wars++;
                warsThisYear++;
            }

            if(actionA == ActionType.Ally)
            {
                alliances++;
                alliancesThisYear++;
            }

            A.ExecuteAction(actionA, B);

            CSVLogger.Log(year, warsThisYear, alliancesThisYear);
        }
    }
    */

}
