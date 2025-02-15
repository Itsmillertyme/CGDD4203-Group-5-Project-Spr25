using UnityEngine;

public class ShipStatistics {

    //**PROPERTIES**
    //Statistics
    int shieldPower; //0-100
    float fireRate; //seconds of cool down between shots
    int thrustForce; //3-10?


    //Caps
    int shieldPowerMin;
    int shieldPowerMax;
    float fireRateMin;
    float fireRateMax;
    int thrustForceMin;
    int thrustForceMax;

    //**FIELDS**
    public int ShieldPower { get => shieldPower; }
    public float FireRate { get => fireRate; }
    public int ThrustForce { get => thrustForce; }
    public int ShieldPowerMax { get => shieldPowerMax; set => shieldPowerMax = value; }

    //**CONSTRUCTOR**
    public ShipStatistics() {

        //Initialize caps
        shieldPowerMin = 0;
        shieldPowerMax = 100;
        fireRateMin = 0.1f; //?
        fireRateMax = 1f;
        thrustForceMin = 3;
        thrustForceMax = 10; //?

        //Initialize statistics
        shieldPower = shieldPowerMax; //full shields
        fireRate = fireRateMax; //slowest fire rate
        thrustForce = thrustForceMin; //Min thrust force
    }

    //**UTILITY METHODS**
    public void ApplyStatisticsMod(ShipStatisticModifierData newStatModData) {
        shieldPower = Mathf.Clamp(shieldPower + newStatModData.ShieldPowerMod, shieldPowerMin, shieldPowerMax);
        fireRate = Mathf.Clamp(fireRate + newStatModData.FireRateMod, fireRateMin, fireRateMax);
        thrustForce = Mathf.Clamp(thrustForce + newStatModData.ThrustForceMod, thrustForceMin, thrustForceMax);
    }

    public override string ToString() {
        return $"Shield Power: {shieldPower}/{shieldPowerMax} | Fire Rate: {fireRate}s | ThrustForce: {ThrustForce}";
    }
}



public struct ShipStatisticModifierData {
    //**PROPERTIES**
    int shieldPowerMod;
    float fireRateMod;
    int thrustForceMod;

    // **FIELDS**
    public int ShieldPowerMod { get => shieldPowerMod; }
    public float FireRateMod { get => fireRateMod; }
    public int ThrustForceMod { get => thrustForceMod; }

    //**CONSTRUCTOR**  
    public ShipStatisticModifierData(int shipPowerModIn, float fireRateModIn, int thrustForceModIn) {
        shieldPowerMod = shipPowerModIn;
        fireRateMod = fireRateModIn;
        thrustForceMod = thrustForceModIn;
    }
}
