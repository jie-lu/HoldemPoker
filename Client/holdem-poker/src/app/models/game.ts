import { Player } from './player';
import { PlayingCard } from './playing-card';

export enum GameStage {
  NotStarted = -1,
  Preflop,
  Flop,
  Turn,
  River,
  End
}

export enum GameEventType {
  Start = 0,
  End,
  Reset,
  Join,
  Leave,
  Call,
  Raise,
  Check,
  Fold,
  Wait,
  RoundChanged
}

export class GameEvent {
  playerId: string;
  eventType: GameEventType;
  amount: number;
  stage: GameStage;

  getStageName(stage: GameStage) {
    switch(stage) {
      case GameStage.NotStarted:
        return 'NotStarted';
      case GameStage.Preflop:
        return 'Preflop';
      case GameStage.Flop:
        return 'Flop';
      case GameStage.Turn:
        return 'Turn';
      case GameStage.River:
        return 'River';
      case GameStage.End:
        return 'End';
    }
  }

    get eventMessage(): string {
        let msg = '';
        switch(this.eventType) {
            case GameEventType.Start:
                msg = `${this.playerId} started the game.`;
                break;
            case GameEventType.End:
                msg = `${this.playerId} ended the game.`;
                break;
            case GameEventType.Reset:
                msg = `${this.playerId} reset the game.`;
                break;
            case GameEventType.Join:
                msg = `${this.playerId} joined the game.`;
                break;
            case GameEventType.Leave:
                msg = `${this.playerId} left the game.`;
                break;
            case GameEventType.Call:
                msg = `${this.playerId} called with ${this.amount}`;
                break;
            case GameEventType.Raise:
                msg = `${this.playerId} raised to ${this.amount}`;
                break;
            case GameEventType.Check:
                msg = `${this.playerId} checked.`;
                break;
            case GameEventType.Fold:
                msg = `${this.playerId} folded.`;
                break;
            case GameEventType.Wait:
                msg = `${this.playerId} is being waited.`;
                break;
            case GameEventType.RoundChanged:
                msg = `Game moved to the ${this.getStageName(this.stage)} round`;
                break;
        }
        return msg;
    }
}

export class GameState {
    players: Player[];
    audiencePlayers: Player[];
    dealerIndex: number;
    currentPlayerIndex: number;
    sharedCards: PlayingCard[];
    stage: GameStage;
    maxBetAtThisStage: number;
    winnerIds: string[];
    gameEvent: GameEvent;
    blind: number;

    // The properties and methods below only exist at front end.
    pot: number;

    get isInProcess(): boolean {
        return this.stage !== GameStage.NotStarted && this.stage !== GameStage.End;
    }
}

