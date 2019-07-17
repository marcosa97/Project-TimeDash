using UnityEngine;

[CreateAssetMenu]
public class PlayerSPValues : ScriptableObject {
    public int NormalAttackSPGain = 1;
    public int WarpStrikeSPCost = 10;

    //Potions
    public int HPPotionGain = 5;
    public int SPPotionGain = 10; //1 full bar
}
