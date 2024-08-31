using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerColors", menuName = "RhythmBreaker/Colours/PlayerColors")]
public class PlayerColorsSO : ScriptableObject
{
    [SerializeField] private Color _mainColor;
    [SerializeField] private Color _secondaryColor;

    public Color mainColor => _mainColor;
    public Color secondaryColor => _secondaryColor;
}
