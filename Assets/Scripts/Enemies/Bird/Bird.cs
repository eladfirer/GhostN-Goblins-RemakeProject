using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bird: MonoBehaviour, IEnemyEntity
{
    [Header("Up/Down Motion")]
    [SerializeField] private float moveOffsetYMax; // How far it goes up/down
    [SerializeField] private float moveOffsetYMin;

    [SerializeField] private float floatDuration;// Time to go up/down once

    [Header("Forward Motion")] [SerializeField]
    private float forwardDistance; 

    [SerializeField] private float forwardDuration;  
    
    private float _startY;
    [SerializeField] private UnityEngine.Camera mainCamera;
    private bool _isDead;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Collider2D birdCollider;
    [SerializeField] private Animator birdAnimator;
    [SerializeField] private float deathTimeAnimation;
    [SerializeField] private float timeForActivationMin;
    [SerializeField] private float timeForActivationMax;
    [SerializeField] private Vector3 position;
    [SerializeField] private float startDelay;
    public bool IsActive { get; private set; }
    

    public void Activate()
    {
        transform.position = position;
        birdAnimator.ResetTrigger("Dead");
        birdAnimator.SetBool("Move", false);
        birdAnimator.SetBool("Activate", false);
        IsActive = false;
        _isDead = false;
        GameConfig.Instance.ControlObjectsCollision(playerCollider,birdCollider,true);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        transform.DOKill();
        StopAllCoroutines();
        IsActive = false;
        _isDead = false;
        birdAnimator.ResetTrigger("Dead");
        birdAnimator.SetBool("Move", false);
        birdAnimator.SetBool("Activate", false);
        GameConfig.Instance.ControlObjectsCollision(playerCollider,birdCollider,false);
    }
    public void HitByWeapon()
    { 
        IsActive = false;
        _isDead = true;
        StopAllCoroutines();
        transform.DOKill();
        GameConfig.Instance.ControlObjectsCollision(playerCollider,birdCollider,false);
        birdAnimator.SetTrigger("Dead");
        DOVirtual.DelayedCall(deathTimeAnimation, () => { gameObject.SetActive(false); });
    }
    
    private void Update()
    {
        if (!mainCamera) return;
        
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
            
        bool isVisible = (viewPos.z > 0f)
                         && (viewPos.x >= 0f && viewPos.x <= 1f)
                         && (viewPos.y >= 0f && viewPos.y <= 1f);
            
        if (isVisible && !IsActive && !_isDead)
        {
            IsActive = true;
            StartCoroutine(StartAttack());
        }
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & GameConfig.Instance.borderLayer) != 0)
        {
            Deactivate();
        }
    }

   


    private IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(startDelay);
        _startY = transform.position.y;
        birdAnimator.SetBool("Activate", true);
        float timeForActivation = Random.Range(timeForActivationMin, timeForActivationMax);
        yield return new WaitForSeconds(timeForActivation);
        birdAnimator.SetBool("Activate", false);
        birdAnimator.SetBool("Move",true);
        transform.DOMoveX(transform.position.x + forwardDistance, forwardDuration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental); 
        
        while (IsActive)
        {
            
            yield return transform
                .DOMoveY(_startY + moveOffsetYMin, floatDuration)
                .SetEase(Ease.InOutSine)
                .WaitForCompletion();
            
            float moveOffsetY = Random.Range(moveOffsetYMin, moveOffsetYMax);
            yield return transform
                .DOMoveY(_startY - moveOffsetY, floatDuration)
                .SetEase(Ease.InOutSine)
                .WaitForCompletion();
            
        }
    }

}