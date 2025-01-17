using UnityEngine;

public class LevelManager1 : MonoBehaviour
{
    [SerializeField] private GameObject levelPrefab;

    private GameObject currentLevel;

    private void Start()
    {
        currentLevel = Instantiate(levelPrefab);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player") 
        {
            Destroy(currentLevel);

            currentLevel = Instantiate(levelPrefab);
        }
    }
}
