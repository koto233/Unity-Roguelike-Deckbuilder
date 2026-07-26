using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapLine : MonoBehaviour
{
    public Image LineImage { get; private set; }
    private void Awake() => LineImage = GetComponent<Image>();
}
