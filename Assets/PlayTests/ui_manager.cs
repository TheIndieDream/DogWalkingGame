using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ui_manager
    {
        [UnityTest]
        public IEnumerator updates_player_health_when_signaled_by_player()
        {
            // Arrange
            CreateUIManager(out GameObject uiManagerObject, out UIManager uiManager);
            CreateGameManager(out GameObject gameManagerObject, out GameManager gameManager);
            CreatePlayer(out GameObject playerObject, out Player player);
            CreateObstacle(out GameObject obstacleObject);

            // Act
            obstacleObject.GetComponent<Rigidbody>().AddForce(Vector3.back, ForceMode.VelocityChange);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(2, uiManager.PlayerHealth);

            // Clean
            uiManagerObject.SetActive(false);
            gameManagerObject.SetActive(false);
            playerObject.SetActive(false);
            obstacleObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator updates_player_score_according_to_spawn_manager_speed()
        {
            // Arrange
            CreateUIManager(out GameObject uiManagerObject, out UIManager uiManager);
            uiManager.PlayerScore = 0;
            CreateSpawnManager(out GameObject spawnManagerObject, out SpawnManager spawnManager);
            SpawnManager.CurrentSpeed = 1.0f;

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(1.0f, uiManager.PlayerScore);

            // Clean
            uiManagerObject.SetActive(false);
            spawnManagerObject.SetActive(false);
        }
        #region Helper Functions
        private void CreateGameManager(out GameObject gameManagerObject, out GameManager gameManager)
        {
            gameManagerObject = new GameObject("Game Manager");
            gameManager = gameManagerObject.AddComponent<GameManager>();
        }
        private void CreateObstacle(out GameObject obstacleObject)
        {
            obstacleObject = new GameObject("Obstacle");
            obstacleObject.transform.tag = "Obstacle";
            obstacleObject.transform.position = new Vector3(1, 0, 0);
            obstacleObject.AddComponent<Obstacle>();
        }
        private void CreatePlayer(out GameObject playerObject, out Player player)
        {
            playerObject = new GameObject("Player");
            playerObject.transform.position = Vector3.zero;
            player = playerObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
        }
        private void CreateSpawnManager(out GameObject spawnManagerObject, out SpawnManager spawnManager)
        {
            spawnManagerObject = new GameObject("Spawn Manager");
            spawnManager = spawnManagerObject.AddComponent<SpawnManager>();
        }
        private void CreateUIManager(out GameObject uiManagerObject, out UIManager uiManager)
        {
            uiManagerObject = new GameObject("UI Manager");
            uiManager = uiManagerObject.AddComponent<UIManager>();
        }
        #endregion
    }
}

