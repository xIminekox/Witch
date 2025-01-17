using System.Security.Principal;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject Tree;
    [SerializeField] private GameObject Bush;
    [SerializeField] private int quantity = 5;
    [SerializeField] private Transform parentObject;

    private Vector2 boxSize = new Vector2(4, 4);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        for (int i = 0; i < quantity; i++)
        {
            Vector2 rand = new Vector2(Random.Range(3, 20f), Random.Range(3, 20f));

            if (!Physics2D.BoxCast(rand, boxSize, 0f, Vector2.zero)){
                Instantiate(Tree, rand, Quaternion.identity, parentObject);
            }
        }
    }
}
