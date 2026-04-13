using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    List<Country> countries = new List<Country>();
    List<Relation> relations = new List<Relation>();

    void Awake() // initialize countries and relations at the start of the game
    {
        CreateCountries();
    }

    void CreateCountries()
    {
        countries.Add(new Country("France", militaryPower: 100, economicStrength: 90, aggressionLevel: 0.6f));
        countries.Add(new Country("England", militaryPower: 60, economicStrength: 60, aggressionLevel: 0.2f));
        countries.Add(new Country("Ottomans", militaryPower: 120, economicStrength: 80, aggressionLevel: 0.8f));
        countries.Add(new Country("Poland", militaryPower: 70, economicStrength: 40, aggressionLevel: 0.5f));
    }

    public Country GetCountry(int id)
    {
        return countries[id];
    }

    public List<Country> GetCountries()
    {
        return countries;
    }

    public Relation GetRelation(int idA, int idB)
    {
        // find relation between idA and idB, if it doesn't exist, create it
        Relation relation = relations.Find(r => (r.idA == idA && r.idB == idB) || (r.idA == idB && r.idB == idA)); // find relation where idA and idB are either in order or reversed, since relations are bidirectional
        if (relation == null)
        {
            relation = new Relation(idA, idB);
            relations.Add(relation);
        }
        return relation;
    }

    public List<Relation> GetRelations()
    {
        return relations;
    }
}
