using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                    instance = new GameObject(nameof(GameManager)).AddComponent<GameManager>();
            }
            return instance;
        }
    }
}
