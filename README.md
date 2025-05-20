SoulGatherer
이미지 표시
본 레포지토리는 Unity로 개발된 3D 로그라이트 게임 'SoulGatherer'의 핵심 소스코드를 포함하고 있습니다.
프로젝트 개요

개발 기간: 2024년 11월 ~ 2025년 4월 (5개월)
엔진: Unity 2022.3.12f1
장르: 3D 로그라이트
개발 인원: 1인 (신민수)

기술적 특징

컴포넌트 기반 설계와 다양한 디자인 패턴을 활용한 확장 가능한 몬스터 시스템
상태 패턴(FSM), 전략 패턴, 팩토리 패턴을 활용한 유연한 몬스터 AI
CSV 데이터 기반 몬스터 및 능력 시스템으로 데이터 주도 설계 구현
보스의 페이즈, 패턴, 기믹 등 복잡한 전투 시스템 모듈화
효율적인 레벨 디자인과 게임 요소 배치를 위한 커스텀 에디터 도구 구현

기술 영상
이미지 표시
폴더 구조
SoulGatherer/
├── 던전능력/
│   └── ... (던전능력 관련 스크립트)
├── 몬스터관련/
│   ├── 몬스터 CSV/
│   ├── 몬스터 상태 및 전략/
│   ├── 몬스터 AI/
│   ├── 보스패턴/
│   └── 팩토리/
└── 커스텀에디터/
    ├── SpawnPointEditorWindow.cs
    ├── BoundaryToolEditor.cs
    └── ColliderGenerator.cs
핵심 파일 안내
<details>
<summary><b>몬스터 AI 시스템</b></summary>

CreatureAI.cs - 몬스터 AI의 기본 뼈대가 되는 추상 클래스
BasicCreatureAI.cs - 일반 몬스터용 AI 구현
BossAI.cs - 보스 몬스터 전용 AI 시스템

</details>
<details>
<summary><b>상태 패턴 구현</b></summary>

IMonsterState.cs - 몬스터 상태 인터페이스
AttackState.cs - 몬스터의 공격 상태 관리
SkillState.cs - 몬스터의 스킬 사용 상태 관리
PatternState.cs - 보스의 패턴 상태 관리

</details>
<details>
<summary><b>전략 패턴 구현</b></summary>

IAttackStrategy.cs - 공격 전략 인터페이스
BasePhysicalAttackStrategy.cs - 물리 공격 전략 기본 클래스
ChargeAttackStrategy.cs - 돌진 공격 전략 구현
ISkillStrategy.cs - 스킬 전략 인터페이스
BossMultiAttackStrategy.cs - 보스의 다중 공격 전략 관리
BossMultiSkillStrategy.cs - 보스의 다중 스킬 전략 관리

</details>
<details>
<summary><b>스킬 시스템</b></summary>

ISkillEffect.cs - 스킬 효과 인터페이스
ProjectileSkillEffect.cs - 발사체 기반 스킬 효과
IProjectileMovement.cs - 발사체 이동 인터페이스
ParabolicMovement.cs - 포물선 이동 구현

</details>
<details>
<summary><b>팩토리 패턴 구현</b></summary>

StrategyFactory.cs - 전략 객체 생성 팩토리
MonsterFactoryBase.cs - 몬스터 생성을 담당하는 추상 팩토리
RatMonsterFactory.cs - 특정 몬스터 생성 팩토리 구현

</details>
<details>
<summary><b>보스 패턴 및 미니게임</b></summary>

BossPattern.cs - 보스 패턴의 기반 클래스
BossPatternManager.cs - 보스 패턴 관리 클래스
IMiniGame.cs - 미니게임 인터페이스
DodgeMiniGame.cs - 회피 미니게임 구현
MiniGameManager.cs - 미니게임 매니저

</details>
<details>
<summary><b>던전능력</b></summary>

BossData.cs - 보스 데이터 클래스
DungeonAbility.cs - 던전 능력 기본 클래스
AttackAbility.cs - 공격 능력 구현
MovementAbility.cs - 이동 능력 구현
PassiveAbility.cs - 패시브 능력 구현
SpecialAbility.cs - 특수 능력 구현

</details>
<details>
<summary><b>커스텀 에디터 도구</b></summary>

SpawnPointEditorWindow.cs - 몬스터, 플레이어, 포털 등의 스폰 포인트를 시각적으로 배치하는 에디터 도구
BoundaryToolEditor.cs - 게임 내 경계 영역을 쉽게 생성하고 관리하는 에디터 도구
ColliderGenerator.cs - 여러 점을 연결하여 충돌체를 자동 생성하는 유틸리티

</details>
확장성과 재사용성
이 코드베이스는 새로운 몬스터, 보스, 스킬, 패턴 등을 기존 시스템에 쉽게 추가할 수 있도록 설계되었습니다. 상태 패턴, 전략 패턴, 팩토리 패턴 등 다양한 디자인 패턴과 인터페이스 기반 설계를 통해 코드의 결합도를 낮추고 확장성을 높였습니다.
특히 보스 시스템은 페이즈별로 다른 전략과 패턴을 가질 수 있도록 설계되어, 다양하고 복잡한 보스 전투를 구현할 수 있습니다. 미니게임 시스템을 통해 기존의 전투 시스템을 확장하여 더욱 역동적인 게임플레이를 제공합니다.
커스텀 에디터 도구는 게임 개발 과정에서 레벨 디자인과 게임 요소 배치를 크게 효율화하여 개발 시간을 단축하였습니다. 스폰 포인트 에디터는 CSV 데이터 내보내기/가져오기를 통해 몬스터 배치를 효율적으로 관리합니다.
사용된 기술

C#: 모든 스크립트 구현
Unity Editor Extensions: 커스텀 에디터 도구 개발
Finite State Machine: 몬스터 상태 관리
Strategy Pattern: 공격 및 스킬 전략 관리
Factory Pattern: 몬스터 및 전략 객체 생성
CSV Data Management: 데이터 주도 설계 구현
Event System: 컴포넌트 간 통신

라이센스
이 코드는 포트폴리오 목적으로 공개되었으며, 무단 복제 및 상업적 사용을 금합니다.