using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class MapManager : MonoBehaviour
{
    // 싱글톤
    private static MapManager instance;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<MapManager>();
                }
            }
            return instance;
        }
    }


    public Transform player;
    public Transform camera;
    //시작할 씬
    public SORoom startRoom;
    //현재 열린 씬
    public SORoom currentRoom;
    public List<SORoom> oldRooms = new List<SORoom>();

    private void Start()
    {
        if (startRoom == null)
            startRoom = FindStartRoom();

        Init(startRoom);
    }

    private void Init(SORoom startRoom)
    {
        Enter(startRoom);
    }

    private SORoom FindStartRoom()
    {
        SORoom room = GetComponent<Room>().roomData;

        return room;
    }

    #region Room Events
    //강제 엔터
    public void Enter(SORoom room)
    {
        currentRoom = room;
        /*
        List<SORoom> newRooms = new List<SORoom>();

        newRooms.Clear();
        newRooms.Add(room);
        newRooms.AddRange(room.GetConnectedRooms());

        CloseScenes(newRooms);
        OpenScenes(newRooms);

        oldRooms = new List<SORoom>(newRooms);
        */
        OpenScene(currentRoom);
    }

    public void Enter(PortDirection direction, List<ConnectedPort> ports)
    {
        /*
        currentRoom = room;

        List<SORoom> newRooms = new List<SORoom>();

        newRooms.Clear();
        newRooms.Add(room);
        newRooms.AddRange(room.GetConnectedRooms());

        CloseScenes(newRooms);
        OpenScenes(newRooms);

        oldRooms = new List<SORoom>(newRooms);
        */
        SORoom oldRoom = currentRoom;
        currentRoom = ports[0].room;     //flag
        CloseScene(oldRoom);
        Vector2Int position = ports[0].room.GetRoomPort(direction, ports[0].index).ports[0];
        Vector3 destination = new Vector3(position.x, position.y) + GetMargin(direction);

        Debug.Log($"{oldRoom.name}에서 {currentRoom.name}으로 이동, {direction}, {destination}");

        StartCoroutine(OpenScene(currentRoom, destination));
    }

    //포트 충돌 엔터
    public void Enter(SORoom room, PortDirection direction, int index, float percentage, Vector3 playerPosition)
    {
        /*
        currentRoom = room;

        List<SORoom> newRooms = new List<SORoom>();

        newRooms.Clear();
        newRooms.Add(room);
        newRooms.AddRange(room.GetConnectedRooms());

        CloseScenes(newRooms);
        OpenScenes(newRooms);

        oldRooms = new List<SORoom>(newRooms);
        */

        CloseScene(currentRoom);
        currentRoom = room;
        StartCoroutine(OpenScene(currentRoom, Vector3.zero));

        //플래그
        FindConnectedPosition(room, direction, index, percentage, playerPosition);
    }

    public void Exit(SORoom room)
    {
    }

    public void FindConnectedPosition(SORoom room, PortDirection direction, int index, float percentage, Vector3 playerPosition, int flag = 0)
    {
        if (!oldRooms.Contains(room))
            return; //연결된 방 로드되지 않음.

        //대상 Port
        Debug.Log($"{room.title}의 {direction}의 {index}는 {percentage}");

        RoomPort port = room.GetPort(direction, index);
        List<ConnectedPort> connects = room.GetConnectedPort(direction, index);

        ConnectedPort exitPort = connects[flag];

        Debug.Log($"{exitPort.room.title}의 {GetOppositeDirection(direction)}의 {exitPort.index}의 {port.GetPortPosition(percentage)}연결됨");

        Vector3 transportPosition = port.GetPortPosition(percentage);
        if (port.isHorizontal())
            transportPosition.y = playerPosition.y;
        else
            transportPosition.x = playerPosition.x;

        transportPosition += GetMargin(GetOppositeDirection(direction));
        transportPosition += GetTransportPostion(exitPort, direction);

        player.position = transportPosition;
    }

    public Vector3 GetMargin(PortDirection direction)
    {
        switch (direction)
        {
            case PortDirection.Top:
                return new Vector3(0, -2);
            case PortDirection.Bot:
                return new Vector3(0, 2);
            case PortDirection.Rig:
                return new Vector3(-2, 0);
            case PortDirection.Lef:
                return new Vector3(2, 0);

            default: return Vector3.zero;
        }
    }

    //ConnectedPort를 통해 월드 포지션을 얻어오자.
    public Vector3 GetTransportPostion(ConnectedPort port, PortDirection direction)
    {
        Vector3 position = Vector3.zero;
        SORoom targetRoom = port.room;
        Vector2Int portPosition = targetRoom.GetPort(direction, port.index).ports[0];

        //position += targetRoom.tilemapWorldPosition;

        return position;
    }

    public PortDirection GetOppositeDirection(PortDirection direction)
    {
        switch (direction)
        {
            case PortDirection.Top: return PortDirection.Bot;
            case PortDirection.Bot: return PortDirection.Top;
            case PortDirection.Rig: return PortDirection.Lef;
            case PortDirection.Lef: return PortDirection.Rig;
            default: return PortDirection.Top;
        }
    }
    #endregion

    #region Scene Methods
    public void OpenScenes(List<SORoom> rooms)
    {
        //이미 열려있는 씬이라면, 리턴
        foreach (SORoom room in rooms)
        {
            if (IsOpenScene(room))
                continue;

            OpenScene(room);
        }
    }

    private bool IsOpenScene(SORoom room)
    {
        if(oldRooms.Contains(room))
            return true;

        return false;
    }

    public void OpenScene(SORoom room)
    {
        StartCoroutine(OpenScene(room, Vector3.zero));
    }

    //동기화를 위한 코루틴 사용
    public IEnumerator OpenScene(SORoom room, Vector3 playerPosition)
    {
        SceneField scene = room.scene;
        if (!SceneManager.GetSceneByName(scene.SceneName).isLoaded)
        {
            //비동기 로드
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);

            //로드 완료될 때까지 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            player.position = playerPosition;
        }

        /*
        Scene sce = SceneManager.GetSceneByName(scene.SceneName);
        GameObject[] objects = sce.GetRootGameObjects();
        foreach (GameObject obj in objects)
            obj.GetComponent<Room>()?.Init();
        */
        LoadScene();
    }

    public void CloseScenes(List<SORoom> rooms)
    {
        //차집합이라면, 클로즈
        List<SORoom> differences = oldRooms.Except(rooms).ToList();

        differences.Remove(currentRoom);

        foreach (SORoom room in differences)
            CloseScene(room);
    }

    public void CloseScene(SORoom room)
    {
        SaveScene();

        SceneField scene = room.scene;

        SceneManager.UnloadSceneAsync(scene);
    }

    public void SaveScene()
    { 
    }

    public void LoadScene()
    {
    }
    #endregion
}