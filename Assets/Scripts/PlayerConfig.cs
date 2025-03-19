using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("player movement config")]
    public float speed;
    public float stoppingDistance;
    public float fallSpeedMultiplier;
    public float jumpDuration;

}
