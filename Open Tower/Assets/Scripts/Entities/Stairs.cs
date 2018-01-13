﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : Entity {
    private static DungeonManager dungeon;

    [SerializeField]
    private StairType direction;

    private Stairs destination;

    public StairType Direction {
        get {
            return direction;
        }
    }

    private void Start() {
        if (dungeon == null) {
            dungeon = DungeonManager.Instance;
        }

        int siblingIndex = transform.parent.parent.GetSiblingIndex();
        int offset = 0;
        switch (direction) {
            case StairType.GOES_UP:
                offset = 1;
                break;

            case StairType.GOES_DOWN:
                offset = -1;
                break;
        }

        int offsetFloorIndex = siblingIndex + offset;
        Stairs[] stairsOnOffsetFloor = dungeon.GetFloor(offsetFloorIndex).GetComponentsInChildren<Stairs>();
        foreach (Stairs stairs in stairsOnOffsetFloor) {
            if (stairs.Direction != this.Direction) {
                if (destination != null) {
                    Util.Assert(false, "Multiple stair destinations detected for {0}", destination.name);
                }
                destination = stairs;
            }
        }
        Util.Assert(destination != null, "Unable to find respective stairs on floor {0}", offsetFloorIndex);
    }

    protected override void DoAction(Player player) {
        player.gameObject.transform.SetParent(destination.transform.parent, false);
        player.transform.localPosition = Vector3.zero;
        GetComponentInParent<FloorManager>().gameObject.SetActive(false);
        destination.transform.parent.parent.GetComponent<FloorManager>().gameObject.SetActive(true);
        player.transform.localPosition = destination.transform.localPosition;
    }

    protected override bool IsActionPossible(Player player) {
        return true;
    }
}