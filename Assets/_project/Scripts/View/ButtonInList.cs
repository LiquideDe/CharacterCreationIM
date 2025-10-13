using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [field: SerializeField] public Button Button;

    public void SetName(string name) => textMeshPro.text = name;
}
