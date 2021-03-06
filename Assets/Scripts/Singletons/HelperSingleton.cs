//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18444
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Blocks;
using LevelCreation;
using Enums;
using OwnDebug;
using System.Collections;

namespace Singleton
{
    public class HelperSingleton
    {
        public string LastLoadedLevel { get; set; }

        private static HelperSingleton _instance;

        /// <summary>
        /// Gets instance
        /// </summary>
        public static HelperSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HelperSingleton();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        /// <param name="newCursor">New cursor.</param>
        public GameObject GetTopMostGO(GameObject gameObject, bool getLastTagged)
        {
            if (gameObject == null)
            {
                return null;
            }

            GameObject intermediateResult = null;
            GameObject parent = gameObject;
            while (parent.transform.parent != null)
            {
                var newParent = parent.transform.parent.gameObject;

                if (getLastTagged && newParent.tag != "Untagged")
                {
                    intermediateResult = newParent;
                }

                if (newParent.transform.position == Vector3.zero)
                {
                    // This is only a container - we reached the end!
                    return parent;
                }

                parent = newParent;
            }

            if (parent.tag == "Untagged" && getLastTagged)
            {
                // If we wanted to return the last tagged, return it.
                return intermediateResult != null
                    ? intermediateResult
                    : parent;
            }

            return parent;
        }

        /// <summary>
        /// Creates a new seed for the next level
        /// </summary>
        /// <returns></returns>
        public int CreateSeed(int? newSeed = null)
        {
            int seed = newSeed.HasValue
                ? newSeed.Value
                : (DateTime.Now.Hour * DateTime.Now.Minute) + (DateTime.Now.Second * 4) - (DateTime.Now.Second * 2);

            Debug.Log("Last seed: " + seed.ToString());

            return seed;
        }

        /// <summary>
        /// Gets the GO which is the nearest to the player.
        /// </summary>
        /// <param name="">.</param>
        /// <param name="myPosition">My position.</param>
        public GameObject GetNearestGameObject(IEnumerable<GameObject> objects, Vector3 myPosition)
        {
            objects = objects.Distinct();
            Dictionary<GameObject, float> distanceToObject = new Dictionary<GameObject, float>();

            if (!objects.Any() || objects.All(ob => ob == null))
            {
                return null;
            }

            foreach (var go in objects)
            {
                distanceToObject.Add(go, (go.transform.position - myPosition).magnitude);
            }

            return distanceToObject.OrderBy(pair => pair.Value).First().Key;
        }

        /// <summary>
        /// Returns the center of the gameobject, using the collidor attached to the go. If no collidier is attached, transform.position is returned.
        /// </summary>
        /// <returns>The center of game object.</returns>
        /// <param name="gameObject">Game object.</param>
        public Vector3 GetCenterOfGameObject(GameObject gameObject)
        {
            var collidor = gameObject.GetComponent<Collider>();
            if (collidor != null)
            {
                return collidor.bounds.center;
            }

            return gameObject.transform.position;
        }

        /// <summary>
        /// Splits up.
        /// </summary>
        /// <returns>The up.</returns>
        /// <param name="splitUp">Split up.</param>
        public string SplitUp(string splitUp)
        {
            string output = String.Empty;
            foreach (char letter in splitUp)
            {
                if (Char.IsUpper(letter) && output.Length > 0)
                    output += " " + letter;
                else
                    output += letter;
            }

            return output;
        }

        /// <summary>
        /// Destroies the level.
        /// </summary>
        public void DestroyLevel()
        {
            PrefabSingleton.Instance.CeillingParent.transform.Cast<Transform>().ToList().ForEach(tr => GameObject.Destroy(tr.gameObject));
            PrefabSingleton.Instance.LevelParent.transform.Cast<Transform>().ToList().ForEach(tr => GameObject.Destroy(tr.gameObject));
            PrefabSingleton.Instance.PickupParent.transform.Cast<Transform>().ToList().ForEach(tr => GameObject.Destroy(tr.gameObject));
            PrefabSingleton.Instance.StandBlockParent.transform.Cast<Transform>().ToList().ForEach(tr => GameObject.Destroy(tr.gameObject));
            PrefabSingleton.Instance.DebugParent.transform.Cast<Transform>().ToList().ForEach(tr => GameObject.Destroy(tr.gameObject));

            // Destroy all Hearts
            var hearts = GameObject.FindGameObjectsWithTag("Heart");
            foreach (var heart in hearts)
            {
                GameObject.Destroy(heart);
            }

            CalculationSingleton.Instance.ActualCreationScope = new CreationScope();
        }

