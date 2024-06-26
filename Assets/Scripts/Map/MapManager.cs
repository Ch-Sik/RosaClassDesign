using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using JetBrains.Annotations;

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
    public void Enter(SORoom room)
    {
        currentRoom = room;

        List<SORoom> newRooms = new List<SORoom>();

        newRooms.Clear();
        newRooms.Add(room);
        newRooms.AddRange(room.GetConnectedRooms());

        CloseScenes(newRooms);
        OpenScenes(newRooms);

        oldRooms = new List<SORoom>(newRooms);
    }

    public void Enter(SORoom room, PortDirection direction, int index)
    {
        currentRoom = room;

        List<SORoom> newRooms = new List<SORoom>();

        newRooms.Clear();
        newRooms.Add(room);
        newRooms.AddRange(room.GetConnectedRooms());

        CloseScenes(newRooms);
        OpenScenes(newRooms);

        oldRooms = new List<SORoom>(newRooms);

        //플래그
        FindConnectedPort(room, direction, index);
    }

    public void Exit(SORoom room)
    {
    }

    //충돌한 방, 포트의 방향, 번호 앎.
    //충돌한 방, 포트의 방향에 번호를 통해 ConnectedPorts 수집
    public void FindConnectedPort(SORoom room, PortDirection direction, int index, int flag = 0)
    {
        if (!oldRooms.Contains(room))
            return; //연결된 방 로드되지 않음.

        //대상 Port
        Debug.Log($"{room.title}의 {direction}의 {index}는");

        List<ConnectedPort> connects = room.GetConnectedPort(direction, index);

        ConnectedPort exitPort = connects[flag];

        Debug.Log($"{exitPort.room.title}의 {GetOppositeDirection(direction)}의 {exitPort.index}와 연결됨");
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
        StartCoroutine(OpenScene(room, Vector2Int.zero));
    }

    //동기화를 위한 코루틴 사용
    public IEnumerator OpenScene(SORoom room, Vector2Int anchor)
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