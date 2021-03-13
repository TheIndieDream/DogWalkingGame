using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int PrefabPoolSize;
    public GameObject[] Prefabs;
    public float SpawnRate { get; set; }
    public static float CurrentSpeed { get; set; }
    public GameObject[][] PrefabPools { get; set; }
    public Dog Dog;
    public int ObjectsSpawned { get; set; }

    public void Start()
    {
        PrefabPools = new GameObject[Prefabs.Length][];
        InitializeObjectPools();
        InvokeRepeating("RandomSpawn", 0.0f, SpawnRate);
    }

    private GameObject GetObjectFromPool(int objectIndex)
    {
        GameObject[] prefabPool = PrefabPools[objectIndex];
        for (int i = 0; i < prefabPool.Length; i++)
        {
            if (!prefabPool[i].activeInHierarchy)
            {
                prefabPool[i].GetComponent<Rigidbody>().Sleep();
                return prefabPool[i];
            }
        }
        return null;
    }

    private void InitializeObjectPools()
    {
        for(int i = 0; i < Prefabs.Length; i ++)
        {
            PrefabPools[i] = new GameObject[PrefabPoolSize];
            for(int j = 0; j < PrefabPoolSize; j++)
            {
                PrefabPools[i][j] = Instantiate(Prefabs[i], transform);
                PrefabPools[i][j].SetActive(false);
            }
        }
    }

    public void RandomSpawn()
    {
        // Randomly choose object to spawn
        int randomObject = Random.Range(0, PrefabPools.Length);

        // Randomly choose position to spawn
        float randomXPos = Random.Range(Obstacle.BoundXMin, Obstacle.BoundXMax);

        GameObject objectToSpawn = GetObjectFromPool(randomObject);
        if(objectToSpawn == null)
        {
            return;
        }
        else
        {
            objectToSpawn.transform.position = new Vector3(randomXPos, 0.0f, transform.position.z);
            objectToSpawn.SetActive(true);
            Rigidbody objectRb = objectToSpawn.GetComponent<Rigidbody>();
            if(objectRb != null)
            {
                objectRb.AddForce(Vector3.back * CurrentSpeed, ForceMode.VelocityChange);

                if(Dog != null)
                {
                    Dog.AddObjectToFocusQueue(objectToSpawn);
                }
                ObjectsSpawned += 1;
            }
            else
            {
                Debug.LogError("SpawnManager.cs :: Tried to spawn object without a Rigidbody.");
            }
        }
    }

    
}