        /// <summary>
        /// Create an empty GO at a specific point with a given text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rotation"></param>
        public void CreateDebugGOAtPosition(string text, Vector3 position)
        {
            GameObject empty = new GameObject();
            empty.transform.position = position;
            empty.transform.name = "DEBUG";
            empty.transform.parent = PrefabSingleton.Instance.DebugParent;

            var info = empty.AddComponent<DebugInfo>();
            info.DebugInfoValue = text;
        }

        /// <summary>
        /// Gets a Wall Descriptor
        /// </summary>
        /// <param name="block"></param>
        /// <param name="specificWall"></param>
        /// <returns></returns>
        public WallDescriptor GetWallDescription(GameObject block, int specificWall = 1)
        {
            if (block == null)
            {
                Debug.LogError("block was NULL - no Walldescriptor!");
                return null;
            }

            var wallDescriptors = block.GetComponentsInChildren<WallDescriptor>();
            return wallDescriptors.First(wd => wd.gameObject.name == String.Concat("Wall", specificWall.ToString()));
        }

        /// <summary>
        /// Gets a Wall Descriptor
        /// </summary>
        /// <param name="block"></param>
        /// <param name="specificWall"></param>
        /// <returns></returns>
        public IEnumerable<WallDescriptor> GetAllWallsOfType(GameObject block, WallDescription type)
        {
            if (block == null)
            {
                Debug.LogWarning("block was NULL - no Walldescriptor!");
                return null;
            }

            var wallDescriptors = block.GetComponentsInChildren<WallDescriptor>();
            return wallDescriptors.Where(wd => wd.Descriptor == type).ToList();
        }

        /// <summary>
        /// Gets a Wall Descriptor
        /// </summary>
        /// <param name="block"></param>
        /// <param name="specificWall"></param>
        /// <returns></returns>
        public IEnumerable<WallDescriptor> GetAllEntryWalls(GameObject block)
        {
            if (block == null)
            {
                Debug.LogWarning("block was NULL - no Walldescriptor!");
                return null;
            }

            var wallDescriptors = block.GetComponentsInChildren<WallDescriptor>();
            return wallDescriptors.Where(wd => wd.Descriptor == WallDescription.Door || wd.Descriptor == WallDescription.Nothing).ToList();
        }

        /// <summary>
        /// Gets a Wall Descriptor
        /// </summary>
        /// <param name="block"></param>
        /// <param name="specificWall"></param>
        /// <returns></returns>
        public IEnumerable<WallDescriptor> GetAllRealWalls(GameObject block)
        {
            if (block == null)
            {
                Debug.LogError("block was NULL - no Walldescriptor!");
                return null;
            }

            var wallDescriptors = block.GetComponentsInChildren<WallDescriptor>();
            return wallDescriptors.Where(wd => wd.Descriptor == WallDescription.Wall).ToList();
        }

        /// <summary>
        /// Returns the Opposite of the actual HorzDirection enum
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public HorzDirection GetOpposite(HorzDirection direction)
        {
            if (direction == HorzDirection.Backwards)
            {
                return HorzDirection.Forward;
            }
            else if (direction == HorzDirection.Forward)
            {
                return HorzDirection.Backwards;
            }
            else if (direction == HorzDirection.Left)
            {
                return HorzDirection.Right;
            }
            else
            {
                return HorzDirection.Left;
            }
        }

        /// <summary>
        /// Returns the Opposite of the actual HorzDirection enum
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public VertDirection GetOpposite(VertDirection direction)
        {
            if (direction == VertDirection.Down)
            {
                return VertDirection.Up;
            }
            else
            {
                return VertDirection.Down;
            }
        }

