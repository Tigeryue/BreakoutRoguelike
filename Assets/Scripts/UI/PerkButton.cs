using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PerkButton : MonoBehaviour
{
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkDescription;
    public Image perkIcon;

    public void Initialize(Perk perk)
    {
        perkName.text = perk.name;
        perkDescription.text = perk.description;
        perkIcon.sprite = perk.icon;
    }
}
