using UnityEngine;

public static class GameTheory
{
    // SIMPLE payoffs (matrix) for Hawk-Dove game
    // A and B represent the strategies of two countries
    public static Vector2 HawkDovePayoff(StrategyType A, StrategyType B)
    {
        if (A == StrategyType.Hawk && B == StrategyType.Hawk)
            return new Vector2(-2, -2);

        if (A == StrategyType.Hawk && B == StrategyType.Dove)
            return new Vector2(4, -1);

        if (A == StrategyType.Dove && B == StrategyType.Hawk)
            return new Vector2(-1, 4);

        return new Vector2(2, 2);
    }
}