        /// <summary>
        /// Returns the next positon for the next Horizontal element, which has been created.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Not needed anymore! Use 'CalculationSingleton.Instance.ActualCreationScope.CalculatePositionForNextHorizontal'", true)]
        public Vector3 GetNextHorizonzalPositon(GameObject transitonBlock, GameObject nextItem)
        {
            transitonBlock = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock;

            var lastBlockSize = HelperSingleton.Instance.GetSize(CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock);
            var lastPosition = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position;
            var blockSize = HelperSingleton.Instance.GetSize(nextItem);

            // Create block
            float x = 0;
            float z = 0;
            if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left)
            {
                int dirMulti = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left ? 1 : -1;
                x = lastPosition.x + (lastBlockSize.x * dirMulti);
                z = transitonBlock == null ? 0 : transitonBlock.transform.position.z;
            }
            else if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Backwards)
            {
                int dirMulti = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward ? 1 : -1;
                x = transitonBlock == null ? 0 : transitonBlock.transform.position.x;
                z = lastPosition.z + (lastBlockSize.z * dirMulti);
            }

            return new Vector3(x, transitonBlock == null ? 0 : transitonBlock.transform.position.y, z);
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <returns>The size.</returns>
        /// <param name="objectToCheck">Object to check.</param>
        /// <param name="getForMaster">If TRUE, the MASTERRENDERTAG is used - this one is used for level blocks - DEFAULT!
        /// Otherwise RENDEROBJECT is used - it is used for all other objects</param>
        public Vector3 GetSize(GameObject objectToCheck, bool getForMaster = true)
        {
            string tag = getForMaster
                ? "MasterRenderObject"
                : "RenderObject";

            if (objectToCheck == null)
            {
                Debug.LogError("Object to check is null!");
            }

            // Rotate everything to the same rotation before checking, otherwise sizes will eb diffent
            Quaternion oldRotation = objectToCheck.transform.rotation;
            objectToCheck.transform.rotation = Quaternion.Euler(Vector3.zero);

            // TODO: This does not work in all cases, as a level block may contain severla other pieces which do also contain a RenderObject.
            // When trying to allign to level blocks they may overlap. Seed == 109.

            Renderer renderer = objectToCheck.tag == tag
                ? objectToCheck.GetComponent<Renderer>()
                : objectToCheck.GetComponentsInChildren<Renderer>(true).ToList().FirstOrDefault(rend => rend.gameObject.tag == tag);
            if (renderer == null)
            {
                Debug.LogError(String.Concat("No ", tag, " found for: ", objectToCheck.name));
            }

            var result = renderer != null
                ? renderer.bounds.size
                : Vector3.one;

            // Rotate back;
            objectToCheck.transform.rotation = oldRotation;

            return result;
        }

        /// <summary>
        /// Adapts the Position so that the created item fits the exit of the previous item
        /// </summary>
        public void AdaptPositonForExit()
        {
            GameObject previousItem = CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock;
            var allWallsWithExitPrevious = this.GetAllEntryWalls(CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock);
            var allWallsWithExitActual = this.GetAllEntryWalls(CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock);

            var actualBlockDescriptor = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.GetComponent<BlockDescriptor>();

            if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Left)
            {
                WallDescriptor exitWallPrevExit;
                WallDescriptor exitWallActEntry;
                if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right)
                {
                    exitWallPrevExit = allWallsWithExitPrevious != null ? allWallsWithExitPrevious.OrderBy(wall => wall.transform.localPosition.x).FirstOrDefault() : null;
                    exitWallActEntry = allWallsWithExitActual != null ? allWallsWithExitActual.OrderBy(wall => wall.transform.localPosition.x).LastOrDefault() : null;
                }
                else
                {
                    exitWallPrevExit = allWallsWithExitPrevious != null ? allWallsWithExitPrevious.OrderBy(wall => wall.transform.localPosition.x).LastOrDefault() : null;
                    exitWallActEntry = allWallsWithExitActual != null ? allWallsWithExitActual.OrderBy(wall => wall.transform.localPosition.x).FirstOrDefault() : null;
                }

