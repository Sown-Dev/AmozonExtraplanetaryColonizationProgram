using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Block;
using Unity.Collections;
using UnityEngine;

public class CardinalPoleBlock : BaseConnector{

    public Vector2Int area = new(2, 2); //provided value is the topleft area
    public int poleRange = 5;

    public Material LineMaterial;
    public Color WireColor = Color.yellow;
    public int SegmentCount = 16;
    public float SagAmount = 0.25f;

    // One line renderer for each connected endpoint
    private readonly Dictionary<IPowerBlock, LineRenderer> blockWires = new();
    private readonly Dictionary<IPowerConnector, LineRenderer> connectorWires = new();

    void Update() {
        SyncWires(connectedBlocks.ToList(), blockWires, isConnector: false);
        SyncWires(connectors.ToList(), connectorWires, isConnector: true);
    }

    void SyncWires<T>(List<T> targets, Dictionary<T, LineRenderer> wireMap, bool isConnector)
        where T : class {
        foreach (var kv in wireMap.ToList()) {
            if (!targets.Contains(kv.Key)) {
                Destroy(kv.Value.gameObject);
                wireMap.Remove(kv.Key);
            }
        }

        foreach (var t in targets) {
            if (!wireMap.TryGetValue(t, out var lr)) {
                var go = new GameObject($"Wire_{t.GetHashCode()}");
                go.transform.parent = transform;
                lr = go.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.material = LineMaterial;
                lr.startColor = WireColor;
                lr.endColor = WireColor;
                lr.positionCount = SegmentCount;
                lr.widthCurve = AnimationCurve.Constant(0, 1, 0.0625f);
                wireMap[t] = lr;
            }

            UpdateWire(lr, isConnector
                ? ((IPowerConnector)t).myBlock.transform.position + Vector3.left*(2/16)+ Vector3.up * ((IPowerConnector)t).myBlock.properties.height
                : ((IPowerBlock)t).myBlock.transform.position + Vector3.up * ((IPowerBlock)t).myBlock.properties.height/2
            );
        }
    }

    void UpdateWire(LineRenderer lr, Vector3 targetPos) {
        Vector3 start = transform.position + Vector3.up * properties.height;
        for (int i = 0; i < SegmentCount; i++) {
            float t = i / (float)(SegmentCount - 1);
            Vector3 pos = Vector3.Lerp(start, targetPos, t);
            float sag = Mathf.Sin(Mathf.PI * t) * SagAmount;
            pos.y -= sag;
            lr.SetPosition(i, pos);
        }
    }

    public override void Disconnect(IPowerBlock block) {
        base.Disconnect(block);
        if (blockWires.TryGetValue(block, out var lr)) {
            Destroy(lr.gameObject);
            blockWires.Remove(block);
        }
    }

    public void DisconnectConnector(IPowerConnector connector) {
        if (connectors.Contains(connector)) {
            connectors.Remove(connector);
            if (connectorWires.TryGetValue(connector, out var lr)) {
                Destroy(lr.gameObject);
                connectorWires.Remove(connector);
            }
        }
    }
    

    //these functions sucks but don't care enough to rewrite it. doable without lists
    public override Vector2Int[] GetConnectorCoverage(){
        List<Vector2Int> coverage = new();

        foreach (Vector2Int dir in new Vector2Int[]
                     { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }){
            for (int i = 1; i < poleRange; i++){
                coverage.Add(data.origin+dir*i);
            }
        }
        return coverage.ToArray();
    }
    public override Vector2Int[] GetBlockCoverage(){
        //get all v2ints in area, ie from -2 to 2, -2 to 2, as list in one line
        List<Vector2Int> coverage = new();
        for (int i = -area.x; i <= area.x; i++){
            for (int j = -area.y; j <= area.y; j++){
                coverage.Add(data.origin +new Vector2Int(i, j));
            }
        }
        return coverage.ToArray();
    }
    

  

}