using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOptionSize : MenuOption
{
    public float Size;

    public override void Select() {
        base.Select();

        Paintable.sizeEvent.Invoke(Size);
    }
}
