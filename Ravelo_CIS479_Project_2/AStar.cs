using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ravelo_CIS479_Project_2
{
    public class AStar : BranchAndBound
    {
        private static readonly AStar aster = new AStar();
        public static bool Solvable;
        public event EventHandler NewPathFound1;

        private AStar() {
            Solvable = true;
        }

        new public static AStar getInstance() {
            return aster;
        }
        new public static void OnNewPathFound(EventArgs args)
        {
            aster.NewPathFound1?.Invoke(aster, args);
        }
        new public static void FindPath(string start, string finish) {
            
            _final = finish;
            var distance = 0;
            _partial_paths.Add(new KeyValuePair<string, int>(start, distance + GetEstimate(start)));
            KeyValuePair<string, int> path;
            do {
                if (!_partial_paths.Any() && !_done)
                {
                    Solvable = false;
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
                        //print optimal path
                        OnNewPathFound(new NewPathFoundEventArgs() { Path = PrintOptimalPath(), Optimal = true });
                    }
                    else
                    {
                        //generate a new move and add to the front of the queue;
                        aster.GenerateNewPaths(path.Key, distance);
                        //sort the queue by distance travelled + estimate to goal
                        SortPaths();
                        //remove redundant paths
                        CleanPaths();
                        //SaveMemory();
                        //print Queue
                        OnNewPathFound(new NewPathFoundEventArgs() { Path = PrintQueue() });
                    }
                }
                
            } while (!_done);
            
            Reset();
        }

        protected override void GenerateNewPaths(string path, int distance) {
            
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
                //see if the current character is where is should be
                else if (!NeedToMove(path, i)) {
                    continue;
                }
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
                if (!String.IsNullOrEmpty(mut_path)) {
                    new_path = new KeyValuePair<string, int>(mut_path, distance + GetEstimate(mut_path));
                    _partial_paths.Insert(0, new_path);

                }

            }
        }
        private static int GetEstimate(string input)
        {
            /*
             * the sum of the distances foreach character in the string from their starting point to where they need to be minus 1
             * e.g. art--- => ---tar, 4(a) + 4(r) + 1(t) - 1 = 8
            */
            int est = 0;
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == '-') continue;
                var y = _final.IndexOf(c);
                var diff = y - i;
                if (diff < 0) diff *= -1;
                est += diff;
            }
            return (est > 1 ? est-1 : (est));
        }
       
        private static void CleanPaths() {
            _partial_paths = new List<KeyValuePair<string, int>>(_partial_paths.Distinct());
        }
        
    }//end of AStar.class


    public class NewPathFoundEventArgs : EventArgs {
        public string Path { get; set; }
        public bool Optimal { get; set; }
    }//end of NewPathFoundEventArgs.class
}
