using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GravityVisualManager : MonoBehaviour
{
    [SerializeField] public GameObject _selectionSprite;
    private List<GameObject> _additionalSprites; // Used only for visualizing ZERO gravity
    public bool active { get; private set; }

    private void Awake()
    {

        _selectionSprite.SetActive(false);

        _additionalSprites = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            _additionalSprites.Add(Instantiate(_selectionSprite));

        }

        // All additional sprites are always inverted
        foreach (GameObject sprite in _additionalSprites)
        {
            SetDirectionInverted(sprite, true);
        }
    }

    public void SetActive(bool active)
    {
        this.active = active;

        // Turn of all sprites
        if (!this.active)
        {
            _selectionSprite.SetActive(false);

            foreach (GameObject sprite in _additionalSprites)
            {
                sprite.SetActive(false);
            }
        }
    }

    public void SetSelectionAnimation(GravityDirection direction, Bounds objectBounds)
    {
        SetDirectionInverted(_selectionSprite, false);
        UpdateSpriteVisibility(direction);

        Transform selectionSpriteTransform = _selectionSprite.transform;
        selectionSpriteTransform.rotation = Quaternion.identity;

        ResetSpriteTransform(_selectionSprite, objectBounds);

        // Rotations are done counter clock wise
        switch (direction)
        {
            case GravityDirection.NORTH:
                selectionSpriteTransform.RotateAround(objectBounds.center, Vector3.forward, 0);
                return;
            case GravityDirection.SOUTH:
                selectionSpriteTransform.RotateAround(objectBounds.center, Vector3.forward, 180);
                return;
            case GravityDirection.EAST:
                selectionSpriteTransform.RotateAround(objectBounds.center, Vector3.forward, 270);
                return;
            case GravityDirection.WEST:
                selectionSpriteTransform.RotateAround(objectBounds.center, Vector3.forward, 90);
                return;
            default:
                SetDirectionInverted(_selectionSprite, true);
                // Place the sprites around the center of the boudns
                for (int i = 0; i < 3; i ++)
                {
                    GameObject sprite = _additionalSprites[i];
                    sprite.transform.position = _selectionSprite.transform.position;
                    sprite.transform.rotation = _selectionSprite.transform.rotation;
                    sprite.transform.localScale = _selectionSprite.transform.localScale;

                    int rotation = i * 90 + 90;
                    sprite.transform.RotateAround(objectBounds.center, Vector3.forward, rotation);
                }
                return;
        }
    }

    private void UpdateSpriteVisibility(GravityDirection direction)
    {
        if (!active)
            return;

        _selectionSprite.SetActive(true); // All directions activate the selection sprite

        // Additional sprites are activated if gravity direction is NONE
        bool activateAdditionalSprites = direction == GravityDirection.NONE;
        foreach (GameObject sprite in _additionalSprites)
        {
            sprite.SetActive(activateAdditionalSprites);
        }
        
    }


    /* Moves the selection sprite to the center of the provided bounds of the reference sprite
     @param objectbounds  Bounds  Bounds of the reference sprite
    */
    private void ResetSpriteTransform(GameObject sprite, Bounds objectBounds)
    {
        Transform transform = sprite.transform;
        Bounds selectionSpriteBounds = sprite.GetComponent<SpriteRenderer>().bounds;

        // Get adjustments
        Vector2 offset = GetSelectionSpriteOffset(objectBounds, selectionSpriteBounds);
        Vector3 scaleMultiplier = GetSelectionSpriteScaleMultiplier(objectBounds, selectionSpriteBounds);

        // Apply adjustments
        transform.localScale = Vector3.Scale(transform.localScale, scaleMultiplier);
        transform.position = (Vector2)objectBounds.center + offset;
    }

    /* Gets the offset vector needed to place the selection sprite just above the reference sprite
     * 
     @param objectBounds  Bounds   Bounds of the reference sprite
     @params selectionSpriteBounds  Bounds  Bounds of the selection sprite

     Returns the offset as a Vector2
     */
    private Vector2 GetSelectionSpriteOffset(Bounds objectBounds, Bounds selectionSpriteBounds)
    {
        float selectionSpriteHeight = selectionSpriteBounds.size.y;
        float objectHeight = objectBounds.size.y;

        Vector2 offset = new Vector2(0, selectionSpriteHeight / 2 + objectHeight / 2);
        return offset;
    }


    /* Gets the scale multiplier that scales the selection sprite's WIDTH to the reference sprite's WIDTH
     * 
     @param objectBounds  Bounds   Bounds of the reference sprite
     @params selectionSpriteBounds  Bounds  Bounds of the selection sprite

     Returns the multiplier as a Vector3
     */
    private Vector3 GetSelectionSpriteScaleMultiplier(Bounds objectBounds, Bounds selectionSpriteBounds)
    {
        // Get WIDTHS, not heights
        float selectionSpriteWidth = selectionSpriteBounds.size.x;
        float objectWidth = objectBounds.size.x;

        Vector3 scaleMultiplier = new Vector3(objectWidth / selectionSpriteWidth, 1, 1);
        return scaleMultiplier;
    }

    private void Update()
    {

        if (_selectionSprite.activeSelf)
        {
            UpdateTimeMaterialProperty(_selectionSprite);
        }

        foreach(GameObject sprite in _additionalSprites)
        {
            UpdateTimeMaterialProperty(sprite);
        }
    }

    private void UpdateTimeMaterialProperty(GameObject sprite)
    {
        // Pass unscaled time variable to shader
        SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
        renderer.material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }

    private void SetDirectionInverted(GameObject sprite, bool inverted)
    {
        Vector2 direction = inverted ? new Vector2(0, 1) : new Vector2(0, -1);

        UpdateDirectionMaterialProperty(sprite, direction);
    }

    private void UpdateDirectionMaterialProperty(GameObject sprite, Vector2 direction)
    {
        // Pass unscaled time variable to shader
        SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
        renderer.material.SetVector("_Direction", direction);
    }
}
