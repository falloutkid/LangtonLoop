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

            InitializeCellColors();
            PrepareBitmap();
            InitializeLangtonLoops();
        }

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
        }

        private void UpdateBitmap(int[,] lives)
        {
            bitmap_.ForEach((x, y) => cell_colors_[lives[y, x]]);
        }


        /*
        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
        */

        private bool isRunning_;
        private bool isStopped_ = true;

        private async Task RunLoops()
        {
            isRunning_ = true;
            isStopped_ = false;

            DateTimeOffset startTime = DateTimeOffset.Now;
            int count = 0;

            while (isRunning_)
            {
                await Task.Run(() => langton_loops_.Update());
                UpdateBitmap(langton_loops_.Lives);

                count++;
                TimeSpan duration = DateTimeOffset.Now.Subtract(startTime);
                this.textCycleTime.Text = string.Format("{0:0.000}秒", duration.TotalMilliseconds / count / 1000.0);

//                await Task.Yield(); //画面更新の機会を与える
            }

            isStopped_ = true;
        }

        private async Task Stop()
        {
            isRunning_ = false;

            while (!isStopped_)
                await Task.Delay(100);
        }

        private async void ResetButton_Tapped(object sender, RoutedEventArgs e)
        {
            await Stop();
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
                await Stop();
            }
        }
    }
}
