# SoundManager 사용법

`SoundManager`는 BGM과 SFX을 재생합니다.

## SoundManager 가져오기

```csharp
var sound = App.Get<SoundManager>();
```

한 줄로 바로 사용할 수도 있습니다.

```csharp
App.Get<SoundManager>().PlaySFX("Click");
```

## BGM 재생

```csharp
App.Get<SoundManager>().PlayBGM("Title");
```

`"Title"`은 `SoundManager`의 BGM 목록에 등록된 이름과 같아야 합니다.

## BGM 정지

```csharp
App.Get<SoundManager>().StopBGM();
```

## 효과음 재생

```csharp
App.Get<SoundManager>().PlaySFX("Click");
```

효과음 이름도 Inspector에 등록된 이름과 같아야 합니다.

## 특정 효과음 정지

```csharp
App.Get<SoundManager>().StopSFX("Rain");
```

같은 이름으로 재생 중인 효과음을 정지합니다.

## 모든 효과음 정지

```csharp
App.Get<SoundManager>().StopAllSFX();
```

## 음소거 전환

```csharp
var sound = App.Get<SoundManager>();

sound.ToggleMute(EVolumeType.Master);
sound.ToggleMute(EVolumeType.BGM);
sound.ToggleMute(EVolumeType.SFX);
```

## 새로운 사운드 추가

1. `SoundManager`가 있는 GameObject를 선택합니다.
2. Inspector에서 `Bgm Clips` 또는 `Sfx Clips` 목록을 엽니다.
3. 목록 크기를 늘립니다.
4. `Name`에 코드에서 사용할 이름을 입력합니다.
5. `Clip`에 AudioClip을 연결합니다.

예시:

```text
Name: Click
Clip: UI_Click.wav
```

코드에서는 다음처럼 사용합니다.

```csharp
App.Get<SoundManager>().PlaySFX("Click");
```

## 이름을 작성할 때 주의할 점

다음 두 이름은 서로 다르게 처리됩니다.

```text
Click
click
```

이름 형식을 사전에 정해 하나로 통일하는 것이 좋습니다.

예를 들어:

```text
BGM_Title
BGM_InGame

SFX_Click
SFX_Attack
SFX_DoorOpen
```

## 효과음 풀은 무엇인가요?

효과음을 재생할 때마다 새로운 `AudioSource`를 계속 만들면 불필요한 작업이 늘어납니다.

그래서 미리 몇 개를 만들어두고 재사용합니다.

```text
효과음 재생
→ 풀에서 AudioSource를 가져옴
→ 재생
→ 재생이 끝나면 풀에 돌려놓음
```

처음 만들어두는 개수는 Inspector의 `Initial Pool Size`에서 설정합니다.
