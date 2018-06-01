# Ravelo_CIS479_Project_2

Application that compares the speed/accuracy of Branch&Bound vs. A* hueristic search algorithms.
tl;dr - A* is MUCH FASTER and MORE PERFORMANT

Implemented as a Universal Windows Package (UWP)

Both the A* and B&B were stored in C#’s List<KeyValuePairs<string, int>>. By taking advantage of C#’s List<T> built-in functions, I was able to simulate a queue (adding at the end, and removing from the bottom/head). Furthermore, all paths were kept as string literals and new paths were generated using C#’s built in string manipulation methods.
For the path generator function I chose to give priority to rule 3 (jumping) as it covered more distance than a simple slide or swap. The idea was that jumping reduced/covered more distance than swapping thus reducing the number of iterations it might potentially take to produce a correct match. For the A* implementation I also added a check to see if the values current state matched the final expected state; if so, continue on (don’t process), otherwise continue evaluating. This reduced unnecessary moves from producing new (possibly invalid) states which subsequently reduced sorting time and potentially the number of redundant states needed to be removed. The decision to include this was to better emulate human interaction.
When sorting by distance, distance was a simple calculation of determining the states height when viewed as a tree. Having to factor in A*’s underestimate I used the sum of the displacement of each character in a path relative to the desired end-state position. I accounted for negative displacement by simply multiplying the displacement by (-1) guaranteeing a non-negative value. While arbitrary, I felt it was a truer reflection of the states position; this would have caused B&B to explore/eliminate unsolvable paths first. Additionally, to absolutely guarantee an underestimate, I subtracted 1 from the sum so long as the sum was greater than 1 and stored it’s value plus the distance traveled with the path.
e.g. Path = ‘a-tr-’, final_path = ‘--tar’, distance = 1
distance + underestimate -1 = Value
distance + a(3) + r(1) + t(1) -1 = 1 + 5 - 1 = < ‘a-tr-’, 5> 

Solvability was screened prior to path generation by comparing starting and final string. Factors included string length, instances of distinct characters, whether the starting string began with a contiguous string of ‘-’, and whether or not both the starting and final string contained the same characters. This prevented both algorithms from needless attempting the unsolvable.  
Lastly, for performance reasons solely, I ensured that if A* was unable to find a solution that B&B would not make an attempt. This was just prevent the system from crashing on large inputs.

A* significantly out performs Branch & Bound even in small cases. Because the number of states grows n^m, where n is the size of the state (string length) and m are the number of distinct values (unique characters including ‘-’), it has a space-time complexity of O(n^m). Even so, A* has no issues dealing with larger state spaces. Even when handling cases where there is no possible solution, it is able to determine that in a fraction of the time comparatively to Branch and Bound, which is more likely to throw an OutOfMemoryException (or some type of System error) and crash, than it is to actually make that determination. We can see from the table that the number of iterations (path size) for A* to find a solution was significantly less than that of Branch and Bound over larger state-spaces. 
