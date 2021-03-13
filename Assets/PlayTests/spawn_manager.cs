using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class spawn_manager
    {
        [UnityTest]
        public IEnumerator creates_three_pools_when_given_three_prefabs()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("Spawn Manager");
            SpawnManager spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
            spawnManager.Prefabs = new GameObject[3];

            spawnManager.Prefabs[0] = CreateObstacle(Vector3.zero);
            spawnManager.Prefabs[1] = CreatePole(Vector3.zero);
            spawnManager.Prefabs[2] = CreateWaste(Vector3.zero);

            // Act
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.IsTrue(spawnManager.PrefabPools[0] != null);
            Assert.IsTrue(spawnManager.PrefabPools[1] != null);
            Assert.IsTrue(spawnManager.PrefabPools[2] != null);

            // Clean
            spawnManagerObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator instantiates_30_pool_objects_when_given_three_prefabs_and_10_prefab_pool_size()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("Spawn Manager");
            SpawnManager spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
            spawnManager.Prefabs = new GameObject[3];
            spawnManager.PrefabPoolSize = 10;

            spawnManager.Prefabs[0] = CreateObstacle(Vector3.zero);
            spawnManager.Prefabs[1] = CreatePole(Vector3.zero);
            spawnManager.Prefabs[2] = CreateWaste(Vector3.zero);

            // Act
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(30, spawnManager.transform.childCount);

            // Clean
            spawnManagerObject.SetActive(false);
        }
        //[UnityTest]
        //public IEnumerator spawned_object_is_passed_to_dog_focus_queue()
        //{
        //    // Arrange
        //    GameObject spawnManagerObject = new GameObject("Spawn Manager");
        //    SpawnManager spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
        //    spawnManager.PrefabPoolSize = 3;
        //    spawnManager.Prefabs = new GameObject[1];

        //    spawnManager.Prefabs[0] = CreateObstacle(Vector3.zero);

        //    GameObject dogGameObject = new GameObject("Dog");
        //    dogGameObject.transform.position = Vector3.zero;
        //    Dog dog = dogGameObject.AddComponent<Dog>();

        //    spawnManager.Dog = dog;

        //    // Act
        //    yield return new WaitForSeconds(0.3f);
        //    spawnManager.RandomSpawn();

        //    // Assert
        //    Assert.AreEqual("Obstacle", dog.FocusQueue.Peek().transform.tag);

        //    // Clean
        //    spawnManagerObject.SetActive(false);
        //    dogGameObject.SetActive(false);
        //}
        [UnityTest]
        public IEnumerator spawns_objects_at_spawn_rate()
        {
            // Arrange
            GameObject spawnManagerObject = new GameObject("SpawnManager");
            SpawnManager spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
            spawnManager.SpawnRate = 0.30f;
            spawnManager.PrefabPoolSize = 5;
            spawnManager.Prefabs = new GameObject[1];

            spawnManager.Prefabs[0] = CreateObstacle(Vector3.zero);

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(4, spawnManager.ObjectsSpawned);

            // Clean
            spawnManagerObject.SetActive(false);
        }
        private static GameObject CreateObstacle(Vector3 spawnPos)
        {
            GameObject obstacleGameObject = new GameObject("Obstacle");
            obstacleGameObject.transform.tag = "Obstacle";
            obstacleGameObject.layer = 9;
            obstacleGameObject.transform.position = spawnPos;
            obstacleGameObject.AddComponent<Obstacle>();
            return obstacleGameObject;
        }
        private static GameObject CreatePole(Vector3 spawnPos)
        {
            GameObject poleGameObject = new GameObject("Pole");
            poleGameObject.transform.tag = "Pole";
            poleGameObject.transform.position = spawnPos;
            poleGameObject.AddComponent<Obstacle>();
            return poleGameObject;
        }
        private static GameObject CreateWaste(Vector3 spawnPos)
        {
            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = spawnPos;
            wasteGameObject.AddComponent<Obstacle>();
            return wasteGameObject;
        }
    }

}
