using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangtonLoop
{
    /// <summary>
    /// 入力となる情報
    /// </summary>
    public class InputLangtonData
    {
        public int Center;  // 中心
        public int North;   // 北
        public int East;    // 東
        public int Sounth;  // 南
        public int West;    // 西
        public InputLangtonData(int c, int n, int e, int s, int w)
        {
            Center = c;
            North = n;
            East = e;
            Sounth = s;
            West = w;
        }
    }

    public class Rule
    {
        LangtonRules langton_rules;
        public Rule()
        {
            langton_rules = new LangtonRules();
        }

        public int Next(InputLangtonData input_data)
        {
            // Rule4 対応のため
            int key = getLangtonRule(input_data.Center, input_data.North, input_data.East , input_data.Sounth , input_data.West);
            int return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;

            key = getLangtonRule(input_data.Center, input_data.West, input_data.North, input_data.East, input_data.Sounth);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;
            
            key = getLangtonRule(input_data.Center, input_data.Sounth, input_data.West, input_data.North, input_data.East);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;
            
            key = getLangtonRule(input_data.Center, input_data.East, input_data.Sounth, input_data.West, input_data.North);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;
            return 0;
        }

        public int Next(int Center, int North, int East, int Sounth, int West)
        {
            // Rule4 対応のため
            int key = getLangtonRule(Center, North, East, Sounth, West);
            int return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;

            key = getLangtonRule(Center, West, North, East, Sounth);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;

            key = getLangtonRule(Center, Sounth, West, North, East);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;

            key = getLangtonRule(Center, East, Sounth, West, North);
            return_code = langton_rules.getRule(key);
            if (return_code != -1)
                return return_code;
            return 0;
        }

        private static int getLangtonRule(int c, int n, int e, int s, int w)
        {
            int key = c * 10000 + n * 1000 + e * 100 + s * 10 + w;
            return key;
        }
    }
}
