using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class CorruptAbilities : MonoBehaviour
{
    [Header("Corrupted Abilities")] 
    public bool abilitiesAreOverRidden;
    [SerializeField] private ECorruptedAbilities list;
    public List<bool> isCorrupted;

    [Header("Dive")] 
    [SerializeField] private int maxTimeUseable;
    private int timesUsed;

    [Header("Wall Jump")] 
    private List<GameObject> usedWalls;

    [Header("Movement")] 
    [SerializeField] private float speedAtWhichCorruptIsTriggered;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float minCorruptDuration;
    [SerializeField] private GameObject toxicTrailPrefab;
    [SerializeField] private GameObject folderForToxicTrails;
    [SerializeField] private Transform spawnPosition;
    private float lastSpawnTime;
    private bool speedCorruptActive;
    private float timeSpeedCorruptWasActivated;

    [Header("JumpCombo")] 
    [SerializeField]private double decayFactor = 0.9f;
    private int jumpComboTimesUsed= 0;

    
    
    [Header("References")]
    //Hierarchy needs to stay the same
    private LocalSoundManager _soundManager; 
    protected StarterAssetsInputs _input;
    [HideInInspector]public CharacterController _controller;

    public enum ECorruptedAbilities
    {
        removeGround,
        baseJump,
        doubleJump,
        diveLimitUse,
        wallJump,
        movement,
        jumpCombo,
        backFlip,
    }
    
    

    // Start is called before the first frame update
    void Start()
    {
        usedWalls = new List<GameObject>();
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
        if (!abilitiesAreOverRidden) return false;
        bool result = false;
        if (!isCorrupted[(int)ECorruptedAbilities.baseJump]) return false;
        if (_input.move.y > 0) result = true;

        if (result) _soundManager.Play(SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt);


        return result;
    }
    /// <summary>
    /// Limits the number of times the dive can be used
    /// </summary>
    /// <returns>Yes if was used to often and cant be used further</returns>
    public bool CorruptDive()
    {
        if (!abilitiesAreOverRidden) return false;
        if (!isCorrupted[(int)ECorruptedAbilities.diveLimitUse]) return false;
        if (timesUsed >= maxTimeUseable)
        {
            _soundManager.Play(SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt);
            return true;
        }
        timesUsed += 1;

        return false;
    }
    /// <summary>
    /// Each wall can only be used once for the wall jump
    /// </summary>
    /// <param name="wall"></param>
    /// <returns></returns>
    public bool WallJump(GameObject wall)
    {
        if (!abilitiesAreOverRidden) return false;
        if (!isCorrupted[(int)ECorruptedAbilities.wallJump]) return false;
        if (usedWalls.Contains(wall))
        {
            _soundManager.Play(SoundManager.EAudioClips.abilityWasBlockedDueToCorrupt);
            return true;
        }
        usedWalls.Add(wall);
        //Apply Effect to wall
        return false;
    }
    
    /// <summary>
    /// Places puddles of slime on the ground when movement speed is over a certain border.
    /// Hurts the player to walk on them.
    /// </summary>
    /// <param name="speed"></param>
    public void CorruptedMovement(float speed, bool grounded)
    {
        if (!abilitiesAreOverRidden) return;
        if (!isCorrupted[(int)ECorruptedAbilities.movement]) return;
        if (speed >= speedAtWhichCorruptIsTriggered && grounded)
        {
            speedCorruptActive = true;
            timeSpeedCorruptWasActivated= Time.time;
        }
        else if (speedCorruptActive)
        {
            if (Time.time - timeSpeedCorruptWasActivated > minCorruptDuration) speedCorruptActive = false;
        }

        if (speedCorruptActive && grounded && (Time.time - lastSpawnTime > spawnInterval || lastSpawnTime == 0))
        {
            lastSpawnTime = Time.time; 
            Instantiate(toxicTrailPrefab, spawnPosition.position, 
                Quaternion.identity, folderForToxicTrails.transform);
        }
    }
    /// <summary>
    /// The more it is used the less powerful it becomes. Each time decayFActor is multiplied more times
    /// </summary>
    /// <returns>1 if nothing is changed else the jumpower for the jump</returns>
    public float JumpCombo(int currentJumpIndex)
    {
        if (!abilitiesAreOverRidden) return 1;
        if (!isCorrupted[(int)ECorruptedAbilities.jumpCombo]) return 1;
        if(currentJumpIndex == 2) timesUsed += 1;
        return (float) Math.Pow(decayFactor,timesUsed);
    }
    /// <summary>
    /// When the backflip is corrupt it can't be controlled for longer.
    /// In addition this looks the user out of using the dive to cancel it.
    /// </summary>
    /// <returns>a time of 1 for no corruption. A time of 2 seconds for corruption</returns>
    public float BackFlip()
    {
        if (!abilitiesAreOverRidden) return 1;
        if (!isCorrupted[(int)ECorruptedAbilities.backFlip]) return 1;
        return 2;
    }

    public void Reset()
    {
        timesUsed = 0;
        usedWalls.Clear();
    }

    public bool DoubleJump()
    {
        if (!isCorrupted[(int)ECorruptedAbilities.doubleJump]) return false;
        return true;
    }
}
