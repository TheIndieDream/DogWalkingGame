using System.Collections;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class dog
    {
        #region Completed Tests
        [UnityTest]
        public IEnumerator avoids_obstacle()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog);

            GameObject obstacleGameObject = CreateObstacle(new Vector3(0.0f, 0.0f, 4.0f));
            obstacleGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleGameObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesCollected);

            // Clean
            dogGameObject.SetActive(false);
            obstacleGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator avoids_obstacle_and_picks_up_waste_when_obstacle_is_first()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(0.0f, 0.0f, 0.0f));

            GameObject obstacleGameObject = CreateObstacle(new Vector3(0.0f, 0.0f, 4.0f));
            obstacleGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            GameObject wasteGameObject = CreateWaste(new Vector3(0.0f, 0.0f, 8.0f));
            wasteGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleGameObject);
            dog.AddObjectToFocusQueue(wasteGameObject);
            yield return new WaitForSeconds(4.5f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesCollected);
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogGameObject.SetActive(false);
            obstacleGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator avoids_obstacle_and_picks_up_waste_when_waste_is_first()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(4.0f, 0.0f, 0.0f));

            GameObject wasteGameObject = CreateWaste(new Vector3(2.0f, 0.0f, 4.0f));
            wasteGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            GameObject obstacleGameObject = CreateObstacle(new Vector3(2.0f, 0.0f, 8.0f));
            obstacleGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(wasteGameObject);
            dog.AddObjectToFocusQueue(obstacleGameObject);
            yield return new WaitForSeconds(4.5f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesCollected);
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogGameObject.SetActive(false);
            obstacleGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator avoids_obstacle_by_moving_left_if_obstacle_is_too_far_right()
        {
            // Arrange
            Vector3 dogStartPos = new Vector3(4.0f, 0.0f, 0.0f);
            CreateDog(out GameObject dogGameObject, out Dog dog, dogStartPos);

            GameObject obstacleGameObject = CreateObstacle(new Vector3(4.0f, 0.0f, 4.0f));
            obstacleGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleGameObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesCollected);
            Assert.IsTrue(dogGameObject.transform.position.x < dogStartPos.x);

            // Clean
            dogGameObject.SetActive(false);
            obstacleGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator avoids_obstacle_by_moving_right_if_obstacle_is_too_far_left()
        {
            // Arrange
            Vector3 dogStartPos = new Vector3(-4.0f, 0.0f, 0.0f);
            CreateDog(out GameObject dogGameObject, out Dog dog, dogStartPos);

            GameObject obstacleGameObject = CreateObstacle(new Vector3(-4.0f, 0.0f, 4.0f));
            obstacleGameObject.GetComponent<Rigidbody>().velocity = Vector3.back * 2.0f;

            // Act
            dog.AddObjectToFocusQueue(obstacleGameObject);
            yield return new WaitForSeconds(2.0f);

            // Assert
            Assert.AreEqual(0, dog.ObstaclesCollected);
            Assert.IsTrue(dogGameObject.transform.position.x > dogStartPos.x);

            // Clean
            dogGameObject.SetActive(false);
            obstacleGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator drags_player_left()
        {
            // Arrange
            CreatePlayer(out GameObject playerGameObject, out Player player);
            CreateDog(out GameObject dogGameObject, out Dog dog);
            player.Dog = dog;
            dog.Player = player;

            // Act
            dog.targetPos = new Vector3(-6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(playerGameObject.transform.position.x < 0.0f);

            // Clean
            playerGameObject.SetActive(false);
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator drags_player_right()
        {
            // Arrange
            CreatePlayer(out GameObject playerGameObject, out Player player);
            CreateDog(out GameObject dogGameObject, out Dog dog);
            player.Dog = dog;
            dog.Player = player;

            // Act
            dog.targetPos = new Vector3(6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.IsTrue(playerGameObject.transform.position.x > 0.0f);

            // Clean
            playerGameObject.SetActive(false);
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator ignores_waste_object_to_left_if_not_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(4.9f, 0.0f, 2.0f));

            GameObject wasteGameObject = CreateWaste(new Vector3(-4.9f, 0.0f, 3.0f));
            wasteGameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 2, ForceMode.VelocityChange);

            // Act
            yield return new WaitForSeconds(0.3f);
            dog.AddObjectToFocusQueue(wasteGameObject);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(new Vector3(4.9f, 0.0f, 2.0f), dogGameObject.transform.position);

            // Clean
            dogGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator ignores_waste_object_to_right_if_not_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(-4.9f, 0.0f, 2.0f));

            GameObject wasteGameObject = CreateWaste(new Vector3(4.9f, 0.0f, 3.0f));
            wasteGameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * 2, ForceMode.VelocityChange);

            // Act
            yield return new WaitForSeconds(0.3f);
            dog.AddObjectToFocusQueue(wasteGameObject);
            yield return new WaitForSeconds(0.3f);

            // Assert
            Assert.AreEqual(new Vector3(-4.9f, 0.0f, 2.0f), dogGameObject.transform.position);

            // Clean
            dogGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator obeys_max_x_pos_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(4.8f, 0.0f, 0.0f));
            Rigidbody dogRb = dogGameObject.GetComponent<Rigidbody>();

            // Act
            dog.targetPos = new Vector3(6, 0, 0);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMax, dogRb.transform.position.x, 0.1f);

            // Clean
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator obeys_min_x_pos_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(-4.8f, 0.0f, 0.0f));
            Rigidbody dogRb = dogGameObject.GetComponent<Rigidbody>();

            // Act
            dog.targetPos = new Vector3(-6, 0, 0);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(Obstacle.BoundXMin, dogRb.transform.position.x, 0.1f);

            // Clean
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator obeys_negative_max_velocity_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(4.8f, 0.0f, 0.0f));

            // Act
            dog.targetPos = new Vector3(-6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(-dog.MaxVel, dog.Rb.velocity.x, 0.1f);

            // Clean
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator obeys_positive_max_velocity_constraint()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog, new Vector3(-4.8f, 0.0f, 0.0f));

            // Act
            dog.targetPos = new Vector3(6.0f, 0.0f, 0.0f);
            yield return new WaitForSeconds(1f);

            // Assert
            Assert.AreEqual(dog.MaxVel, dog.Rb.velocity.x, 0.1f);

            // Clean
            dogGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator picks_up_waste()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog);
            GameObject wasteGameObject = CreateWaste(new Vector3(0, 0, 4));
            wasteGameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -4.0f);

            // Act
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(1, dog.WasteCollected);

            // Clean
            dogGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator seeks_waste_object_to_left_if_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog);

            GameObject wasteGameObject = CreateWaste(new Vector3(-2.0f, 0.0f, 4.0f));
            wasteGameObject.GetComponent<Rigidbody>().velocity = Vector3.back;

            // Act
            dog.AddObjectToFocusQueue(wasteGameObject);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(wasteGameObject.transform.position.x, dogGameObject.transform.position.x, dog.DogCollider.size.x);

            // Clean
            dogGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        [UnityTest]
        public IEnumerator seeks_waste_object_to_right_if_close_enough()
        {
            // Arrange
            CreateDog(out GameObject dogGameObject, out Dog dog);

            GameObject wasteGameObject = CreateWaste(new Vector3(-2.0f, 0.0f, 4.0f));
            wasteGameObject.GetComponent<Rigidbody>().velocity = Vector3.back;

            // Act
            dog.AddObjectToFocusQueue(wasteGameObject);
            yield return new WaitForSeconds(1.0f);

            // Assert
            Assert.AreEqual(wasteGameObject.transform.position.x, dogGameObject.transform.position.x, dog.DogCollider.size.x);

            // Clean
            dogGameObject.SetActive(false);
            wasteGameObject.SetActive(false);
        }
        #endregion

        #region Helper Functions
        private static void CreateDog(out GameObject dogGameObject, out Dog dog)
        {
            dogGameObject = new GameObject("Dog");
            dogGameObject.transform.position = Vector3.zero;
            dog = dogGameObject.AddComponent<Dog>();
        }
        private static void CreateDog(out GameObject dogGameObject, out Dog dog, Vector3 spawnPosition)
        {
            dogGameObject = new GameObject("Dog");
            dogGameObject.transform.position = spawnPosition;
            dog = dogGameObject.AddComponent<Dog>();
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
        private void CreatePlayer(out GameObject playerGameObject, out Player player)
        {
            playerGameObject = new GameObject("Player");
            player = playerGameObject.AddComponent<Player>();
            player.PlayerInput = Substitute.For<IPlayerInput>();
        }
        private static GameObject CreateWaste(Vector3 spawnPos)
        {
            GameObject wasteGameObject = new GameObject("Waste");
            wasteGameObject.transform.tag = "Waste";
            wasteGameObject.transform.position = spawnPos;
            wasteGameObject.AddComponent<Obstacle>();
            return wasteGameObject;
        }
        #endregion
    }
}