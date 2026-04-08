public enum RelationState {
    Neutral,
    Allied,
    AtWar,
    PeaceDeal   // transient: immediately resolves to Neutral or back to AtWar
}

public enum RelationEvent {
    BothPlayedHawk, // Hawk-Dove outcome = war
    OpinionHighEnough, // opinion > threshold = ally
    AllyCalledToWar, // allied nation attacked = war
    AllianceExpired, // timer ran out = clean reset
    PeaceOffered, // one side wants out of war into peace = pending peace deal
    PeaceAccepted, // both sides want out of war = peace
    PeaceRejected, // one side wants out of war but other wants to keep fighting = back to war
    WarEnded, // war ended by other means (e.g. one side conquered) = clean reset
}