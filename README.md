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



# ARPG 게임의 기능에 대해 핵심 코드를 기반으로 설명

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
```csharp
- 단순히 Unity에서 제공하는 편의 함수인 Vector3.Angle 함수를 사용하여 자신의 정면(transform.forward)과 적(target)까지의 방향(dirToTarget) 벡터 사이의 각도를 계산합니다.
- 이 각도가 시야각(viewAngle)의 절반(viewAngle / 2)보다 작으면 시야에 포함됩니다.
- Physics.Raycast 함수를 사용하여 자신과 적 사이에 장애물이 있는지 검사합니다.
- 검사 대상은 obstacleMask에 해당하는 레이어에 속한 오브젝트들입니다.



# ARPG 프로젝트 설명 영상
https://youtu.be/USFFC2Ag4UM

