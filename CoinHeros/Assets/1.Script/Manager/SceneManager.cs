using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : Singleton<SceneManager>
{
    private static string targetSceneName = "Lobby";
    private LoadingSceneUI loadingUI;
    private AsyncOperation asyncLoad;

    bool isBattleLoading =false;
    
    // 씬 전환 메서드들
    public void LoadScene(string sceneName)
    {
        Debug.Log($"씬 '{sceneName}'으로 직접 전환합니다.");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
    public void LoadSceneWithLoading(string sceneName)
    {
        Debug.Log($"로딩씬을 통해 '{sceneName}' 씬으로 이동합니다.");
        targetSceneName = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
    
    // 로딩씬에서 호출되는 메서드
    public void StartLoading()
    {
        Debug.Log($"로딩 매니저가 시작되었습니다. 대상 씬: {targetSceneName}");
        
        // LoadingSceneUI 찾기
        loadingUI = FindObjectOfType<LoadingSceneUI>();
        if (loadingUI == null)
        {
            Debug.LogError("LoadingSceneUI를 찾을 수 없습니다!");
            return;
        }
        
        StartCoroutine(LoadSceneAsync());
    }
    
    IEnumerator LoadSceneAsync()
    {
        // 로딩 시작
        Debug.Log($"씬 '{targetSceneName}' 로딩을 시작합니다.");
        
        // 비동기 씬 로딩 시작
        asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false; // 자동 전환 방지
        
        float startTime = Time.time;
        float minLoadingTime = 2f; // 최소 로딩 시간
        
        while (!asyncLoad.isDone)
        {
            // 로딩 진행률 계산 (0.9까지)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // UI 업데이트
            if (loadingUI != null)
            {
                loadingUI.Set(progress);
            }
            
            // 최소 로딩 시간과 실제 로딩 완료를 모두 확인
            if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minLoadingTime)
            {
                Debug.Log("로딩이 완료되었습니다. 씬을 전환합니다.");
                asyncLoad.allowSceneActivation = true;
                yield return new WaitForSecondsRealtime(1.0f);
                if(isBattleLoading)
                    BattleManager.Instance.BattleStart(1.0f);
                isBattleLoading=false;
                break;
            }
            
            yield return null;
        }
    }
    
    // 편의 메서드들
    public void LoadLobby()
    {
        isBattleLoading=false;
        LoadSceneWithLoading("Lobby");
    }
    
    public void LoadBattleScene()
    {
        isBattleLoading= true;
        LoadSceneWithLoading("testMap_forest");
    }
    
}
