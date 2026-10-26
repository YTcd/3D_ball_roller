# 3D Ball Roller

구 형태의 플레이어를 굴리고 점프시켜 도착지까지 가는 3D 게임.

- 조작: 키보드 `WASD` + `Space` / 모바일 가상 조이스틱 + 점프 버튼
- 환경: Unity 6000.2.12f1, URP, Input System 1.14
- 에셋: AurynSky Forest Pack (무료)

## 공부
- 3D Plane
- RigidBody

## 작업 순서

완료한 항목은 지우거나 `[x]`로 체크.

### 1. 프로젝트 기본 세팅
- [x] `Assets/` 폴더 구조 정리 (`_Project/Scripts`, `Prefabs`, `Materials`, `Scenes` 등 내 작업물과 에셋 분리)
- [x] 작업용 씬 생성 (`SampleScene` 정리 또는 `Level01` 새로 만들기)
- [x] Player Settings 확인 (Active Input Handling = Input System, 타깃 플랫폼 PC/모바일)

### 2. 플레이어 기본 이동 (키보드)
- [x] 바닥 Plane + 구(Sphere) 플레이어 배치, `Rigidbody` + `SphereCollider`
- [x] `InputSystem_Actions`에 Move(WASD) / Jump(Space) 액션 확인·정리
- [ ] `PlayerController` 스크립트: Move 입력 → `AddForce`로 구르기
- [ ] 이동 속도/가속/최대 속도 파라미터 조정 (Inspector 노출)

### 3. 점프
- [ ] 접지 판정 (SphereCast 또는 충돌 법선 체크)
- [ ] 접지 상태에서만 점프 (`ForceMode.Impulse`)
- [ ] 점프 높이·중력감 조정 (필요하면 낙하 시 추가 중력)
- [ ] Physics Material로 마찰·튕김 조정

### 4. 카메라
- [ ] 플레이어를 따라가는 3인칭 카메라 (오프셋 + 부드러운 추적)
- [ ] 이동 방향을 카메라 기준으로 변환 (W = 카메라 앞쪽)
- [ ] (선택) 카메라 회전 / Cinemachine 도입 여부 결정

### 5. 게임 루프: 시작·도착·실패
- [ ] 시작 지점(Spawn Point) 지정
- [ ] 도착 지점 오브젝트 + Trigger → 클리어 처리
- [ ] 낙사 판정 (일정 높이 아래로 떨어지면 리스폰)
- [ ] (선택) 체크포인트
- [ ] `GameManager`: 상태 관리 (플레이 중 / 클리어 / 재시작)

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
