# AppService 구조

## AppService는 무엇인가요?

`AppService`는 프로젝트 전체에서 사용할 객체를 `App`에 등록하기 위한 부모 클래스입니다.

쉽게 말하면 다음 역할을 합니다.

```text
AppService를 상속한 컴포넌트
→ 게임이 시작될 때 App에 등록됨
→ 다른 코드에서 App.Get<T>()로 가져올 수 있음
```

## 사용 예시

```csharp
public class SoundManager : AppService
{
}
```

이 클래스가 씬이나 프리팹에 컴포넌트로 존재하면 다음처럼 가져올 수 있습니다.

```csharp
var soundManager = App.Get<SoundManager>();
```

## 왜 이 구조를 사용하나요?

기존 방식에서는 새로운 매니저를 추가할 때마다 `App`에 필드와 Getter를 추가해야 했습니다.

```csharp
readonly SoundManager sound;

public static SoundManager Sound => instance.sound;
```

매니저가 늘어날수록 `App` 코드도 계속 길어졌습니다.

현재 방식에서는 새 클래스를 만들고 `AppService`만 상속하면 됩니다.

```csharp
public class QuestManager : AppService
{
}
```

사용할 때는 다음처럼 가져옵니다.

```csharp
var questManager = App.Get<QuestManager>();
```

`App` 코드를 다시 수정할 필요는 없습니다.

## 언제 App.Get을 사용하나요?

버튼 클릭, 씬 이동, 이벤트 처리처럼 가끔 실행되는 코드에서는 바로 사용해도 됩니다.

```csharp
public void OnClickButton()
{
    App.Get<SoundManager>().PlaySFX("Click");
}
```

`Update()`처럼 자주 실행되는 곳에서는 한 번 가져와 저장하는 편이 좋습니다.

```csharp
private SoundManager _soundManager;

private void Start()
{
    _soundManager = App.Get<SoundManager>();
}
```

```csharp
private void Update()
{
    // 필요할 때 _soundManager 사용
}
```

## 주의할 점

`App.Get<T>()`를 호출했는데 해당 객체가 등록되어 있지 않으면 오류가 발생합니다.

다음 내용을 확인합니다.

1. 클래스가 `AppService`를 상속했는지
2. 해당 컴포넌트가 씬이나 프리팹에 존재하는지
3. 비활성화된 상태로 시작하지 않는지
4. `Awake()`에서 `base.Awake()`를 호출했는지
