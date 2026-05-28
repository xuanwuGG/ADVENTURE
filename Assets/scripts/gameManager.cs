using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public AudioClip bgmClip;
    public AudioClip battleBgmClip;
    public AudioSource adse;
    public NPCDialog npc;
    public bool hasPetTheDog;
    public int candleNum;
    public int killNum;
    public bool canControlLuna;
    public bool canControlMonster;
    public int dialogInfoIndex;
    public static gameManager instance;
    public bool IsInBattle { get; private set; }
    // 在主 scene 中触发战斗的怪物引用（用于战斗结束后隐藏或处理）
    public GameObject currentEncounteredMonster;
    public Vector3 respawnOffset = new Vector3(1.5f, 0f, 0f);
    public int playerHP;
    public int playerCurrentHP;
    private float playerCurrentHPFloat;

    public int playerMP;
    public int playerCurrentMP;
    private float playerCurrentMPFloat;
    // Start is called before the first frame update
    public int monsterCurrentHP;

    public GameObject battleGo;
    private Vector3 battleGoOriginalLocalPosition;
    private Transform playerTransform;
    private Vector3 lastSafePlayerWorldPosition;

    public void playMusic(AudioClip clip){
        ApplyMasterVolume();
        if(adse.clip != clip)
        {
            adse.clip = clip;
            adse.Play();
        }
    }

    public void Start(){
        ApplyMasterVolume();
        playMusic(bgmClip);
    }

    public void playSound(AudioClip clip)
    {
        if (adse != null)
        {
            ApplyMasterVolume();
            adse.PlayOneShot(clip);
        }
    }

    private void ApplyMasterVolume()
    {
        if (adse == null)
            return;

        adse.volume = GetMasterVolume();
    }

    private float GetMasterVolume()
    {
        if (AudioManager.instance != null)
        {
            return AudioManager.instance.GetVolume();
        }

        return PlayerPrefs.GetFloat("masterVolume", 1f);
    }

    public void Update()
    {
        if (playerTransform != null && !IsInBattle && playerCurrentHP > 0)
        {
            lastSafePlayerWorldPosition = playerTransform.position;
        }

        if (!IsInBattle)
        {
            if(playerCurrentHP < playerHP)
            {
                addorDecreaseHP(Time.deltaTime * 5);
            }
            // 脱战后始终恢复 MP，不依赖于 HP 是否已满
            addorDecreaseMP(Time.deltaTime * 5);
        }
    }
    public void SetContentIndex()
    {
        npc.SetContentIndex();
    }
    void Awake()
    {
        instance = this;
        battleGoOriginalLocalPosition = battleGo != null ? battleGo.transform.localPosition : Vector3.zero;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            lastSafePlayerWorldPosition = playerTransform.position;
        }
        playerHP = 100;
        playerCurrentHP = 100;
        playerCurrentHPFloat = playerCurrentHP;
        playerMP = 100;
        playerCurrentMP = 100;
        playerCurrentMPFloat = playerCurrentMP;
        monsterCurrentHP = 100;
        canControlLuna = true;
        canControlMonster = true;
    }
    public void ShowMonsters()
    {
        monsterCon[] monsters = FindObjectsOfType<monsterCon>(true);
        Debug.Log("找到的怪物数量(含子对象/未激活): " + monsters.Length);
        foreach (monsterCon monster in monsters)
        {
            monster.gameObject.SetActive(true);
        }
    }

    public int  addorDecreaseMonsterHP(int amount)
    {
        return monsterCurrentHP = Mathf.Clamp(monsterCurrentHP + amount, 0, 10);
    }
    public void addorDecreaseHP(float amount)
    {
        playerCurrentHPFloat = Mathf.Clamp(playerCurrentHPFloat + amount, 0f, (float)playerHP);
        playerCurrentHP = Mathf.RoundToInt(playerCurrentHPFloat);
        UImanager.instance.setHpValue(playerCurrentHPFloat / (float)playerHP);
    }

    public void addorDecreaseMP(float amount)
    {
        playerCurrentMPFloat = Mathf.Clamp(playerCurrentMPFloat + amount, 0f, (float)playerMP);
        playerCurrentMP = Mathf.RoundToInt(playerCurrentMPFloat);
        UImanager.instance.setMpValue(playerCurrentMPFloat / (float)playerMP);
    }

    public bool canUseMP(int amount)
    {
        return playerCurrentMPFloat >= amount;
    }


    public bool IsHealthFull()
    {
        return playerCurrentHP >= playerHP;
    }

    public void EnterorExitBattle(bool enter = true)
    {
        IsInBattle = enter;
        canControlLuna = !enter;
        canControlMonster = !enter;
        Debug.Log("进入战斗状态: " + enter);
        if (battleGo != null)
        {
            battleGo.transform.localPosition = battleGoOriginalLocalPosition;
            battleGo.SetActive(enter);
        }
        if (enter)
        {
            // 进入战斗状态，显示战斗UI
            UImanager.instance.showorhideBattleUI(true);
            playMusic(battleBgmClip);
        }
        else
        {
            playMusic(bgmClip);
            // 退出战斗状态，隐藏战斗UI
            currentEncounteredMonster = null;
            UImanager.instance.showorhideBattleUI(false);
            if(playerCurrentHP <= 0)
            {
                RespawnPlayerNearby();
            }
        }
    }

    private void RespawnPlayerNearby()
    {
        playerCurrentHP = playerHP / 2;
        playerCurrentHPFloat = playerCurrentHP;
        playerCurrentMP = 0;
        playerCurrentMPFloat = 0f;

        if (playerTransform != null)
        {
            playerTransform.position = lastSafePlayerWorldPosition + respawnOffset;
        }

        UImanager.instance.setHpValue(playerCurrentHPFloat / (float)playerHP);
        UImanager.instance.setMpValue(playerCurrentMPFloat / (float)playerMP);
    }

    public void RemoveCurrentEncounteredMonster()
    {
        if (currentEncounteredMonster != null)
        {
            // 不再直接销毁，改为在主场景隐藏对象以保留实例用于后续处理（如掉落/检查）
            currentEncounteredMonster.SetActive(false);
            currentEncounteredMonster = null;
        }
    }
}
