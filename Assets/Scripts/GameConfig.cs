using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Ghosts n' Goblins/GameConfig")]
public class GameConfig : ScriptableObject
{
    private static GameConfig _instance;

    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameConfig>("GameConfig");

                if (_instance == null)
                {
                    Debug.LogError("No GameConfig instance found in Resources! Please create one and place it in a Resources folder.");
                }
            }
            return _instance;
        }
    }
    
    
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    public LayerMask sparkLayer;
    public LayerMask graveColliderLayer;
    public LayerMask ladderLayer;
    public LayerMask playerWeaponLayer;
    public LayerMask backgroundLayer;
    public LayerMask marksLayer;
    public LayerMask enemySpawnZoneLayer;
    public LayerMask borderLayer;
    public LayerMask keyLayer;
    public LayerMask enemyWeaponLayer;

    public void ControlLayersCollision(LayerMask layer1, LayerMask layer2, bool control)
    {
        int layer1Index = (int)Mathf.Log(layer1.value, 2);
        int layer2Index = (int)Mathf.Log(layer2.value, 2);
        Physics2D.IgnoreLayerCollision(layer1Index, layer2Index,!control);
    }

    public void ControlObjectsCollision(Collider2D collider1, Collider2D collider2, bool control)
    {
        Physics2D.IgnoreCollision(collider1, collider2, !control);
    }

}
