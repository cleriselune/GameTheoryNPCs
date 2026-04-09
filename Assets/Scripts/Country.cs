using UnityEngine;

public class Country
{
    public string countryName;
    public float militaryPower;
    public float economicStrength;
    public float aggressionLevel; // 0-1 scale, where 1 is most aggressive
    // could change it to structure depending on country type (e.g., democracy, dictatorship, etc.)

    // Country constructor
    public Country(string countryName, float militaryPower, float economicStrength, float aggressionLevel)
    {
        this.countryName = countryName;
        this.militaryPower = militaryPower;
        this.economicStrength = economicStrength;
        this.aggressionLevel = aggressionLevel;
    }

}
