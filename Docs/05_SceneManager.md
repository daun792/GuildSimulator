# SceneManager 사용법

이 문서는 다른 씬으로 이동하거나, 현재 씬 정보를 확인할 때 사용합니다.

```text
타이틀에서 게임으로 이동
→ SceneManager 사용

게임에서 결과 화면으로 이동
→ SceneManager 사용

씬 이동 전후에 작업이 필요함
→ 씬 전환 함수 확인
```

## SceneManager는 무엇인가요?

`SceneManager`는 게임의 화면 단위를 이동하는 기능을 담당합니다.

프로젝트에서는 enum으로 씬 이름을 관리합니다.

```csharp
public enum SceneName
{
    Developer,
    Title,
    Game,
    Empty
}
```

## 씬 이동하기

현재 프로젝트에서는 `SceneManager`을 통해 씬을 이동할 수 있습니다.

```csharp
App.Get<SceneManager>().Load(SceneName.Game);
```

타이틀 씬으로 이동하려면 다음처럼 사용합니다.

```csharp
App.Get<SceneManager>().Load(SceneName.Title);
```

## 버튼에서 씬 이동하기

버튼 클릭 함수에서도 같은 방식으로 사용할 수 있습니다.

```csharp
public void OnClickStartGame()
{
    App.Get<SceneManager>().Load(SceneName.Game);
}
```

```csharp
public void OnClickBackToTitle()
{
    App.Get<SceneManager>().Load(SceneName.Title);
}
```

## 씬 이동 과정

현재 구조에서는 씬을 바로 바꾸는 것이 아니라, 화면을 어둡게 만든 다음 이동할 수 있습니다.

```text
씬 이동 요청
→ BGM 줄이기
→ 화면 어둡게 만들기
→ 새로운 씬 불러오기
→ 새 BGM 재생
```

이 과정을 SceneManager가 자동으로 처리합니다.

## 새로운 씬 추가하기

### 1. Unity에서 씬 만들기

Unity 메뉴에서 새 씬을 만들고 저장합니다.

예시:

```text
Assets/01. Scenes/04. Ending.unity
```

### 2. Build Settings에 씬 추가하기

씬 파일만 만들어서는 빌드된 게임에서 이동할 수 없습니다.

Unity의 Build Settings 또는 Build Profiles에 씬을 추가합니다.

```text
File
→ Build Settings 또는 Build Profiles
→ Scenes In Build
→ 씬 추가
```

### 3. SceneName에 추가하기

```csharp
public enum SceneName
{
    Developer,
    Title,
    Game,
    Ending,
    Empty
}
```

### 4. 씬 이름 연결하기

프로젝트가 enum 이름을 그대로 씬 이름으로 사용한다면 씬 파일 이름도 맞춰야 합니다.

```text
enum 이름: Ending
씬 이름: Ending
```

프로젝트에서 별도의 이름 연결 코드를 사용한다면 그 코드에도 추가해야 합니다.

예시:

```csharp
private static string GetSceneName(SceneName sceneName)
{
    return sceneName switch
    {
        SceneName.Title => "Title",
        SceneName.Game => "Game",
        SceneName.Ending => "Ending",
        _ => throw new ArgumentOutOfRangeException()
    };
}
```

## 씬마다 다른 BGM을 사용한다면

씬에 따라 BGM을 바꾸는 코드가 있다면 새로운 씬의 BGM도 추가해야 합니다.

예시:

```csharp
private static string GetBgmName(SceneName sceneName)
{
    return sceneName switch
    {
        SceneName.Title => "Title",
        SceneName.Game => "InGame",
        SceneName.Ending => "Ending",
        _ => null
    };
}
```

BGM이 필요 없는 씬이라면 추가하지 않아도 됩니다.

## SceneManager와 Unity SceneManager 구분하기

프로젝트 안에 직접 만든 `SceneManager`가 있고, Unity에도 같은 이름의 `SceneManager`가 있을 수 있습니다.

Unity의 SceneManager는 다음 이름 공간에 있습니다.

```csharp
UnityEngine.SceneManagement.SceneManager
```

코드에서 어떤 SceneManager인지 헷갈리지 않도록 주의합니다.

## 씬 이동이 되지 않을 때

다음 순서로 확인합니다.

1. 씬이 저장되어 있는지
2. Build Settings 또는 Build Profiles에 추가되어 있는지
3. enum에 씬이 추가되어 있는지
4. 씬 이름의 철자가 정확한지
5. 씬 연결 코드에 해당 씬이 들어 있는지
6. 씬 이동 중 오류 로그가 발생하지 않았는지

## 작성 기준

씬 이동은 가능하면 한 곳에서 처리하는 것이 좋습니다.

다음처럼 여러 코드에서 Unity SceneManager를 직접 호출하면 관리하기 어려워질 수 있습니다.

```csharp
UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
```

대신 프로젝트에서 정한 함수를 사용합니다.

이렇게 하면 나중에 화면 전환, 로딩 화면, BGM 변경 방식이 바뀌어도 호출하는 코드를 일일이 수정하지 않아도 됩니다.

---

[README로 돌아가기](../README.md)
