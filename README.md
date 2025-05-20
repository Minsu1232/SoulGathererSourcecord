# SoulGatherer 소스코드

본 레포지토리는 Unity로 개발된 3D 로그라이트 게임 'SoulGatherer'의 핵심 소스코드를 포함하고 있습니다.
기술문서 위주로 소스코드가 준비되어 있습니다.

## 프로젝트 개요

- 개발 기간: 2024년 11월 ~ 2025년 4월 (5개월)
- 엔진: Unity 2022.3.12f1
- 장르: 3D 로그라이트
- 개발 인원: 1인 (신민수)

## 기술적 특징

- 컴포넌트 기반 설계와 다양한 디자인 패턴을 활용한 확장 가능한 몬스터 시스템
- 상태 패턴(FSM), 전략 패턴, 팩토리 패턴을 활용한 유연한 몬스터 AI
- CSV 데이터 기반 몬스터 및 능력 시스템으로 데이터 주도 설계 구현
- 보스의 페이즈, 패턴, 기믹 등 복잡한 전투 시스템 모듈화

## 폴더 안내

던전능력 - 던전능력 관련 스크립트 입니다. 


        1. 기술문서에서 설명한 GetSmartAbilitySelection()은 DungeonAbilityManager에 있습니다.

몬스터 관련 - 몬스터 관련 스크립트

        1. 몬스터 CSV - 몬스터 관련 CSV 파일들 입니다.
        2. 몬스터 상태 및 전략 - 일부 상태와 전략 (공격,스킬)위주 입니다.
        3. 몬스터 AI - AI스크립트들 입니다.
        4. 보스패턴 - 보스 특수 상태 중 Pattern과 그와 함께하는 미니게임 스크립트들 입니다. 
        5. 팩토리 - 몬스터 생성 기능 팩토리 파일 일부입니다.

## 핵심 파일 안내


### 몬스터 AI 시스템

- [CreatureAI.cs](몬스터관련/몬스터AI/CreatureAI.cs) - 몬스터 AI의 기본 뼈대가 되는 추상 클래스
- [BasicMonsterAI.cs](몬스터관련/몬스터AI/BasicMonsterAI.cs) - 일반 몬스터용 AI 구현
- [BossAI.cs](몬스터관련/몬스터AI/BossAI.cs) - 보스 몬스터 전용 AI 시스템

### 상태 패턴 구현

- [IMonsterState.cs](몬스터관련/몬스터상태및전략/IMonsterState.cs) - 몬스터 상태 인터페이스
- [AttackState.cs](몬스터관련/몬스터 상태 및 전략/AttackState.cs) - 몬스터의 공격 상태 관리
- [SkillState.cs](몬스터관련/몬스터 상태 및 전략/SkillState.cs) - 몬스터의 스킬 사용 상태 관리
- [PatternState.cs](몬스터관련/보스패턴/PatternState.cs) - 보스의 패턴 상태 관리

### 전략 패턴 구현

- [IAttackStrategy.cs](1몬스터시스템/IAttackStrategy.cs) - 공격 전략 인터페이스
- [BasePhysicalAttackStrategy.cs](1몬스터시스템/BasePhysicalAttackStrategy.cs) - 물리 공격 전략 기본 클래스
- [ChargeAttackStrategy.cs](1몬스터시스템/ChargeAttackStrategy.cs) - 돌진 공격 전략 구현
- [ISkillStrategy.cs](1몬스터시스템/ISkillStrategy.cs) - 스킬 전략 인터페이스
- [BossMultiAttackStrategy.cs](2보스시스템/BossMultiAttackStrategy.cs) - 보스의 다중 공격 전략 관리
- [BossMultiSkillStrategy.cs](2보스시스템/BossMultiSkillStrategy.cs) - 보스의 다중 스킬 전략 관리

### 스킬 시스템

- [ISkillEffect.cs](1몬스터시스템/ISkillEffect.cs) - 스킬 효과 인터페이스
- [ProjectileSkillEffect.cs](1몬스터시스템/ProjectileSkillEffect.cs) - 발사체 기반 스킬 효과
- [IProjectileMovement.cs](1몬스터시스템/IProjectileMovement.cs) - 발사체 이동 인터페이스
- [ParabolicMovement.cs](1몬스터시스템/ParabolicMovement.cs) - 포물선 이동 구현

### 팩토리 패턴 구현

- [StrategyFactory.cs](1몬스터시스템/StrategyFactory.cs) - 전략 객체 생성 팩토리
- [MonsterFactoryBase.cs](1몬스터시스템/MonsterFactoryBase.cs) - 몬스터 생성을 담당하는 추상 팩토리
- [RatMonsterFactory.cs](1몬스터시스템/RatMonsterFactory.cs) - 특정 몬스터 생성 팩토리 구현

### 보스 패턴 및 미니게임

- [BossPattern.cs](2보스시스템/BossPattern.cs) - 보스 패턴의 기반 클래스
- [BossPatternManager.cs](2보스시스템/BossPatternManager.cs) - 보스 패턴 관리 클래스
- [IMiniGame.cs](2보스시스템/IMiniGame.cs) - 미니게임 인터페이스
- [DodgeMiniGame.cs](2보스시스템/DodgeMiniGame.cs) - 회피 미니게임 구현
- [MiniGameManager.cs](2보스시스템/MiniGameManager.cs) - 미니게임 매니저

### 던전능력

- [BossData.cs](2보스시스템/BossData.cs) - 보스 데이터 클래스
- [DungeonAbility.cs](3능력시스템/DungeonAbility.cs) - 던전 능력 기본 클래스
- [AttackAbility.cs](3능력시스템/AttackAbility.cs) - 공격 능력 구현
- [MovementAbility.cs](3능력시스템/MovementAbility.cs) - 이동 능력 구현
- [PassiveAbility.cs](3능력시스템/PassiveAbility.cs) - 패시브 능력 구현
- [SpecialAbility.cs](3_능력시스템/SpecialAbility.cs) - 특수 능력 구현

### 커스텀 에디터 도구

- [SpawnPointEditorWindow.cs](커스텀에디터/SpawnPointEditorWindow.cs) - 몬스터, 플레이어, 포털 등의 스폰 포인트를 시각적으로 배치하는 에디터 도구
- [BoundaryToolEditor.cs](커스텀에디터/BoundaryToolEditor.cs) - 게임 내 경계 영역을 쉽게 생성하고 관리하는 에디터 도구
- [ColliderGenerator.cs](커스텀에디터/ColliderGenerator.cs) - 여러 점을 연결하여 충돌체를 자동 생성하는 유틸리티


## 기술 영상 링크

[SoulGatherer 기술 영상](https://www.youtube.com/watch?v=ltqY_8huh2c&t=0s)

## 확장성과 재사용성


이 코드베이스는 새로운 몬스터, 보스, 스킬, 패턴 등을 기존 시스템에 쉽게 추가할 수 있도록 설계되었습니다. 상태 패턴, 전략 패턴, 팩토리 패턴 등 다양한 디자인 패턴과 인터페이스 기반 설계를 통해 코드의 결합도를 낮추고 확장성을 높였습니다.
특히 보스 시스템은 페이즈별로 다른 전략과 패턴을 가질 수 있도록 설계되어, 다양하고 복잡한 보스 전투를 구현할 수 있습니다. 미니게임 시스템을 통해 기존의 전투 시스템을 확장하여 더욱 역동적인 게임플레이를 제공합니다.
## 라이센스
이 코드는 포트폴리오 목적으로 공개되었으며, 무단 복제 및 상업적 사용을 금합니다.