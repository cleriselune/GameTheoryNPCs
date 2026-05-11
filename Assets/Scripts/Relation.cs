using UnityEngine;

public class Relation
{
    public int idA;
    public int idB;
    public RelationState state = RelationState.Neutral; // default state is neutral
    public float opinion = 0; // opinion of each other
    public float allicanceTurns = 0; // how many turns have been in alliance, used for a timer of a timed alliance
    public float warTurns = 0; // how many turns have been at war, used for a timer of a timed war
    public int truceTurnsRemaining = 0;
    public WorldState world;

    public Relation(int idA, int idB)
    {
        if (idA != idB)
        {
            this.idA = idA;
            this.idB = idB;
        }
        else
        {
            Debug.LogError("Cannot create a relation with the same country.");
        }
    }

    public void ModifyOpinion(float amount)
    {
        opinion += amount;
        opinion = Mathf.Clamp(opinion, -100f, 100f); // clamp opinion between -100 and 100
    }

    public void ResetOpinion()
    {
        opinion = 0;
    }
    
    public void Fire(RelationEvent relationEvent, WorldState world)
    {
        RelationState next = Evaluate(relationEvent);
        if (next == state) return; // no change in state, so do nothing

        Debug.Log("Relation between " + world.GetCountry(idA).countryName + " and " + world.GetCountry(idB).countryName + " changed from " + state + " to " + next + " due to event: " + relationEvent);
        state = next;
    }

    // FSM TRANSITIONS
    RelationState Evaluate(RelationEvent relationEvent) => (state, relationEvent) switch
    {
        // neutral state transitions
        (RelationState.Neutral, RelationEvent.BothPlayedDove) => RelationState.Allied,
        (RelationState.Neutral, RelationEvent.BothPlayedHawk) => RelationState.AtWar,
        (RelationState.Neutral, RelationEvent.OpinionHighEnough) => RelationState.Allied,
        // allied state transitions
        (RelationState.Allied, RelationEvent.AllyCalledToWar) => RelationState.AtWar,
        (RelationState.Allied, RelationEvent.AllianceExpired) => RelationState.Neutral,
        // war state transitions
        (RelationState.AtWar, RelationEvent.PeaceOffered) => RelationState.PeaceDeal,
        (RelationState.AtWar, RelationEvent.WarEnded) => RelationState.Neutral,
        // peace deal state transitions
        (RelationState.PeaceDeal, RelationEvent.PeaceAccepted) => RelationState.Neutral,
        (RelationState.PeaceDeal, RelationEvent.PeaceRejected) => RelationState.AtWar,
        _ => state // no change for unhandled events
    };


    //PER TURN UPDATE TICKS
    public void Tick()
    {
        switch (state) {
            case RelationState.Neutral:
                TickNeutral();
                break;
            case RelationState.Allied:
                TickAllied();
                break;
            case RelationState.AtWar:
                TickAtWar();
                break;
            case RelationState.PeaceDeal:
                TickPeaceDeal();
                break;
        }
    }
    
    void TickNeutral()
    {
    }

    void TickAtWar()
    {
        warTurns++;
    }

    void TickAllied()
    {
        allicanceTurns++;
    }

    void TickPeaceDeal()
    {
        truceTurnsRemaining = 5; // set truce timer to prevent immediate re-escalation? after resolving
    }
}
