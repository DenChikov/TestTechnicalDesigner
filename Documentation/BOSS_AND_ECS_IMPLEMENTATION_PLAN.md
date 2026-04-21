# План реализации: босс по ТЗ + ECS-стиль на GameObjects

Документ описывает, **какие скрипты создать/изменить**, **порядок внедрения** и **связь с игровым ТЗ**. Стек: обычные `GameObject`, без DOTS и без сторонних плагинов.

---

## Word или Excel: что лучше для чего

| Задача | Рекомендация |
|--------|----------------|
| Текстовое ТЗ, архитектура, последовательность шагов, описание классов | **Word** (или этот `.md` в репозитории — его можно открыть в Word и сохранить как `.docx`) |
| Таблица «система → ответственность → зависимости», чеклист задач с фильтрами, оценки по дням | **Excel** |
| Версионирование и ревью в Git | **Markdown в репозитории** (основной источник правды); Word/Excel — копии для презентаций или личного трекинга |

**Практичный вариант:** держать **этот файл** как главный план в проекте; при необходимости экспортировать в Word. Для спринтов скопировать раздел «Чеклист внедрения» в Excel и проставить статусы/даты.

---

## 1. Цели геймплея (сводка ТЗ + правки)

1. Самонаводящиеся проджектайлы с **контролируемым** временем жизни и возможностью контр-игры (TTL, граница арены, выстрел игрока по снаряду — настраивается в конфиге).
2. Удар коллайдером по игроку; **отдельно** урон в точке удара и **волна**: урон игроку только если он **на земле** (grounded).
3. После удара на боссе активируются **точки уязвимости**; игрок должен их поразить (raycast/урон только по помеченным коллайдерам).
4. После **нескольких циклов** (паттерн: стрельба → слэм → уязвимость → сброс), когда выполнено условие по числу попаданий/циклов — **завершение стадии 1** (событие для фазы 2 или кат-сцены).

---

## 2. Архитектура «ECS на GameObjects»

### 2.1 Роли

- **Entity** — логический объект (босс, снаряд, игрок): стабильный id или ссылка на корневой `GameObject` + набор данных.
- **Компоненты (данные)** — поля в `struct`/`class`, `ScriptableObject` или тонкие `MonoBehaviour` только с `[SerializeField]` (без игровой логики в `Update`).
- **Системы** — классы с `Tick` / `FixedTick`; **не** размазывать логику по десятку `Update` на префабах.
- **Раннер** — один `MonoBehaviour`, который в фиксированном порядке вызывает системы.

### 2.2 Раннер и контекст (новые скрипты)

Рекомендуемая структура папок:

```text
Assets/Scripts/Core/
  GameBootstrap.cs          // опционально: только создание GameManager
  MainGameLoop.cs           // на GameManager: Update / FixedUpdate, порядок систем
  IGameSystem.cs            // интерфейсы Init / Tick
  GameBattleContext.cs      // ссылки: игрок, босс, конфиг, реестр снарядов

Assets/Scripts/Boss/Systems/
  BossPhaseSystem.cs        // FSM стадий боя, счётчики циклов и weak points
  BossProjectileSpawnSystem.cs   // когда спавнить из пула (или вызов существующего Enemy)
  BossSlamOrchestratorSystem.cs    // координация корутин/анимации слэма (тонкий слой)
  ...

Assets/Scripts/Boss/Data/
  BossFightConfig.cs        // ScriptableObject: тайминги, TTL снарядов, число циклов, уроны
  BossPhaseState.cs         // struct/class снимок состояния (enum фазы, счётчики)

Assets/Scripts/Projectiles/
  ProjectileSimulationSystem.cs    // FixedUpdate: homing, TTL, границы
  ProjectileRuntimeData.cs         // запись в списке для каждого активного снаряда
```

**`MainGameLoop.cs` (на объекте `GameManager`):**

- В `Awake`/`Start`: собрать `GameBattleContext` (найти игрока по тегу, ссылка на босса, загрузить `BossFightConfig`).
- `SerializeField` массив систем **или** ручная регистрация в правильном порядке.
- Для каждой системы: `system.Init(context)` один раз.
- `Update`: только системы, которым нужен кадровый `dt` (фазы, ввод, UI-события).
- `FixedUpdate`: снаряды, всё, что крутит `Rigidbody` согласованно с физикой.

