using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static List<RiskySandBox_Tile> findPath(RiskySandBox_Tile startTile, RiskySandBox_Tile targetTile)
    {
        // The set of tiles to be evaluated
        List<RiskySandBox_Tile> openSet = new List<RiskySandBox_Tile> { startTile };

        // The set of tiles already evaluated
        HashSet<RiskySandBox_Tile> closedSet = new HashSet<RiskySandBox_Tile>();

        // A dictionary to keep track of the path
        Dictionary<RiskySandBox_Tile, RiskySandBox_Tile> cameFrom = new Dictionary<RiskySandBox_Tile, RiskySandBox_Tile>();

        // Cost from start to a given tile
        Dictionary<RiskySandBox_Tile, float> gScore = RiskySandBox_Tile.all_instances.ToDictionary(tile => tile, tile => float.MaxValue);
        gScore[startTile] = 0;

        while (openSet.Count > 0)
        {
            // Get the tile in openSet with the lowest gScore value
            RiskySandBox_Tile current = openSet.OrderBy(tile => gScore[tile]).First();

            if (current == targetTile)
            {
                // We found the path to the target tile
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (RiskySandBox_Tile neighbor in current.graph_connections)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue; // Ignore the neighbor which is already evaluated
                }

                if (neighbor.my_Team != startTile.my_Team)
                {
                    //if we are allowed to move through ally???
                    bool _is_ally = startTile.my_Team.ally_ids.Contains(neighbor.my_Team_ID);
                    if (_is_ally == false)
                        continue;

                    if (RiskySandBox_AllianceSettings.allow_move_through_ally_Tiles == false)
                        continue;
                    
                }
                    


                float tentativeGScore = gScore[current] + Distance(current, neighbor);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor); // Discover a new tile
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue; // This is not a better path
                }

                // This path is the best until now. Record it!
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
            }
        }

        // If we reach here, it means there's no path
        return new List<RiskySandBox_Tile>();
    }

    private static List<RiskySandBox_Tile> ReconstructPath(Dictionary<RiskySandBox_Tile, RiskySandBox_Tile> cameFrom, RiskySandBox_Tile current)
    {
        List<RiskySandBox_Tile> totalPath = new List<RiskySandBox_Tile> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse();
        return totalPath;
    }

    private static float Distance(RiskySandBox_Tile a, RiskySandBox_Tile b)
    {
        // In this simple example, we assume the distance between directly connected tiles is always 1
        return 1f;
    }




}
