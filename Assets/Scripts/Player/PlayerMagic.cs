using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 식물 마법 시전을 담당하는 컴포넌트
/// </summary>
public class PlayerMagic : MonoBehaviour
{
    [Header("정보")]
    [SerializeField]
    private PlantMagicSO currentSelectedMagic;
    [SerializeField, ReadOnly]
    private bool isPreviewOn = false;
    [SerializeField, ReadOnly]
    private Vector3? magicPos;          // null이라면 현재 조준하고 있는 위치가 유효하지 않다는 뜻
    [SerializeField, ReadOnly]
    private TerrainType terrainType;    // 벽에 시전 가능한 녀석일 경우 좌/우 어느쪽인지, 아무데나 시전 가능한 녀석일 경우 상/하/좌/우 어느쪽인지

    [Space(10), Header("시전 관련")]
    [SerializeField, Tooltip("시전 위치 미리보기 오브젝트")]
    private GameObject previewObject;
    [SerializeField, Tooltip("지형 바깥쪽으로 얼마나 떨어져있어도 시전 가능한 것으로 판단할지?")]
    private float castDistOutside = 1.5f;
    [SerializeField, Tooltip("지형 안쪽으로 어느정도까지 파고들어도 시전 가능한 것으로 판단할지?")]
    private float castDistInside = 0.5f;

    private InputManager inputInstance;
    private InputAction IAMagicReady;
    private InputAction IAMagicExecute;
    private InputAction IAMagicCancel;

    private RaycastHit2D hitfail = new RaycastHit2D(); // RaycastTerrain() 함수 최적화


    private void Start()
    {
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        inputInstance = InputManager.Instance;
        IAMagicReady = inputInstance._inputAsset.FindAction("MagicReady");
        IAMagicExecute = inputInstance._inputAsset.FindAction("MagicExecute");
        IAMagicCancel = inputInstance._inputAsset.FindAction("MagicCancel");

        IAMagicReady.performed += OnMagicReady;
        IAMagicExecute.performed += OnMagicExecute;
        IAMagicCancel.performed += OnMagicCancel;

        IAMagicExecute.Disable();
        IAMagicCancel.Disable();
    }

    /// <summary>
    /// 마법 시전 키를 처음 눌렀을 때.
    /// 현재 선택된 마법이 유효한지 검사하고, 미리보기 켜기.
    /// </summary>
    private void OnMagicReady(InputAction.CallbackContext context)
    {
        // TODO: PlayerState와 연동하여 현재 선택된 마법이 유효한지 확인하기
        IAMagicReady.Disable();
        IAMagicExecute.Enable();
        IAMagicCancel.Enable();

        Debug.Log("식물 마법 시전 준비");

        ShowPreview();
    }

    /// <summary>
    /// Prepare Cast 상태에서 클릭했을 경우 미리보기로 보여준 위치에 실제로 마법을 시전하기.
    /// </summary>
    private void OnMagicExecute(InputAction.CallbackContext context)
    {
        if(magicPos == null)
        {
            Debug.Log("식물마법 시전 실패: 잘못된 위치 지정");
            return;
        }
        IAMagicReady.Enable();
        IAMagicExecute.Disable();
        IAMagicCancel .Disable();

        Debug.Log($"식물마법 시전\n 종류: {currentSelectedMagic.name}\n 지형타입: {terrainType}");
        DoMagic();

        HidePreview();
    }

    private void DoMagic()
    {
        if(currentSelectedMagic.prefab == null)
        {
            Debug.LogError($"{currentSelectedMagic.name}: 식물마법 프리팹이 지정되지 않음!");
            return;
        }
        Instantiate(currentSelectedMagic.prefab, (Vector3)magicPos, Quaternion.identity);
    }

    /// <summary>
    /// Prepare Cast 상태에서 우클릭했을 경우 마법 시전을 취소하기
    /// </summary>
    private void OnMagicCancel(InputAction.CallbackContext context)
    {
        IAMagicReady.Enable();
        IAMagicExecute.Disable();
        IAMagicCancel.Disable();

        Debug.LogWarning("식물마법 취소");

        HidePreview();
    }

    /// <summary>
    /// 미리보기 ON
    /// </summary>
    [ContextMenu("Set Preview ON")]
    private void ShowPreview()
    {
        isPreviewOn = true;
    }

    /// <summary>
    /// 미리보기 OFF
    /// </summary>
    [ContextMenu("Set Preview OFF")]
    private void HidePreview()
    {
        isPreviewOn = false;
        previewObject.SetActive(false);
    }

