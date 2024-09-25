using Systems.Items;


//CONTAINER UTILITIES
public static class CU{
    //contains necessary static methods for moving items between all types of containers
    
    public static bool Transfer(IContainer from, IContainer to){
        ItemStack s;
        try{
            s= from.Extract();
        }catch{
            return false;
        }

        if(s == null) return false;  //if there is no item to transfer, return, since its not like we can insert it back anyways
        
        
        if (to.Insert(ref s)){
            //we actually do nothing, since we can just insert the remainder back
            return true;
        }
        //s should be null if we fully inserted it
        if( s != null){
            from.Insert(ref s);
        }
        return false;
    }
    //works
    public static bool Transfer(Slot from, IContainer to){
        from.OnChange?.Invoke();
        return to.Insert(ref from.ItemStack);
    }
    
    
    
    
}