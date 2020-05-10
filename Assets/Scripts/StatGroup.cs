using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatGroup : MonoBehaviour
{
    [SerializeField]
    private RectTransform statParent = null;
    [SerializeField]
    private Image characterFrame = null;
    [SerializeField]
    private TMP_Text characterName = null;

    public RectTransform StatParent => statParent;
    public Image CharacterFrame => characterFrame;
    public TMP_Text CharacterName => characterName;
}