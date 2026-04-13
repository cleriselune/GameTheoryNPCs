using UnityEngine;

public class Relation
{
    public int idA;
    public int idB;
    public RelationState state = RelationState.Neutral; // default state is neutral
    public float opinion = 0; // opinion of each other
    float allicanceTurns = 0; // how many turns have been in alliance, used for a timer of a timed alliance
    float warTurns = 0; // how many turns have been at war, used for a timer of a timed war
    public int truceTurnsRemaining = 0;

    //
    public const float ALLY_THRESHOLD = 80f; // opinion threshold to form alliance
    public const float ALLIANCE_DURATION = 5f; // duration of alliance in turns (years)
    public const float WAR_DURATION = 5f; // duration of war in turns (years);; will change this

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
    
    public void Fire(RelationEvent relationEvent)
    {
        RelationState next = Evaluate(relationEvent);
        if (next == state) return; // no change in state, so do nothing

        Debug.Log("Relation between " + idA + " and " + idB + " changed from " + state + " to " + next + " due to event: " + relationEvent);
        state = next;
    }

    // FSM TRANSITIONS
    RelationState Evaluate(RelationEvent relationEvent) => (state, relationEvent) switch
    {
        // neutral state transitions
        (RelationState.Neutral, RelationEvent.BothPlayedDove) => RelationState.Allied,
        (RelationState.Neutral, RelationEvent.BothPlayedHawk) => RelationState.AtWar,
        (RelationState.Neutral, RelationEvent.OpinionHighEnough) => RelationState.Allied, // pausee...
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

    public void Tick(WorldState world)
    {
        switch (state) {
            case RelationState.Neutral:
                TickNeutral();
                break;
            case RelationState.Allied:
                TickAllied();
                break;
            case RelationState.AtWar:
                TickAtWar(world);
                break;
            case RelationState.PeaceDeal:
                TickPeaceDeal();
                break;
        }
    }
    
    void TickNeutral()
    {
        if (truceTurnsRemaining > 0) {
            truceTurnsRemaining -= 1;
        }
    }

    void TickAtWar(WorldState world)
    {
        warTurns += 1;
        if (warTurns >= WAR_DURATION) {
            Country countryA = world.GetCountry(idA);
            Country countryB = world.GetCountry(idB);
            // combat resolution
            float loss = Mathf.Min(countryA.militaryPower, countryB.militaryPower) * 0.2f;
            countryA.militaryPower = Mathf.Max(countryA.militaryPower - loss, 5f);
            countryB.militaryPower = Mathf.Max(countryB.militaryPower - loss, 5f);

            warTurns = 0;
            truceTurnsRemaining = 4; // can't re-declare war for 4 turns
            ResetOpinion();
            Fire(RelationEvent.WarEnded);
        }
    }

    void TickAllied()
    {
        allicanceTurns += 1;
        if (allicanceTurns >= ALLIANCE_DURATION) {
            allicanceTurns = 0;
            Fire(RelationEvent.AllianceExpired); // alliance expired by timer
        }
    }

    void TickPeaceDeal()
    {
        // nothing for now
    }

}
