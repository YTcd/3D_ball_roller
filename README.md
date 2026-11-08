# 3D Ball Roller

구 형태의 플레이어를 굴리고 점프시켜 도착지까지 가는 3D 게임.

- 조작: 키보드 `WASD` + `Space` / 모바일 가상 조이스틱 + 점프 버튼
- 환경: Unity 6000.2.12f1, URP, Input System 1.14
- 에셋: AurynSky Forest Pack (무료)

## 공부
- 3D Plane
- RigidBody
    - Mass, Linear Damping, Anglular Damping
    - ForceMode
    - sleep
- DefaultExecutionOrder
- getter,setter
- Material 속성
- SceneManager

## 작업 순서

완료한 항목은 지우거나 `[x]`로 체크.

### 1. 프로젝트 기본 세팅
- [x] `Assets/` 폴더 구조 정리 (`_Project/Scripts`, `Prefabs`, `Materials`, `Scenes` 등 내 작업물과 에셋 분리)
- [x] 작업용 씬 생성 (`SampleScene` 정리 또는 `Level01` 새로 만들기)
- [x] Player Settings 확인 (Active Input Handling = Input System, 타깃 플랫폼 PC/모바일)

### 2. 플레이어 기본 이동 (키보드)
- [x] 바닥 Plane + 구(Sphere) 플레이어 배치, `Rigidbody` + `SphereCollider`
- [x] `InputSystem_Actions`에 Move(WASD) / Jump(Space) 액션 확인·정리
- [x] `PlayerController` 스크립트: Move 입력 → `AddForce`로 구르기
- [x] 이동 속도/가속/최대 속도 파라미터 조정 (Inspector 노출)

### 3. 점프
- [x] 접지 판정 (SphereCast 또는 충돌 법선 체크)
- [x] 접지 상태에서만 점프 (`ForceMode.Impulse`)
- [x] 점프 높이·중력감 조정 (필요하면 낙하 시 추가 중력)
- [x] Physics Material로 마찰·튕김 조정
- [x] 점프 버퍼 만들기
- [x] 점프 높이 가변

### 4. 카메라
- [x] 플레이어를 따라가는 3인칭 카메라 (오프셋 + 부드러운 추적)
- [x] 마우스 입력 또는 가상 조이스틱 입력으로 카메라 위치 변경
- [x] 이동 방향을 카메라 기준으로 변환 (W = 카메라 앞쪽)
- [ ] (선택) Cinemachine 도입 여부 결정 — 벽 많은 레벨에서 재검토

### 5. 게임 시스템: 레벨(씬) 구조

먼저 알아둘 것
- `SceneManager.LoadScene` / `GetActiveScene().buildIndex`
- Build Profiles(Build Settings)의 씬 목록과 순서 = 레벨 순서
- `DontDestroyOnLoad`와 싱글톤 수명 (씬마다 새로 생기는지 vs 계속 유지되는지)
- 씬 로드 후 초기화 시점 (`Awake`/`Start`, `SceneManager.sceneLoaded`)

작업
- [ ] 구조 결정: 레벨 씬에는 레벨 고유 요소(지형, `SpawnPoint`, `Goal`)만 두고, 공통 요소(Player, Camera, `GameManager`, UI)는 프리팹으로 만들어 각 씬에 배치
- [ ] `Level_Base`를 새 레벨용 템플릿 씬으로 정리 (공통 프리팹 포함) → 복제해서 `Level_01`, `Level_02` … 생성
- [ ] Build Profiles 씬 목록 정리 (존재하지 않는 `SampleScene` 제거, `Level_00`부터 순서대로 등록)
- [ ] `GameManager`: 상태 관리 (플레이 중 / 클리어), 씬 시작 시 `SpawnPoint` 찾아 플레이어 배치
- [ ] `Goal` Trigger → `GameManager`에 클리어 알림
- [ ] 낙사 판정 (일정 높이 아래로 떨어지면 `SpawnPoint`로 리스폰)
- [ ] 재시작 = 현재 씬 다시 로드, 다음 레벨 = `buildIndex + 1` 로드 (마지막 레벨이면 처음으로)
- [ ] 임시 `Level_01`을 만들어 `Level_00` → `Level_01` 전환 확인
- [ ] (선택) 체크포인트
- [ ] (선택) 진행도 저장 (`PlayerPrefs`로 해금된 레벨)
- [ ] (나중에 검토) 공통 요소를 Bootstrap 씬에 두고 레벨 씬을 Additive 로드하는 방식

### 6. 레벨 디자인 (그레이박스)
- [ ] 단순 큐브로 발판·경사로·간격 배치해서 첫 레벨 구성
- [ ] 점프 거리·높이에 맞게 발판 간격 튜닝
- [ ] (선택) 움직이는 발판 등 장애물

### 7. 모바일 입력
- [ ] Canvas에 가상 조이스틱 UI 배치
- [ ] Input System `On-Screen Stick` / `On-Screen Button`으로 Move·Jump에 연결 (기존 `PlayerController` 그대로 재사용)
- [ ] 플랫폼/터치 여부에 따라 모바일 UI 표시·숨김
- [ ] Device Simulator로 화면 비율·터치 테스트

### 8. UI
- [ ] 클리어 화면 (다시하기 / 다음 레벨)
- [ ] (선택) 타이머, 시작 화면, 일시정지

### 9. 비주얼 다듬기
- [ ] 그레이박스 레벨에 Forest Pack 에셋 적용
- [ ] 플레이어 머티리얼, 조명, 스카이박스
- [ ] (선택) 착지·클리어 파티클, 사운드

### 10. 빌드 & 테스트
- [ ] PC 빌드 테스트
- [ ] 모바일(Android 등) 빌드 및 실기기 조작감 테스트
- [ ] 조작감 최종 튜닝
