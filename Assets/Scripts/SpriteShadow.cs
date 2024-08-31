using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShadow : MonoBehaviour
{
    [SerializeField] protected ShadowSO _shadowSO;

    protected SpriteRenderer _caster;
    protected Transform _shadowTransform;

    protected virtual void Start()
    {
        _caster = transform.GetComponent<SpriteRenderer>();

        _shadowTransform = Instantiate(_shadowSO.shadowPrefab).transform;
        SpriteRenderer _shadowRenderer = _shadowTransform.GetComponent<SpriteRenderer>();

        _shadowRenderer.sprite = _caster.sprite;
        _shadowRenderer.color = _shadowSO.color;
        _shadowRenderer.sortingOrder = _caster.sortingOrder + _shadowSO.zOrder;

        _shadowTransform.SetParent(transform, false);
    }

    protected virtual void Update()
    {
        _shadowTransform.position = transform.position + _shadowSO.offset;
    }
}
