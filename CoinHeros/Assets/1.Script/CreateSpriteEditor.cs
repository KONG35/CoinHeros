using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;
using UnityEngine.UI;
using System.IO;
using Unity.Burst;

#if UNITY_EDITOR
public class CreateSpriteEditor : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera renderCamera;
    public RenderTexture renderTexture;
    
    [Header("스프라이트 설정")]
    public string spriteName = "GeneratedSprite";
    public string savePath = "Assets/GeneratedSprites/";
    
    [Button("Test Capture")]
    void Test()
    {
        Camera cam = renderCamera;
        Texture2D tex = CaptureUtil.CaptureCameraToTexture2D(cam, renderTexture.width, renderTexture.height);

        // 원하는 경로에 저장
        System.IO.File.WriteAllBytes(savePath, tex.EncodeToPNG());
        Debug.Log("저장 완료: " + savePath);

        // 저장 후 Asset DB 갱신
        AssetDatabase.Refresh();
    }
}
#endif
