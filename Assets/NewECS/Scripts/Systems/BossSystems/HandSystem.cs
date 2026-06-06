using System.Collections;
using UnityEngine;
using static PhaseEnum;

public class HandSystem : MonoBehaviour, ISystem
{
    private BossCombatDataComponent data;
    private HandComponent hand;
    private WaveComponent wave;
    public void Init()
    {
        data = FindAnyObjectByType<BossCombatDataComponent>();
        hand = FindAnyObjectByType<HandComponent>();
        wave = FindAnyObjectByType<WaveComponent>();

        wave.oldScale = wave.waveGo.transform.localScale;
        wave.time = wave.timeLife;
        wave.currentInnerRadius = wave.innerRadius;
    }

    public void Execute()
    {
        if (data != null && data.phaseData.Value == Phase.Hand)
        {
            if (!hand.flagHand)
            {
                StartHand();
            }
        }
        Wave();
    }

    //hand and wave attack logic
    private void StartHand()
    {
        hand.flagHand = true;
        StartCoroutine(Hand(data.player.transform.position));
    }

    private IEnumerator Hand(Vector3 pos)
    {
        pos.y = 0;
        data.transform.LookAt(data.player.transform, Vector3.up);
        float elapsed = 0;
        Vector3 oldPosition = hand.goMove.transform.position;


        hand.effectHit.transform.position = pos;
        hand.effectHit.SetActive(true);


        yield return new WaitForSeconds(hand.windUpTime);

        hand.effectHit.SetActive(false);


        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime / hand.punchTime;
            hand.goMove.transform.position = Vector3.Slerp(oldPosition,
                pos, elapsed);
            yield return null;
        }
        //code for wave
        wave.waveActivate = true;
        wave.waveGoPos = pos;

        hand.damageEnemy.SetActive(true);
        yield return new WaitForSeconds(hand.damageZoneTime);
        hand.damageEnemy.SetActive(false);

        elapsed = 0;
        Debug.Log(oldPosition);

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime / hand.returnTime;
            hand.goMove.transform.position = Vector3.Slerp(pos,
                oldPosition, elapsed);
            yield return null;
        }
        data.transform.LookAt(data.player.transform, Vector3.up);
        hand.flagHand = false;
        data.phaseData.Value = Phase.Shooting;
    }
    private void Wave()
    {
        if (!wave.waveActivate)
            return;

        if (wave.time > 0)
        {
            if (!wave.waveGo.activeSelf)
            {
                wave.waveGo.SetActive(true);
                wave.currentInnerRadius = wave.innerRadius;
                wave.currentOutsideRadius = wave.outsideRadius;
                wave.damageDealt = false;
            }

            wave.waveGo.transform.position = wave.waveGoPos;
            wave.time -= Time.deltaTime;

            // Визуал и радиусы растут синхронно
            wave.waveGo.transform.localScale += wave.scaleAdd;
            float radiusGrowth = wave.scaleAdd.x;
            wave.currentInnerRadius   += radiusGrowth;
            wave.currentOutsideRadius   += radiusGrowth;

            CheckWaveDamage();
        }
        else
        {
            wave.waveGo.SetActive(false);
            wave.waveGo.transform.localScale = wave.oldScale;
            wave.currentInnerRadius = wave.innerRadius;
            wave.currentOutsideRadius = wave.outsideRadius;
            wave.waveActivate = false;
            wave.time = wave.timeLife;
        }
    }

    private void CheckWaveDamage()
    {
        // Волна бьёт игрока только один раз за активацию, а не каждый кадр,
        // пока он находится внутри растущего кольца.
        if (wave.damageDealt) return;

        Collider[] hits = Physics.OverlapSphere(wave.waveGoPos, wave.currentOutsideRadius,
            data.playerLayerMask);

        foreach (var hit in hits)
        {
            Vector3 playerFlat = new Vector3(
                hit.transform.position.x, wave.waveGoPos.y, hit.transform.position.z);
            float dist = Vector3.Distance(playerFlat, wave.waveGoPos);

            if (dist < wave.currentInnerRadius) continue;

            if (hit.TryGetComponent<PlayerHealthDataComponent>(out var health))
            {
                // Волна бьёт только если игрок стоит на земле — в прыжке урона нет.
                // Если игрок сейчас в воздухе, не наносим урон и не помечаем волну
                // как "ударившую", чтобы она могла попасть позже, если игрок приземлится,
                // пока кольцо ещё проходит через него.
                var controller = hit.GetComponentInParent<StarterAssets.FirstPersonController>();
                if (controller != null && !controller.Grounded)
                    continue;

                health.pendingDamage += wave.waveGo.GetComponent<DamageComponent>().damage;
                wave.damageDealt = true;
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (wave == null) return;

        bool active = wave.waveActivate;
        Vector3 center = active ? wave.waveGoPos : transform.position;
        float outer = active ? wave.currentOutsideRadius : wave.outsideRadius;
        float inner = active ? wave.currentInnerRadius   : wave.innerRadius;

        // Внешнее кольцо
        Gizmos.color = active ? Color.cyan : new Color(0f, 0.6f, 1f, 0.5f);
        DrawFlatCircle(center, outer);

        // Внутреннее кольцо
        Gizmos.color = active ? new Color(0f, 1f, 1f, 0.4f) : new Color(0f, 0.6f, 1f, 0.2f);
        DrawFlatCircle(center, inner);

        // Заливка кольца линиями (видно толщину зоны урона)
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.15f);
        int seg = 32;
        for (int i = 0; i < seg; i++)
        {
            float a = i * Mathf.PI * 2f / seg;
            Vector3 dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
            Gizmos.DrawLine(center + dir * inner, center + dir * outer);
        }

        // Маркер центра удара
        if (active)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(center, 0.15f);
        }
    }

    private static void DrawFlatCircle(Vector3 center, float radius, int seg = 48)
    {
        if (radius <= 0f) return;
        float step = Mathf.PI * 2f / seg;
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = i * step;
            Vector3 next = center + new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}
