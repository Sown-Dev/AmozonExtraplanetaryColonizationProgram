using System.Collections.Generic;
using Systems.Items;
using UI.BlockUI;

public class QuantumContainerBlock:ContainerBlock{
    
    public static List<Container> QuantumContainers = new List<Container>(10);

    public NumberSelector selector;
    
    
    
        
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
    


    protected override void Awake(){
        base.Awake();
        
        selector = new NumberSelector(ChangeContainer,0,9);
        selector.Priority = 21;
        
        output = QuantumContainers[selector.value];
    }

    public void ChangeContainer(){
        output = QuantumContainers[selector.value];
        /*BlockUIManager.Instance.CloseBlockUI(); for refresh, but looks ugly
        BlockUIManager.Instance.GenerateBlockUI(this);*/
    }
}