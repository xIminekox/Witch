using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;


public class PerlinMapGenerator : MonoBehaviour {
    [SerializeField] private Tilemap tilemapGrass; // Ссылка на ваш Tilemap
    [SerializeField] private Tilemap tilemapWater;

    [SerializeField] private Tile groundTileMain; // Тайл для земли
    [SerializeField] private Tile groundTile1;
    [SerializeField] private Tile groundTile2;
        
    [SerializeField] private Tile waterTileTop;
    [SerializeField] private Tile waterTileBottom;
    [SerializeField] private Tile waterTileLeft;
    [SerializeField] private Tile waterTileRight;

    [SerializeField] private Tile waterTileUpperLeftCorner;
    [SerializeField] private Tile waterTileUpperRightCorner;
    [SerializeField] private Tile waterTileLowerLeftCorner;
    [SerializeField] private Tile waterTileLowerRightCorner;

    [SerializeField] private Tile waterTileLoopLeft;
    [SerializeField] private Tile waterTileLoopRight;
    [SerializeField] private Tile waterTileLoopTop;
    [SerializeField] private Tile waterTileLoopBottom;

    [SerializeField] private Tile waterTileCircle;
    [SerializeField] private Tile waterTileHorRiver;
    [SerializeField] private Tile waterTileVerRiver;

    [SerializeField] private Tile waterTileUpperLeftVertex;
    [SerializeField] private Tile waterTileUpperRightVertex;
    [SerializeField] private Tile waterTileLowerLeftVertex;
    [SerializeField] private Tile waterTileLowerRightVertex;

    [SerializeField] private Tile waterTile; // Тайл для воды
    [SerializeField] private int width = 20; // Ширина карты
    [SerializeField] private int height = 20; // Высота карты
    [SerializeField] private int borders = 20;
    [SerializeField] private float scale = 1.0f; // Масштаб шума
    [SerializeField] private int maxSid = 10;

    [SerializeField] private GameObject Skillet;
    [SerializeField] private GameObject Tree;
    [SerializeField] private GameObject Bush;
    [SerializeField] private int numberOfTree = 5;
    [SerializeField] private int numberOfBushes = 5;
    [SerializeField] private int numberOfSkillets = 5;

    [SerializeField] private Transform parentObject;

    private GameObject navMeshSurfaceOdj;
    private NavMeshSurface navMeshSurface;
    private GameObject Player;
    private GameObject Teleport;

    private int fullWidth;
    private int fullHeight;

    private int count = 0;

    void Start()
    {
        fullWidth = width + borders;
        fullHeight = height + borders;

        Player = GameObject.Find("Player");
        Teleport = GameObject.Find("Teleport");
        navMeshSurfaceOdj = GameObject.Find("NavMeshSurface");
        navMeshSurface = navMeshSurfaceOdj.GetComponent<NavMeshSurface>();

        GenerateMap();
    }

    void GenerateMap()
    {
        // Очищаем Tilemap перед генерацией
        tilemapGrass.ClearAllTiles();
        tilemapWater.ClearAllTiles();

        int sid = Random.Range(0, maxSid);

        float[,] samples = new float[fullWidth, fullHeight];

        for (int x = borders / 2 + 1; x <= width + borders / 2; x++)
        {
            for (int y = borders / 2 + 1; y <= height + borders / 2; y++)
            {
                // Генерация значения шума Перлина
                float xCoord = ((float)x + sid) / width * scale;
                float yCoord = ((float)y + sid) / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                samples[x, y] = sample;
            }
        }

        for (int x = 0; x < borders / 2; x++)
        {
            for (int y = 0; y < borders / 2; y++)
            {
                samples[x, y] = 0;
            }
        }

        for (int x = width + borders / 2 + 1; x < fullWidth; x++)
        {
            for (int y = height + borders / 2 + 1; y < fullHeight; y++)
            {
                samples[x, y] = 0;
            }
        }

        DefiningBoundaries(samples);
    }

