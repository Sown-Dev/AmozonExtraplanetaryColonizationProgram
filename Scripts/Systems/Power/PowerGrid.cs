using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerGrid {
    public List<IPowerBlock> blocks;
    public List<IPowerConnector> connectors;

    public float capacity;
    public float storedPower;
    public int producing;
    public int productionCapacity;
    public int consuming;
    public int powerNeeded;
    public float unitDivider = 1f;
    public float storedPerSecond;
    public float usedPerSecond;

    private int id = 0;
    public PowerGrid() {
        blocks = new List<IPowerBlock>();
        connectors = new List<IPowerConnector>();
        TerrainManager.Instance.powerGrids.Add(this);
        id = Random.Range(0, 6000);
        capacity = 0f;
        storedPower = 0f;
    }

    public void GridTick() {
        producing = 0;
        productionCapacity = 0;
        powerNeeded = 0;
        consuming = 0;
        storedPerSecond = 0f;
        usedPerSecond = 0f;
        capacity = 0f;
        storedPower = 0f;

        float delta = Time.deltaTime * unitDivider;

        var batteries = blocks.OfType<IPowerBattery>().ToList();
        var producers = blocks.OfType<IPowerProducer>().OrderBy(x => x.Priority).ToList();
        var consumers = blocks.OfType<IPowerConsumer>().ToList();

        foreach (var b in batteries) {
            capacity += b.capacity;
            storedPower += b.storedPower;
        }

        foreach (var c in consumers) {
            powerNeeded += c.needed;
        }

        foreach (var p in producers) {
            productionCapacity += p.maxProduction;
            producing += p.producing;
        }

        if (producing < powerNeeded) {
            foreach (var p in producers) {
                if (!p.neededOn) {
                    p.neededOn = true;
                    producing += p.producing;
                }
            }
        }

        float availablePower = producing * delta;

        if (producing < powerNeeded) {
            float deficit = (powerNeeded - producing) * delta;
            foreach (var b in batteries) {
                float provide = Mathf.Min(deficit, b.storedPower, b.transferRate * delta);
                b.storedPower -= provide;
                storedPower -= provide;
                usedPerSecond += provide / Time.deltaTime;
                availablePower += provide;
                deficit -= provide;
                if (deficit <= 0f) break;
            }
        }
        else if (producing > powerNeeded) {
            float surplus = (producing - powerNeeded) * delta;
            foreach (var b in batteries) {
                float space = b.capacity - b.storedPower;
                float store = Mathf.Min(surplus, space, b.transferRate * delta);
                b.storedPower += store;
                storedPower += store;
                storedPerSecond += store / Time.deltaTime;
                surplus -= store;
                if (surplus <= 0f) break;
            }
        }

        foreach (var c in consumers) {
            float needed = c.needed * delta;
            if (availablePower >= needed) {
                c.providedPower = c.needed;
                availablePower -= needed;
                consuming += c.needed;
            } else {
                c.providedPower = Mathf.FloorToInt(availablePower / delta);
                consuming += c.providedPower;
                availablePower = 0f;
            }
        }
    }

    public bool HasBlock(IPowerBlock block) {
        return blocks.Contains(block);
    }

    public void AddBlock(IPowerBlock block) {
        block.myGrid?.RemoveBlock(block);
        blocks.Add(block);
        block.myGrid = this;
    }

    public void AddConnector(IPowerConnector connector) {
        connector.myGrid?.RemoveConnector(connector);
        connectors.Add(connector);
        connector.myGrid = this;
    }

    public void RemoveBlock(IPowerBlock block) {
        blocks.Remove(block);
        block.myGrid = null;
    }

    public void RemoveConnector(IPowerConnector connector) {
        connectors.Remove(connector);
        connector.myGrid = null;
    }

    public void MergeGrid(PowerGrid other) {
        if (other == null) return;
        foreach (var block in other.blocks.ToList()) {
            other.RemoveBlock(block);
            AddBlock(block);
        }
        foreach (var connector in other.connectors.ToList()) {
            other.RemoveConnector(connector);
            AddConnector(connector);
        }
        other.KillGrid();
    }

    public void KillGrid() {
        foreach (var block in blocks.ToList()) {
            RemoveBlock(block);
        }
        foreach (var connector in connectors.ToList()) {
            RemoveConnector(connector);
        }
        TerrainManager.Instance.powerGrids.Remove(this);
    }
}
