using System.Collections.Generic;
using UnityEngine;

public class DiplomacyUI : MonoBehaviour
{
    public WorldState world;
    public GameManager gameManager;

    // country nodes in the UI, IN THE SAME ORDER AS THEIR ID (HOW THEY ARE CREATEDI N WORLDSTATE.CS)
    public GameObject[] countryNodes; 

    [Header("Colors")]
    public Color neutralColor = Color.white;
    public Color alliedColor = Color.green;
    public Color atWarColor = Color.red;
    public Color peaceColor = Color.yellow;

    Dictionary<(int, int), LineRenderer> lines = new();

    void Start()
    {
        BuildLines();
    }

    // initial setup of lines
    void BuildLines()
    {
        var relations = world.GetRelations();

        foreach (var rel in relations)
        {
            // create a new GameObject per relation to hold the LineRenderer
            var go = new GameObject($"Line_{rel.idA}_{rel.idB}"); // names: Line_0_1, Line_1_2, etc... corresponding to country IDs
            go.transform.SetParent(transform); // parent to this UI object

            LineRenderer lr = go.AddComponent<LineRenderer>(); // add LineRenderer component
            lr.positionCount = 2; // two endpoints: country A and country B
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.sortingOrder = -1;

            lines[(rel.idA, rel.idB)] = lr; // stores reference for updates
        }

    }

    void Update()
    {
        // update line positions and colors based on current relations
        // it updates witch each process turn
        var relations = world.GetRelations();

        foreach (var rel in relations)
        {
            if (!lines.ContainsKey((rel.idA, rel.idB))) continue; // safety check

            LineRenderer lr = lines[(rel.idA, rel.idB)]; 
            Vector3 posA = countryNodes[rel.idA].transform.position;
            Vector3 posB = countryNodes[rel.idB].transform.position;

            lr.SetPosition(0, posA); // set start point to country A
            lr.SetPosition(1, posB); // end point country B, makes it so the line connects the two nodes

            Color c = rel.state switch {
                RelationState.Allied    => alliedColor,
                RelationState.AtWar     => atWarColor,
                RelationState.PeaceDeal => peaceColor,
                _                       => neutralColor
            };

            lr.startColor = c;
            lr.endColor   = c;
        }
    }

    // called by the Process Turn button in the UI 
    public void OnProcessTurnButton()
    {
        gameManager.ProcessTurn();
    }

}