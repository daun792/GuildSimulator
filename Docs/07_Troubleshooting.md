# 자주 발생하는 문제

## `is not registered` 오류가 발생해요

예시:

```text
SoundManager is not registered.
```

다음 순서로 확인합니다.

1. 클래스가 `AppService`를 상속했는지 확인
2. 해당 컴포넌트가 씬이나 프리팹에 있는지 확인
3. GameObject가 비활성화되어 있지 않은지 확인
4. `Awake()`에서 `base.Awake()`를 호출했는지 확인
5. 해당 객체보다 먼저 `App.Get<T>()`를 호출하고 있지 않은지 확인

## 효과음이 재생되지 않아요

다음 항목을 확인합니다.

1. SFX 목록에 이름이 등록되어 있는지
2. 코드의 이름과 Inspector 이름이 같은지
3. AudioClip이 연결되어 있는지
4. SFX Mixer Group이 연결되어 있는지
5. SFX가 음소거 상태인지

## BGM이 재생되지 않아요

다음 항목을 확인합니다.

1. BGM 목록에 이름이 등록되어 있는지
2. BGM AudioSource가 연결되어 있는지
3. AudioClip이 연결되어 있는지
4. BGM 또는 Master가 음소거 상태인지

## 같은 효과음이 이상하게 끊겨요

효과음 풀에 문제가 생겼거나, 같은 AudioSource가 중복으로 사용되고 있을 수 있습니다.

다음 내용을 확인합니다.

```text
CreateSfxSource에서 active 목록에 추가하지 않았는지
PlaySFX에서만 active 목록에 추가하는지
ReturnToPool에서 active 목록에서 제거하는지
```

정상적인 흐름은 다음과 같습니다.

```text
CreateSfxSource
→ AudioSource 생성만 함

PlaySFX
→ active 목록에 추가

ReturnToPool
→ active 목록에서 제거
→ pool에 추가
```

## 문서에 없는 문제가 생겼어요

문제를 해결한 뒤 이 문서에 짧게 추가합니다.

```text
문제
원인
해결 방법
```

예시:

```text
문제:
효과음 이름을 찾지 못함

원인:
Inspector에는 Click으로 등록했지만 코드에서는 click으로 호출함

해결:
두 이름을 동일하게 수정
```

---

[README로 돌아가기](../README.md)
