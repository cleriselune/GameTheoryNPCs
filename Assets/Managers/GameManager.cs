using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<Country> countries = new List<Country>();

    public Country getCountry(int id)
    {
        return countries[id];
    }

    public int years = 50;
    int wars = 0;
    int alliances = 0;

    void Start()
    {
        CSVLogger.CreateFile();
        CreateCountries();
        Simulate();
    }

    void CreateCountries()
    {
        // country, military power, economic strength, aggression level
        countries.Add(new Country("France", 100, 50, 0.7f));
        countries.Add(new Country("Austria", 80, 60, 0.4f));
        countries.Add(new Country("Poland", 70, 40, 0.5f));
        countries.Add(new Country("Ottomans", 120, 70, 0.8f));
    }

    void Simulate()
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

}
