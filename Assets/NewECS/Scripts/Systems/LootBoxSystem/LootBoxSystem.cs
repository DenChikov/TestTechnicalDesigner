using UnityEngine;
using UnityEngine.UIElements;

public class LootBoxSystem : MonoBehaviour, ISystem
{
    private UIDocument document;
    private LootBoxComponent lootBoxComponent;
    Button buttonStart;
    Button buttonExit;
    UnityEngine.UIElements.Label label;
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

        buttonStart.clicked += LootBox;
        buttonExit.clicked += Application.Quit;


    }

    public void Execute()
    {

    }
    void LootBox()
    {
        label.text = "";
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
    }

}
