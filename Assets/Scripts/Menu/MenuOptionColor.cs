using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionColor : MenuOption
{
    public Color Option;

    public override void Select() {
        base.Select();

        Paintable.colorEvent.Invoke(Option);
    }
}
