# 🏹 Archero-like Unity Game

![image](https://github.com/user-attachments/assets/071832fb-31fb-4250-a172-1b29b611bca2)


탑다운 뷰 기반의 로그라이크 액션 게임입니다. 플레이어는 다양한 무기와 스킬을 조합하며 스테이지를 진행하고, 일반 몬스터와 보스를 처치하며 성장해나갑니다.  

---

## 📂 목차

1. [프로젝트 개요](#프로젝트-개요)
2. [팀원 및 역할 분담](#팀원-및-역할-분담)
3. [기능 구조도](#기능-구조도)
4. [주요 기능](#주요-기능)
5. [설치 및 실행 방법](#설치-및-실행-방법)
6. [기술 스택](#기술-스택)
7. [참고](#참고)

---

## 📘 프로젝트 개요

- **장르:** 로그라이크 액션 슈팅  
- **플랫폼:** Unity (PC, Android)  
- **주요 특징:**
  - 스테이지 기반의 적 생성 시스템
  - 다양한 무기 타입 (근접, 원거리, 투사체)
  - 스킬 조합 및 강화 시스템
  - 맵 및 몬스터 풀링 시스템
  - UI, 스테이지, 맵, 장애물 자동 관리

---

## 👥 팀원 및 역할 분담

| 이름 | 역할 |
|------|------|
| 심교인 | Weapon prefabs 종류별로 제작, Weapon prefabs별로 Projectile 생성, Weapon 스크립트  |
| 유도균 | Slime, Necromancer Boss 디자인, 플레이어 구르기 기능 |
| 이자연 | Skill System, Skill Data, 플레이어 달리기, UI이미지 관련 작업 |
| 소윤진 | Map(맵, 장애물)과 Enemy 프리팹 생성 관리 및 스테이지 진행에 따른 Enemy 강화 |
| 백성은 | StartScene, 플레이어/몬스터 체력바, 일시정지, 게임오버 등 게임에 필요한 UI |

---

## 🧩 기능 구조도

### 📌 주요 매니저 및 역할

<img width="4633" alt="Untitled (2)" src="https://github.com/user-attachments/assets/9bbba5b5-ad72-4a16-9a9b-ec66043a1b74" />


- **GameManager**  
  프로젝트의 중심 매니저. UI, 사운드, 무기/스킬, 스테이지 매니저를 제어합니다.

- **UIManager**  
  UI 및 스탯 처리, Skill UI 버튼 관리.

- **EnemyManager**  
  EnemyPool과 연동하여 일반 적과 보스를 생성 및 관리합니다.  
  인터페이스 `IEnemy`를 기반으로 `EnemyController`, `SlimeBossController`, `NecromancerBossController` 관리.

- **WeaponHandler**  
  근접 무기(`MeleeWeaponHandler`), 원거리 무기(`RangeWeaponHandler`), 투사체(`ProjectileManager`)를 통합 관리합니다.

- **SkillManager**  
  스킬 선택 및 실행 처리. `SkillDataBase` 및 `RunTimeSkill`과 연결되며 `PlayerSkillHandler`에 스킬을 전달합니다.

- **StageManager**  
  스테이지 진행 제어, 맵 풀 및 장애물 관리.

- **SoundManager**  
  게임 내 오디오 전반을 담당.

---

## ⚙️ 주요 기능

- **오브젝트 풀링 시스템**
  - `EnemyPool`, `MapPool`, `ObstacleManager`를 통해 풀링 처리
  - 보스 및 일반 몬스터의 효율적 재사용

- **적 분리 및 소환 기능**
  - `SlimeBossController`: 분열된 보스 슬라임 생성
  - `NecromancerBossController`: 소환수 생성 로직 포함

- **무기/투사체 시스템**
  - 무기 타입에 따른 핸들러 분리
  - 투사체는 `ProjectileManager` 및 `ProjectileController`를 통해 관리

- **스킬 시스템**
  - 실시간 선택/적용 가능한 `SkillButtonData`, `RunTimeSkill` 기반 구조
  - 스킬 강화 및 시너지 효과 구현 가능

- **스테이지 관리**
  - `StageManager`와 `MapPool`을 통해 맵 및 포탈 순차 생성
  - 5스테이지마다 보스 맵 등장

---

## 💻 기술 스택

| 분류       | 내용                                |
|------------|-------------------------------------|
| 게임 엔진  | Unity 2022.3.17f                   |
| 프로그래밍 | C#                                   |
| 데이터 관리 | ScriptableObject 기반 구조             |
| 최적화     | Object Pooling 패턴, Singleton                   |
| UI 시스템  | Unity UI, 커스텀 Skill 버튼 시스템    |
| 오디오     | AudioSource 기반 SoundManager 사용   |
| 애니메이션 | Animator 및 StateMachine 기반 제어   |

---

