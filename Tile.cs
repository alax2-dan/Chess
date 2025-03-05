using System.Windows.Forms;

namespace Chess
{
    class Tile
    {
        public PictureBox picture { get; }
        public int x { get; set; }
        public int y { get; set; }
        public Piece piece { get; set; }

        public Tile (PictureBox picture, int x, int y)
        {
            this.picture = picture;
            this.x = x;
            this.y = y;
        }
    }
}
