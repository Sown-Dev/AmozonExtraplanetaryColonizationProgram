using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Systems.Block{
    public class BaseConnector : Block, IPowerConnector{
        public int Priority{ get; } = 0;
        public Block myBlock => this;

        public PowerGrid myGrid{ get; set; }
        public bool Visited{ get; set; }
        public List<IPowerBlock> connectedBlocks{ get; set; }
        public List<IPowerConnector> connectors{ get; set; }

        public void SetVisitedRecursive(bool visited = false){
            if (Visited == visited){
                return;
            }

            Visited = visited;
            foreach (var connector in connectors){
                connector.SetVisitedRecursive(visited);
            }
        }

        public void SetGridRecursive(PowerGrid grid){
            if(grid == null){
                grid = new PowerGrid();
            }
        
            if (Visited){
                return;
            }
            else{
                Visited = true;
                if (myGrid == grid){
                    //do nothing, still recur on children
                }
                else{
                    if (myGrid == null){
                        grid.AddConnector(this);
                    }
                    else{
                        grid.MergeGrid(myGrid);
                    
                    }
                }

                foreach (var connector in connectors){
                    connector.SetGridRecursive(grid);
                }
            }
        
            foreach (var block in connectedBlocks){
                Connect(block);
                
            }
        }

        public void Connect(IPowerBlock block){
            
           

            //if ( !myGrid.HasBlock(block)){
                
                
                block.myConnector?.Disconnect(block);
                
                if(!connectedBlocks.Contains(block))
                    connectedBlocks.Add(block);
                
                myGrid.AddBlock(block);
                block.myConnector = this;
                Debug.Log("Connecting "+block.myBlock.origin);
            //}
        }
        public void Disconnect(IPowerBlock block){
            Debug.Log("Disconnecting "+block.myBlock.origin);
            if (connectedBlocks.Contains(block)){
                connectedBlocks.Remove(block);
                myGrid.RemoveBlock(block);
                block.myConnector = null;
            }
        }

        
        private void Start(){
            GetConnected();
            GetGrid();
            foreach (var block in connectedBlocks){
                Connect(block);
            }

            foreach (var pos in GetBlockCoverage()){
                if(!TerrainManager.Instance.powerClaims.ContainsKey(pos) || TerrainManager.Instance.powerClaims[pos]?.Priority>Priority){
                    TerrainManager.Instance.powerClaims.Add(pos, this);
                }
                
            }
        }

        public virtual Vector2Int[] GetBlockCoverage(){
            return TerrainManager.Instance.GetAdjacentPositions(origin, properties.size.x, properties.size.y);
        }

        public virtual Vector2Int[] GetConnectorCoverage(){
            return GetBlockCoverage();
        }


        public void GetConnected(){
            List<Block> blocksCovered = GetBlockCoverage().AsEnumerable().Select(x => TerrainManager.Instance.GetBlock(x)).ToList();
            List<Block> connectorsCovered = GetConnectorCoverage().AsEnumerable().Select(x => TerrainManager.Instance.GetBlock(x)).ToList();

            connectedBlocks = new();
            //for each block add 
            foreach (var pblock in  blocksCovered.FindAll(block => block is IPowerBlock powerBlock)
                         .ConvertAll(block => (IPowerBlock)block)){
                
                //could also just disconnect all and then only connect  if it is null
                if(pblock.myConnector != null){
                    pblock.myConnector.Disconnect(pblock);
                }   
                connectedBlocks.Add(pblock);
                
                

            }
        
            
            connectors = connectorsCovered.FindAll(block => block is IPowerConnector powerConnector)
                .ConvertAll(block => (IPowerConnector)block);
            
            
        
        }

        public void GetGrid(){
            foreach (var connector in connectors.ToList()){
                if (myGrid == null && connector.myGrid != null){
                    connector.myGrid.AddConnector(this);
                }
                else{
                    if (connector.myGrid != myGrid){

                        myGrid.MergeGrid(connector.myGrid);
                    }
                }
                connector.connectors.Add(this); //needs to be both ways
            }
            if(myGrid==null){
                myGrid = new PowerGrid();
                myGrid.AddConnector(this);
            }

       
        }

        public override bool BlockDestroy(bool dropLoot){
            if (!base.BlockDestroy(dropLoot)){
                return false;
            }
            
            
            foreach (var pos in GetBlockCoverage()){
                if(TerrainManager.Instance.powerClaims.ContainsKey(pos) && TerrainManager.Instance.powerClaims[pos] == this)
                    TerrainManager.Instance.powerClaims.Remove(pos);
            }
            
            foreach (var pblock in connectedBlocks.ToList()){
                Disconnect(pblock);
                pblock.GetConnected();
            }
       

            if (connectors.Count > 1){

                myGrid?.KillGrid();


                foreach (IPowerConnector connector in connectors){
                    connector.GetConnected();
                    connector.SetVisitedRecursive(false);

                    //connector.connectors.Remove(this);
                    if (connector.myGrid == null){
                        PowerGrid grid = new PowerGrid();
                        grid.AddConnector(connector);
                        connector.SetGridRecursive(grid);
                    }

                }
            }
            else{
                myGrid?.RemoveConnector(this);
                foreach (var connector in connectors){
                    connector.connectors.Remove(this);
                
                }
            }

            return true;
        }

        public override void Use(Unit user){
            WindowManager.Instance.CreateGridWindow(myGrid);
        }

        public void OnDrawGizmos(){
            //draw yellow to all blocks, red to all connectors
            Gizmos.color = Color.yellow;
            foreach (var block in connectedBlocks){
                Gizmos.DrawLine((Vector2)origin, (Vector2)block.myBlock.origin);
            }

            Gizmos.color = Color.red;
            foreach (var connector in connectors){

                Gizmos.DrawLine((Vector2)origin, (Vector2)connector.myBlock.origin);
                //Gizmos.DrawLine((Vector2)origin, (Vector2)connector.myBlock.origin+Vector2.up);

            }
        
            if(myGrid!=null){
                Gizmos.color = Utils.GenerateUniqueColor(myGrid);
                Gizmos.DrawSphere( (Vector2)origin, 0.25f);
            }
        }
    
   
    }
}