using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class player
    {
        [UnityTest]
        public IEnumerator alerts_game_manager_when_health_is_empty()
        {
            // Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 70;
            player.Health = new Health(1);

            GameObject obstacleGameObject = new GameObject("Obstacle");
            obstacleGameObject.transform.tag = "Obstacle";
            obstacleGameObject.transform.position = new Vector3(2, 0, 0);
            Collider obstacleCollider = obstacleGameObject.AddComponent<BoxCollider>();
            obstacleCollider.isTrigger = true;

            GameManager gameManager = ScriptableObject.CreateInstance<GameManager>();

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            yield return new WaitForSeconds(1.0f);
            Assert.IsTrue(gameManager.GameOver);
        }
        [UnityTest]
        public IEnumerator contact_with_waste_grants_debuff()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 70;

            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = new Vector3(2, 0, 0);
            Collider wasteCollider = wasteGameObject.AddComponent<BoxCollider>();
            wasteCollider.isTrigger = true;

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            yield return new WaitForSeconds(1f);
            Assert.IsTrue(player.IsDebuffed);
        }
        [UnityTest]
        public IEnumerator contact_with_obstacle_reduces_health_by_one()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 70;

            GameObject wasteGameObject = new GameObject("Obstacle");
            wasteGameObject.transform.tag = "Obstacle";
            wasteGameObject.transform.position = new Vector3(2, 0, 0);
            Collider wasteCollider = wasteGameObject.AddComponent<BoxCollider>();
            wasteCollider.isTrigger = true;

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(player.Health.Max - 1, player.Health.Current);
        }
        [UnityTest]
        public IEnumerator contact_with_pole_reduces_health_to_zero()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 70;

            GameObject poleGameObject = new GameObject("Pole");
            poleGameObject.transform.tag = "Pole";
            poleGameObject.transform.position = new Vector3(2, 0, 0);
            Collider poleCollider = poleGameObject.AddComponent<BoxCollider>();
            poleCollider.isTrigger = true;

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            yield return new WaitForSeconds(1f);
            Assert.AreEqual(0, player.Health.Current);
        }
        [UnityTest]
        public IEnumerator debuff_wears_off_after_three_seconds()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 70;

            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = new Vector3(2, 0, 0);
            Collider wasteCollider = wasteGameObject.AddComponent<BoxCollider>();
            wasteCollider.isTrigger = true;

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            while (!player.IsDebuffed)
            {
                yield return null;
            }
            yield return new WaitForSeconds(3.0f);
            Assert.IsFalse(player.IsDebuffed);
        }
        [UnityTest]
        public IEnumerator debuffed_with_negative_horizontal_input_moves_right()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 1;
            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            //Act
            player.PlayerInput.Horizontal.Returns(-1);
            player.IsDebuffed = true;

            // Assert
            yield return new WaitForSeconds(0.3f);

            Assert.IsTrue(player.transform.position.x > 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);
        }
        [UnityTest]
        public IEnumerator debuffed_with_positive_horizontal_input_moves_left()
        {
            //Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 1;
            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            //Act
            player.PlayerInput.Horizontal.Returns(1);
            player.IsDebuffed = true;

            // Assert
            yield return new WaitForSeconds(0.3f);

            Assert.IsTrue(player.transform.position.x < 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);
        }
        [UnityTest]
        public IEnumerator mouse_to_right_of_player_moves_dog_right()
        {
            //Arrange
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 10, 0);
            cameraObject.transform.eulerAngles = new Vector3(67.72f, -0.103f, 0);

            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();

            GameObject dogGameObject = new GameObject("Dog");
            Rigidbody dogRb = dogGameObject.AddComponent<Rigidbody>();
            dogRb.useGravity = false;
            dogRb.constraints = RigidbodyConstraints.FreezeRotation;
            dogGameObject.transform.position = new Vector3(0, 0, 2);
            player.Dog = dogGameObject;
            float dogInitY = dogGameObject.transform.position.y;
            float dogInitZ = dogGameObject.transform.position.z;

            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(playerGameObject.transform.position);

            // Act
            player.PlayerInput.MousePosition.Returns(playerScreenPosition + new Vector3(2, 0, 0));

            // Assert
            yield return new WaitForSeconds(1.0f);
            Assert.IsTrue(dogGameObject.transform.position.x > 0);
            Assert.AreEqual(dogInitY, dogGameObject.transform.position.y);
            Assert.AreEqual(dogInitZ, dogGameObject.transform.position.z);
        }
        [UnityTest]
        public IEnumerator mouse_to_left_of_player_moves_dog_left()
        {
            //Arrange
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.AddComponent<Camera>();
            cameraObject.transform.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 10, 0);
            cameraObject.transform.eulerAngles = new Vector3(67.72f, -0.103f, 0);

            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();

            GameObject dogGameObject = new GameObject("Dog");
            Rigidbody dogRb = dogGameObject.AddComponent<Rigidbody>();
            dogRb.useGravity = false;
            dogRb.constraints = RigidbodyConstraints.FreezeRotation;
            dogGameObject.transform.position = new Vector3(0, 0, 2);
            player.Dog = dogGameObject;
            float dogInitY = dogGameObject.transform.position.y;
            float dogInitZ = dogGameObject.transform.position.z;

            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(playerGameObject.transform.position);

            // Act
            player.PlayerInput.MousePosition.Returns(playerScreenPosition - new Vector3(2, 0, 0));

            // Assert
            yield return new WaitForSeconds(1.0f);
            Assert.IsTrue(dogGameObject.transform.position.x < 0);
            Assert.AreEqual(dogInitY, dogGameObject.transform.position.y);
            Assert.AreEqual(dogInitZ, dogGameObject.transform.position.z);
        }
        [UnityTest]
        public IEnumerator with_negative_horizontal_input_moves_left()
        {
            // Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 1;
            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            // Act
            player.PlayerInput.Horizontal.Returns(-1);

            // Assert
            yield return new WaitForSeconds(0.3f);

            Assert.IsTrue(player.transform.position.x < 0);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);
        }
        [UnityTest]
        public IEnumerator with_positive_horizontal_input_moves_right()
        {
            // Arrange
            GameObject playerGameObject = new GameObject("Player");
            Player player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
            player.MoveSpeed = 1;
            float initY = player.transform.position.y;
            float initZ = player.transform.position.z;

            // Act
            player.PlayerInput.Horizontal.Returns(1);

            // Assert
            yield return new WaitForSeconds(0.3f);

            Assert.IsTrue(player.transform.position.x > 0);
            Debug.Log(player.transform.position.x);
            Assert.AreEqual(initY, player.transform.position.y);
            Assert.AreEqual(initZ, player.transform.position.z);
        }
    }
}
