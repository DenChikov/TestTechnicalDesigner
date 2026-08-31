using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class LootBoxSystem : MonoBehaviour, ISystem
{
    private UIDocument document;
    private LootBoxComponent lootBoxComponent;
    Button buttonStart;
    Button buttonExit;
    UnityEngine.UIElements.Label label;
    UnityEngine.UIElements.Label labelBaseChanceText;
    UnityEngine.UIElements.Label labelHardPityThresholdText;
    public void Init()
    {
        lootBoxComponent = FindAnyObjectByType<LootBoxComponent>();
        lootBoxComponent.baseChanceNow = lootBoxComponent.baseChance;
        lootBoxComponent.hardPityThresholdNow = lootBoxComponent.hardPityThreshold;

        //ui
        document = FindAnyObjectByType<UIDocument>();
        var root = document.rootVisualElement;
        buttonStart =  root.Q<Button>("StartLoot");
        buttonExit =  root.Q<Button>("Exit");
        label = root.Q<UnityEngine.UIElements.Label>("labelResult");
        labelBaseChanceText = root.Q<UnityEngine.UIElements.Label>("baseChanceText");
        labelHardPityThresholdText = root.Q<UnityEngine.UIElements.Label>("hardPityText");

        buttonStart.clicked += LootBox;
        buttonExit.clicked += Application.Quit;


    }

    public void Execute()
    {

    }
    void LootBox()
    {
        label.text = "";
        labelBaseChanceText.text = "baseChance=" + lootBoxComponent.baseChanceNow.ToString();
        labelHardPityThresholdText.text = "grantRare="+lootBoxComponent.hardPityThresholdNow.ToString();
       lootBoxComponent.currentChance = Random.Range(0f,1f);
        if(lootBoxComponent.currentChance < lootBoxComponent.baseChanceNow ||
             lootBoxComponent.hardPityThresholdNow < 1)
        {
            label.text = "Rare drop";
            lootBoxComponent.baseChanceNow = lootBoxComponent.baseChance / 2f;
            lootBoxComponent.hardPityThresholdNow = lootBoxComponent.hardPityThreshold;
        }
        else
        {
            label.text = "Trash drop";
            lootBoxComponent.baseChanceNow += lootBoxComponent.chanceIncrement;
            lootBoxComponent.hardPityThresholdNow--;
        }

        // Анимация выдачи результата
        AnimateResult();
    }
    void AnimateResult()
    {
        bool rare = label.text == "Rare drop";

        label.RemoveFromClassList("result-rare");
        label.RemoveFromClassList("result-trash");
        label.AddToClassList(rare ? "result-rare" : "result-trash");

        label.transform.scale = new Vector3(0.5f, 0.5f, 1f);
        label.style.opacity = 0f;
        label.experimental.animation.Scale(1f, 280).Ease(Easing.OutBack);
        label.experimental.animation.Start(0f, 1f, 200, (e, v) => e.style.opacity = v);
    }

}
