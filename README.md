# Unity-VContainer-UniRx-MVP-Example

---

작성자 : 진호성

궁금한 점 있을 시 Issue 남겨주시면 친절히 답변해드립니다!

# 목차

- [개요](#개요)
  * [작성 의의](#작성-의의)
  * [소개](#소개)
    + [함께 사용된 라이브러리](#함께-사용된-라이브러리)
    + [문서 내용](#문서-내용)
- [Part 1. 레퍼런스 게임 분석](#part-1-레퍼런스-게임-분석)
  * [구현된 기능 리스트](#구현된-기능-리스트)
    + [젤리](#젤리)
    + [젤리팜 메인 UI](#젤리팜-메인-ui)
    + [젤리 구매 및 해금 UI](#젤리-구매-및-해금-ui)
    + [젤리 업그레이드 UI](#젤리-업그레이드-ui)
- [Part 2. MVP 패턴 소개](#part-2-mvp-패턴-소개)
  * [MVC, MVP, MVVM 패턴](#mvc-mvp-mvvm-패턴)
    + [MVC 패턴](#mvc-패턴)
    + [MVP 패턴](#mvp-패턴)
    + [MVVM 패턴](#mvvm-패턴)
    + [Unity에서 MVP 선택 이유](#unity에서-mvp-선택-이유)
- [Part 3. Context 분석](#part-3-context-분석)
  * [VContainer 도입 이유](#vcontainer-도입-이유)
  * [Context란?](#context란)
  * [VContainer 핵심 API 소개](#vcontainer-핵심-api-소개)
  * [JellyFarm의 Context 계층 구조](#jellyfarm의-context-계층-구조)
    + [AppContext](#appcontext)
    + [MainContext](#maincontext)
    + [MainSheetContext, JellyModalContext, PlantModalContext](#mainsheetcontext-jellymodalcontext-plantmodalcontext)
    + [JellyContext](#jellycontext)
- [Part 4. Presenter 분석](#part-4-presenter-분석)
  * [Presenter의 역할](#presenter의-역할)
  * [UniRx 핵심 API 소개](#unirx-핵심-api-소개)
    + [Stream](#stream)
    + [Operator](#operator)
    + [Subscribe](#subscribe)
  * [JellyFarm 내 Presenter 분석](#jellyfarm-내-presenter-분석)
    + [MainSheetPresenter, JellyModalPresenter, PlantModalPresenter](#mainsheetpresenter-jellymodalpresenter-plantmodalpresenter)
    + [JellyAIPresenter, JellyClickPresenter, JellyDragPresenter, JellyGrowUpPresenter](#jellyaipresenter-jellyclickpresenter-jellydragpresenter-jellygrowuppresenter)
- [더 나아가서 고민해볼 것](#더-나아가서-고민해볼-것)

---

# 개요

## 작성 의의

VContainer + UniRx 기반 MVP 패턴 아키텍처 설계 방법을 공유한다.

## 소개

### 함께 사용된 라이브러리

1. VContainer
2. UniRx
3. UniTask
4. DOTween

### 문서 내용

Part 1. 레퍼런스 게임 분석에서는 레퍼런스 게임을 바탕으로 본 프로젝트에 구현된 기능들에 대해 소개한다.

Part 2. MVP 패턴 소개에서는 우리가 학습하고자 하는 MVP 패턴이 무엇이고 유사 패턴들과 비교하여 왜 MVP 패턴을 채택했는지 소개한다.

Part 3. Context 분석에서는 VContainer 채택 이유와 함께 프로젝트 내 Context들이 어떠한 계층으로 관리되고 있는지를 통해 프로젝트의 구조를 분석한다.

Part 4. Presenter 분석에서는 MVP 패턴에서 Presenter의 역할이 무엇인지에 대해 설명한다.

---

# Part 1. 레퍼런스 게임 분석

[유니티 볼트 기초 키우기게임 젤리팜 [VE2]](https://www.youtube.com/playlist?list=PLO-mt5Iu5TeZA0y889ZMi9wJafthif03i)

본 프로젝트는 유튜버 GoldMetal의 유니티 볼트 강좌, Jelly Farm의 기획과 리소스를 빌려 제작하였다.

볼트 기반으로 만들어진 기존 강좌와는 달리 전부 C#으로 개발되었으며 VContainer 및 UniRx 라이브러리를 기반으로 MVP 패턴을 적용시켰다.

강좌 속 모든 기능을 구현하지는 않았으며 핵심 로직 위주로만 구현되었다.

## 구현된 기능 리스트

본 프로젝트는 방치형 클릭커 게임이며,

아래 소개할 내용은 프로젝트에서 구현되어 있는 기능 목록들이다.

### 젤리

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/e1c1f80f-db16-4748-b188-76931ae9a00d)

**젤리 AI**

- Idle 상태
    
    범위 내 랜덤 시간 대기 후 Move 상태로 전환
    
- Move 상태
    
    범위 내 일정 위치로 이동 후 Idle 상태로 전환
    

**클릭커 기능**

- 클릭 시 Jelly의 AI 상태를 Idle로 전환시키고 젤리틴 및 경험치 획득

**드래그 기능**

- 젤리를 드래그 해서 옮길 수 있음
- 필드 밖으로 드래그 시 필드 내 랜덤 위치로 이동
- 우측 하단 판매 버튼으로 드래그 시 젤리 판매 가능

**젤리 성장**

- 일정 시간 마다 경험치 및 젤라틴 획득
- 경험치가 가득 찼을 시 레벨업 및 외견 변화

### 젤리팜 메인 UI

**재화 표기**

- 좌측 상단에 보유 젤라틴 표기 및 갱신
- 우측 상단에 보유 골드 표기 및 갱신

**상점 및 업그레이드 버튼**

- 좌측 하단 젤리 모양 버튼 클릭 시 젤리 구매 및 해금 UI 활성화
- 좌측 하단 망치 모양 버튼 클릭 시 업그레이드 UI 활성화

**판매 버튼**

- 우측 하단 골드 주머니 버튼으로 젤리 드래그 시 젤리 판매 가능

### 젤리 구매 및 해금 UI

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/00bec750-5c14-4a35-ba74-dde1bcf79cae)

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/756c05ed-25f3-407a-b1c5-c2a5ba18131b)

**젤리 구매**

- 젤리를 판매하여 번 골드로 젤리 구매 가능

**젤리 해금**

- 젤리를 성장시켜 번 젤라틴으로 새로운 젤리 해금 가능

### 젤리 업그레이드 UI

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/7a110db9-bfa4-41dc-9011-627e569ce5ba)

**젤리 아파트**

- 골드를 소비하여 젤리 수용량 업그레이드 가능

**젤리 꾹꾹이**

- 골드를 소비하여 클릭 시 생산량 업그레이드 가능

---

# Part 2. MVP 패턴 소개

본 튜토리얼 문서의 목표는 MVP 아키텍처에 대한 이해다.

고로 본 내용에 들어가기 전에 MVP 아키텍처란 무엇인가에 대해 먼저 소개하고 넘어가고자 한다.

## MVC, MVP, MVVM 패턴

MVC, MVP, MVVM 패턴은 웹 개발에서 넘어온 개념으로 Data와 Input/Output을 비즈니스 로직으로부터 분리하고자 하는 공통 목적을 가지고 있다.

**Model :** Data의 집합체

**View :** Input과 Output을 담당하는 객체, Button, Text, Image 등이 여기에 해당

여기서 드는 근본적인 질문은, 왜 Data와 Input/Output을 비즈니스 로직과 분리하는가이다.

객체 지향 설계의 핵심은 클래스 간에 중복 사항은 하나로 합치고 자주 변하는 것과 자주 변하지 않는 것을 분리하는 것이다.

일반적으로 Data는 특정 클래스에 종속되지 않고 다양한 클래스에서 공용으로 사용되는 경우가 많다.

즉, Data를 각 비즈니스 로직 클래스에서 관리하다보면 중복이 생기기 쉽다. 그래서 분리한다.

Input/Output의 경우 대표적으로 자주 변하는 요소에 속한다.

늘 연출은 클라이언트의 요청에 따라 이리저리 바뀌기 마련이다.

자주 변하는 것과 자주 변하지 않는 것을 같이 두면 자주 변할 필요가 없는 부분도 잦은 수정이 이뤄지기 때문에 분리한다.

결과적으로 개발자는 비즈니스 로직에만 집중할 수가 있게 된다.

### MVC 패턴

Model과 View를 로직에서 분리하여 독립적인 기능을 수행하도록 설계하는 디자인 패턴으로 관련된 유사 패턴들 중 가장 초기 버전에 해당한다.

Controller는 Model과 View를 잇는 징검다리 역할을 해주며 View를 통한 사용자의 입력에 따른 로직을 처리하고 Model을 업데이트한다.

다만, Model의 업데이트가 이루어지면 Model에서 View에 직접 접근하여 처리한다는 점에서 Model과 View 사이의 의존성을 완전히 끊어내지 못했다는 단점이 있다. 

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/99ecbb0b-8216-4b30-831c-b4365c4fd25c)

### MVP 패턴

MVC 패턴에서 파생된 디자인 패턴으로 Controller 대신 Presenter가 추가되었다.

Presenter는 Model과 View 사이의 상호작용을 담당하며 화면 갱신과 데이터 갱신을 모두 처리한다.

Unity 환경에서 가장 적합하다는 평가를 받고 있다.

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/441c5758-1c12-4f29-b916-de28389ce4f5)

### MVVM 패턴

MVP 패턴에서 파생된 디자인 패턴으로 Presenter 대신에 ViewModel이 추가되었다.

View Model은 Model 데이터를 연출을 위한 데이터로 가공하는 역할을 하며 View와 View Model 사이에 바인딩 처리가 되어있어 View Model이 갱신되면 자동으로 View도 갱신된다는 점이 특징이다.

게임 엔진으로는 언리얼이 이러한 데이터 바인딩 기능을 엔진에서 지원하고 있으며 유니티는 아쉽게도 지원하지 않고 있다.

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/2b82bddf-c9fe-4223-9c94-99b9b7af8a9e)

### Unity에서 MVP 선택 이유

커뮤니티를 둘러보면 유니티 환경에서는 MVP를 추천하는 글이 많다.

MVC는 Model과 View 사이의 의존성을 완전히 끊어내지 못해 코드 복잡성이 올라갈 여지가 높으며,

MVVM은 유니티에서 데이터 바인딩을 정식으로 지원하지 않아 구현하려면 너무 복잡해지며 오버헤드가 너무 크다는 문제가 있다.

MVP의 경우 UniRx라는 무료 라이브러리를 사용한다면 보다 간결하게 구현이 가능하다.

이러한 방식으로 Reactive 방식으로 Presenter를 설계하는 패턴을 Reactive Presenter라고 부르며 합쳐서 MV(R)P 패턴이라고 표현하기도 한다.

---

# Part 3. Context 분석

## VContainer 도입 이유

VContainer는 일본에서 만든 DI 프레임워크로 의존성 주입을 별도의 Container가 담당하여 처리해주는 프레임워크이다.

유니티에서 다른 유명한 DI 프레임워크로는 Zenject가 있다.

VContainer를 도입하게 된 이유는 다음과 같다.

1. **VContainer를 사용한다면 객체 간에 의존 정의를 뒤로 미룰 수 있다.**
    
    즉, 모듈 설계 단계에서 외부 모듈의 의존성 주입을 할 필요가 없기 때문에 모듈 간에 결합도가 사라진다.
    
    모듈 간의 결합은 각 프로젝트 별, 각 씬 별로 상황에 맞게 적절하게 세팅해주면 된다.
    

1. **뷰와 로직의 완전한 분리가 가능해진다.**
    
    MVC, MVP, MVVM 패턴에서 이야기하듯 뷰와 로직은 분리되어야 한다.
    
    문제는 유니티의 Monobehaviour는 그 자체로 C# 코드의 진입점인 동시에 보기 구성 요소의 일부라는 점이다.
    
    VContainer에서는 EntryPoint라는 새로운 Pure C# 코드에서의 진입점을 제공하며
    
    이를 통해 비즈니스 로직과 보기 구성 요소로서의 MonoBehaviour를 완전히 분리시킬 수 있게 된다.
    

1. **객체의 사용 범위를 명시적으로 지정해줄 수 있다.**
    
    VContainer에 의해 빌드되는 객체들은 LifetimeScope라는 매개체를 통해 이루어지며
    
    LifetimeScope와 LifetimeScope 사이에 계층을 정의함으로써 객체들의 사용 범위가 명시적으로 결정해줄 수 있다.
    
    즉, 객체들은 설계된 범위 내에서만 쓰이고 있다는 것을 보장 받는다.
    

다만, 애시당초 제대로 된 이해와 설계 없이 VContainer를 사용한다면 위와 같은 장점들이 하나같이 의미가 퇴색되어 버리며,

학습 코스트가 높고 낯설어 새로 합류하는 개발자마다 교육이 필요하다는 치명적 단점도 동반하고 있기에 도입 결정은 신중하게 내려야한다.

## Context란?

본 프로젝트에서는 웹 프로젝트의 네이밍 방식을 참고하여 LifetimeScope를 상속 받는 객체를 Context라고 명명하고 있다.

그리고 Context는 객체의 생성 방식을 정의하며 프로젝트를 씬보다 더 세분화된 단위로 나눠주는 매개체이다.

프로젝트 내에 어떠한 Context를 살펴보고 EntryPoint가 있는 객체부터 차근차근 접근해보면 보다 쉽게 구조를 파악할 수 있다.

## VContainer 핵심 API 소개

VContainer는 크게 Registering(등록) 및 Resolving(풀이)으로 구성되어 있다.

객체의 생성 방법을 등록하고 필요할 때 미리 등록된 방법대로 생성하여 주입한다.

1. Register<T>(Lifetime lifetime)
    
    의존성 주입 방식을 매핑하는 가장 기본 API, 지정된 타입의 객체를 요청하면 새로운 객체를 생성하여 주입해준다.
    
    지정한 Lifetime에 따라 생성된 객체의 수명이 결정된다.
    
    - Singleton : 모든 Context에서 첫번째로 생성된 객체 공유
    - Scoped : 본인 및 자식 Context에서 첫번째로 생성된 객체 공유
    - Transient : 요청 시마다 각각 새로은 객체 공유
2. RegisterInstance<TInrerface>(TInrerface instance)
    
    이미 생성된 객체를 주입하도록 매핑, Lifetime은 Singleton으로 고정된다.
    
3. RegisterComponent<TInterface>(TInrerface instance)
    
    MonoBehaviour를 상속받은 객체의 경우 Register나 RegisterInstance 대신 사용
    
4. RegisterEntryPoint<T>()
    
    VContainer에서 정의된 Entry Point LifetimeScope를 따르게 하고 싶다면 사용, 의존성 주입에 대한 매핑이 필요하다면 뒤에 .AsSelf()를 붙인다.
    

## JellyFarm의 Context 계층 구조

![image](https://github.com/jinhosung96/GoldMetal_JellyFarm/assets/42510802/b601d584-fee3-48a5-b481-1492130d56fc)

### AppContext

**AppContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // Setting 등록
    builder.RegisterInstance(UISetting);
    builder.RegisterInstance(MainSetting);

    // Manager 등록
    builder.RegisterEntryPoint<SoundManager>().AsSelf();

    // Model 등록
    JellyFarmDBModel.LoadDB("JellyPreset", "Currency", "Field", "Upgrade", "Plant");
    builder.RegisterInstance(JellyFarmDBModel);
}
```

AppContext는 VContainerSetting이라는 ScriptableObject에 의해 관리되며 씬 전환과 무관하게 모든 Context의 Root이다.

AppContext는 DB의 초기화 및 SoundManager와 다른 ScriptableObject의 초기화를 맡고 있다.

### MainContext

**MainContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // System 등록
    builder.Register<ShopSystem>(Lifetime.Singleton);
    builder.Register<UpgradeSystem>(Lifetime.Singleton);
    builder.RegisterEntryPoint<SaveSystem>().AsSelf();

    // Model 등록
    builder.RegisterInstance(MainFolderModel);
    builder.Register<CurrencyModel>(Lifetime.Singleton);
    builder.Register<FieldModel>(Lifetime.Singleton);
    builder.Register<UpgradeModel>(Lifetime.Singleton);

    // Presenter 등록
    builder.RegisterEntryPoint<BackPresenter>();
}
```

MainContext는 Main Scene의 Root에 해당되는 Context이며 씬 전반에서 관리되는 데이터를 처리하는 Model들과 주요 로직이 정의되어 있는 System을 초기화해주고 있다.

### MainSheetContext, JellyModalContext, PlantModalContext

**MainSheetContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // View 등록
    builder.RegisterInstance(View);

    // Presenter 등록
    builder.RegisterEntryPoint<MainSheetPresenter>();
}
```

**JellyModalContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // Model 등록
    builder.Register<UIModel>(Lifetime.Scoped);

    // View 등록
    builder.RegisterInstance(View);

    // Presenter 등록
    builder.RegisterEntryPoint<JellyModalPresenter>();
}
```

**PlantModalContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // View 등록
    builder.RegisterInstance(View);

    // Presenter 등록
    builder.RegisterEntryPoint<PlantModalPresenter>();
}
```

UI에 대한 Context들이며 각 UI 처리에 사용되는 Model, View, Presenter들을 초기화해주고 있다.

### JellyContext

**JellyContext.cs**

```csharp
protected override void Configure(IContainerBuilder builder)
{
    // System 등록
    builder.Register<ClickerSystem>(Lifetime.Singleton);
    builder.Register<GrowUpSystem>(Lifetime.Singleton);

    // Model 등록
    builder.RegisterInstance(Model);

    // View 등록
    builder.RegisterComponent(gameObject);
    builder.RegisterComponent(Animator);

    // Presenter 등록
    builder.RegisterEntryPoint<JellyAIPresenter>();
    builder.RegisterEntryPoint<JellyClickPresenter>();
    builder.RegisterEntryPoint<JellyDragPresenter>();
    builder.RegisterEntryPoint<JellyGrowUpPresenter>();
}
```

JellyContext는 Jelly라는 유닛 객체의 초기화를 담당하고 있다.

Presenter를 세분화해서 관리하고 있는 것을 확인할 수 있다.

---

# Part 4. Presenter 분석

## Presenter의 역할

Presenter는 MVP 패턴에서 Model과 View의 상호작용을 담당한다.

이는 화면의 갱신과 데이터의 갱신을 모두 처리한다는 것을 의미한다.

때문에 Presenter는 본 프로젝트의 대표적인 진입점이라고 볼 수 있으며,

Context를 살펴보는 것이 구조를 파악하는데에 도움이 된다면 Presenter를 살펴보는 것은 로직의 흐름을 파악하기 좋다.

Presenter가 주로 하는 일은 아래와 같다.

1. Model이 갱신되었을 때, Model 혹은 View를 갱신한다.
2. View에서 입력이 들어왔을 때, Model 혹은 View를 갱신한다.
3. Scheduler에 의해 Model 혹은 View를 갱신한다.

위와 같은 이벤트에 대한 처리는 UniRx 라이브러리를 통해 비동기로 처리된다.

## UniRx 핵심 API 소개

UniRx는 크게 Stream(스트림 발행), Operator(데이터 가공), Subscribe(구독)로 구성된다.

### Stream

UniRx에서는 다양한 Stream 생성 수단을 제공하지만 자주 쓰이는 것 위주로 소개한다.

- Subject 시리즈를 사용
    
    주로 메소드가 실행됬을 때 이를 알리기 위해 사용
    
- ReactiveProperty 시리즈를 사용
    
    데이터 수정이 일어났을 때 이를 알리기 위해 사용
    
- UniRx.Triggers 시리즈를 사용
    
    Unity의 콜백 이벤트를 Stream으로 변환
    
- uGUI 이벤트를 변환하여 사용
    
    uGUI 이벤트를 Stream으로 변환
    

### Operator

Operator 또한 정말 다양한 종류의 Operator를 지원하지만 가장 기본적이고 자주 쓰이는 것 위주로 소개한다.

- 팩토리 메서드
    - Observable.Timer/Observable.TimerFrame
        
        일정 시간 후 메시지 발행
        
    - Observable.Interval/Observable.IntervalFrame
        
        일정 간격으로 메시지 발행
        
    - Observable.EveryUpdate
        
        매 프레임 메시지 발행
        
- 메시지 필터
    - Where
        
        조건을 만족시키는 값만 통과
        
    - FirstOrDefault
        
        첫번째 값만 통과
        
    - Take
        
        주어진 개수 만큼만 통과
        
    - TakeUntil
        
        지정한 Stream이 메시지를 받을 때까지만 통과
        
    - TakeWhile
        
        조건이 false가 될 때까지만 메시지 통과
        
    - Skip
        
        처음으로 발행되는 매시지를 주어진 개수만큼 무시
        
    - SkipUntil
        
        지정한 Stream이 메시지를 받을 때까지 무시
        
    - SkipWhile
        
        조건이 true인동안 메시지 무시
        
- Observable 합성
    - Merge
        
        여러개의 Stream을 하나로 합친 새로운 Observable 생성
        
    - SelectMany
        
        메시지를 수신하면 별도의 Stream으로 전환
        
- 메시지 변환
    - Select
        
        메시지의 값을 변경
        
- 메시지 지연, 대기
    - Buffer
        1. 메시지가 일정 개수에 도달하면 모아둔 메시지를 Buffer에 담아 발행
        2. 지정한 Observable에 메시지가 도달하면 모아둔 메시지를 Buffer에 담아 발행
    - Delay/DelayFrame
        
        메시지의 발행을 지정 시간만큼 지연
        
    - Throttle/ThrottleFrame
        
        연속된 메시지가 발행 됬을 때 지정한 시간이 경과하는 동안 새로운 메시지가 발행되지 않을 때까지 기다린 후, 그동안 발행된 메시지 중 마지막 메시지 발행
        
    - ThrottleFirst/ThrottleFirstFrame
        
        메시지 발행 이후 지정한 시간동안 메시지 무시
        

### Subscribe

메시지가 발행됬을 때, 처리할 내용을 최종적으로 등록

## JellyFarm 내 Presenter 분석

### MainSheetPresenter, JellyModalPresenter, PlantModalPresenter

위 세 종류의 Presenter는 UI에 대한 Presenter들이다.

Model 갱신 및 View 갱신에 대한 이벤트를 처리해주고 있다.

**MainSheetPresenter.cs**

```csharp
void InitializeModel()
{
    // CurrencyModel의 젤라틴 및 골드 데이터가 갱신될 시 이를 UI 텍스트에 반영한다.
    currencyModel.Gelatin.Subscribe(gelatin => view.GelatinText.text = gelatin.ToString("N0")).AddTo(Context);
    currencyModel.Gold.Subscribe(gold => view.GoldText.text = gold.ToString("N0")).AddTo(Context);
}

void InitializeView()
{
    // 젤리 버튼을 클릭 시 JellySheet를 연다.
    view.JellyButton.OnClickAsObservable().Subscribe(_ =>
    {
        ModalContainer.Main.NextAsync<JellyModalContext>().Forget();
        soundManager.PlaySfx(uISetting.Button);
    }).AddTo(Context);

    // 플랜트 버튼을 클릭 시 JellySheet를 연다.
    view.PlantButton.OnClickAsObservable().Subscribe(_ =>
    {
        ModalContainer.Main.NextAsync<PlantModalContext>().Forget();
        soundManager.PlaySfx(uISetting.Button);
    }).AddTo(Context);

    // 판매 버튼 위에 손가락을 올려진 상태인지를 체크한다.
    // 젤리를 드래그한 상태로 판매 버튼 위에서 손가락을 때면 젤리를 판매한다.
    view.SellButton.OnPointerEnterAsObservable().Subscribe(_ => shopSystem.IsActiveSell = true).AddTo(Context);
    view.SellButton.OnPointerExitAsObservable().Subscribe(_ => shopSystem.IsActiveSell = false).AddTo(Context);
}
```

**JellyModalPresenter.cs**

```csharp
void InitializeModel()
{
    // 상점 Modal을 활성화 시킬 시 Jelly Preset 목록 중 가장 최근에 Unlock된 Jelly의 Page를 상점에 띄워준다.
    model.CurrentPage.Value = jellyFarmDBModel.JellyPresets.LastOrDefault(preset => preset.Value<bool>("isUnlocked")).Value<int>("id");
    // Model의 현재 Page에 대한 데이터가 갱신될 시 UI 텍스트에 반영한다.
    model.CurrentPage.Subscribe(index => view.JellyIndexText.text = $"#{(index + 1):00}").AddTo(Context.gameObject);
        
    // Model 갱신에 따른 Unlock 및 Lock Page 연출 처리 방식을 정의 한다.
    InitializeUnlock();
    InitializeLock();
}
        
void InitializeView()
{
    // Left Button을 클릭 했을 시 Page을 전환한다.
    view.LeftButton.OnClickAsObservable().Subscribe(_ =>
    {
        soundManager.PlaySfx(uISetting.Button);
        model.CurrentPage.Value--;
    }).AddTo(Context.gameObject);

    // Right Button을 클릭 했을 시 Page을 전환한다.
    view.RightButton.OnClickAsObservable().Subscribe(_ =>
    {
        soundManager.PlaySfx(uISetting.Button);
        model.CurrentPage.Value++;
    }).AddTo(Context.gameObject);

    // Buy Button을 누를 시 현재 Page의 Jelly를 구매한다.
    view.BuyButton.OnClickAsObservable().Subscribe(_ =>
    {
        shopSystem.Buy(model.CurrentPage.Value);
    }).AddTo(Context.gameObject);

    // Unlock Button을 누를 시 현재 Page의 Jelly를 해금한다.
    view.UnlockButton.OnClickAsObservable().Subscribe(_ =>
    {
        if (shopSystem.Unlock(model.CurrentPage.Value))
        {
            // 해금 후 현재 Page의 정보를 갱신한다.
            model.CurrentPage.SetValueAndForceNotify(model.CurrentPage.Value);
        }
    }).AddTo(Context.gameObject);
}

void InitializeUnlock()
{
    // 현재 Page가 Unlock된 Page일 경우 연출 정의
    model.CurrentPage.Select(index => jellyFarmDBModel.JellyPresets[index]).Where(preset => (bool)preset["isUnlocked"]).Subscribe(preset =>
    {
        view.LockFolder.SetActive(false);
        view.UnlockFolder.SetActive(true);
        view.UnlockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
        view.UnlockJellyNameText.text = preset["jellyName"].ToString();
        view.UnlockJellyCostText.text = preset["jellyCost"].ToString();
    }).AddTo(Context.gameObject);
}

void InitializeLock()
{
    // 현재 Page가 Lock된 Page일 경우 연출 정의
    model.CurrentPage.Select(index => jellyFarmDBModel.JellyPresets[index]).Where(preset => !(bool)preset["isUnlocked"]).Subscribe(preset =>
    {
        view.UnlockFolder.SetActive(false);
        view.LockFolder.SetActive(true);
        view.LockJellyImage.sprite = Resources.Load<Sprite>(preset["jellySpritePath"].ToString());
        view.LockJellyCostText.text = preset["jellyCost"].ToString();
    }).AddTo(Context.gameObject);
}
```

**PlantModalPresenter.cs**

```csharp
void InitializeModel()
{
    // UpgradeModel의 ApartmentLevel 데이터의 갱신 시 UI 갱신
    upgradeModel.ApartmentLevel.Subscribe(level =>
    {
        view.ApartmentSubText.text = $"젤리 수용량 {(level + 1) * 2}";
        if(level < upgradeModel.ApartmentMaxLevel) view.ApartmentCostText.text = $"{upgradeModel.ApartmentUpgradeCost}";
        else view.ApartmentUpgradeButton.gameObject.SetActive(false);
    }).AddTo(Context);

    // UpgradeModel의 ClickLevel 데이터의 갱신 시 UI 갱신    
    upgradeModel.ClickLevel.Subscribe(level =>
    {
        view.ClickSubText.text = $"클릭 생산량 {level + 1}";
        if(level < upgradeModel.ClickMaxLevel) view.ClickCostText.text = $"{upgradeModel.ClickUpgradeCost}";
        else view.ClickUpgradeButton.gameObject.SetActive(false);
    }).AddTo(Context);
}

void InitializeView()
{
    // Apartment Upgrade Button 클릭 시 필드의 젤리 수용량 업그레이드
    view.ApartmentUpgradeButton.OnClickAsObservable().Subscribe(_ =>
    {
        soundManager.PlaySfx(uISetting.Button);
        upgradeSystem.ApartmentUpgrade();
    }).AddTo(Context);

    // Click Upgrade Button 클릭 시 클릭 재화 수화량 업그레이드
    view.ClickUpgradeButton.OnClickAsObservable()
        .Where(_ => upgradeModel.ClickLevel.Value < upgradeModel.ClickMaxLevel)
        .Subscribe(_ =>
        {
            soundManager.PlaySfx(uISetting.Button);
            upgradeSystem.ClickUpgrade();
        })
        .AddTo(Context);
}
```

### JellyAIPresenter, JellyClickPresenter, JellyDragPresenter, JellyGrowUpPresenter

위 네 종류의 Presenter들은 유닛에 대한 Presenter들이다.

기본적으로 UI와 마찬가지로 Model 및 View의 갱신에 따른 이벤트를 정의해주는 것으로 설계가 되며

추가로 JellyGrowUpPresenter에서는 Scheduler라는 개념이 새롭게 등장한다.

Scheduler이란 쉽게 말해 어느 타이밍에 메세지를 처리하는가를 관리하는 존재이다.

일정 시간 마다, 일정 시간 후에 등의 처리는 모두 Scheduler가 담당한다.

**JellyAIPresenter.cs**

```csharp
void InitializeModel()
{
    // AI State 정의
    InitializeIdleState();
    InitializeMoveState();

    // AI 작동 시작
    model.AI.StartFsm(JellyModel.JellyState.Idle);
}

void InitializeIdleState()
{
    // Idle 상태 진입 시 처리
    // 애니메이션 갱신 및 일정 시간 후 Move 상태로 전환
    model.AI.OnBeginState(JellyModel.JellyState.Idle).Subscribe(_ =>
    {
        Context.GetComponent<Animator>().SetBool(Constants.IsWalk, false);
        disposable = Observable.Timer(TimeSpan.FromSeconds(Random.Range(0.5f, 3f)))
            .Subscribe(_ => model.AI.Transition(JellyModel.JellyState.Move))
            .AddTo(Context.gameObject);
    }).AddTo(Context.gameObject);

    // Idle 상태 종료 시 처리
    // 중복 실행 방지를 위해 스트림 종료
    model.AI.OnEndState(JellyModel.JellyState.Idle).Subscribe(_ => disposable.Dispose()).AddTo(Context.gameObject);
}

void InitializeMoveState()
{
    // Move 상태 진입 시 처리
    // 애니메이션 갱신 및 랜덤 위치로 이동 및 도착 시 Idle 상태로 전환
    model.AI.OnBeginState(JellyModel.JellyState.Move).Subscribe(_ =>
    {
        Context.GetComponent<Animator>().SetBool(Constants.IsWalk, true);
    
        // 랜덤한 위치로 일정 속도로 이동
        var randomPosition = mainSetting.RandomPositionInField;
        Context.GetComponent<SpriteRenderer>().flipX = Context.transform.position.x > randomPosition.x;
        Context.transform.DOMove(randomPosition, 1f)
            .SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() => model.AI.Transition(JellyModel.JellyState.Idle));
    }).AddTo(Context.gameObject);

    // Move 상태 종료 시 처리
    // 중복 실행 방지를 위한 스트림 종료
    model.AI.OnEndState(JellyModel.JellyState.Move).Subscribe(_ => Context.transform.DOKill()).AddTo(Context.gameObject);
}
```

**JellyClickPresenter.cs**

```csharp
void InitializeView()
{
		// 젤리 클릭 시 처리
    Context.OnMouseDownAsObservable().Subscribe(_ => clickerSystem.Click(Context)).AddTo(Context.gameObject);
}
```

**JellyDragPresenter.cs**

```csharp
void InitializeView()
{
    Vector3 delta = Vector2.zero;

    // 젤리 클릭 시 마우스 포인터와 젤리 사이의 delta 값 캐싱 및 해당 젤리의 렌더링 순서를 UI보다 앞으로 전환
    Context.OnMouseDownAsObservable().Subscribe(_ =>
    {
        delta = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - Context.transform.position.DropZ();
        Context.GetComponent<SpriteRenderer>().sortingOrder = 11;
    }).AddTo(Context.gameObject);

    // 젤리 드래그 시 마우스 포인터를 delta값을 유지한 체 따라오게 하고 Idle 상태로 강제 전환
    Context.OnMouseDragAsObservable().Subscribe(_ =>
    {
        Context.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).DropZ() - delta;
        model.AI.Transition(JellyModel.JellyState.Idle);
    }).AddTo(Context.gameObject);

    // 판매 버튼 위에서 마우스 클릭을 멈출 시 판매
    // 필드 밖일 경우 랜덤 위치에 반납
    // 필드 안일 경우 해당 위치에 반납
    Context.OnMouseUpAsObservable()
        .DelayFrame(1) // 판매 가능 여부를 판단하기 위한 시간을 번다.
        .Subscribe(_ =>
        {
            if (shopSystem.IsActiveSell) shopSystem.Sell((JellyContext)Context);
            else if (!Context.transform.position.IsInRange(mainSetting.MinRange, mainSetting.MaxRange))
                Context.transform.position = mainSetting.RandomPositionInField;
            Context.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }).AddTo(Context.gameObject);
}
```

**JellyGrowUpPresenter.cs**

```csharp
void InitializeModel()
{
    // Model의 경험치 데이터가 갱신됬을 때 최대 경험치를 충족 시켰을 시 레벨 업 처리
    model.Exp.Where(exp => exp >= maxExp).Subscribe(exp => growUpSystem.LevelUp(Context)).AddTo(Context.gameObject);
    model.Level.Subscribe(_ => growUpSystem.LevelUpEvent(Context)).AddTo(Context.gameObject);
}

void InitializeScheduler()
{
    // 1초마다 경험치 획득
    Observable.Interval(TimeSpan.FromSeconds(1))
        .TakeWhile(_ => model.Level.Value < maxLevel)
        .Where(_ => Context.gameObject.activeInHierarchy)
        .Subscribe(_ => growUpSystem.GetExpByTime(Context))
        .AddTo(Context.gameObject);
	
    // 3초마다 Gelatin 획득
    Observable.Interval(TimeSpan.FromSeconds(3f))
        .Where(_ => Context.gameObject.activeInHierarchy)
        .Subscribe(_ => growUpSystem.AutoGetGelatin(Context))
        .AddTo(Context.gameObject);
}
```

---

# 더 나아가서 고민해볼 것

MVP 패턴에서는 **데이터 관리 객체와 입출력 제어 객체, 그리고 이 둘 간의 상호작용을 담당하는 객체**로 모든 객체를 분류하고 있다.

이를 각각 Model, View, Presenter라고 부르며 클래스의 접미사로 사용된다.

이제 고민할 것은 과연 각각의 역할을 어디까지로 정의할 것이냐이다.

Presenter의 역할은 Model 및 View로부터 이벤트를 받아 Model과 View를 갱신시키는 역할이다.

이 때, Model과 View를 갱신시키는 로직을 Presenter에 포함시킬 것인가 Model과 View에 각각 정의시킬 것인가는 아직 고민 중인 내용이다.

이상적으로 봤을 때는 후자의 방식이 전자에 비해 Presenter의 역할이 명확해지고 깔끔해지는 결과가 나올 것으로 예상한다.

하지만 현실적으로 봤을 때는 생각보다 Model과 View 갱신에 복잡한 로직을 요구하는 경우가 많지 않고

괜히 학습 코스트만 높아지는 결과만 나올 수도 있다는 생각이 들었다.

이에 대해서는 아직까지도 고민 중인 내용이고 이번 예제 프로젝트에서는 전자의 방법으로 개발을 했지만 프로젝트의 규모에 따라 후자의 방법으로 개발하는 것 또한 충분히 고려해볼 필요가 있다고 판단한다.
