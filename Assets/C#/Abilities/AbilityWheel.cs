using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityWheel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject wheelUI;
    [SerializeField] private Image[] abilitySlots;
    [SerializeField] private Color highlightedColor = Color.yellow; //color for highlighted slot
    [SerializeField] private Color normalColor = Color.white; //default color for slots
    [SerializeField] private GameObject[] abilitySlotOverlays;

    [Header("Gameplay Settings")]
    [SerializeField] private float timeScaleWhileOpen = 0.2f;

    private int selectedSlot = -1; //currently selected slot index
    [SerializeField] private Ability[] abilityScripts;

    private void Start() => ToggleWheel(false);
    
    private void Update()
    {
        //open/close the wheel with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleWheel(!wheelUI.activeSelf);

        //handle slot selection if the wheel is open
        if (wheelUI.activeSelf)
            HandleSelection();
    }

    private void ToggleWheel(bool toggle)
    {
        wheelUI.SetActive(toggle);
        LevelManager.instance.isUIActive = toggle;
        LevelManager.instance.OnButtonClick();
        Time.timeScale = toggle ? timeScaleWhileOpen : 1;
        UpdateAbilityUI();
    }

    private void HandleSelection()
    {
        //get mouse position
        Vector2 mousePosition = Input.mousePosition;
        Vector2 centerPosition = wheelUI.transform.position;

        //calculate the angle between the mouse and the center of the wheel
        float angle = Mathf.Atan2(mousePosition.y - centerPosition.y, -(mousePosition.x - centerPosition.x)) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        //determine the selected slot based on angle
        int newSelectedSlot = Mathf.FloorToInt(angle / (360f / abilitySlots.Length));

        //highlight the selected slot
        if (newSelectedSlot != selectedSlot)
        {
            if (selectedSlot >= 0)
                abilitySlots[selectedSlot].color = normalColor; //reset previous slot color

            selectedSlot = newSelectedSlot;
            abilitySlots[selectedSlot].color = highlightedColor; //highlight new slot
        }

        //activate selected ability on click
        if (Input.GetMouseButtonDown(0) && selectedSlot >= 0 && !IsAbilityActive(selectedSlot))
            UseAbility(selectedSlot);
    }

    private void UseAbility(int slotIndex)
    { 
        abilityScripts[slotIndex].UseAbility(); //call ability logic (e.g., abilityManager.UseAbility(slotIndex))
        ToggleWheel(false);
    }

    private void UpdateAbilityUI()
    {
        for (int i = 0; i < abilityScripts.Length; i++)
        {
            if (abilityScripts[i].IsActive())
            {
                abilitySlotOverlays[i].SetActive(true); //show overlay for active abilities
                abilitySlots[i].color = Color.gray; //disable ability slot
            }
            else
            {
                abilitySlotOverlays[i].SetActive(false); //hide overlay for inactive abilities
                abilitySlots[i].color = normalColor; //enable ability slot
            }
        }
    }

    private bool IsAbilityActive(int slotIndex) => abilityScripts[slotIndex].IsActive();
}