    /// <summary>
    /// 미리보기 Update
    /// </summary>
    private void UpdatePreview()
    {
        // 마우스 위치 가져오기
        Vector2 mouseScreenPos2 = InputManager.Instance._inputAsset.FindAction("Aim").ReadValue<Vector2>();
        float zDistance = transform.position.z - Camera.main.transform.position.z; // 플레이어와 카메라의 z좌표 차이
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos2.x, mouseScreenPos2.y, zDistance));

        // 식물 마법 종류에 따라 캐스팅 방향 판단하기
        MagicCastType castType = currentSelectedMagic.castType;

        // 마우스 위치로부터 가장 가까운 캐스팅 위치 가져오기
        bool previewable = false;
        float minDistance = float.PositiveInfinity;     // '가장 가까운' 것을 판별
        RaycastHit2D hitResult = new RaycastHit2D();
        RaycastHit2D hitTemp;
        if (castType == MagicCastType.GROUND_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            if(RaycastTerrain(mouseWorldPosition, TerrainType.Floor, out hitTemp) && hitTemp.distance < minDistance)
            {
                previewable = true;
                minDistance = hitTemp.distance;
                hitResult = hitTemp;
                terrainType = TerrainType.Floor;
            }
        }
        if(castType == MagicCastType.WALL_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            if (RaycastTerrain(mouseWorldPosition, TerrainType.Wall_facing_right, out hitTemp) && hitTemp.distance < minDistance)
            {
                previewable = true;
                minDistance = hitTemp.distance;
                hitResult = hitTemp;
                terrainType = TerrainType.Wall_facing_right;
            }
            if (RaycastTerrain(mouseWorldPosition, TerrainType.Wall_facing_left, out hitTemp) && hitTemp.distance < minDistance)
            {
                previewable = true;
                minDistance = hitTemp.distance;
                hitResult = hitTemp;
                terrainType = TerrainType.Wall_facing_left;
            }
        }
        if (castType == MagicCastType.CEIL_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            if (RaycastTerrain(mouseWorldPosition, TerrainType.Ceil, out hitTemp) && hitTemp.distance < minDistance)
            {
                previewable = true;
                minDistance = hitTemp.distance;
                hitResult = hitTemp;
                terrainType = TerrainType.Ceil;
            }
        }

        // 해당 위치에 프리뷰 표시하기
        if(previewable)
        {
            previewObject.SetActive(true);
            previewObject.transform.position = hitResult.point;
            magicPos = hitResult.point;
        }
        else
        {
            previewObject.SetActive(false);
            magicPos = null;
        }
    }

    /// <summary>
    /// Raycast를 수행하고 layer 검사까지 수행한다. 성공하면 true, 실패하면 false 반환.
    /// </summary>
    private bool RaycastTerrain(Vector2 origin, TerrainType tType, out RaycastHit2D result)
    {
        bool succeed = false;
        Vector2 rayDir;
        string layerName;
        RaycastHit2D frontResult, backResult;

        result = hitfail;

        switch(tType)
        {
            case TerrainType.Ceil:
                rayDir = Vector2.up;    // raydir는 기본적으로 마우스가 지형 바깥쪽에 있다고 생각하고 설정.
                layerName = "Ceil";
                break;
            case TerrainType.Floor:
                rayDir = Vector2.down;
                layerName = "Floor";
                break;
            case TerrainType.Wall_facing_right:
                rayDir = Vector2.left;
                layerName = "Wall_right";
                break;
            default: // case TerrainType.Wall_facing_left:
                rayDir = Vector2.right;
                layerName = "Wall_left";
                break;
        }

        // 마우스가 지형 바깥에 있을 경우
        frontResult = Physics2D.Raycast(origin, rayDir, castDistOutside, LayerMask.GetMask(layerName));
        if (frontResult.collider != null)
        {
            succeed = true;
            result = frontResult;
        }
        // 마우스가 지형 안쪽에 있을 경우
        backResult = Physics2D.Raycast(origin, -rayDir, castDistInside, LayerMask.GetMask(layerName));
        if (backResult.collider != null)
        {
            if (result.collider == null || result.distance > backResult.distance)
            {
                succeed = true;
                result = backResult;
            }
        }

        return succeed;
    }

    private void Update()
    {
        if (isPreviewOn)
            UpdatePreview();
    }

}
