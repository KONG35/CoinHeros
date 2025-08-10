using UnityEngine;

public static class CaptureUtil
{
    /// <summary>
    /// 카메라 화면을 Texture2D로 캡처
    /// </summary>
    public static Texture2D CaptureCameraToTexture2D(Camera cam, int width, int height)
    {
        if (cam == null)
        {
            Debug.LogError("Camera is null");
            return null;
        }

        // 1. RenderTexture 생성
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        // 2. 카메라로 렌더
        cam.Render();

        // 3. RenderTexture → Texture2D 변환
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // 4. 초기화
        cam.targetTexture = null;
        RenderTexture.active = prev;

        Object.DestroyImmediate(rt);

        return tex;
    }
    
}
