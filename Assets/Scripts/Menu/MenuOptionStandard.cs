using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionStandard : MenuOption
{
    public StandardMenuOption standardMenuOption;

    public override void Select() {
        base.Select();

        MenuManager.standardMenuEvent.Invoke(standardMenuOption);
    }
}

public enum StandardMenuOption
{
    ReturnToStart,
    Exit
}
