using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UImanager : MonoBehaviour
{
    public Image hpManager;
    public Image mpManager;
    public float hpOriginalSize;
    public float mpOriginalSize;
    public static UImanager instance;
    public GameObject battleGo;
    public GameObject dialogGo;
    public Image characterImage;
    public Sprite[] characterSprites; // 0: Luna, 1: Nala
    public Text nameText;
    public Text contentText;
    public void ShowDialog(string content = "", string name = "")
    {
        if (string.IsNullOrEmpty(content) && string.IsNullOrEmpty(name))
        {
            dialogGo.SetActive(false);
        }
        else
        {
            dialogGo.SetActive(true);
            if (!string.IsNullOrEmpty(name))
            {
                if (name == "Luna")
                {
                    characterImage.sprite = characterSprites[0];
                }
                else if (name == "Nala")
                {
                    characterImage.sprite = characterSprites[1];
                }
                characterImage.SetNativeSize();
            }
            contentText.text = content ?? "";
            nameText.text = name ?? "";
        }
    }
    void Awake()
    {
        instance = this;


        hpOriginalSize = hpManager.rectTransform.rect.width;
        mpOriginalSize = mpManager.rectTransform.rect.width;
    }

    void Start()
    {
        if (gameManager.instance != null && gameManager.instance.playerHP > 0)
        {
            setHpValue(gameManager.instance.playerCurrentHP / (float)gameManager.instance.playerHP);
            setMpValue(gameManager.instance.playerCurrentMP / (float)gameManager.instance.playerMP);
        }
        else
        {
            setHpValue(1f);
            setMpValue(1f);
        }
    }


    public void setHpValue(float fillPercent)
    {
        hpManager.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillPercent * hpOriginalSize);
    }

    public void setMpValue(float fillPercent)
    {
        mpManager.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillPercent * mpOriginalSize);
    }

    public void RefreshStatusBars()
    {
        if (gameManager.instance == null || gameManager.instance.playerHP <= 0 || gameManager.instance.playerMP <= 0)
            return;

        setHpValue(gameManager.instance.playerCurrentHP / (float)gameManager.instance.playerHP);
        setMpValue(gameManager.instance.playerCurrentMP / (float)gameManager.instance.playerMP);
    }

    public void showorhideBattleUI(bool show)
    {
        battleGo.SetActive(show);
    }
}
