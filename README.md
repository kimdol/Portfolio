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



# ARPG 프로젝트 설명 영상
https://youtu.be/USFFC2Ag4UM



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
State Machine은 상태 패턴을 활용하였으며 여러 상태(State)를 가지고 있고, 
각 상태는 State Machine이 가지고 있는 데이터와 함께 작동합니다.
또한, State Machine은 상태를 추가, 업데이트, 변경할 수 있으며 
상태 변경할 때 변경된 상태에 따라 필요한 작업을 수행합니다.

### 핵심 코드 1: 상태(State) 객체를 추가
```csharp
state.SetMachineAndContext(this, context);
states[state.GetType()] = state;
```
- 전달받은 State 객체의 SetMachineAndContext 메서드를 호출하여 해당 State 객체에 StateMachine 객체와 Context 객체를 설정합니다.
- 설정된 State 객체를 State Machine의 states 딕셔너리에 추가합니다. 이때 Key 값으로는 State 객체의 Type을 사용하고, Value 값으로는 설정된 State 객체를 사용합니다.

### 핵심 코드 2: State Machine의 현재 상태(currentState)를 업데이트
```csharp
elapsedTimeInState += deltaTime;

currentState.PreUpdate();
currentState.Update(deltaTime);
```
- elapsedTimeInState 변수에 deltaTime을 더하여 현재 상태에서 경과된 시간을 계산합니다.
- currentState.PreUpdate() 함수를 호출하여 현재 상태의 PreUpdate 작업을 수행합니다. 
- currentState.Update(deltaTime) 함수를 호출하여 현재 상태의 Update 작업을 수행합니다.

### 핵심 코드 3: 새로운 상태로 상태 변경
```csharp
currentState = states[newType];
currentState.OnEnter();
elapsedTimeInState = 0.0f;
```
- 새로운 상태를 State Machine의 currentState에 할당합니다.
- 새로운 상태의 OnEnter 함수를 호출하여, 해당 상태에 진입함을 처리합니다.
- elapsedTimeInState 변수를 0으로 설정하여 새롭게 변경된 현재 상태에서 경과된 시간을 0으로 초기화합니다.

### 핵심 코드 4: 상태가 변경될 때, OnChangedState 이벤트 발생
```csharp
if (OnChangedState != null)
{
    OnChangedState();
}
```
- OnChangedState 이벤트는 현재 상태가 변경될 때마다 호출되며, 이벤트를 구독하고 있는 다른 객체들이 이를 처리할 수 있습니다.


## 기능명 : Record Line Parsing
### 기능 설명
입력받은 문자열(line)을 바이트 배열로 변환한 후, 이를 구조체(TMarshalStruct)로 변환하는 기능입니다. 이 과정에서 마샬링(marshalling)이 이루어집니다.

### 핵심 코드 1: int 타입의 바이트 배열로 변환
```csharp
fieldByte = BitConverter.GetBytes(int.Parse(splite));
```
- int 타입일 경우, BitConverter.GetBytes 함수를 사용하여 int.Parse 함수로 파싱된 값을 해당 타입의 바이트 배열로 변환합니다.

### 핵심 코드 2: float 타입의 바이트 배열로 변환
```csharp
fieldByte = BitConverter.GetBytes(float.Parse(splite));   
```
- float 타입일 경우, BitConverter.GetBytes 함수를 사용하여 float.Parse 함수로 파싱된 값을 해당 타입의 바이트 배열로 변환합니다.

### 핵심 코드 3: bool 타입의 바이트 배열로 변환
```csharp
bool value = bool.Parse(splite);
int temp = value ? 1 : 0;

fieldByte = BitConverter.GetBytes((int)temp);
```
- bool 타입일 경우, bool.Parse 함수를 사용하여 문자열을 bool 타입으로 파싱합니다.
- bool 타입의 값을 int 타입으로 변환하기 위해, 삼항 연산자를 사용하여 true일 경우 1, false일 경우 0으로 값을 변환합니다.
- 변환된 값을 BitConverter.GetBytes 함수를 사용하여 int 타입의 바이트 배열로 변환합니다.

### 핵심 코드 4: string 타입의 바이트 배열로 변환
```csharp
fieldByte = new byte[MarshalTableConstant.charBufferSize]; 
byte[] byteArr = Encoding.UTF8.GetBytes(splite);  

Buffer.BlockCopy(byteArr, 0, fieldByte, 0, byteArr.Length);
```
- string 타입일 경우, Encoding.UTF8.GetBytes 함수를 사용하여 해당 문자열을 UTF-8 형식의 바이트 배열로 변환합니다.
- 변환된 바이트 배열을 Buffer.BlockCopy 함수를 사용하여 fieldByte 배열에 복사합니다.
(MarshalTableConstant.charBufferSize는 최대 크기로 256으로 잡았고, 마샬링을 하기 위한 고정크기 버퍼를 생성하려고 존재합니다.)

### 핵심 코드 5: Marshal 메모리 할당
```csharp
int size = Marshal.SizeOf(typeof(T));
IntPtr ptr = Marshal.AllocHGlobal(size);
```
- Marshal.SizeOf 함수를 사용하여 T 타입의 크기를 구합니다.
- Marshal.AllocHGlobal 함수를 사용하여 해당 크기만큼의 Marshal 메모리를 할당합니다.

### 핵심 코드 6: 데이터 복사
```csharp
Marshal.Copy(bytes, 0, ptr, size);
```
- Marshal.Copy 함수를 사용하여 bytes 배열에서 size 만큼 데이터를 복사합니다.
복사된 데이터는 ptr 포인터가 가리키는 메모리 영역에 저장됩니다.

### 핵심 코드 7: 구조체로 변환
```csharp
T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));
```
- Marshal.PtrToStructure 함수를 사용하여 ptr 포인터가 가리키는 메모리 영역에 있는 데이터를 T 타입의 구조체로 변환합니다.

### 핵심 코드 8: 메모리 해제
```csharp
Marshal.FreeHGlobal(ptr);
```
- Marshal.FreeHGlobal 함수를 사용하여 ptr 포인터가 가리키는 메모리를 해제합니다.







