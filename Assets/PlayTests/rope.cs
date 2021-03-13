using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RopeMinikit;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Mathematics;

namespace Tests
{
    public class rope
    {
        [UnityTest]
        public IEnumerator alerts_game_manager_on_pole_contact()
        {
            // Arrange
            List<GameObject> cleanupList = new List<GameObject>();

            GameObject gameManagerObject = new GameObject("gameManager");
            GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
            cleanupList.Add(gameManagerObject);

            GameObject poleGameObject = new GameObject("Pole");
            poleGameObject.transform.tag = "Pole";
            poleGameObject.transform.position = new Vector3(0, 0, 1);
            poleGameObject.AddComponent<Obstacle>();
            cleanupList.Add(poleGameObject);

            GameObject cube0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube0.transform.position = new Vector3(-1.0f, 0.0f, 0.0f);
            Rigidbody cube0Rb = cube0.AddComponent<Rigidbody>();
            cube0Rb.useGravity = false;
            cleanupList.Add(cube0);

            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.transform.position = new Vector3(1.0f, 0.0f, 0.0f);
            Rigidbody cube1Rb = cube1.AddComponent<Rigidbody>();
            cube1Rb.useGravity = false;
            cleanupList.Add(cube1);

            List<float3> spawnPoints = new List<float3>();
            spawnPoints.Add(new float3(-1, 0, 0));
            spawnPoints.Add(new float3(1, 0, 0));

            GameObject ropeGameObject = new GameObject("Rope");
            Rope rope = ropeGameObject.AddComponent<Rope>();
            rope.spawnPoints = spawnPoints;
            rope.simulation.enabled = true;
            rope.collisions.enabled = true;
            rope.simulation.lengthMultiplier = 0.1f;
            rope.simulation.gravityMultiplier = 0.0f;

            RopeRigidbodyConnection ropeRBC0 = ropeGameObject.AddComponent<RopeRigidbodyConnection>();
            ropeRBC0.rope = rope;
            ropeRBC0.ropeLocation = 0;
            ropeRBC0.automaticallyFindRopeLocation = false;
            ropeRBC0.rigidbody = cube0Rb;
            ropeRBC0.localPointOnBody = Vector3.zero;
            ropeRBC0.rigidbodyDamping = 0.1f;
            ropeRBC0.stiffness = 1.0f;

            RopeRigidbodyConnection ropeRBC1 = ropeGameObject.AddComponent<RopeRigidbodyConnection>();
            ropeRBC1.rope = rope;
            ropeRBC1.ropeLocation = 0;
            ropeRBC1.automaticallyFindRopeLocation = false;
            ropeRBC1.rigidbody = cube1Rb;
            ropeRBC1.localPointOnBody = Vector3.zero;
            ropeRBC1.rigidbodyDamping = 0.1f;
            ropeRBC1.stiffness = 1.0f;

            // Act
            poleGameObject.GetComponent<Rigidbody>().AddForce(Vector3.back, ForceMode.VelocityChange);
            yield return new WaitForSeconds(0.5f);

            // Assert
            Assert.IsTrue(gameManager.IsGameOver);

            // Clean
            foreach (GameObject item in cleanupList)
            {
                item.SetActive(false);
            }
        }

    }
}
