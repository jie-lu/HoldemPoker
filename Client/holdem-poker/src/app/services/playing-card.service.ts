import { Injectable } from '@angular/core';
import { CardSuit, PlayingCard } from '../models/playing-card';

@Injectable({
  providedIn: 'root'
})
export class PlayingCardService {

  constructor() { }

  cardNumberArray = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K', 'A'];
  suitSymbolArray = ['♠', '♥', '♣', '♦'];
  rankValues = {
    2: 2,
    3: 3,
    4: 4,
    5: 5,
    6: 6,
    7: 7,
    8: 8,
    9: 9,
    10: 10,
    11: 11,
    12: 12,
    13: 13,
    14: 14,
    J: 11, 
    Q: 12, 
    K: 13, 
    A: 14 
  };

  suitValues = {
    s: CardSuit.Spades,
    h: CardSuit.Hearts,
    c: CardSuit.Clubs,
    d: CardSuit.Diamonds
  }

  cardFromString(str: string) {
    if(str === null || str.length < 2) throw "A card string cannot be null or less than 2 characters";
    let suitNum = this.suitValues[str.substr(str.length - 1, 1)];

    return {
      number: this.cardNumberArray[this.rankValues[str.substr(0, str.length - 1)] - 2],
      suit: suitNum,
      suitSymbol: this.suitSymbolArray[suitNum]
    }
  }
}
