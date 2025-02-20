using System;
using UnityEngine;

[Serializable]
public class ShipStatistics {

    //Caps
    int shieldPowerMin;
    int shieldPowerMax;
    float fireRateMin;
    float fireRateMax;
    int thrustForceMin;
    int thrustForceMax;


    //Statistics
    [SerializeField] private int shieldPower;
    [SerializeField] private float fireRate;
    [SerializeField] private float thrustForce;
    [SerializeField] private int shieldPowerMax1;

    public int ShieldPower { get => shieldPower; protected set => System.Math.Clamp(value, 0, ShieldPowerMax); }  //0-100
    public float FireRate { get => fireRate; protected set => fireRate = value; } //seconds of cool down between shots
    public float ThrustForce { get => thrustForce; protected set => thrustForce = value; } //3-10?
    public int ShieldPowerMax { get => shieldPowerMax1; protected set => shieldPowerMax1 = value; }

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
        ShieldPower = shieldPowerMax; //full shields
        FireRate = fireRateMax; //slowest fire rate
        ThrustForce = thrustForceMin; //Min thrust force
    }

    //**UTILITY METHODS**
    public void ApplyStatisticsMod(ShipStatisticModifierData newStatModData) {
        ShieldPower = Mathf.Clamp(ShieldPower + newStatModData.ShieldPowerMod, shieldPowerMin, shieldPowerMax);
        FireRate = Mathf.Clamp(FireRate + newStatModData.FireRateMod, fireRateMin, fireRateMax);
        ThrustForce = Mathf.Clamp(ThrustForce + newStatModData.ThrustForceMod, thrustForceMin, thrustForceMax);
    }

    public override string ToString() {
        return $"Shield Power: {ShieldPower}/{shieldPowerMax} | Fire Rate: {FireRate}s | ThrustForce: {ThrustForce}";
    }
}


[Serializable]
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
