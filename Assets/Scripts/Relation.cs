using UnityEngine;

public class Relation : MonoBehaviour
{
    int idA;
    int idB;
    RelationState state = RelationState.Neutral; // default state is neutral
    float opinion = 0; // opinion of each other, dunno if needed for now but im leaving it for now
    float allicanceTurns = 0; // how many turns have been in alliance, used for a timer of a timed alliance
    float warTurns = 0; // how many turns have been at war, used for a timer of a timed war

    //
    public const float ALLY_THRESHOLD = 80f; // opinion threshold to form alliance
    public const float ALLIANCE_DURATION = 5f; // duration of alliance in turns (years)
    public const float WAR_DURATION = 5f; // duration of war in turns (years);; will change this

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
        (RelationState.Neutral, RelationEvent.OpinionHighEnough) => RelationState.Allied,
        (RelationState.Neutral, RelationEvent.BothPlayedHawk) => RelationState.AtWar,
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
        // nothing for now
    }

    void TickAtWar()
    {
        warTurns += 1;
        if (warTurns >= WAR_DURATION) {
            //Country countryA = GameManagergetCountry(idA); smthh like this but game manager is not static so i need to figure out how to access it from here, maybe a different class?
            //Country countryB = GameManager.getCountry(idB);

            // combat resolution here: A and B both lose 20% of the weaker side's military power
            warTurns = 0;
            Fire(RelationEvent.WarEnded); // war ended by timer
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
