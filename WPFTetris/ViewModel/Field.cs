

namespace WPFTetris.ViewModel
{
    internal class Field
    {
        public string Color { get; set; }
        public int Size { get; set; }
        public Field(string color, int size)
        {
            Color = color;
            Size = size;
        }
    }
}