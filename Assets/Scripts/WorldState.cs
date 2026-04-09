using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour
{
    List<Country> countries = new List<Country>();
    List<Relation> relations = new List<Relation>();

    void Start()
    {
        CreateCountries();
    }

    void CreateCountries()
    {
        Country france = new Country("France", 100, 50, 0.7f);
        Country austria = new Country("Austria", 80, 60, 0.4f);
        Country poland = new Country("Poland", 70, 40, 0.5f);
        Country ottomans = new Country("Ottomans", 120, 70, 0.8f);
        countries.Add(france);
        countries.Add(austria);
        countries.Add(poland);
        countries.Add(ottomans);
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
