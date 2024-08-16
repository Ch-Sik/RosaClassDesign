using UnityEngine;
using Sirenix.OdinInspector;

public class TerrainShadowGenerator : MonoBehaviour
{
    public Texture2D sourceTexture; // 입력 타일맵 텍스처
    public ComputeShader minFilterShader;
    public int minFilterIterations = 10; // 필터 반복 적용 횟수
    public ComputeShader blurShader;
    public int blurRadius = 10;
    public string outFileName;  // 출력 파일명

    RenderTexture tempTexture;      // 중간 결과

    private RenderTexture resultTexture;

    [Button("그림자 이미지 생성")]
    void GenerateShadowImage()
    {
        Debug.Assert(sourceTexture != null);
        Debug.Assert(minFilterShader != null);

        if (sourceTexture != null && minFilterShader != null)
        {
            // 결과를 담을 텍스처 설정
            resultTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0, RenderTextureFormat.ARGB32);
            resultTexture.enableRandomWrite = true;
            resultTexture.Create();

            // 임시 텍스쳐에 소스 텍스쳐 복사
            tempTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0);
            Graphics.Blit(sourceTexture, tempTexture);

            // 최소값 필터를 반복적으로 적용
            for (int i = 0; i < minFilterIterations; i++)
            {
                ApplyMinimumFilter();
            }

            // 다음 블러 필터를 적용
            ApplyBlurFilter();

            // 결과 텍스처를 저장
            SaveRenderTextureAsPNG(resultTexture, "./Assets/Scripts/Terrain/TerrainShadow/" + outFileName + ".png");
        }
    }

    void ApplyMinimumFilter()
    {
        int kernelHandle = minFilterShader.FindKernel("MinimumFilter");

        // 텍스처 크기와 관련된 변수를 컴퓨트 셰이더에 전달
        minFilterShader.SetInts("textureSize", new int[] { tempTexture.width, tempTexture.height });

        // 컴퓨트 셰이더에 입력 텍스처와 출력 텍스처 설정
        minFilterShader.SetTexture(kernelHandle, "SourceTexture", tempTexture);
        minFilterShader.SetTexture(kernelHandle, "ResultTexture", resultTexture);

        // 컴퓨트 셰이더 실행 (8x8 스레드 그룹)
        minFilterShader.Dispatch(kernelHandle, tempTexture.width / 8, tempTexture.height / 8, 1);

        // 중간 결과를 다음 반복에서 입력으로 사용하기 위해 텍스처 교체
        Graphics.Blit(resultTexture, tempTexture);
    }

    void ApplyBlurFilter()
    {
        int kernelHandle = blurShader.FindKernel("ApplyBlur");

        // 텍스처 크기와 관련된 변수를 컴퓨트 셰이더에 전달
        blurShader.SetInts("textureSize", new int[] { tempTexture.width, tempTexture.height });
        blurShader.SetInt("blurRadius", blurRadius);

        // 컴퓨트 셰이더에 입력 텍스처와 출력 텍스처 설정
        blurShader.SetTexture(kernelHandle, "SourceTexture", tempTexture);
        blurShader.SetTexture(kernelHandle, "ResultTexture", resultTexture);

        // 컴퓨트 셰이더 실행 (8x8 스레드 그룹)
        blurShader.Dispatch(kernelHandle, tempTexture.width / 8, tempTexture.height / 8, 1);

        // 중간 결과를 다음 반복에서 입력으로 사용하기 위해 텍스처 교체
        Graphics.Blit(resultTexture, tempTexture);
    }

    void SaveRenderTextureAsPNG(RenderTexture rt, string path)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved texture to {path}");
    }
}
