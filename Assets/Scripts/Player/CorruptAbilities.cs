using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class CorruptAbilities : MonoBehaviour
{
    [Header("Corrupted Abilities")] 
    [SerializeField] private ECorruptedAbilities list;
    public List<bool> isCorrupted;

    [Header("References")]
    //Hierarchy needs to stay the same
    private LocalSoundManager _soundManager; 
    protected StarterAssetsInputs _input;
    [HideInInspector]public CharacterController _controller;

    public enum ECorruptedAbilities
    {
        baseJump,
        doubleJump,
        dive,
        wallJump,
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _soundManager = transform.parent.GetComponentInChildren<LocalSoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// This jump is supposed to work when the player is jumping to the left, right or backwards
    /// </summary>
    /// <param name="wallJump"></param>
    /// <param name="jumpType"></param>
    /// <param name="overwriteJumpCurve"></param>
    /// <param name="initialJump"></param>
    /// <param name="jumpBufferWasUsed"></param>
    /// <returns>Returns true if corrupt logic is working right now </returns>
    public bool CorruptJump()
    {
        bool result = false;
        if (!isCorrupted[(int)ECorruptedAbilities.baseJump]) return false;
        if (_input.move.y > 0) result = true;

        if (result) _soundManager.Play(SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt);


        return result;
    }
}