    void DefiningBoundaries(float[,] samples)
    {
        for (int x = 1; x < fullWidth - 1; x++)
        {
            for (int y = 1; y < fullHeight - 1; y++)
            {
                // Установка тайла в зависимости от значения шума
                // if ((tilemap.GetTile(new Vector3Int(x, y + 1, 0)).name == "groundTileMain") && (tilemap.GetTile(new Vector3Int(x, y, 0)).name == "waterTile"))
                if (samples[x, y] >= 0.3)
                {
                    int rand = Random.Range(0, 10);

                    if (rand == 5)
                    {
                        tilemapGrass.SetTile(new Vector3Int(x, y, 0), groundTile1);
                    }
                    else if (rand == 8)
                    {
                        tilemapGrass.SetTile(new Vector3Int(x, y, 0), groundTile2);
                    }
                    else
                    {
                        tilemapGrass.SetTile(new Vector3Int(x, y, 0), groundTileMain);
                    }
                }
                else
                {
                    if (samples[x + 1, y] >= 0.3 && samples[x - 1, y] >= 0.3 && samples[x, y + 1] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileCircle);
                    }

                    else if (samples[x - 1, y] >= 0.3 && samples[x, y + 1] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLoopLeft);
                    }
                    else if (samples[x + 1, y] >= 0.3 && samples[x, y + 1] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLoopRight);
                    }
                    else if (samples[x, y + 1] >= 0.3 && samples[x + 1, y] >= 0.3 && samples[x - 1, y] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLoopTop);
                    }
                    else if (samples[x, y - 1] >= 0.3 && samples[x + 1, y] >= 0.3 && samples[x - 1, y] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLoopBottom);
                    }

                    else if (samples[x + 1, y] >= 0.3 && samples[x - 1, y] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileVerRiver);
                    }
                    else if (samples[x, y + 1] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileHorRiver);
                    }

                    else if (samples[x - 1, y] >= 0.3 && samples[x, y + 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileUpperLeftCorner);
                    }
                    else if (samples[x + 1, y] >= 0.3 && samples[x, y + 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileUpperRightCorner);
                    }
                    else if (samples[x - 1, y] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLowerLeftCorner);
                    }
                    else if (samples[x + 1, y] >= 0.3 && samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLowerRightCorner);
                    }

                    else if (samples[x, y + 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileTop);
                    }
                    else if (samples[x, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileBottom);
                    }
                    else if (samples[x + 1, y] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileRight);
                    }
                    else if (samples[x - 1, y] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLeft);
                    }

                    else if (samples[x - 1, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLowerLeftVertex);
                    }
                    else if (samples[x - 1, y + 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileUpperLeftVertex);
                    }
                    else if (samples[x + 1, y + 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileUpperRightVertex);
                    }
                    else if (samples[x + 1, y - 1] >= 0.3)
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTileLowerRightVertex);
                    }
                    else
                    {
                        tilemapWater.SetTile(new Vector3Int(x, y, 0), waterTile);
                    }
                }
            }
        }

        Spawn();
        PlayerTeleport();
    }

    private void Spawn()
    {
        for (int i = 0; i < numberOfTree; i++)
        {
            Vector2 rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));

            while (Physics2D.OverlapCircle(rand, 3) || tilemapWater.GetTile(tilemapWater.WorldToCell(rand)) || count < 5)
            {
                rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));
                count++;
            }

            count = 0;

            if (!Physics2D.OverlapCircle(rand, 3) && !tilemapWater.GetTile(tilemapWater.WorldToCell(rand)))
                Instantiate(Tree, rand, Quaternion.identity, parentObject);
        }

        for (int i = 0; i < numberOfBushes; i++)
        {
            Vector2 rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));

            while (Physics2D.OverlapCircle(rand, 3) || tilemapWater.GetTile(tilemapWater.WorldToCell(rand)) || count < 5)
            {
                rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));
                count++;
            }

            count = 0;

            if (!Physics2D.OverlapCircle(rand, 3) && !tilemapWater.GetTile(tilemapWater.WorldToCell(rand)))
                Instantiate(Bush, rand, Quaternion.identity, parentObject);
        }

        navMeshSurface.BuildNavMesh();

        for (int i = 0; i < numberOfSkillets; i++)
        {
            Vector2 rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));

            while (Physics2D.OverlapCircle(rand, 3) || tilemapWater.GetTile(tilemapWater.WorldToCell(rand)) || count < 5)
            {
                rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));
                count++;
            }

            count = 0;

            if (!Physics2D.OverlapCircle(rand, 3) && !tilemapWater.GetTile(tilemapWater.WorldToCell(rand)))
                Instantiate(Skillet, rand, Quaternion.identity, parentObject);
        }

        Vector2 randTP = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));

        while (Physics2D.OverlapCircle(randTP, 3) || tilemapWater.GetTile(tilemapWater.WorldToCell(randTP)) || count < 5)
        {
            randTP = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));
            count++;
        }

        count = 0;

        if (!Physics2D.OverlapCircle(randTP, 3) && !tilemapWater.GetTile(tilemapWater.WorldToCell(randTP)))
            Teleport.transform.position = randTP;
    }

    private void PlayerTeleport()
    {
        Vector2 rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));

        while (Physics2D.OverlapCircle(rand, 3) || tilemapWater.GetTile(tilemapWater.WorldToCell(rand)) || count < 5)
        {
            rand = new Vector2(Random.Range(borders / 2, width + borders / 2), Random.Range(borders / 2, height + borders / 2));
            count++;
        }

        count = 0;

        if (!Physics2D.OverlapCircle(rand, 3) && !tilemapWater.GetTile(tilemapWater.WorldToCell(rand)))
            Player.transform.position = rand;
    }

}
