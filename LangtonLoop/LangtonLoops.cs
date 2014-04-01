using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangtonLoop
{
    public class LangtonLoops : System.ComponentModel.INotifyPropertyChanged
    {
        // 世界のサイズ(正方形の一辺)
        int size_;

        // 世界(intの2次元配列)
        int[,] lives_;
        public int[,] Lives { get { return lives_; } }

        // セル・オートマトンの規則
        Rule rule_;
        bool isRuleLoaded_;

        // 初期配置(実行するときに世界の中央付近に置く)
        readonly int[,] DefaultLives ={
            {0,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
            {2,1,7,0,1,4,0,1,4,2,0,0,0,0,0},
            {2,0,2,2,2,2,2,2,0,2,0,0,0,0,0},
            {2,7,2,0,0,0,0,2,1,2,0,0,0,0,0},
            {2,1,2,0,0,0,0,2,1,2,0,0,0,0,0},
            {2,0,2,0,0,0,0,2,1,2,0,0,0,0,0},
            {2,7,2,0,0,0,0,2,1,2,0,0,0,0,0},
            {2,1,2,2,2,2,2,2,1,2,2,2,2,2,0},
            {2,0,7,1,0,7,1,0,7,1,1,1,1,1,2},
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                                      };

        // 観測結果(各方向の隣のセルの値)
        int[,] north_life_;
        int[,] east_life_;
        int[,] south_life_;
        int[,] west_life_;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;



        public LangtonLoops()
        {
            Prepare(64); //デフォルトサイズ
        }

        public LangtonLoops(int size)
        {
            Prepare(size);
        }

        private void Prepare(int size)
        {
            size_ = size;
            LoadRule();
            CreateLives();
            PrepareWatchingArray();
        }

        // Lives配列を作成し、初期状態を設定する
        private void CreateLives()
        {
            lives_ = new int[size_, size_];

            int defaultRow = (size_ - DefaultLives.GetLength(0)) / 2;
            int defaultColmn = (size_ - DefaultLives.GetLength(1)) / 2;
            for (int r0 = 0; r0 < DefaultLives.GetLength(0); r0++)
            {
                for (int c0 = 0; c0 < DefaultLives.GetLength(1); c0++)
                {
                    int r = defaultRow + r0;
                    int c = defaultColmn + c0;
                    lives_[r, c] = DefaultLives[r0, c0];
                }
            }
        }

        // 観測用の配列を生成
        private void PrepareWatchingArray()
        {
            north_life_ = new int[size_, size_];
            east_life_ = new int[size_, size_];
            south_life_ = new int[size_, size_];
            west_life_ = new int[size_, size_];
        }

        private async void LoadRule()
        {
            rule_ = new Rule();
            isRuleLoaded_ = true;
        }

        private bool isRunning_;
        private bool isStopped_ = true;

        public async Task RunLoopsAsync()
        {
            isRunning_ = true;
            isStopped_ = false;

            while (!isRuleLoaded_)
                await Task.Delay(100);

            await Task.Run(() =>
            {
                while (isRunning_)
                {
                    Update();

                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Lives"));
                }
            });

            isStopped_ = true;
        }

        public async Task StopAsync()
        {
            isRunning_ = false;

            while (!isStopped_)
                await Task.Delay(10);
        }

        /// <summary>
        /// アップデート用の関数
        /// </summary>
        private void Update()
        {
            while (!isRuleLoaded_)
                Task.Delay(100).GetAwaiter().GetResult();

            // 隣を見る (観測し終わるまで lives_ を変更してはいけない)
            Parallel.For ( 1,  size_ - 1, (r)=>
            {
                for (int c = 1; c < size_ - 1; c++)
                {
                    north_life_[r, c] = lives_[r - 1, c];
                    east_life_[r, c] = lives_[r, c + 1];
                    south_life_[r, c] = lives_[r + 1, c];
                    west_life_[r, c] = lives_[r, c - 1];
                }
            });

            // 次ステップの状態を計算して書き換える
            Parallel.For(1, size_ - 1, (r) =>
            {
                for (int c = 1; c < size_ - 1; c++)
                {
                    InputLangtonData data = new InputLangtonData(lives_[r, c], north_life_[r, c], east_life_[r, c], south_life_[r, c], west_life_[r, c]);
                    lives_[r, c] = rule_.Next(lives_[r, c], north_life_[r, c], east_life_[r, c], south_life_[r, c], west_life_[r, c]);
                }
            });
        }
    }
}
