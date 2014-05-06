using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LangtonLoop
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int SIZE = 512;

        LangtonLoops langton_loops_;

        WriteableBitmap bitmap_;
        Color[] cell_colors_ = new Color[8];

        public MainPage()
        {
            this.InitializeComponent();

            this_dispatcher_ = Window.Current.Dispatcher;

            InitializeCellColors();
            PrepareBitmap();
            InitializeLangtonLoops();
        }

        Windows.UI.Core.CoreDispatcher this_dispatcher_;
        /*
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this_dispatcher_ = Window.Current.Dispatcher;
        }
         * */

        private void InitializeCellColors()
        {
            cell_colors_[0] = Colors.Black;
            cell_colors_[1] = Colors.Blue;
            cell_colors_[2] = Colors.Red;
            cell_colors_[3] = Colors.Green;
            cell_colors_[4] = Colors.Yellow;
            cell_colors_[5] = Colors.Magenta;
            cell_colors_[6] = Colors.White;
            cell_colors_[7] = Colors.Cyan;
        }

        private void PrepareBitmap()
        {
            bitmap_ = BitmapFactory.New(SIZE, SIZE);
            this.imageLiveSpace.Source = bitmap_;
        }

        private void InitializeLangtonLoops()
        {
            langton_loops_ = new LangtonLoops(SIZE);
            UpdateBitmap(langton_loops_.Lives);

            langton_loops_.PropertyChanged += LangtonsLoops_PropertyChanged;
        }

        private void UpdateBitmap(int[,] lives)
        {
            bitmap_.ForEach((x, y) => cell_colors_[lives[y, x]]);
        }

        DateTimeOffset start_time_;
        int step_count_;
        int draw_count_;
        private async Task RunLoops()
        {
            step_count_ = 0;
            draw_count_ = 0;

            start_time_ = DateTimeOffset.Now;

            await langton_loops_.RunLoopsAsync();
        }

        private async void ResetButton_Tapped(object sender, RoutedEventArgs e)
        {
            await langton_loops_.StopAsync();
            InitializeLangtonLoops();
        }

        private async void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            const string START = "START";
            const string STOP = "STOP";

            var button = sender as Button;
            var label = button.Content as string;
            if (label == START)
            {
                button.Content = STOP;

                await RunLoops();

                button.Content = START;
            }
            else
            {
                await langton_loops_.StopAsync();
            }
        }

        bool is_updating_;

        async void LangtonsLoops_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Lives")
            {
                step_count_++;

                if (is_updating_)
                    return;

                is_updating_ = true;

                await this_dispatcher_.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        UpdateBitmap(langton_loops_.Lives);

                        draw_count_++;
                        TimeSpan duration = DateTimeOffset.Now.Subtract(start_time_);
                        this.textCycleTime.Text = string.Format("{0:0.000}秒", duration.TotalMilliseconds / step_count_ / 1000.0);
                        int drawRate = (int)(draw_count_ * 100.0 / step_count_);
                        this.textDrawRate.Text = string.Format("{0}% ({1}/{2})", drawRate, draw_count_, step_count_);
                    }
                  );

                is_updating_ = false;
            }
        }
    }
}
