using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation.Background
{
    public class ToggleGroupForTalents : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Text = null;
        [field: SerializeField] public ToggleGroup ToggleGroup = null;
    }
}

