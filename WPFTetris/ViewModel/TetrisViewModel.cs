using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Media;
using System.Text;
using System.Threading;
using WPFTetris.Model;

namespace WPFTetris.ViewModel
{
    class TetrisViewModel : ViewModelBase
    {
        public ObservableCollection<Field> PlayingArea { get; set; }
        public DelegateCommand New { get; set; }
        public DelegateCommand Save { get; set; }
        public DelegateCommand Load { get; set; }
        public DelegateCommand Pause { get; set; }
        private int size;
        public int Size { 
            get 
            {
                return size;
            } 
            set 
            {
                size = value;
                OnPropertyChanged("Size");
            } 
        }
        public event EventHandler<int> NewGame;
        public event EventHandler SaveGame;
        public event EventHandler LoadGame;
        public event EventHandler PauseGame;
        public TetrisViewModel()
        {
            New = new DelegateCommand((object obj) => {
                int size = Convert.ToInt32(obj);
                NewGame?.Invoke(this, size);
            });
            Save = new DelegateCommand(_ =>
            {
                SaveGame?.Invoke(this, null);
            });
            Load = new DelegateCommand(_ =>
            {
                LoadGame?.Invoke(this, null);
            });
            Pause = new DelegateCommand(_ =>
            {
                PauseGame?.Invoke(this, null);
            });
            PlayingArea = new ObservableCollection<Field>();
        }
        public void EndGame()
        {
            PlayingArea?.Clear();
        }
        public void InitBoard(int size)
        {
            PlayingArea = new ObservableCollection<Field>();
            Size = size;
            for (int row = 0; row < 16; ++row)
            {
                for (int column = 0; column < size; ++column)
                {
                    PlayingArea.Add(new Field("Purple")); 
                }
            }
        }
        internal void UpdateTable(int size, int[,] table, List<(int, int)> currentPieceCoordinates)
        {
            ObservableCollection<Field> playingArea = new ObservableCollection<Field>();
            for (int row = 0; row < 16; ++row)
            {
                for (int column = 0; column < size; ++column)
                {
                    switch (table[row, column])
                    {
                        case (int)PieceType.Smashboy + 1:
                            playingArea.Add(new Field("Yellow"));
                            break;
                        case (int)PieceType.Hero + 1:
                            playingArea.Add(new Field("Blue"));
                            break;
                        case (int)PieceType.Ricky + 1:
                            playingArea.Add(new Field("Orange"));
                            break;
                        case (int)PieceType.Z + 1:
                            playingArea.Add(new Field("Green"));
                            break;
                        case (int)PieceType.TeeWee + 1:
                            playingArea.Add(new Field("Purple"));
                            break;
                        default:
                            playingArea.Add(new Field("White"));
                            break;
                    }
                }
            }
            PlayingArea = playingArea;
            OnPropertyChanged("PlayingArea");
        }
    }
}
