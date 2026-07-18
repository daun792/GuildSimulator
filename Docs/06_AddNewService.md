# Docs/06_AddNewService.md

# 새로운 서비스 추가하기

이 문서는 새로운 Manager나 Data 클래스를 추가할 때 확인합니다.

## 1. 클래스 만들기

예를 들어 퀘스트 기능을 관리하는 클래스를 만듭니다.

```csharp
public class QuestManager : AppService
{
}
```

## 2. GameObject에 컴포넌트 추가하기

클래스를 만들기만 하면 실제 객체가 생기지는 않습니다.

다음 중 적절한 위치에 컴포넌트를 추가합니다.

```text
App 프리팹의 자식 GameObject
전역 시스템을 모아둔 GameObject
해당 기능이 필요한 씬의 GameObject
```

프로젝트 전체에서 계속 사용해야 한다면 `App`과 함께 유지되는 오브젝트에 두는 편이 좋습니다.

## 3. 다른 코드에서 가져오기

```csharp
var questManager = App.Get<QuestManager>();
```

## 4. Awake를 작성할 때 주의하기

`AppService`를 상속한 클래스에서 `Awake()`를 직접 작성한다면 `base.Awake()`를 호출해야 합니다.

```csharp
protected override void Awake()
{
    base.Awake();

    // QuestManager 초기화
}
```

`base.Awake()`가 빠지면 `App`에 등록되지 않을 수 있습니다.

## 5. App 코드는 수정하지 않아도 됩니다

새로운 서비스를 추가할 때 다음 작업은 하지 않아도 됩니다.

```text
App에 필드 추가
App에 Getter 추가
App에 TryGet 메서드 추가
App Inspector에 연결
```

클래스를 만들고 컴포넌트를 배치하면 됩니다.

---

[README로 돌아가기](../README.md)
