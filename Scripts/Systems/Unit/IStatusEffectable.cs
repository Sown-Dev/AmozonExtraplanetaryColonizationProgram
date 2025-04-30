    public interface IStatusEffectable{
        public Debuffs debuffs{ get; set; }
        public void ApplyStatusEffect(StatusEffect statusEffect);
    }