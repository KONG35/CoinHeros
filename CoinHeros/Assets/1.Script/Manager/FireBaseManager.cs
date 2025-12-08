using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using NaughtyAttributes;

public class FireBaseManager : Singleton<FireBaseManager>
{
    public string UID;
    public Task<FirebaseUser> User;
    public bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        if (!GetPlayerUID())
        {
            FirebaseTask();
        }
#else
        FirebaseTask();
#endif
        StartCoroutine(WaitLoad());
    }

    public IEnumerator WaitLoad()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        while (!isInitialized)
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("[FireBaseManager] Firebase 초기화 완료, 게임 데이터 로드 시작...");
        StartCoroutine(LoadGameDataCoroutine());
    }

    private IEnumerator LoadGameDataCoroutine()
    {
        var loadTask = LoadAllGameDataAsync();

        while (!loadTask.IsCompleted)
        {
            yield return null;
        }

        if (loadTask.Result)
        {
            Debug.Log("[FireBaseManager] 게임 데이터 로드 완료");

            // UI 업데이트
            var lobby = FindObjectOfType<LobbyUI>();
            if (lobby && lobby.UnitListUI)
            {
                lobby.UnitListUI.SetListItem();
            }
        }
        else
        {
            Debug.LogWarning("[FireBaseManager] 게임 데이터 로드 실패");
        }
    }
    public void OnApplicationPause()
    {
        //Debug.Log("[FireBaseManager] 앱 일시정지 - 자동 저장 시작");
        //AutoSaveGameData();
    }
    public void OnApplicationFocus()
    {
        //Debug.Log("[FireBaseManager] 포커스 해제 - 자동 저장 시작");
        //AutoSaveGameData();
    }
    public void OnApplicationQuit()
    {
        Debug.Log("[FireBaseManager] 앱 종료 - 자동 저장 시작");
        // 앱 종료 시에는 동기적으로 저장 (비동기 저장은 완료를 기다리지 않음)
        StartCoroutine(SyncSaveOnQuit());
    }

    private IEnumerator SyncSaveOnQuit()
    {
        var saveTask = SaveAllGameDataAsync();

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }

        if (saveTask.Result)
        {
            Debug.Log("[FireBaseManager] 앱 종료 시 저장 완료");
        }
        else
        {
            Debug.LogWarning("[FireBaseManager] 앱 종료 시 저장 실패");
        }
    }
    private async void AutoSaveGameData()
    {
        try
        {
            if (isInitialized && !string.IsNullOrEmpty(UID))
            {
                Debug.Log("[FireBaseManager] 자동 저장 시작...");
                var success = await SaveAllGameDataAsync();
                if (success)
                {
                    Debug.Log("[FireBaseManager] 자동 저장 완료");
                }
                else
                {
                    Debug.LogWarning("[FireBaseManager] 자동 저장 실패");
                }
            }
            else
            {
                Debug.Log("[FireBaseManager] Firebase가 초기화되지 않아 자동 저장을 건너뜁니다.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FireBaseManager] 자동 저장 중 오류: {e.Message}");
        }
    }


    public bool GetPlayerUID()
    {
        try
        {
            if (PlayerPrefs.HasKey("Edit_FirebaseUID"))
            {
                UID = PlayerPrefs.GetString("Edit_FirebaseUID");
                Debug.Log($"[FireBaseManager] PlayerPrefs에서 UID 로드: {UID}");

                // 테스트 데이터 확인
                if (PlayerPrefs.HasKey("TestIntData"))
                {
                    int testInt = PlayerPrefs.GetInt("TestIntData");
                    Debug.Log($"[FireBaseManager] 테스트 정수값: {testInt}");
                }

                if (PlayerPrefs.HasKey("test"))
                {
                    string testStr = PlayerPrefs.GetString("test");
                    Debug.Log($"[FireBaseManager] 테스트 문자열: {testStr}");
                }

                isInitialized = true;
                return true;
            }
            else
            {
                Debug.Log("[FireBaseManager] PlayerPrefs에 UID가 없습니다.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FireBaseManager] PlayerPrefs 읽기 오류: {e.Message}");
        }
        return false;
    }

    public void SetPlayerUID(string uid)
    {
        try
        {
            Debug.Log($"[FireBaseManager] PlayerPrefs 저장 시작: {uid}");
            PlayerPrefs.SetString("Edit_FirebaseUID", uid);
            PlayerPrefs.Save();
            Debug.Log($"[FireBaseManager] PlayerPrefs 저장 완료");
            Debug.Log($"[FireBaseManager] 저장된 UID: {PlayerPrefs.GetString("Edit_FirebaseUID")}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FireBaseManager] PlayerPrefs 저장 오류: {e.Message}");
        }
    }

    public void FirebaseTask()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            UID = result.User.UserId;
            isInitialized = true;
        });

    }

    // 초기화 확인
    private async Task EnsureInitAsync()
    {
        if (!isInitialized)
        {
            await FirebaseApp.CheckAndFixDependenciesAsync();
            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null)
            {
                await auth.SignInAnonymouslyAsync();
            }
            UID = auth.CurrentUser.UserId;
            isInitialized = true;
        }
    }

    // 사용자 데이터 저장
    public async Task<bool> SaveUserDataAsync(UserData userData)
    {
        try
        {
            await EnsureInitAsync();
            if (string.IsNullOrEmpty(UID))
            {
                Debug.LogError("[FireBaseManager] UID가 없습니다.");
                return false;
            }

            // 기존 데이터에서 createdAt 가져오기
            string existingCreatedAt = null;
            try
            {
                var existingSnapshot = await FirebaseDatabase.DefaultInstance.RootReference
                    .Child("users").Child(UID)
                    .Child("createdAt")
                    .GetValueAsync();

                if (existingSnapshot.Exists && existingSnapshot.Value != null)
                {
                    existingCreatedAt = existingSnapshot.Value.ToString();
                }
            }
            catch
            {
                // 기존 데이터가 없으면 null로 유지 (새로 생성)
            }

            var userDto = UserDTO.FromUserData(userData, UID, existingCreatedAt);
            var dict = userDto.ToDictionary();

            // SetValueAsync 대신 UpdateChildrenAsync 사용하여 기존 캐릭터 데이터 보존
            await FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(UID)
                .UpdateChildrenAsync(dict);

            Debug.Log($"[FireBaseManager] 사용자 데이터 저장 완료: {UID}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 사용자 데이터 저장 실패: {e.Message}");
            return false;
        }
    }

    // 사용자 데이터 로드
    public async Task<UserDTO> LoadUserDataAsync()
    {
        try
        {
            await EnsureInitAsync();
            if (string.IsNullOrEmpty(UID))
            {
                Debug.LogError("[FireBaseManager] UID가 없습니다.");
                return null;
            }

            var snapshot = await FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(UID)
                .GetValueAsync();

            if (snapshot.Exists)
            {
                var dict = snapshot.Value as Dictionary<string, object>;
                var userDto = UserDTO.FromDictionary(dict);
                Debug.Log($"[FireBaseManager] 사용자 데이터 로드 완료: {UID}");
                return userDto;
            }
            else
            {
                Debug.Log($"[FireBaseManager] 사용자 데이터가 없습니다. 새로 생성합니다: {UID}");
                return await CreateNewUserData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 사용자 데이터 로드 실패: {e.Message}");
            return null;
        }
    }

    // 새 사용자 데이터 생성
    private async Task<UserDTO> CreateNewUserData()
    {
        var userDto = new UserDTO
        {
            uid = UID,
            displayName = "플레이어",
            gold = 10000,
            maxStage = 1,
            isFirstLogin = true
        };

        // 기본 캐릭터 생성 및 저장
        await CreateDefaultCharacter();

        return userDto;
    }

    // 기본 캐릭터 생성
    private async Task CreateDefaultCharacter()
    {
        try
        {
            Debug.Log("[FireBaseManager] 기본 캐릭터 생성 시작...");

            var userData = UserData.Instance;
            if (userData == null)
            {
                Debug.LogError("[FireBaseManager] UserData가 없어 기본 캐릭터를 생성할 수 없습니다.");
                return;
            }

            // 기본 캐릭터 생성 (UserData의 AddCharacter 로직 사용)
            var CharacterList = DataTableManager.Instance.characterPrefabList;
            if (CharacterList == null || CharacterList.Count == 0)
            {
                Debug.LogError("[FireBaseManager] 캐릭터 프리팹 리스트가 비어있습니다.");
                return;
            }

            int index = UnityEngine.Random.Range(0, CharacterList.Count);
            var Unit = UnityEngine.Object.Instantiate(CharacterList[index], userData.transform);
            userData.UnitList.Add(Unit);

            // 캐릭터 초기화 대기
            await WaitUntilCharacterInit(Unit);

            // 캐릭터 데이터 설정
            var DTM = DataTableManager.Instance;
            Unit._name = DTM.CharNameList[UnityEngine.Random.Range(0, DTM.CharNameList.Count)];

            int grade = 0;
            //STR
            float value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            float Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.STR, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_STR, Grade);
            //MAG
            value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.MAG, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_MAG, Grade);
            //CON
            value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.CON, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_CON, Grade);
            //AGI
            value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.AGI, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_AGI, Grade);
            //SPR
            value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.SPR, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_SPR, Grade);
            //LCK
            value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
            Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
            Unit.SetBaseState(GASAttributeData.Instance.LUK, value);
            Unit.SetBaseState(GASAttributeData.Instance.Grade_LUK, Grade);

            Unit.SetCalcBaseStateToDetailState();
            Unit.gameObject.SetActive(false);

            // 기본 캐릭터를 Firebase에 저장 (새로 생성되므로 createdAt은 새로 생성됨)
            var characterDto = CharacterDTO.FromCharacterData(Unit, null);
            var dict = characterDto.ToDictionary();

            await FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(UID)
                .Child("characters").Child(characterDto.instanceId)
                .UpdateChildrenAsync(dict);

            Debug.Log($"[FireBaseManager] 기본 캐릭터 생성 및 저장 완료: {Unit._name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FireBaseManager] 기본 캐릭터 생성 실패: {e.Message}");
        }
    }

    // 캐릭터 초기화 대기
    private async Task WaitUntilCharacterInit(CharacterData character)
    {
        int maxWaitTime = 5000; // 5초 최대 대기
        int waitTime = 0;

        while (!character.isInit && waitTime < maxWaitTime)
        {
            await Task.Delay(100);
            waitTime += 100;
        }

        if (!character.isInit)
        {
            Debug.LogWarning("[FireBaseManager] 캐릭터 초기화 대기 시간 초과");
        }
    }

    // 캐릭터 데이터 저장
    public async Task<bool> SaveCharacterAsync(CharacterData characterData)
    {
        try
        {
            await EnsureInitAsync();
            if (string.IsNullOrEmpty(UID))
            {
                Debug.LogError("[FireBaseManager] UID가 없습니다.");
                return false;
            }

            // 기존 데이터에서 createdAt 가져오기
            string existingCreatedAt = null;
            try
            {
                var existingSnapshot = await FirebaseDatabase.DefaultInstance.RootReference
                    .Child("users").Child(UID)
                    .Child("characters").Child(characterData.UniqueId)
                    .Child("createdAt")
                    .GetValueAsync();

                if (existingSnapshot.Exists && existingSnapshot.Value != null)
                {
                    existingCreatedAt = existingSnapshot.Value.ToString();
                }
            }
            catch
            {
                // 기존 데이터가 없으면 null로 유지 (새로 생성)
            }

            var characterDto = CharacterDTO.FromCharacterData(characterData, existingCreatedAt);
            var dict = characterDto.ToDictionary();

            await FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(UID)
                .Child("characters").Child(characterDto.instanceId)
                .UpdateChildrenAsync(dict);

            Debug.Log($"[FireBaseManager] 캐릭터 저장 완료: {characterDto.name} ({characterDto.instanceId})");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 캐릭터 저장 실패: {e.Message}");
            return false;
        }
    }

    // 모든 캐릭터 데이터 저장
    public async Task<bool> SaveAllCharactersAsync(List<CharacterData> characters)
    {
        try
        {
            await EnsureInitAsync();
            if (string.IsNullOrEmpty(UID))
            {
                Debug.LogError("[FireBaseManager] UID가 없습니다.");
                return false;
            }

            var updates = new Dictionary<string, object>();

            foreach (var character in characters)
            {
                if (character != null)
                {
                    // 기존 캐릭터 데이터에서 createdAt 가져오기
                    string existingCreatedAt = null;
                    try
                    {
                        var existingSnapshot = await FirebaseDatabase.DefaultInstance.RootReference
                            .Child("users").Child(UID)
                            .Child("characters").Child(character.UniqueId)
                            .Child("createdAt")
                            .GetValueAsync();

                        if (existingSnapshot.Exists && existingSnapshot.Value != null)
                        {
                            existingCreatedAt = existingSnapshot.Value.ToString();
                        }
                    }
                    catch
                    {
                        // 기존 데이터가 없으면 null로 유지 (새로 생성)
                    }

                    var characterDto = CharacterDTO.FromCharacterData(character, existingCreatedAt);
                    var dict = characterDto.ToDictionary();
                    updates[$"users/{UID}/characters/{characterDto.instanceId}"] = dict;
                }
            }

            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(updates);

            Debug.Log($"[FireBaseManager] 모든 캐릭터 저장 완료: {characters.Count}개");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 모든 캐릭터 저장 실패: {e.Message}");
            return false;
        }
    }

    // 캐릭터 데이터 로드
    public async Task<List<CharacterDTO>> LoadAllCharactersAsync()
    {
        try
        {
            await EnsureInitAsync();
            if (string.IsNullOrEmpty(UID))
            {
                Debug.LogError("[FireBaseManager] UID가 없습니다.");
                return new List<CharacterDTO>();
            }

            var snapshot = await FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(UID)
                .Child("characters")
                .GetValueAsync();

            var characters = new List<CharacterDTO>();

            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    var dict = child.Value as Dictionary<string, object>;
                    if (dict != null)
                    {
                        var characterDto = CharacterDTO.FromDictionary(dict);
                        characters.Add(characterDto);
                    }
                }
            }

            Debug.Log($"[FireBaseManager] 캐릭터 로드 완료: {characters.Count}개");
            return characters;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 캐릭터 로드 실패: {e.Message}");
            return new List<CharacterDTO>();
        }
    }

    // 전체 게임 데이터 저장 (사용자 + 캐릭터)
    public async Task<bool> SaveAllGameDataAsync()
    {
        try
        {
            await EnsureInitAsync();
            var userData = UserData.Instance;
            if (userData == null)
            {
                Debug.LogError("[FireBaseManager] UserData가 없습니다.");
                return false;
            }

            Debug.Log($"[FireBaseManager] 전체 저장 시작 - 캐릭터 수: {userData.UnitList.Count}");

            // 사용자 데이터와 캐릭터 데이터를 한 번에 저장 (덮어쓰기 방지)
            var allUpdates = new Dictionary<string, object>();

            // 기존 사용자 데이터에서 createdAt 가져오기
            string existingUserCreatedAt = null;
            try
            {
                var existingUserSnapshot = await FirebaseDatabase.DefaultInstance.RootReference
                    .Child("users").Child(UID)
                    .Child("createdAt")
                    .GetValueAsync();

                if (existingUserSnapshot.Exists && existingUserSnapshot.Value != null)
                {
                    existingUserCreatedAt = existingUserSnapshot.Value.ToString();
                }
            }
            catch
            {
                // 기존 데이터가 없으면 null로 유지 (새로 생성)
            }

            // 사용자 데이터 추가
            var userDto = UserDTO.FromUserData(userData, UID, existingUserCreatedAt);
            var userDict = userDto.ToDictionary();
            foreach (var kvp in userDict)
            {
                allUpdates[$"users/{UID}/{kvp.Key}"] = kvp.Value;
            }

            // 캐릭터 데이터 추가 (각 캐릭터의 기존 createdAt 확인)
            foreach (var character in userData.UnitList)
            {
                if (character != null)
                {
                    // 기존 캐릭터 데이터에서 createdAt 가져오기
                    string existingCharCreatedAt = null;
                    try
                    {
                        var existingCharSnapshot = await FirebaseDatabase.DefaultInstance.RootReference
                            .Child("users").Child(UID)
                            .Child("characters").Child(character.UniqueId)
                            .Child("createdAt")
                            .GetValueAsync();

                        if (existingCharSnapshot.Exists && existingCharSnapshot.Value != null)
                        {
                            existingCharCreatedAt = existingCharSnapshot.Value.ToString();
                        }
                    }
                    catch
                    {
                        // 기존 데이터가 없으면 null로 유지 (새로 생성)
                    }

                    var characterDto = CharacterDTO.FromCharacterData(character, existingCharCreatedAt);
                    var characterDict = characterDto.ToDictionary();
                    foreach (var kvp in characterDict)
                    {
                        allUpdates[$"users/{UID}/characters/{characterDto.instanceId}/{kvp.Key}"] = kvp.Value;
                    }
                }
            }

            // 한 번에 모든 데이터 저장
            await FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(allUpdates);
            Debug.Log($"[FireBaseManager] 전체 게임 데이터 저장 완료 - 업데이트된 항목: {allUpdates.Count}개");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 전체 게임 데이터 저장 실패: {e.Message}");
            return false;
        }
    }

    // 전체 게임 데이터 로드
    public async Task<bool> LoadAllGameDataAsync()
    {
        try
        {
            await EnsureInitAsync();
            var userData = UserData.Instance;
            if (userData == null)
            {
                Debug.LogError("[FireBaseManager] UserData가 없습니다.");
                return false;
            }

            Debug.Log($"[FireBaseManager] 로드 시작 - 현재 캐릭터 수: {userData.UnitList.Count}");

            // 사용자 데이터 로드
            var userDto = await LoadUserDataAsync();
            if (userDto == null) return false;

            userDto.ApplyToUserData(userData);

            // 캐릭터 데이터 로드
            var characterDtos = await LoadAllCharactersAsync();

            // 기존 캐릭터들을 백업
            var existingCharacters = new List<CharacterData>(userData.UnitList);
            var existingBattleUnits = userData.BattleUnit;
            
            Debug.Log($"[FireBaseManager] 기존 캐릭터 백업: {existingCharacters.Count}개");


            // Firebase에서 로드된 캐릭터들로 교체
            if (userData && userData.UnitList != null)
            {
                userData.UnitList.Clear();
                userData.BattleUnit = new CharacterData[6];
            }
            foreach (var characterDto in characterDtos)
            {
                var basePrefab = DataTableManager.Instance.characterPrefabList[0]; // 적절한 프리팹 선택 필요
                var characterData = characterDto.ToCharacterData(basePrefab.gameObject, userData.transform);
                if (characterData != null)
                {
                    if (userData && userData.UnitList != null)
                        userData.UnitList.Add(characterData);
                }
            }
            // 고유 ID 중복 검사 및 수정
            userData.ValidateCharacterUniqueIds();

            // Firebase에 캐릭터가 없으면 기존 캐릭터들 복원
            if (userData.UnitList.Count == 0 && existingCharacters.Count > 0)
            {
                Debug.Log("[FireBaseManager] Firebase에 캐릭터가 없어 기존 캐릭터들을 복원합니다.");
                userData.UnitList.AddRange(existingCharacters);
            }


            Debug.Log($"[FireBaseManager] 전체 게임 데이터 로드 완료 - 최종 캐릭터 수: {userData.UnitList.Count}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[FireBaseManager] 전체 게임 데이터 로드 실패: {e.Message}");
            return false;
        }
    }

    // 테스트용 저장 버튼
    [ContextMenu("테스트 - 전체 저장")]
    [Button]
    public async void TestSaveAll()
    {
        Debug.Log("[FireBaseManager] 테스트 저장 시작...");
        var userData = UserData.Instance;
        Debug.Log($"[FireBaseManager] 저장 전 캐릭터 수: {userData.UnitList.Count}");

        var success = await SaveAllGameDataAsync();
        Debug.Log($"[FireBaseManager] 테스트 저장 결과: {(success ? "성공" : "실패")}");
        Debug.Log($"[FireBaseManager] 저장 후 캐릭터 수: {userData.UnitList.Count}");
    }

    // 테스트용 로드 버튼
    [ContextMenu("테스트 - 전체 로드")]
    [Button]
    public async void TestLoadAll()
    {
        Debug.Log("[FireBaseManager] 테스트 로드 시작...");
        var success = await LoadAllGameDataAsync();
        Debug.Log($"[FireBaseManager] 테스트 로드 결과: {(success ? "성공" : "실패")}");
    }

}
