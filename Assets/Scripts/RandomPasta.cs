using UnityEngine;

[CreateAssetMenu(fileName = "RandomPasta", menuName = "Scriptable Objects/RandomPasta")]
public class RandomPasta : ScriptableObject
{
    public PastaNoodle noodle;
    public PastaSauce sauce;
    public PastaTopping topping;
    
}
