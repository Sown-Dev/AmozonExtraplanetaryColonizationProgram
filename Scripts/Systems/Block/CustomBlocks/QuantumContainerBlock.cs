using System;
using System.Collections.Generic;
using Systems.Block;
using Systems.Items;
using UI.BlockUI;

public class QuantumContainerBlock:ContainerBlock{
    
    
    //public new QuantumContainerBlockData data => (QuantumContainerBlockData) base.data;
    public static List<Container> QuantumContainers = new List<Container>(10);

    public NumberSelector selector;

    
    void Awake(){
       selector = new NumberSelector(ChangeContainer,0,9);
    }
        
    //Called in terrain manager
    public static void InitContainers(){
        ContainerProperties properties = new ContainerProperties();
        properties.size = 16;
        properties.gridWidth = 8;
        properties.name = "Quantum Container";
        
        
        for (int i = 0; i < 10; i++){
            QuantumContainers.Add(new Container(properties));
        }
    }
    

    public override void Init(Orientation orientation){
        base.Init(orientation);
        
        selector.Priority = 21;
        
        output = QuantumContainers[selector.value];
    }

    public void ChangeContainer(){
        output = QuantumContainers[selector.value];
        BlockUIManager.Instance.CloseBlockUI();  // for refresh, but looks ugly
        BlockUIManager.Instance.GenerateBlockUI(this);
    }

    public override BlockData Save(){
        BlockData save = base.Save();
        save.data.SetInt("selector", selector.value);
        return save;
    }
    
    public override void Load(BlockData save){
        base.Load(save);
        selector.Change(save.data.GetInt("selector"));
    }
}
[Serializable]
public class QuantumContainerBlockData : ContainerBlockData{
    public NumberSelector selector;

}