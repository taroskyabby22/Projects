using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

interface Pile
{
    /// <summary>
    /// Attempt to add a card to a pile (also add all the children to the pile)
    /// </summary>
    /// <param name="card">The card to add to the pile</param>
    /// <returns></returns>
    bool AddToPile(Transform card);

    /// <summary>
    /// Remove from the pile -- when the card has gone to another stack -- also remove all children under the card
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    void RemoveFromPile();

    /// <summary>
    /// When dealing, add to the pile
    /// </summary>
    /// <param name="card">The card to add to the pile</param>
    void InitiazeAddToPile(Transform card);
}
