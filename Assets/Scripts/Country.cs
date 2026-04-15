public class Country
{
    public string countryName;
    public float militaryPower;
    public float economicStrength;
    public float treasury; // the rescource available to the country, can be 'won' from war
    
    //public float aggressionLevel; // 0-1 scale, where 1 is most aggressive
    // changing it to structure depending on country personality (e.g., democracy, dictatorship, etc.)
    public AiPersonality aiPersonality;
    public enum AiPersonality
    {
        Democratic, // more likely to form alliances, less likely to declare war
        Balanced, // middle ground
        Militaristic // war centric
    }

    public float GetAiPersonality(AiPersonality aiPersonality)
    {
        switch (aiPersonality)
        {
            case AiPersonality.Democratic:
                return 0.2f;
            case AiPersonality.Balanced:
                return 0.5f;
            case AiPersonality.Militaristic:
                return 0.8f;
            default:
                return 0.5f; // default to balanced
        }
    }

    // Country constructor
    public Country(string countryName, float militaryPower, float economicStrength, float treasury, AiPersonality personality)
    {
        this.countryName = countryName;
        this.militaryPower = militaryPower;
        this.economicStrength = economicStrength;
        this.treasury = treasury;
        this.aiPersonality = personality;
    }

}
