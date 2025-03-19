using UnityEngine;
[CreateAssetMenu(fileName = "GridConfig", menuName = "GridConfig")]
public class GridConfig : ScriptableObject
{
    [Header("grid movement config")]
    public float nodeSize;
    public LayerMask platformLayer;
    public Vector2 gridSize;
}
