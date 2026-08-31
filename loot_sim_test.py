"""
Тест корректности алгоритма лутбокса.
Логика 1:1 скопирована с LootBoxSystem.LootBox() + значений из GameBalanceConfig.LootSection.
Сам код игры НЕ меняется — это внешняя проверка поведения.
"""
import random

# --- Параметры как в GameBalanceConfig.LootSection ---
BASE_CHANCE        = 0.20   # baseChance
CHANCE_INCREMENT   = 0.06   # chanceIncrement
HARD_PITY          = 8      # hardPityThreshold

N = 2_000_000

def run(seed=12345):
    random.seed(seed)
    base_now  = BASE_CHANCE
    pity_now  = HARD_PITY

    rares = 0
    forced = 0                 # сработал hard pity (а не натуральный ролл)
    cur_drought = 0            # текущая серия мусора
    max_drought = 0
    drought_hist = {}          # длина засухи -> сколько раз
    cur_streak = 0             # текущая серия редких подряд
    max_streak = 0
    streak_hist = {}
    gaps = []                  # интервалы между редкими (в спинах)
    spins_since_rare = 0

    for _ in range(N):
        roll = random.random()           # Random.Range(0,1) == [0,1)
        spins_since_rare += 1
        natural = roll < base_now
        pity_hit = pity_now < 1

        if natural or pity_hit:
            rares += 1
            if pity_hit and not natural:
                forced += 1
            # учёт засухи, что закончилась
            drought_hist[cur_drought] = drought_hist.get(cur_drought, 0) + 1
            max_drought = max(max_drought, cur_drought)
            cur_drought = 0
            # учёт серии редких
            cur_streak += 1
            max_streak = max(max_streak, cur_streak)
            gaps.append(spins_since_rare)
            spins_since_rare = 0
            # reset как в коде (текущая версия: baseChance / 2 после редкого)
            base_now = BASE_CHANCE / 2.0
            pity_now = HARD_PITY
        else:
            # закрыть серию редких
            if cur_streak > 0:
                streak_hist[cur_streak] = streak_hist.get(cur_streak, 0) + 1
                cur_streak = 0
            cur_drought += 1
            base_now += CHANCE_INCREMENT
            pity_now -= 1

    return dict(rares=rares, forced=forced, max_drought=max_drought,
                max_streak=max_streak, drought_hist=drought_hist,
                streak_hist=streak_hist, gaps=gaps)

r = run()
rate = r['rares'] / N
print(f"Спинов:                 {N:,}")
print(f"Редких выпало:          {r['rares']:,}")
print(f"ЭФФЕКТИВНЫЙ шанс:        {rate*100:.2f}%   (номинальный baseChance = {BASE_CHANCE*100:.0f}%)")
print(f"Из них по hard pity:    {r['forced']:,}  ({r['forced']/r['rares']*100:.2f}% всех редких)")
print()
print(f"МАКС засуха (мусор подряд):     {r['max_drought']}")
print(f"МАКС серия редких подряд:       {r['max_streak']}")
print()

gaps = r['gaps']
gaps_sorted = sorted(gaps)
import statistics
print(f"Интервал между редкими (спинов): min={min(gaps)} avg={statistics.mean(gaps):.2f} "
      f"median={statistics.median(gaps)} max={max(gaps)}")
# перцентили
p = lambda q: gaps_sorted[int(q*(len(gaps_sorted)-1))]
print(f"  перцентили: p50={p(0.5)}  p90={p(0.9)}  p99={p(0.99)}  p999={p(0.999)}")
print()
print("Распределение длины засухи (мусор подряд перед редким):")
for k in sorted(r['drought_hist']):
    cnt = r['drought_hist'][k]
    print(f"  {k:>2} подряд: {cnt:>9,} ({cnt/r['rares']*100:5.2f}%)")
print()
print("Серии редких подряд:")
for k in sorted(r['streak_hist']):
    cnt = r['streak_hist'][k]
    print(f"  {k:>2} подряд: {cnt:>9,}")

# Сравнение с чистым рандомом p=0.20
print()
print("--- Сравнение с ЧИСТЫМ рандомом p=0.20 (та же проблема из ТЗ) ---")
random.seed(999)
pure_max_drought = 0; cur=0; pure_max_streak=0; cs=0; pure_rare=0
for _ in range(N):
    if random.random() < 0.20:
        pure_rare+=1; cur=0; cs+=1; pure_max_streak=max(pure_max_streak,cs)
    else:
        cur+=1; pure_max_drought=max(pure_max_drought,cur); cs=0
print(f"Чистый рандом: шанс={pure_rare/N*100:.2f}%  макс засуха={pure_max_drought}  макс серия={pure_max_streak}")
