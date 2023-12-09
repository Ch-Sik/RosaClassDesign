using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SubsystemsImplementation;

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
    private TerrainType terrainType;    // 설치하려는 곳이 천장/벽/바닥 어느쪽인지
    [SerializeField, ReadOnly]
    private LR wallLR;                  // 벽일 경우, 왼쪽을 바라보는 벽인지, 오른쪽을 바라보는 벽인지

    [Space(10), Header("시전 관련")]
    [SerializeField, Tooltip("시전 위치 미리보기 오브젝트")]
    private GameObject previewObject;
    [SerializeField, Tooltip("지형 경계면과 마우스가 얼마나 떨어져있어도 시전 가능한 것으로 판단할지?")]
    private float castDist = 1.5f;

    [Space(10), Header("스폰된 오브젝트")]
    [SerializeField, ReadOnly]
    private GameObject[] spawnedObject = new GameObject[8];

    [Space(10), Header("디버깅용")]
    [SerializeField, ReadOnly]
    private bool debug_isMouseInTerrain;
    [SerializeField, ReadOnly]
    private Vector3 debug_mousePosition;
    [SerializeField, ReadOnly]
    private Vector3 debug_raycastPosition;
    [SerializeField, ReadOnly]
    private Vector3 debug_targetTilePosition;

    private const string layerStringGround = "Ground";
    private const string layerStringCurtain = "HiddenRoom";

    private InputManager inputInstance;
    private InputAction IAMagicReady;
    private InputAction IAMagicExecute;
    private InputAction IAMagicCancel;
    private int layerGround;
    private int layerCurtain;



    private void Start()
    {
        layerGround = LayerMask.GetMask(layerStringGround);
        layerCurtain = LayerMask.GetMask(layerStringCurtain);
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

    private void Update()
    {
        if (isPreviewOn)
            UpdatePreview();
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
        if (magicPos == null)
        {
            Debug.Log("식물마법 시전 실패: 잘못된 위치 지정");
            CancelMagic();
            return;
        }
        IAMagicReady.Enable();
        IAMagicExecute.Disable();
        IAMagicCancel.Disable();

        Debug.Log($"식물마법 시전\n 종류: {currentSelectedMagic.name}\n 지형타입: {terrainType}");
        DoMagic();

        HidePreview();
    }

    /// <summary>
    /// Prepare Cast 상태에서 우클릭했을 경우 마법 시전을 취소하기
    /// </summary>
    private void OnMagicCancel(InputAction.CallbackContext context)
    {
        CancelMagic();
    }




    private void DoMagic()
    {
        GameObject magicInstance;
        if (currentSelectedMagic.prefab == null)
        {
            Debug.LogError($"{currentSelectedMagic.name}: 식물마법 프리팹이 지정되지 않음!");
            return;
        }

        if (currentSelectedMagic.castType == MagicCastType.EVERYWHERE)
        {
            // 천장-벽-바닥 아무데나 설치가능한 경우
            // '지형과 수직인 방향'을 가르키기 위해 '회전'을 사용
            float rotation;
            switch(terrainType)
            {
                case TerrainType.Ceil: rotation = 180f; break;
                case TerrainType.Floor: rotation = 0f; break;
                default:        // case TerrainType.Wall: 
                    rotation = wallLR.isRIGHT()?270f:90f; break;
            }
            magicInstance = Instantiate(currentSelectedMagic.prefab, (Vector3)magicPos, Quaternion.Euler(0, 0, rotation));
        }
        else
        {
            // 그 외의 경우 '지형과 수직인 방향'을 가르키기 위해 '좌우 대칭'을 사용
            magicInstance = Instantiate(currentSelectedMagic.prefab, (Vector3)magicPos, Quaternion.identity);
            // wall_facing_left는 기본 형태, wall_facing_right가 반전된 형태
            if (wallLR.isRIGHT())
            {
                magicInstance.transform.localScale = Vector3.Scale(magicInstance.transform.localScale, new Vector3(-1, 1, 1));
            }
        }

        // 식물 마법의 Init까지 수행
        if (currentSelectedMagic.code == PlantMagicCode.IVY)
        {
            magicInstance.GetComponent<MagicIvy>().Init((Vector2)magicPos);
        }

        // 오브젝트 풀 관리: 동시에 유지 가능한 오브젝트는 최대 1개
        if (spawnedObject[(int)currentSelectedMagic.code] != null)
        {
            // TODO: 기존에 설치된 식물이 자연스럽게 사라지는 것 연출
            Destroy(spawnedObject[(int)currentSelectedMagic.code], 1f);
        }
        spawnedObject[(int)currentSelectedMagic.code] = magicInstance;
    }

    private void CancelMagic()
    {
        IAMagicReady.Enable();
        IAMagicExecute.Disable();
        IAMagicCancel.Disable();

        Debug.LogWarning("식물마법 취소");

        HidePreview();
    }

    /// <summary> 미리보기 ON </summary>
    [ContextMenu("Set Preview ON")]
    private void ShowPreview()
    {
        isPreviewOn = true;
    }

    /// <summary> 미리보기 OFF </summary>
    [ContextMenu("Set Preview OFF")]
    private void HidePreview()
    {
        isPreviewOn = false;
        previewObject.SetActive(false);
    }

    /// <summary> 미리보기 Update </summary>
    private void UpdatePreview()
    {
        // 마우스 위치 가져오기
        Vector2 mouseScreenPos2 = InputManager.Instance._inputAsset.FindAction("Aim").ReadValue<Vector2>();
        float zDistance = transform.position.z - Camera.main.transform.position.z; // 플레이어와 카메라의 z좌표 차이
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos2.x, mouseScreenPos2.y, zDistance));
        debug_mousePosition = mouseWorldPosition;

        // 식물 마법 종류에 따라 캐스팅 방향 판단하기
        MagicCastType castType = currentSelectedMagic.castType;

        // 마우스 위치로부터 가장 가까운 캐스팅 위치 가져오기
        bool previewable = false;
        float minDistance = float.PositiveInfinity;     // '가장 가까운' 것 판별용
        Vector2 previewPoint = Vector2.zero;
        Vector2? castPoint = null;                      // 임시 변수
        if (castType == MagicCastType.GROUND_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            castPoint = RaycastGround(mouseWorldPosition);
            if (castPoint != null && Vector2.Distance((Vector2)castPoint, mouseWorldPosition) < minDistance)
            {
                previewable = true;
                previewPoint = (Vector2)castPoint;
                minDistance = Vector2.Distance((Vector2)castPoint, mouseWorldPosition);
                terrainType = TerrainType.Floor;
            }
        }
        if (castType == MagicCastType.WALL_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            castPoint = RaycastWall(mouseWorldPosition);
            if (castPoint != null && Vector2.Distance((Vector2)castPoint, mouseWorldPosition) < minDistance)
            {
                previewable = true;
                previewPoint = (Vector2)castPoint;
                minDistance = Vector2.Distance((Vector2)castPoint, mouseWorldPosition);
                terrainType = TerrainType.Wall;
            }
        }
        if (castType == MagicCastType.CEIL_ONLY || castType == MagicCastType.EVERYWHERE)
        {
            castPoint = RaycastCeil(mouseWorldPosition);
            if (castPoint != null && Vector2.Distance((Vector2)castPoint, mouseWorldPosition) < minDistance)
            {
                previewable = true;
                previewPoint = (Vector2)castPoint;
                minDistance = Vector2.Distance((Vector2)castPoint, mouseWorldPosition);
                terrainType = TerrainType.Ceil;
            }
        }

        // 해당 위치에 프리뷰 표시하기
        if (previewable)
        {
            previewObject.SetActive(true);
            previewObject.transform.position = previewPoint;
            magicPos = previewPoint;
        }
        else
        {
            previewObject.SetActive(false);
            magicPos = null;
        }
    }




    private Vector2? RaycastGround(Vector2 origin)
    {
        Vector2 rayHitWorldPosition, cellWorldPosition;
        Collider2D overlapTest = Physics2D.OverlapPoint(origin, layerGround);

        if (overlapTest != null)    // 마우스가 지형 안쪽인 경우
        {
            debug_isMouseInTerrain = true;
            // 지형 바깥쪽으로 castDist만큼 뻗어도 지형 안쪽일 경우 '너무' 안쪽인 것으로 판정
            overlapTest = Physics2D.OverlapPoint(origin + new Vector2(0, castDist), layerGround | layerCurtain);
            if (overlapTest != null)
            {
                return null;
            }
            // 그게 아니라면 레이캐스팅해서 위치 선정
            else
            {
                RaycastHit2D result = Physics2D.Raycast(origin + new Vector2(0, castDist), Vector2.down, castDist, layerGround);
                if (result.collider != null)
                {
                    rayHitWorldPosition = result.point;
                    cellWorldPosition = result.point + 0.1f * Vector2.down;
                }
                else    
                {
                    // 정상적인 상태에서는 논리적으로 이쪽 루틴에 진입하는 것은 불가능
                    Debug.LogError("식물 마법 위치 선정에 논리적 결함 발생");
                    return null;
                }
            }
        }
        else                // 마우스가 지형 바깥쪽인 경우
        {
            overlapTest = Physics2D.OverlapPoint(origin, layerCurtain);
            if (overlapTest != null)
            {
                Debug.Log("뒤에 숨겨진 공간이 있어 식물 마법을 시전할 수 없음");
                return null;
            }

            debug_isMouseInTerrain = false;
            RaycastHit2D result = Physics2D.Raycast(origin, Vector2.down, castDist, layerGround);
            if(result.collider != null)
            {
                rayHitWorldPosition = result.point;
                cellWorldPosition = result.point + 0.1f * Vector2.down;
            }
            else return null;
        }

        debug_raycastPosition = rayHitWorldPosition;
        debug_targetTilePosition = cellWorldPosition;
        TileData tileData = TilemapManager.Instance.GetTileDataByWorldPosition(cellWorldPosition);
        if (!tileData.magicAllowed)
        {
            return null;
        }

        return rayHitWorldPosition;
    }

    private Vector2? RaycastCeil(Vector2 origin)
    {
        Vector2 rayHitWorldPosition, cellWorldPosition;
        Collider2D overlapTest = Physics2D.OverlapPoint(origin, layerGround);

        if (overlapTest != null)    // 마우스가 지형 안쪽인 경우
        {
            // 지형 바깥쪽으로 castDist만큼 뻗어도 지형 안쪽일 경우 '너무' 안쪽인 것으로 판정
            overlapTest = Physics2D.OverlapPoint(origin + new Vector2(0, -castDist), layerGround | layerCurtain);
            if (overlapTest != null)
            {
                return null;
            }
            // 그게 아니라면 레이캐스팅해서 위치 선정
            else
            {
                RaycastHit2D result = Physics2D.Raycast(origin + new Vector2(0, -castDist), Vector2.up, castDist, layerGround);
                if (result.collider != null)
                {
                    rayHitWorldPosition = result.point;
                    cellWorldPosition = result.point + 0.1f * Vector2.up;
                }
                else
                {
                    // 정상적인 상태에서는 논리적으로 이쪽 루틴에 진입하는 것은 불가능
                    Debug.LogError("식물 마법 위치 선정에 논리적 결함 발생");
                    return null;
                }
            }
        }
        else                // 마우스가 지형 바깥쪽인 경우
        {
            overlapTest = Physics2D.OverlapPoint(origin, layerCurtain);
            if (overlapTest != null)
            {
                Debug.Log("뒤에 숨겨진 공간이 있어 식물 마법을 시전할 수 없음");
                return null;
            }

            RaycastHit2D result = Physics2D.Raycast(origin, Vector2.up, castDist, layerGround);
            if (result.collider != null)
            {
                rayHitWorldPosition = result.point;
                cellWorldPosition = result.point + 0.1f * Vector2.up;
            }
            else
            {
                return null;
            }
        }

        debug_raycastPosition = rayHitWorldPosition;
        debug_targetTilePosition = cellWorldPosition;
        TileData tileData = TilemapManager.Instance.GetTileDataByWorldPosition(cellWorldPosition);
        if (!tileData.magicAllowed)
        {
            return null;
        }

        return rayHitWorldPosition;
    }

    private Vector2? RaycastWall(Vector2 origin)
    {
        Vector2 rayHitWorldPosition = Vector2.zero, cellWorldPosition = Vector2.zero;
        Collider2D overlapTest = Physics2D.OverlapPoint(origin, layerGround);

        if (overlapTest != null)    // 마우스가 지형 안쪽인 경우
        {
            Collider2D leftOverlapTest, rightOverlapTest;
            leftOverlapTest = Physics2D.OverlapPoint(origin + new Vector2(-castDist, 0), layerGround | layerCurtain);
            rightOverlapTest = Physics2D.OverlapPoint(origin + new Vector2(castDist, 0), layerGround | layerCurtain);
            // 지형 바깥쪽으로 castDist만큼 뻗어도 지형 안쪽일 경우 '너무' 안쪽인 것으로 판정
            if (leftOverlapTest != null && rightOverlapTest != null)
            {
                return null;
            }
            else if(leftOverlapTest == null && rightOverlapTest == null)
            {
                // TODO: 이 부분에 더 좋은 방법 없을지 고민해보기
                Debug.LogWarning("벽이 너무 얇음! 식물 마법 위치 선정에 오류 발생");
                return null;
            }
            // 그게 아니라면 레이캐스팅해서 위치 선정
            // 왼쪽을 바라보는 벽인 경우
            else if (leftOverlapTest == null)
            {
                RaycastHit2D result = Physics2D.Raycast(origin + new Vector2(-castDist, 0), Vector2.right, castDist, layerGround);
                if (result.collider != null)
                {
                    rayHitWorldPosition = result.point;
                    cellWorldPosition = result.point + 0.1f * Vector2.right;
                }
                else
                {
                    // 정상적인 상태에서는 논리적으로 이쪽 루틴에 진입하는 것은 불가능
                    Debug.LogError("식물 마법 위치 선정에 논리적 결함 발생");
                    return null;
                }
            }
            // 오른쪽을 바라보는 벽인 경우
            else // if(rightOverlapResult == null)
            {
                RaycastHit2D result = Physics2D.Raycast(origin + new Vector2(castDist, 0), Vector2.left, castDist, layerGround);
                if (result.collider != null)
                {
                    rayHitWorldPosition = result.point;
                    cellWorldPosition = result.point + 0.1f * Vector2.left;
                }
                else
                {
                    // 정상적인 상태에서는 논리적으로 이쪽 루틴에 진입하는 것은 불가능
                    Debug.LogError("식물 마법 위치 선정에 논리적 결함 발생");
                    return null;
                }
            }
        }
        else                // 마우스가 지형 바깥쪽인 경우
        {
            overlapTest = Physics2D.OverlapPoint(origin, layerCurtain);
            if(overlapTest != null)
            {
                Debug.Log("뒤에 숨겨진 공간이 있어 식물 마법을 시전할 수 없음");
                return null;
            }

            float distance = float.PositiveInfinity;
            RaycastHit2D leftResult = Physics2D.Raycast(origin, Vector2.left, castDist, layerGround);
            if (leftResult.collider != null)
            {
                wallLR = LR.LEFT;
                distance = leftResult.distance;
                rayHitWorldPosition = leftResult.point;
                cellWorldPosition = leftResult.point + 0.1f * Vector2.left;
            }
            RaycastHit2D rightResult = Physics2D.Raycast(origin, Vector2.right, castDist, layerGround);
            if (rightResult.collider != null && rightResult.distance < distance)
            {
                wallLR = LR.RIGHT;
                distance = rightResult.distance;
                rayHitWorldPosition = rightResult.point;
                cellWorldPosition = rightResult.point + 0.1f * Vector2.right;
            }
            if(distance > castDist)
            {
                return null;
            }
        }

        debug_raycastPosition = rayHitWorldPosition;
        debug_targetTilePosition = cellWorldPosition;
        TileData tileData = TilemapManager.Instance.GetTileDataByWorldPosition(cellWorldPosition);
        if (!tileData.magicAllowed)
        {
            return null;
        }

        return rayHitWorldPosition;
    }



}
