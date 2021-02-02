using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFTetris.Model;
using WPFTetris.ViewModel;

namespace WPFTetris
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        TetrisModel model;
        TetrisViewModel viewModel;
        TetrisView view;
        System.Windows.Threading.DispatcherTimer timer;
        DateTime startTime;
        SoundPlayer korobeiniki;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            model = new TetrisModel();
            viewModel = new TetrisViewModel();
            view = new TetrisView() { DataContext = viewModel };

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);

            model.UpdateTable += Model_UpdateTable;
            model.GameOver += Model_GameOver;

            viewModel.NewGame += ViewModel_NewGame;
            viewModel.SaveGame += ViewModel_SaveGame;
            viewModel.LoadGame += ViewModel_LoadGame;
            viewModel.PauseGame += ViewModel_PauseGame;

            view.KeyDown += View_KeyDown;
            view.Show();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            model.MovePieceDown();
        }
        private void ViewModel_PauseGame(object sender, EventArgs e)
        {
            model.PauseGame();
            timer.Stop();
            korobeiniki?.Stop();
            if (MessageBox.Show("Game Paused\nPress OK to continue", "Game Paused", MessageBoxButton.OK) == MessageBoxResult.OK)
            {
                model.ContinueGame();
                timer.Start();
                if (korobeiniki == null)
                {
                    korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
                    // **TODO** Resource folder might not be in the right place
                }
                korobeiniki.PlayLooping();
            }
        }
        private async void ViewModel_LoadGame(object sender, EventArgs e)
        {
            model.PauseGame();
            timer.Stop();
            korobeiniki?.Stop();
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fileDialog.FileName;
                await model.LoadGameAsync(path);
                if (MessageBox.Show("Game Loaded\nPress OK to continue", "Game Loaded", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    model.ContinueGame();
                    timer.Start();
                    if (korobeiniki == null)
                    {
                        korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
                        // **TODO** Resource folder might not be in the right place
                    }
                    korobeiniki.PlayLooping();
                }
            }
            if (MessageBox.Show("Game Paused\nPress OK to continue", "Game Paused", MessageBoxButton.OK) == MessageBoxResult.OK)
            {
                model.ContinueGame();
                timer.Start();
                if (korobeiniki == null)
                {
                    korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
                    // **TODO** Resource folder might not be in the right place
                }
                korobeiniki.PlayLooping();
            }
        }
        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            //System.Windows.Forms.
            model.PauseGame();
            timer.Stop();
            korobeiniki?.Stop();
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fileDialog.FileName;
                await model.SaveGameAsync(path);
                if (MessageBox.Show("Game Saved\nPress OK to continue game", "Game Saved", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    model.ContinueGame();
                    timer.Start();
                    if (korobeiniki == null)
                    {
                        korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
                        // **TODO** Resource folder might not be in the right place
                    }
                    korobeiniki?.PlayLooping();
                    return;
                }
            }
            if (MessageBox.Show("Game Paused\nPress OK to continue game", "Game Paused", MessageBoxButton.OK) == MessageBoxResult.OK)
            {
                model.ContinueGame();
                timer.Start();
                if (korobeiniki == null)
                {
                    korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
                    // **TODO** Resource folder might not be in the right place
                }
                korobeiniki?.PlayLooping();
                return;
            }
        }

        private void ViewModel_NewGame(object sender, int size)
        {
            model.NewGame(size);
            viewModel.InitBoard(size);
            viewModel.UpdateTable(model.Size, model.Table, model.CurrentPiece.Coordinates);
            timer.Start();
            startTime = DateTime.Now;
            korobeiniki = new SoundPlayer(@"..\Resources\Korobeiniki.wav");
            // **TODO** Resource folder might not be in the right place
            korobeiniki.PlayLooping();
        }

        private void View_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                model.MovePieceRight();
            }
            else if (e.Key == Key.Left)
            {
                model.MovePieceLeft();
            }
            else if (e.Key == Key.Down)
            {
                model.MovePieceDown();
            }
            else if (e.Key == Key.Up)
            {
                model.RotatePiece();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S)) || (Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.S)))
            {
                ViewModel_SaveGame(null, null);
            }
            else if ( (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.L)) || ( Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.L)) )
            {
                ViewModel_LoadGame(null, null);
            }
        }

        private void Model_GameOver(object sender, EventArgs e)
        {
            timer.Stop();
            korobeiniki?.Stop();
            TimeSpan elapsedTime = DateTime.Now - startTime;
            MessageBox.Show($"Game lasted for {elapsedTime.Minutes} minutes {elapsedTime.Seconds} seconds", "Game Over", MessageBoxButton.OK);
            model.EndGame();
            viewModel.EndGame();
        }

        private void Model_UpdateTable(object sender, EventArgs e)
        {
            viewModel.UpdateTable(model.Size, model.Table, model.CurrentPiece.Coordinates);
        }
    }
}
