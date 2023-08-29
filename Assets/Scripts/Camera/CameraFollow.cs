using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 따라다니는 카메라
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // 싱글톤
    private static CameraFollow _instance;
    public static CameraFollow Instance { get { return _instance; } }
    private Camera _camera;
    private PlayerRef _player;

    public void Init()
    {
        _camera = GetComponent<Camera>();
        _player = PlayerRef.Instance;
    }

    private void Update()
    {
        
    }
}
