using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardLibrary : IModel
{
    Card CreateCard(string cardId);
    Card CreateRandomCard();

}
