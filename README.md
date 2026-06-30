# Village Chef

> Unity 6 기반으로 개발한 2D 요리 & 생활 시뮬레이션 개인 프로젝트입니다.
> 단순히 게임을 만드는 것을 넘어, **확장성과 유지보수성을 고려한 구조 설계**와 **실무에서 사용하는 개발 방식**을 적용하는 것을 목표로 제작했습니다.

## 프로젝트 소개

Village Chef는 플레이어가 재료를 수집하고 요리를 제작하며 마을을 성장시키는 2D 생활 시뮬레이션 게임입니다.

이 프로젝트에서는 게임 콘텐츠 자체보다 **클라이언트 아키텍처와 시스템 설계**에 집중하여 구현했습니다.

## 개발 환경

* Unity 6
* C#
* Addressables
* ScriptableObject
* Jenkins
* Git

---

# 주요 구현 내용

## 데이터 관리

* JSON 기반 데이터 로드
* DataManager를 통한 중앙 집중식 데이터 관리
* Dictionary를 이용한 빠른 데이터 조회
* Save / Load 시스템 구현

## 로딩 시스템

* Waterfall 방식 초기화 프로세스
* 여러 초기화 작업을 순차적으로 수행
* 최소 로딩 시간 보장
* 로딩 진행률 UI 구현

## 팝업 시스템

* PopupManager를 통한 팝업 관리
* Enum 기반 자동 생성
* Stack 구조를 이용한 팝업 관리
* 새로운 팝업 추가 시 최소한의 수정만으로 확장 가능

## 요리 시스템 (Command Pattern)

요리 제작 과정을 Command Pattern으로 구현했습니다.

* 재료 소비
* 결과 아이템 지급
* 다양한 요리 동작을 독립적인 Command로 분리

이를 통해 새로운 요리를 추가하거나 기존 기능을 수정할 때 다른 코드에 영향을 최소화하도록 설계했습니다.

## 재료 수급 시스템 (Strategy Pattern)

재료 획득 방식을 Strategy Pattern으로 분리했습니다.

예를 들어

* 채집
* 낚시
* 농사

등의 획득 방식을 각각 독립적인 전략으로 구현하여 기능 확장이 쉽도록 구성했습니다.

## Addressables

* 리소스 비동기 로드
* Remote Addressables 적용
* 메모리 해제(Addressables.Release)
* 다운로드 용량 확인
* 캐싱 동작 확인

GitHub Pages를 활용하여 Remote Asset 서버도 직접 구성했습니다.

## 최적화

* Object Pool 적용
* ScrollView 재사용
* Dictionary
* Addressables 메모리 관리
* 불필요한 Instantiate 최소화

---


# 프로젝트에서 고민한 부분

단순히 기능을 구현하는 것보다,

* 데이터 중심 설계
* 새로운 콘텐츠를 쉽게 추가할 수 있는 구조
* 유지보수가 쉬운 코드

을 가장 중요하게 생각하며 개발했습니다.

---

# 시연 영상
https://youtu.be/PKQRGS8UhUE?si=yYwony-E_tlF12eh

# Portfolio
https://app.notion.com/p/3869bc0bd4e680b9a65dcd368e9a4216?source=copy_link
