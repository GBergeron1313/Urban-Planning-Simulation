using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Vector2Int gridSize = new Vector2Int(5, 5); // 5x5 grid
    public float cellSize = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomJobLocation()
    {
        int x = Random.Range(0, gridSize.x);
        int y = Random.Range(0, gridSize.y);
        return new Vector3(x * cellSize, 0, y * cellSize);
    }
}