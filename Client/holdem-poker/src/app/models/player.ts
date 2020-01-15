import { GameEvent } from './game'

export interface Player {
    id: string,
    chips: number,
    bets: number[],
    isConnected: boolean,
    cards: string[],
    isCurrentPlayer: boolean,
    isDealer: boolean,
    countDown: number,
    isWinner: boolean,
    betInCurrentStage: number,
    hasFolded: boolean,
    lastActionInCurrentStage: GameEvent,
    isYou: boolean,
    isHandVisible: boolean,
}

export interface PlayerBet {
    value: number;
    min: number;
    max: number;
}