                if (exitWallPrevExit != null && exitWallActEntry != null)
                {
                    var actPos = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position;
                    if (IsOppositeWall(exitWallPrevExit, exitWallActEntry))
                    {
                        // The exit is on the oder side of the block - adapt Z Position for actual created item
                        // Calculate for the übergang between the old and actual block
                        var difference = Math.Abs(exitWallActEntry.transform.localPosition.z) + Math.Abs(exitWallPrevExit.transform.localPosition.z);
                        float newZ = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right
                            ? CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.z + difference
                            : CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.z - difference;
                        float newY = exitWallPrevExit.transform.position.y - 2.25f;
                        CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position = new Vector3(actPos.x, newY, newZ);
                    }
                    else
                    {
                        // The exit is not on the oder side of the block, but on a side- adapt X Position for actual created item
                        float newZ = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Right
                            ? CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.z + Math.Abs(exitWallActEntry.transform.localPosition.z)
                            : CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.z - Math.Abs(exitWallPrevExit.transform.localPosition.z);
                        float newY = exitWallPrevExit.transform.position.y - 2.25f;
                        CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position = new Vector3(actPos.x, newY, newZ);
                    }
                }
            }
            else if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                || CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Backwards)
            {
                WallDescriptor exitWallPrevExit;
                WallDescriptor exitWallActEntry;
                if (CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward)
                {
                    exitWallPrevExit = allWallsWithExitPrevious != null ? allWallsWithExitPrevious.OrderBy(wall => wall.transform.localPosition.x).FirstOrDefault() : null;
                    exitWallActEntry = allWallsWithExitActual != null ? allWallsWithExitActual.OrderBy(wall => wall.transform.localPosition.x).LastOrDefault() : null;
                }
                else
                {
                    exitWallPrevExit = allWallsWithExitPrevious != null ? allWallsWithExitPrevious.OrderBy(wall => wall.transform.localPosition.x).LastOrDefault() : null;
                    exitWallActEntry = allWallsWithExitActual != null ? allWallsWithExitActual.OrderBy(wall => wall.transform.localPosition.x).FirstOrDefault() : null;
                }

                if (exitWallPrevExit != null && exitWallActEntry != null)
                {
                    // Adapt Z Position for actual created item
                    var actPos = CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position;
                    if (IsOppositeWall(exitWallPrevExit, exitWallActEntry))
                    {
                        // The exit is on the oder side of the block - adapt X Position for actual created item
                        // Calculate for the übergang between the old and actual block
                        var difference = Math.Abs(exitWallActEntry.transform.localPosition.z) + Math.Abs(exitWallPrevExit.transform.localPosition.z);
                        float newX = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                            ? CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.x + difference
                            : CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.x - difference;
                        float newY = exitWallPrevExit.transform.position.y - 2.25f;
                        CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position = new Vector3(newX, newY, actPos.z);
                    }
                    else
                    {
                        // The exit is not on the oder side of the block, but on a side- adapt Z Position for actual created item
                        float newX = CalculationSingleton.Instance.ActualCreationScope.ActualHorizontalDirection == HorzDirection.Forward
                            ? CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.x + Math.Abs(exitWallActEntry.transform.localPosition.z)
                            : CalculationSingleton.Instance.ActualCreationScope.PreviouslyCreatedLevelBlock.transform.position.x - Math.Abs(exitWallActEntry.transform.localPosition.x);
                        float newY = exitWallPrevExit.transform.position.y - 2.25f;
                        CalculationSingleton.Instance.ActualCreationScope.ActualCreatedLevelBlock.transform.position = new Vector3(newX, newY, actPos.z);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the both Wall Descriptors are opposite of each other.
        /// </summary>
        /// <param name="descriptorA"></param>
        /// <param name="descriptorB"></param>
        public bool IsOppositeWall(WallDescriptor descriptorA, WallDescriptor descriptorB)
        {
            return Math.Abs(descriptorA.WallNumber - descriptorB.WallNumber) == 2
                || Math.Abs(descriptorA.WallNumber - descriptorB.WallNumber) == 0;
        }

        /// <summary>
        /// Waits for an amount of frames.
        /// </summary>
        /// <param name="frameCount">Amount of frames to be waited.</param>
        /// <returns></returns>
        public IEnumerator WaitForFrames(int frameCount)
        {
            if (frameCount <= 0)
            {
                throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
            }

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        /// <summary>
        /// Waits for an amount of seconds
        /// </summary>
        /// <param name="frameCount">Amount of seconds to be waited.</param>
        /// <returns></returns>
        public IEnumerator WaitForSeconds(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
    }
}