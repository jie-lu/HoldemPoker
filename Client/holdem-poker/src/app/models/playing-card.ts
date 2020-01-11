export enum CardSuit {
    Spades = 0,
    Hearts,
    Clubs,
    Diamonds
}

export interface PlayingCard {
    number: string,
    suit: CardSuit,
    suitSymbol: string
}
