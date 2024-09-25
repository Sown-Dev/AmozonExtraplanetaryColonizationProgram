using Systems.Block;

namespace Systems.BlockUI{
    public class DirectionSelect: IBlockUI{
        public int Priority{ get; set; } = -1;
        public bool Hidden{ get; set; }

        public Orientation inputOrientation = Orientation.Up;
        public Orientation outputOrientation = Orientation.Down;
    }
}