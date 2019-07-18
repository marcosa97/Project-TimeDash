
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueText", menuName = "ScriptableObjects/DialogueTexts", order = 1)]
public class DialogueTexts : ScriptableObject {

    public Dialogue GotHPPotion;
    public Dialogue GotSPPotion;
    public Dialogue GotKey;

    public Dialogue NoDoorKey;
}
