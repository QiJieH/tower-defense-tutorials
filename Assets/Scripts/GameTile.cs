using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour {
    [SerializeField]
    Transform arrow = default;

    GameTile north, east, south, west, nextOnPath;

    int distance;

    public bool HasPath => distance != int.MaxValue;

    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    public Direction PathDirection { get; private set;}

    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public bool IsAlternative { get; set;}

    GameTileContent content;

    public GameTile NextTileOnPath => nextOnPath;
    public Vector3 Exitpoint {get; private set;}

    public GameTileContent Content {
        get => content;
        set {
            Debug.Assert(value != null, "Null assigned to content");
            if(content != null) {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public static void MakeEastWestNeighbors(GameTile east, GameTile west) {
        Debug.Assert(
            west.east == null && east.west == null, "Redefined neighbors!"
        );
        west.east = east;
        east.west = west;
    }
    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south) {
        Debug.Assert(
            south.north == null && north.south == null, "Redefined neighbors!"
        );
        south.north = north;
        north.south = south;
    }

    public void ClearPath() {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination() {
        distance = 0;
        nextOnPath = null;
        Exitpoint = transform.localPosition;
    }

    GameTile GrowPathTo(GameTile neighbor, Direction direction) {
        Debug.Assert(HasPath, "No Path!");
        if(neighbor == null || neighbor.HasPath) {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.Exitpoint =
            neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return 
            neighbor.Content.BlocksPath ? null : neighbor;
    }

    public void ShowPath() {
        if(distance == 0) {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation : 
            westRotation;
    }

    public void HidePath() {
        arrow.gameObject.SetActive(false);
    }
}
