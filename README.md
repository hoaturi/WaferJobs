# WaferJobs

## 1. 개요
- WaferJobs는 반도체 산업에 특화된 구인구직 플랫폼의 백엔드 어플리케이션입니다. 구직자와 기업 간의 매칭을 위한 다양한 기능을 제공합니다.

## 2. 배포 및 API 명세
- **[배포 링크](https://waferjobs-g2efemcdb9eff9ds.eastus2-01.azurewebsites.net/swagger/index.html)**

## 3. 기술 스택과 채택 이유

### 프레임워크 및 라이브러리
- **[.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0), [EF Core](https://docs.microsoft.com/en-us/ef/core/)**
- **[MediatR](https://github.com/jbogard/MediatR)**
  - CQRS 패턴 구현에 용이
  - Pipeline behaviour를 이용한 유효성 검사 및 예외 처리에 유용
- **[Hangfire](https://www.hangfire.io/)**
  - 백그라운드 작업 스케줄링 및 관리 용이
  - 웹 대시보드를 통한 작업 모니터링 및 관리 기능 제공
- **[Serilog](https://serilog.net/)**

### ERD & System Architecture


![waferjobs_erd](https://github.com/user-attachments/assets/6a0b9e90-b3bb-4ad7-a250-eb87901d5137)


![스크린샷 2024-10-15 014339](https://github.com/user-attachments/assets/6abc94c4-88bf-44af-916c-fdee6e396c1f)



### 데이터베이스
- **[PostgreSQL 16](https://www.postgresql.org/)**
- **[Redis](https://redis.io/)**
  - 인메모리 캐싱으로 응답 시간 개선 및 서버 부하 감소

### 인프라
- **[Docker](https://www.docker.com/)**
  - 일관된 개발 및 배포 환경 제공
  - Azure App Service와 연계하여 손쉬운 배포 자동화 구현
- **[Azure App Service](https://azure.microsoft.com/en-us/services/app-service/)**
  - 도커 허브 웹훅을 이용한 간편한 배포 자동화
  - 스케일링 및 관리의 용이성
- **[Azure Database for PostgreSQL](https://azure.microsoft.com/en-us/services/postgresql/)**
- **[Azure Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)**
  - 애플리케이션 성능 및 사용성에 대한 심층적인 인사이트 제공
  - 실시간 모니터링 및 알림 기능
- **[AWS S3](https://aws.amazon.com/s3/) / [Cloudflare R2](https://www.cloudflare.com/products/r2/)**
  - R2의 경우 타 스토리지 대비 저렴한 비용
  - CDN 자동 적용으로 빠르고 효율적인 콘텐츠 제공
- **[RedisLabs](https://redislabs.com/)**
- **[Stripe](https://stripe.com/)** (결제)
  - 높은 인지도와 신뢰성
  - 다양한 결제 옵션 및 쉬운 애플리케이션 연동
- **[SendGrid](https://sendgrid.com/)** (이메일)
  - 동적 이메일 템플릿 기능으로 맞춤형 이메일 발송 용이
  - 높은 전송률과 쉬운 API 연동
- **[Currency Beacon](https://currencybeacon.com/)** (환율 정보)
  - 타 서비스 대비 넉넉한 무료 티어 제공
  - 사용성 높은 API 제공

### CI/CD
- **[GitHub Actions](https://github.com/features/actions)**
  - YAML 설정 파일을 통한 손쉬운 CI/CD 파이프라인 구성


## 4. 주요 기능

### 관리자
- 컨퍼런스 제출 승인/거절
- 신규 비즈니스 프로필 승인/거절
  - 스팸 및 부적절한 내용 방지를 위한 검토 프로세스 구현

### 비즈니스

- 비즈니스 멤버 초대 및 수락
- 신규 비즈니스 프로필 생성 요청
  - 스팸 방지를 위한 이메일 도메인 필터링
  - 개인 이메일(Gmail, Outlook 등) 사용 제한
- 비즈니스 프로필 소유권 신청
  - 플랫폼 초기 콘텐츠 확보를 위해 크롤링으로 데이터 수집 및 채용공고/기업 프로필 생성
  - 기업 가입 시 이메일 도메인 기반 소유권 신청 프로세스를 통해 기존 프로필 관리권한 부여
### 컨퍼런스
- 새 컨퍼런스 제출 기능

### 채용공고 알림
- 채용 알림 관리 (구독, 수정, 취소)
- 백그라운드 작업을 통한 맞춤형 채용 알림 발송
  - 제공 필터: 키워드, 경력, 카테고리, 위치, 고용 형태

### 채용공고
- 채용공고 등록
- 채용공고 검색 및 조회
  - 제공 필터: 키워드, 경력, 카테고리, 위치, 고용 형태, 최소 연봉(USD 기준)
- 만료된 채용공고 재게시 결제 링크 생성
- 채용공고 지원 클릭 수 저장

### 룩업 데이터
- 채용공고 내 자주 언급되는 키워드 및 빈도 조회
- 게시된 채용공고의 위치 정보 조회
- 개인 이메일(Gmail, Outlook 등) 도메인 필터링 및 유효성 검증

### 시스템
- 실시간성이 낮은 리소스에 대한 Output cache 적용으로 서버 부하 감소
- Result 클래스를 활용한 요청 성공/실패 처리 간소화
- Azure ApplicationInsights 연동을 통한 모니터링 및 로깅

### 기타
- Stripe 웹훅 이벤트 처리
- SendGrid 이메일 발송
- 로고 이미지 업로드 및 저장
- 환율 정보 조회 및 저장

## 5. 화면 예시 (Nuxt.js 사용 개발중)
<details>
<summary>더보기</summary>

### 로그인
![스크린샷 2024-10-15 230305](https://github.com/user-attachments/assets/d44c872f-57c4-4874-a01b-21d409dcfc70)

### 회원가입
![스크린샷 2024-10-15 230251](https://github.com/user-attachments/assets/ffb281c3-3db0-4ae6-b375-4b417659fee6)

### 비즈니스 프로필 생성
![스크린샷 2024-10-15 224528](https://github.com/user-attachments/assets/9725488f-7c32-46d0-9243-7082a58a74f2)
![스크린샷 2024-10-15 224157](https://github.com/user-attachments/assets/974e2f8c-d291-4e96-9198-0d93f1b1b45d)

### 비즈니스 프로필 소유권 신청
![스크린샷 2024-10-15 224228](https://github.com/user-attachments/assets/b0353b6c-e2b8-4f6f-9bf3-df61a1468e9b)

### 채용공고 검색
![스크린샷 2024-10-15 225206](https://github.com/user-attachments/assets/f47b4d12-6fc6-4dbf-9384-19615cf0533a)
![스크린샷 2024-10-15 225348](https://github.com/user-attachments/assets/d0e4184e-639e-49c7-a6c8-cd67f3a4e7ea)

### 채용공고 등록
![스크린샷 2024-10-15 224620](https://github.com/user-attachments/assets/52e51a32-35d2-424e-ac61-0d2a7db32406)

### 채용공고 상세
![스크린샷 2024-10-15 232132](https://github.com/user-attachments/assets/bb9a4208-7ff6-469e-bbd0-e7dee264f2f0)

### 컨퍼런스 조회
![스크린샷 2024-10-15 230124](https://github.com/user-attachments/assets/dfd4a238-0d28-4a98-b06a-26c638409526)

### 컨퍼런스 제출
![스크린샷 2024-10-15 232155](https://github.com/user-attachments/assets/65fc6abd-857f-408f-9d3e-b344f24b3c0d)

</details>

## 6. 작성자
- **[한태준](https://github.com/hoaturi)**
