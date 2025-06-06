using UnityEngine;

public class Player : MonoBehaviour
{
    public string Name
    {
        get => name + $" with tag {tag}";
        set => name = value;
    }
}
