using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ravelo_CIS479_Project_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AStar aster;
        private BranchAndBound bb;
        private TextBox start, finish;
        private ListView astar_lv, BandB_lv;
        private Button submit;
        public delegate void NewPathFoundHandler(object sender, NewPathFoundEventArgs args);
        private static Color red = Color.FromArgb(255, 255, 0, 0);
        private static Brush redBrush = new SolidColorBrush(red);
        public MainPage()
        {
            this.InitializeComponent();
            aster = AStar.getInstance();
            bb = BranchAndBound.getInstance();
            start = this.FindName("startStr_textBox") as TextBox;
            finish = this.FindName("finalStr_textBox") as TextBox;
            astar_lv = this.FindName("astar_listView") as ListView;
            BandB_lv = this.FindName("BandB_listView") as ListView;
            submit = this.FindName("submit_Btn") as Button;
            submit.Click += submitButton_Click;
            //when new paths are found, display the paths in the pathList
            aster.NewPathFound1 += OnNewPathFound1;
            bb.NewPathFound0 += OnNewPathFound0;
        }
        

        void OnNewPathFound1(object sender, EventArgs args) {
            var path = args as NewPathFoundEventArgs;
            this.astar_lv.Items.Add(NewItem(path.Path, path.Optimal));

        }
        void OnNewPathFound0(object sender, EventArgs args)
        {
            var path = args as NewPathFoundEventArgs;
            this.BandB_lv.Items.Add(NewItem(path.Path, path.Optimal));

        }

        private void submitButton_Click(object sender, RoutedEventArgs args) {
            astar_lv.Items.Clear();
            BandB_lv.Items.Clear();
            if (ValidInput(start.Text, finish.Text))
            {
                AStar.FindPath(start.Text, finish.Text);
                if(AStar.Solvable)
                BranchAndBound.FindPath(start.Text, finish.Text);
            }
            else {
                astar_lv.Items.Add(NewItem("Unsolvable", true));
                BandB_lv.Items.Add(NewItem("Unsolvable", true));
            }
        }
        /// <summary>
        /// Check to make sure that it is solvable. not 100% as it's too hard to think of all the valid permutations
        /// </summary>
        /// <param name="s">starting string</param>
        /// <param name="f">desired ending string</param>
        /// <returns>true is solvable, false otherwise</returns>
        private static bool ValidInput(string s, string f)
        {
            //Check is starting string length is > 2
            if (s.Length < 3) return false;

            //check starting and finish string's are the same length
            if (s.Length != f.Length) {
                return false;
            }
            //check if finish string is contained in starting string
            foreach (var c in f) {
                if (!s.Contains(c)) {
                    return false;
                }
            }
            //check if start string is contained in finished string
            foreach (var c in s) {
                if (!f.Contains(c)) {
                    return false;
                }
            }
            // if they are the same length and contain the same elememnts, check instances of each character
            foreach (var c in s) {
                if (s.Count(x => x == c) != s.Count(x => x == c)) {
                    return false;
                }
            }
            //finally check that the starting string does not begin with contiguous '-'
            var num_spaces = s.Count(x => x == '-');
            string spaces = "";
            for (int i = 0; i < num_spaces; i++) {
                spaces += '-';
            }
            if (s.Contains(spaces) && s[0] == '-') {
                return false;
            }
            return true;
        }

        private static TextBlock NewItem(string msg, bool optimal) {
            var lvi = new TextBlock()
            {
                Text = msg,
                FontSize = 10,
                TextWrapping = TextWrapping.WrapWholeWords,
                Margin = new Thickness(0, 0, 0, 0),
                Padding = new Thickness(0, 0, 0, 0)
            };
            if (optimal)
                lvi.Foreground = redBrush;
            return lvi; 
        }
    }
    
}
