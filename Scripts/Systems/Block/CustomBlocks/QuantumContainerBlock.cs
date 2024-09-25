using System.Collections.Generic;
using Systems.Items;

public class QuantumContainerBlock:ContainerBlock{
    
    public static List<Container> QuantumContainers = new List<Container>(10);


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
    
    public int myIndex;


    protected override void Awake(){
        base.Awake();
        myIndex = 1;
        
        output = QuantumContainers[myIndex];
    }
}