**`GameBattleContext.cs` (пример полей):**

- `PlayerHealth`, `PlayerShoot`, `Transform playerRoot`
- `BossRoot`, ссылка на **новый** фасад `BossActor` (см. ниже) вместо размазанной логики
- `BossFightConfig config`
- `List<ProjectileRuntimeData>` или пул + синхронизация индексов
- События: `Action OnPhase1Completed` и т.д.

Так системы **не обязаны** хранить «все компоненты мира» — они получают контекст и читают только нужное.

### 2.3 Интерфейсы систем (минимум)

```csharp
// Псевдокод — в репозитории реализовать без лишних абстракций
public interface IGameSystem {
    void Init(GameBattleContext ctx);
}
public interface ITickSystem : IGameSystem {
    void Tick(float deltaTime);
}
public interface IFixedTickSystem : IGameSystem {
    void FixedTick(float fixedDeltaTime);
}
```

`MainGameLoop` хранит два списка: `ITickSystem[]`, `IFixedTickSystem[]`.

---

## 3. Рефакторинг существующих скриптов

### 3.1 `Enemy.cs` → роль «исполнитель», не «мозг»

Сейчас: FSM + стрельба + корутина руки в одном классе.

**План:**

- Вынести **решение «что делать дальше»** в `BossPhaseSystem` (переходы: Idle/Shooting/Slam/Vulnerable/Phase1End).
- Оставить в `Enemy` или переименовать в `BossActor` **императивные методы без FSM внутри**:
  - `BeginShootingPhase()`, `StopShooting()`
  - `TriggerSlam(Vector3 targetOrPlayer)` — запуск корутины/анимации
  - `SetWeakPointsActive(bool)`, ссылки на массив точек
- FSM (`StateMachine`, `IState`) можно **удалить** или оставить только как внутреннюю деталь `BossActor`, если не хотите переписывать сразу; целевое состояние — **один** источник правды состояния в `BossPhaseSystem`.

### 3.2 `bullet.cs` + префаб

- Переименовать класс в `Projectile` (опционально, для ясности).
- Убрать тяжёлую логику из `FixedUpdate` **постепенно**: либо оставить движение в компоненте, но параметры (TTL, homing) брать из `ProjectileRuntimeData`, заполняемой при спавне из `BossFightConfig`.
- Добавить деактивацию: TTL, выход за `Bounds arena`, попадание игрока, **опционально** слой «PlayerBullet» / вызов `TakeDamage` на снаряде.

### 3.3 `BossHealth.cs`

- Разделить:
  - **Глобальное HP босса** — один `BossHealth` / `IDamageable`.
  - **Weak points** — отдельные коллайдеры с компонентом `BossWeakPoint : MonoBehaviour` с полями `int index`, `bool consumedThisCycle`.
- При попадании raycast из `Shoot.cs`:
  - если `BossWeakPoint` — вызвать `context.NotifyWeakPointHit(index)` и нанести урон боссу с **множителем из конфига**;
  - иначе (тело) — малый урон или 0 в фазах без уязвимости (настраивается).
- Защита от отрицательного индекса массива `damageZone` (см. прошлый разбор): считать зоны по **явному списку weak points**, не по `damageZoneCount--` без границ.

### 3.4 `Shoot.cs` (игрок)

- После raycast различать `BossWeakPoint` vs обычный хитбокс.
- Опционально: слой для raycast по **снаряду босса** (контр-игра).

### 3.5 `WaveDamage.cs` / урон волны

- При нанесении урона игроку вызывать не «голый `TakeDamage`», а метод системы `ApplyShockwaveDamage`, внутри:
  - `if (!playerGrounded) return;`
  - иначе урон из конфига.

Источник `playerGrounded`: интерфейс `IPlayerLocomotionState` с реализацией, читающей ваш `FirstPersonController` / raycast вниз.

---

## 4. Конфигурация (`BossFightConfig` ScriptableObject)

Поля (черновик, дополнять по балансу):

