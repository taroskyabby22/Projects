using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Pile {

    bool AddToPile(Transform card);
    Transform RemoveFromPile();

}
