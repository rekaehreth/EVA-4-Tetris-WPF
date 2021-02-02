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
        SoundPlayer korobeiniki;
        ObservableCollection<Field> PlayingArea;
        public DelegateCommand New { get; set; }
        public DelegateCommand Save { get; set; }
        public DelegateCommand Load { get; set; }
        public DelegateCommand Pause { get; set; }
        public int FieldSize { 
            get 
            {
                return FieldSize;
            } 
            set 
            {
                FieldSize = value;
                OnPropertyChanged();
            } 
        }
        public event EventHandler<int> NewGame;
        public event EventHandler SaveGame;
        public event EventHandler LoadGame;
        public event EventHandler PauseGame;

        public void EndGame()
        {
            PlayingArea.Clear();
        }
        public void InitBoard(int size)
        {
            for (int row = 0; row < 16; ++row)
            {
                for (int column = 0; column < size; ++column)
                {
                    PlayingArea.Add(new Field("LightGray", FieldSize)); 
                    // **TODO** Color might be LightGray or Light Gray
                }
            }
        }
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
        }

        internal void UpdateTable(int size, int[,] table, List<(int, int)> currentPieceCoordinates)
        {
            for (int row = 0; row < 16; ++row)
            {
                for (int column = 0; column < size; ++column)
                {
                    switch (table[row, column])
                    {
                        case (int)PieceType.Smashboy + 1:
                            PlayingArea[row * size + column].Color = "Yellow";
                            break;
                        case (int)PieceType.Hero + 1:
                            PlayingArea[row * size + column].Color = "Blue";
                            break;
                        case (int)PieceType.Ricky + 1:
                            PlayingArea[row * size + column].Color = "Orange";
                            break;
                        case (int)PieceType.Z + 1:
                            PlayingArea[row * size + column].Color = "Green";
                            break;
                        case (int)PieceType.TeeWee + 1:
                            PlayingArea[row * size + column].Color = "Purple";
                            break;
                        default:
                            PlayingArea[row * size + column].Color = "LightGray";
                            // **TODO** Color might be LightGray or Light Gray
                            break;
                    }
                }
            }
        }
    }
}
