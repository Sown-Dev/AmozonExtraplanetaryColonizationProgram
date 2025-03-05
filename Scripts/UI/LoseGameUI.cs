
using System.Collections;
using System.Collections.Generic;
using Systems.Items;
using TMPro;
using UnityEngine;

public class LoseGameUI : MonoBehaviour
{

    [SerializeField] private TMP_Text DeathText;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private SignatureCapture signature;

    void Awake()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public void LoseScreen(int moneyEarned, List<Item> discovered)
    {
        CursorManager.Instance.OpenUI();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        signature.ClearSignature();

        DeathText.text = "Your quota was not reach in sufficent time. We've decided to move in a different direction that no longer involves your role, effective immediately.\n\n"
        + "During your employment, you made " + moneyEarned + " credits for us.\n\n"
        + "Please sign below to complete your termination";
    }

    public void ConfirmSign()
    {
        CursorManager.Instance.CloseUI();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        
        StartCoroutine(DelayedDeath());
    }
    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(1.5f);
        Player.Instance.Die();
        yield return new WaitForSeconds(2f);
        GameManager.Instance.CharacterSelectScreen();


    }


}