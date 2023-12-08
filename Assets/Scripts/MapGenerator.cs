using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public bool GenerateOnStart = true;
    [Range(3, 100)]
    public int RoomCount = 9;
    public LayerMask CellLayer;

    public GameObject InsteadDoor;
    public GameObject[] DoorPrefabs;
    public List<Cell> CellRoomPrefabs;
    public List<Cell> CellCorridorPrefabs;

    private void Start()
    {
        if (GenerateOnStart) StartCoroutine(StartGeneration());
    }

    /// <summary>
    /// Map generation function
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGeneration()
    {
        //Create starting room and initialize
        List<Transform> CreatedExits = new List<Transform>();
        Cell StartRoom = Instantiate(CellRoomPrefabs[Random.Range(0, CellRoomPrefabs.Count)], Vector3.zero, Quaternion.identity);
        for (int i = 0; i < StartRoom.Exits.Length; i++) CreatedExits.Add(StartRoom.Exits[i].transform);
        StartRoom.TriggerBox.enabled = true;
        Vector3 playerStartPos = StartRoom.transform.position + StartRoom.GetComponent<BoxCollider>().bounds.center;
        GameManager.instance.InitializePlayer(playerStartPos);

        //Set a limit to avoid infinite loop
        int limit = 1000, roomsLeft = RoomCount - 1;
        //Stop generating if limit reached, all required rooms were generated or there isn't more free doorways
        while (limit > 0 && roomsLeft > 0 && CreatedExits.Count > 0)
        {
            limit--;

            bool collided = true;
            Transform selectedExit;
            Transform createdExit = CreatedExits[Random.Range(0, CreatedExits.Count)]; // Pick a random existing exit
            Cell selectedPrefab;
            List<Cell> shuffledCells;

            //If adding a cell next to a room: choose corridor, else: choose room
            if (createdExit.GetComponentInParent<Cell>().cellType == GameManager.CellType.Room)
            {
                shuffledCells = ShuffleCellList(CellCorridorPrefabs);
            }
            else
            {
                shuffledCells = ShuffleCellList(CellRoomPrefabs);
            }

            //Try each prefab to get one that have enough space
            foreach (Cell currentCell in shuffledCells)
            {
                selectedPrefab = Instantiate(currentCell, Vector3.zero, Quaternion.identity);

                selectedPrefab.TriggerBox.enabled = false; // Disable triggerbox during checks
                //Try each prefab door to see if there is a position that matches
                foreach (GameObject currentExit in selectedPrefab.Exits)
                {
                    selectedExit = currentExit.transform;
                    // rotation
                    float shiftAngle = createdExit.eulerAngles.y + 180 - selectedExit.eulerAngles.y;
                    selectedPrefab.transform.Rotate(new Vector3(0, shiftAngle, 0)); // выходы повернуты друг напротив друга

                    // position
                    Vector3 shiftPosition = createdExit.position - selectedExit.position;
                    selectedPrefab.transform.position += shiftPosition; // выходы состыковались

                    // check
                    Vector3 center = selectedPrefab.transform.position + selectedPrefab.TriggerBox.center.z * selectedPrefab.transform.forward
                        + selectedPrefab.TriggerBox.center.y * selectedPrefab.transform.up
                        + selectedPrefab.TriggerBox.center.x * selectedPrefab.transform.right; // selectedPrefab.TriggerBox.center
                    Vector3 size = selectedPrefab.TriggerBox.size / 2f; // half size
                    Quaternion rot = selectedPrefab.transform.localRotation;
                    collided = Physics.CheckBox(center, size, rot, CellLayer, QueryTriggerInteraction.Collide);

                    yield return new WaitForEndOfFrame();
                    //If the prefab have space: keep it
                    if (!collided)
                    {
                        roomsLeft--;

                        for (int j = 0; j < selectedPrefab.Exits.Length; j++) CreatedExits.Add(selectedPrefab.Exits[j].transform); //Add the new prefab doorways to the global list

                        //Remove used doorways from the list
                        CreatedExits.Remove(createdExit);
                        CreatedExits.Remove(selectedExit);

                        //Add a door to the used doorway
                        Instantiate(DoorPrefabs[Random.Range(0, DoorPrefabs.Length)], createdExit.transform.position, createdExit.transform.rotation);
                        //Destroy the doorways
                        DestroyImmediate(createdExit.gameObject);
                        DestroyImmediate(selectedExit.gameObject);
                        //Stop searching for this prefab
                        break;
                    }
                }
                selectedPrefab.TriggerBox.enabled = true; // Re-enable trigger box

                if (collided)
                    //If there wasn't possible positions destroy and try another prefab
                    DestroyImmediate(selectedPrefab.gameObject);
                else
                    //If there was a possible position skip to the next room
                    break;
            }

            //If any prefab have space: block the doorway and continue
            if (collided)
            {
                Instantiate(InsteadDoor, createdExit.position, createdExit.rotation);
                CreatedExits.Remove(createdExit);
                DestroyImmediate(createdExit.gameObject);
            }

            yield return new WaitForEndOfFrame();
        }

        // Cover the empty doorways
        for (int i = 0; i < CreatedExits.Count; i++)
        {
            Instantiate(InsteadDoor, CreatedExits[i].position, CreatedExits[i].rotation);
            DestroyImmediate(CreatedExits[i].gameObject);
        }

        Debug.Log("Finished " + Time.time);
    }

    /// <summary>
    /// Randomize the list of prefabs
    /// </summary>
    /// <param name="list"></param>
    /// <returns>The randomized list</returns>
    private List<Cell> ShuffleCellList(List<Cell> list)
    {
        List<Cell> tempList = new();
        tempList.AddRange(list);
        List<Cell> shuffled = new();

        for (int i = 0; i < list.Count; i++)
        {
            int index = Random.Range(0, tempList.Count - 1);
            shuffled.Add(tempList[index]);
            tempList.RemoveAt(index);
        }
        return shuffled;
    }
}