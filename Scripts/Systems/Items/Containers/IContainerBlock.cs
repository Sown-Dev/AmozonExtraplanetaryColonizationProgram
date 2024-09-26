using Systems.Items;


//kind of unecessary, some might even say completely , but this is to have a distinction between blocks that are containers and general containers with no blocks attached to them
public interface IContainerBlock: IContainer{
    
}

// The most basic definition of a container
//might delete this later
public interface IContainer{
    public bool Insert(ref ItemStack s, bool simulate = false);
    public ItemStack Extract();
    
}