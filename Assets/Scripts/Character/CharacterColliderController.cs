using System;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

namespace Character
{
    public class CharacterColliderController : MonoBehaviour, IColliderController<CharacterForm>
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CapsuleCollider2D capsuleCollider;
        [SerializeField] private List<SpriteData> spriteDataList;
        [SerializeField] private Vector2 arthurStandColliderSize;
        [SerializeField] private Vector2 arthurCrouchColliderSize;
        [SerializeField] private Vector2 frogColliderSize;
        [SerializeField] private Vector2 arthurFinalClimbColliderSize;
        [SerializeField] private float flickerSpeed = 0.1f; 

        private readonly Dictionary<SpriteColliderSize, Vector2> _spriteColliderSizeDictionary =
            new Dictionary<SpriteColliderSize, Vector2>();

        public CapsuleCollider2D CapsuleCollider => capsuleCollider;
        private Vector3 _lastSpriteSize;

        void Awake()
        {
            _lastSpriteSize = spriteRenderer.bounds.size;
            _spriteColliderSizeDictionary.Add(SpriteColliderSize.ArthurStand, arthurStandColliderSize);
            _spriteColliderSizeDictionary.Add(SpriteColliderSize.ArthurCrouch, arthurCrouchColliderSize);
            _spriteColliderSizeDictionary.Add(SpriteColliderSize.Frog, frogColliderSize);
            _spriteColliderSizeDictionary.Add(SpriteColliderSize.ArthurFinalClimb, arthurFinalClimbColliderSize);
        }

        public void ChangeSprite(CharacterForm form, string spriteName)
        {
            SpriteData spriteData = spriteDataList.Find(data => data.name == spriteName);

            if (spriteData != null)
            {
                FormSpriteTriplet spriteValues = spriteData.spriteValues.Find(p => p.form == form);

                if (spriteValues != null)
                {
                    Sprite newSprite = spriteValues.sprite;

                    if (spriteRenderer.sprite == newSprite)
                    {
                        return;
                    }


                    spriteRenderer.sprite = newSprite;
                    capsuleCollider.size = _spriteColliderSizeDictionary[spriteValues.colliderSize];


                    Vector3 currentSpriteSize = spriteRenderer.bounds.size;
                    Vector3 positionAdjustment = new Vector3(
                        0,
                        (currentSpriteSize.y - _lastSpriteSize.y) / 2, // Adjust Y position based on size difference
                        0
                    );
                    transform.position += positionAdjustment;


                    _lastSpriteSize = currentSpriteSize;
                }
                else
                {
                    Debug.LogWarning($"No sprite found for form: {form} in {spriteName}");
                }
            }
            else
            {
                Debug.LogWarning($"No sprite data found with name: {spriteName}");
            }
        }
        
        public TweenCallback Flicker()
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            
            Sequence flickerSequence = DOTween.Sequence();
            
            flickerSequence.Append(spriteRenderer.DOFade(0, flickerSpeed)) 
                .Append(spriteRenderer.DOFade(1, flickerSpeed)) 
                .SetLoops(-1, LoopType.Restart);

            TweenCallback tweenCallback = () =>
            {
                flickerSequence.Kill(); 
                spriteRenderer.color = new Color(1, 1, 1, 1); 
                
            };
            
            return tweenCallback;
        }
    }
    


    [System.Serializable]
    public class FormSpriteTriplet
    {
        public CharacterForm form;
        public Sprite sprite;
        public SpriteColliderSize colliderSize;
    }

    [System.Serializable]
    public class SpriteData
    {
        public string name;
        public List<FormSpriteTriplet> spriteValues;
    }

    public enum SpriteColliderSize
    {
        ArthurStand = 1,
        ArthurCrouch = 2,
        Frog = 3,
        ArthurFinalClimb = 4
    }
}