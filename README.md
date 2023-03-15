# ARPG
Action RPG
- ARPG/Assets/ARPG/DialogueSystem : NPC 다이얼로그 시스템
- ARPG/Assets/ARPG/DoorSystem : 위아래로 움직이는 문 구현
- ARPG/Assets/ARPG/Firebase : 사용자 인증, 사용자 순위 (leaderboard), 사용자 데이터 저장과 불러오기 구현
- ARPG/Assets/ARPG/InventorySystem : 아이템, 인벤토리, 인벤토리 UI, 캐릭터 장비 교체, 아이템 사용 구현
- ARPG/Assets/ARPG/ManagerSystem : 적 생성 관리자 구현
- ARPG/Assets/ARPG/PrefabCacheSystem : 오브젝트 풀링 구현
- ARPG/Assets/ARPG/QuestSystem : 사냥과 아이템 획득 퀘스트 구현
- ARPG/Assets/ARPG/SceneControllerSystem : Scene 이동 구현
- ARPG/Assets/ARPG/Scripts : 플레이어 캐릭터(이동, 공격, 아이템 획득, 마우스 커서 UI), 적 캐릭터(이동, 공격, 감지, 체력 UI, 데미지 UI), 상태 머신, 3인칭 TopDown 카메라, 카메라 에디터 구현
- ARPG/Assets/ARPG/StatsSystem : 플레이어 속성과 플레이어 상태 UI 구현
- ARPG/Assets/ARPG/TableMarshalSystem : 마샬링을 이용해서 CSV 테이블의 레코드 문자열(1줄)을 구조체로 변환하고 필요한 형태의 자료 저장소에 기록
- ARPG/Assets/ARPG/TrapSystem : 지속해서 데미지 주는 함정 구현



# ARPG 게임의 기능에 대해 핵심 코드 기반 설명

## 기능명 : 시야 인식
### 기능 설명
이 기능는 주어진 시야 반경(viewRadius) 내에 존재하는 적(target)을 검색하고, 시야각(viewAngle)과 장애물(obstacleMask)을 고려하여 가장 가까운 살아있는 적(nearestTarget)을 찾는 기능입니다.

### 핵심 코드 1: 기본적인 적 검색
```csharp
Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
```
- Physics.OverlapSphere 함수를 사용하여 현재 위치(transform.position)를 중심으로 시야 반경(viewRadius) 내에 있는 모든 콜라이더를 검색합니다.
- 검색 대상은 targetMask에 해당하는 레이어에 속한 오브젝트들입니다.

### 핵심 코드 2: 시야각과 장애물 적용
```csharp
if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
{
    // ...
    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
    {
        // ...
    }
}
```
- 단순히 Unity에서 제공하는 편의 함수인 Vector3.Angle 함수를 사용하여 자신의 정면(transform.forward)과 적(target)까지의 방향(dirToTarget) 벡터 사이의 각도를 계산합니다.
- 이 각도가 시야각(viewAngle)의 절반(viewAngle / 2)보다 작으면 시야에 포함됩니다.
- Physics.Raycast 함수를 사용하여 자신과 적 사이에 장애물이 있는지 검사합니다.
- 검사 대상은 obstacleMask에 해당하는 레이어에 속한 오브젝트들입니다.


## 기능명 : State Machine
### 기능 설명
State Machine은 여러 상태(State)를 가지고 있는 객체로, 각 상태는 State Machine이 가지고 있는 데이터와 함께 작동합니다.
또한, State Machine은 상태를 추가, 업데이트, 변경할 수 있으며 
상태 변경할 때 변경된 상태에 따라 필요한 작업을 수행합니다.

### 핵심 코드 1: 상태(State) 객체를 추가
```csharp
state.SetMachineAndContext(this, context);
states[state.GetType()] = state;
```
- 전달된 State 객체의 SetMachineAndContext 메서드를 호출하여 해당 State 객체에 StateMachine 객체와 Context 객체를 설정합니다.
- 설정된 State 객체를 states 딕셔너리에 추가합니다. 이때 Key 값으로는 State 객체의 Type을 사용하고, Value 값으로는 설정된 State 객체를 사용합니다.

### 핵심 코드 2: 현재 상태(currentState)를 업데이트
```csharp
elapsedTimeInState += deltaTime;

currentState.PreUpdate();
currentState.Update(deltaTime);
```
- elapsedTimeInState 변수에 deltaTime을 더하여 현재 상태에서 경과된 시간을 계산합니다.
- currentState.PreUpdate() 함수를 호출하여 현재 상태의 PreUpdate 작업을 수행합니다. 
- currentState.Update(deltaTime) 함수를 호출하여 현재 상태의 Update 작업을 수행합니다.

### 핵심 코드 3: 현재 상태(currentState)를 새로운 상태(R)로 변경할 때, 현재 상태와 새로운 상태가 같은지 비교
```csharp
var newType = typeof(R);
if (currentState.GetType() == newType)
{
    return currentState as R;
}
```
- 새롭게 변경할 상태(R)가 현재 상태(currentState)인 경우, 현재 상태(currentState)를 반환하고 함수 실행을 종료합니다.

### 핵심 코드 4: 상태 변경하기 전에 예외 처리
```csharp
#if UNITY_EDITOR
if (!states.ContainsKey(newType))
{
    var error = GetType() + 
        ": state " + newType +
        " 존재하지 않습니다. AddState()를 호출하지 않은 것으로 추측이 됩니다.";
    Debug.LogError("error");
    throw new Exception(error);
}
#endif
```
- UnityEditor에서 실행할 경우, 새로운 상태(R)가 State Machine의 states 딕셔너리에 없을 경우, 오류를 출력하고 예외를 발생시킵니다.

### 핵심 코드 5: 새로운 상태(R)로 상태 변경
```csharp
previousState = currentState;
currentState = states[newType];
currentState.OnEnter();
elapsedTimeInState = 0.0f;
```
- 현재 상태(currentState)를 이전 상태(previousState)로 저장합니다.
- 새로운 상태(R)를 currentState에 할당합니다.
- 새로운 상태(R)의 OnEnter 함수를 호출하여, 해당 상태에 진입함을 처리합니다.
- elapsedTimeInState 변수를 0으로 설정하여 현재 상태(R)에서 경과된 시간을 0으로 초기화합니다.

### 핵심 코드 6: 상태가 변경될 때, OnChangedState 이벤트 발생
```csharp
if (OnChangedState != null)
{
    OnChangedState();
}
```
- OnChangedState 이벤트는 현재 상태가 변경될 때마다 호출되며, 이벤트를 구독하고 있는 다른 객체들이 이를 처리할 수 있습니다.





# ARPG 프로젝트 설명 영상
https://youtu.be/USFFC2Ag4UM

