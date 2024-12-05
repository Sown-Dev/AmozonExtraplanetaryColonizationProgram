using System.Collections;
using System.Collections.Generic;
using Systems.Round;
using UnityEngine;
using UnityEngine.UI;

public class ContractSelectUI : MonoBehaviour
{
    
    public GameObject contractUIPrefab;
    public Transform contractList;
    public Contract[] contracts;
    public void Init(Contract[] c){
        CursorManager.Instance.OpenUI();
        contracts = c;
        foreach (Transform child in contractList){
            Destroy(child.gameObject);
        }
        for (int i = 0; i < contracts.Length; i++){
            GameObject go = Instantiate(contractUIPrefab, contractList);
            go.GetComponent<ContractUI>().Init(contracts[i]);
            int i1 = i;
            go.GetComponent<Button>().onClick.AddListener(delegate { SelectContract(i1); });
        }
    }
    
    
    public void SelectContract(int i){
        Destroy(gameObject);
        RoundManager.Instance.StartRound(contracts[i]);
        CursorManager.Instance.CloseUI();
    }
}
