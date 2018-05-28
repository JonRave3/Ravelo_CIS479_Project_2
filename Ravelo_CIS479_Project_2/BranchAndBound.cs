using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ravelo_CIS479_Project_2
{
    public class BranchAndBound
    {
        private static readonly BranchAndBound BandB = new BranchAndBound();
        protected static List<KeyValuePair<string, int>> _partial_paths;
        protected static List<KeyValuePair<string, int>> _optimal_path;
        protected static bool _done = false;
        protected static string _final;
        /*
         create event handler for other class
         */
        public event EventHandler NewPathFound0;
        public static void OnNewPathFound(EventArgs args)
        {
           BandB.NewPathFound0?.Invoke(BandB, args);
        }
        protected BranchAndBound()
        {
            _partial_paths = new List<KeyValuePair<string, int>>();
            _optimal_path = new List<KeyValuePair<string, int>>();
        }

        public static BranchAndBound getInstance()
        {
            return BandB;
        }

        public static void FindPath(string start, string finish)
        {

            _final = finish;
            var distance = 0;
            _partial_paths.Add(new KeyValuePair<string, int>(start, distance));
            KeyValuePair<string, int> path;
            do
            {
                if (!_partial_paths.Any() && !_done)
                {
                    _done = true;
                    OnNewPathFound(new NewPathFoundEventArgs() { Path = "Unsolvable", Optimal = true });
                }
                else {
                    path = _partial_paths.First();
                    distance++;
                    _optimal_path.Add(path);
                    _partial_paths.RemoveAt(0);

                    if (path.Key.Equals(_final))
                    {
                        _done = true;
                        OnNewPathFound(new NewPathFoundEventArgs() { Path = PrintOptimalPath(), Optimal = true });
                    }
                    else
                    {
                        //generate a new move and add to the front of the queue;
                        BandB.GenerateNewPaths(path.Key, distance);
                        //sort the queue by distance travelled + estimate to goal
                        SortPaths();
                        //try and save memory when the List<> doubles at capacity
                        SaveMemory();
                        //print Queue
                        OnNewPathFound(new NewPathFoundEventArgs() { Path = PrintQueue() });
                    }
                }
            } while (!_done);

            Reset();
        }

        protected virtual void GenerateNewPaths(string path, int distance)
        {

            //Jumping is better than swapping (moves 2 places as opposed to 1), so try to jump first, if not, swap right.
            string mut_path = "";
            KeyValuePair<string, int> new_path;

            for (int i = 0; i < path.Length; i++)
            {
                var position_jump = i + 2;
                var position_swap = i + 1;

                //skip spaces
                if (path[i] == '-')
                {
                    continue;
                }
                /*
                 * if this is disabled you will probably run out memory
                //see if the current character is where is should be
                else if (!NeedToMove(path, i))
                {
                    continue;
                }
                */
                //try to jump
                else if (CanJump(path, position_jump))
                {
                    mut_path = SwapRight(path, i, position_jump);
                }
                //try to swap
                else if (CanSwap(path, position_swap))
                {
                    mut_path = SwapRight(path, i, position_swap);
                }
                if (!String.IsNullOrEmpty(mut_path))
                {
                    new_path = new KeyValuePair<string, int>(mut_path, distance);
                    _partial_paths.Insert(0, new_path);

                }

            }
        }
        /// <summary>
        /// Test for invalid array accesses
        /// </summary>
        /// <param name="path"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        protected static bool ValidIndex(string path, int i)
        {
            try
            {
                var c = path[i];
                return true;
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
        }
        protected static bool CanJump(string path, int p)
        {
            if (ValidIndex(path, p))
            {
                var c = path[p];//open space
                var c_1 = path[p - 1];//neighboring characters
                if (c == '-' && c_1 != '-') return true;
            }
            return false;
        }
        protected static bool CanSwap(string path, int p)
        {

            if (ValidIndex(path, p))
            {
                var c = path[p];
                if (c == '-') return true;
            }
            return false;
        }
        protected static bool NeedToMove(string path, int p)
        {
            var c = path[p];
            var y = _final.IndexOf(c);
            if (p == y) return false;
            return true;
        }
        protected static string SwapRight(string input, int i, int swap)
        {
            char a = input[i];
            char b = input[swap];
            char[] new_str = input.ToCharArray();
            new_str[i] = b;
            new_str[swap] = a;
            return new string(new_str);
        }

        protected static void SortPaths()
        {
            _partial_paths.Sort(new KeyComparer());
        }

        protected static string PrintQueue() {
            string path_list = "";
            foreach (var x in _partial_paths)
            {
                path_list = $"({x.Value}:{x.Key}) " + path_list;
            }
            return path_list;
        }
        public static string PrintOptimalPath()
        {
            string path_list = "";
            foreach (var x in _optimal_path)
            {
                path_list = $"({x.Value}:{x.Key}) " + path_list;
            }
            return path_list;
        }

        protected static void Reset()
        {
            _optimal_path.Clear();
            _partial_paths.Clear();
            _done = false;
        }
        protected static void SaveMemory() {
            _optimal_path.TrimExcess();
            _partial_paths.TrimExcess();
        }
    }
    public class KeyComparer : IComparer<KeyValuePair<string, int>>
    {
        public int Compare(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
        {
            if (x.Value > y.Value) return 1;
            else if (x.Value < y.Value) return -1;
            else return 0;
        }

    }
}