| Поле | Назначение |
|------|------------|
| `projectileLifetime` | TTL самонаводящихся |
| `projectileHomingDegPerSec` / falloff | сила хоуминга, ослабление со временем |
| `maxProjectilesInAir` | лимит одновременно |
| `slamWindup`, `slamDamageRadius`, `slamDirectDamage` | удар |
| `shockwaveDamage`, `shockwaveRequiresGrounded` | волна |
| `weakPointDamageMultiplier` | баланс слабого места |
| `hitsRequiredPerCycle` | сколько точек подбить за цикл уязвимости |
| `cyclesToCompletePhase1` | сколько полных циклов до конца стадии 1 |
| `shootingDurationOrVolleyCount` | как долго/сколько волн стрельбы до слэма |
| `arenaBounds` | `Bounds` или центр+размер для деспавна снарядов |

Создать asset: `Assets/Configs/BossFight_Default.asset`.

---

## 5. Конечный автомат фазы боя (логика `BossPhaseSystem`)

Состояния (пример):

1. **Intro** (опционально)
2. **Shooting** — спавн проджектайлов по правилам конфига
3. **SlamWindup** — предупреждение, отключить уязвимые зоны
4. **SlamImpact** — урон в точке, активация волны с проверкой grounded
5. **Vulnerable** — включить weak points, ждать `hitsRequiredPerCycle`
6. **CycleComplete** — инкремент цикла; если `cyclesToCompletePhase1` достигнут → **Phase1Complete**, иначе → **Shooting**

Переходы только из **одного места** (`BossPhaseSystem.Tick`), чтобы отладка была простой.

---

## 6. Порядок вызовов в кадре (`MainGameLoop`)

Рекомендуемый порядок `Update`:

1. Ввод / камера (если вынесено в систему)
2. `BossPhaseSystem.Tick` — решения и таймеры фаз
3. UI / индикаторы (если есть отдельная система)

`FixedUpdate`:

1. `ProjectileSimulationSystem.FixedTick`
2. (опционально) проверки столкновений, если вынесены из `OnTrigger`

Соблюдать **фиксированный порядок**; документировать его в комментарии у `MainGameLoop`.

---

## 7. Чеклист внедрения (по шагам)

Копировать в Excel для статусов и дат при необходимости.

| # | Шаг | Результат |
|---|-----|-----------|
| 1 | Создать `Documentation` + конфиг `BossFightConfig` | Asset с параметрами |
| 2 | Добавить `GameBattleContext`, `MainGameLoop`, интерфейсы систем | Пустой цикл без боя |
| 3 | Подключить контекст к сцене: GameManager, ссылки на игрока/босса | Старт без ошибок |
| 4 | Вынести параметры снарядов в конфиг; ограничить TTL | Играбельная стрельба |
| 5 | Реализовать `ProjectileSimulationSystem` или адаптировать `bullet.cs` | Один источник правды движения |
| 6 | Ввести `BossWeakPoint`, изменить `Shoot` и `BossHealth` | Явные точки и множитель урона |
| 7 | Реализовать `BossPhaseSystem` + упростить `Enemy` до `BossActor` | Цикл ТЗ |
| 8 | Волна: урон только при grounded | Соответствие ТЗ |
| 9 | Событие `Phase1Complete`, заглушка фазы 2 | Расширяемость |
| 10 | Убрать `Debug.Log` из горячих путей, защита индексов, пул снарядов null-check | Стабильность |

---

## 8. Что не входит в этот план (осознанно)

- Полная реализация фазы 2 (только событие и место под второй набор правил в том же FSM).
- Миграция лутбокса (`LootBox.cs`) на тот же `MainGameLoop` — по желанию отдельным документом.
- Unity Test Framework — рекомендуется для `BossPhaseSystem` без сцены; не обязателен для первого прохода.

---

## 9. Краткий ответ «Word или Excel»

- **Полный план в тексте** — удобнее **Word** или **этот Markdown** (логика, классы, порядок).
- **Трекинг задач и таблицы** — **Excel**.
- Для проекта лучше хранить **Markdown в Git**; Word использовать для отчёта работодателю или печати.
