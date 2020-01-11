import { Component, OnInit, OnDestroy, ViewChild, ViewChildren, ElementRef, QueryList, AfterViewChecked } from '@angular/core';
import { SignalRService } from '../services/signalr.service';
import { map } from 'rxjs/operators';
import { PlayingCardService } from '../services/playing-card.service';
import { GameState, GameEvent } from '../models/game';
import { AuthService } from '../services/auth.service';
import { trigger, transition, style, animate, state } from '@angular/animations';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.less'],
  animations: [
    trigger(
      'inOutAnimation', [
        state('in', style({ transform: 'translateY(0)', opacity: 1 })),
        transition('void => *', [
          style({ transform: 'translateY(100%)', opacity: 0 }),
          animate('1s ease-in')
        ]),
        transition('* => void', [
          animate('1s ease-out', style({ transform: 'translateY(-100%)', opacity: 0 }))
        ])
      ]
    )
  ]
})
export class GameComponent implements OnInit, AfterViewChecked, OnDestroy {
  
  game: GameState;
  gameEvents: GameEvent[] = [];
  @ViewChild('eventContainer', {static: false}) eventContainer: ElementRef;
  @ViewChildren('event') events: QueryList<any>;
  private eventContainerEle: any;
  private isEventContainerInitialized = false;

  constructor(public signalr: SignalRService,
    public cardService: PlayingCardService,
    private authService: AuthService) { }

  processGameSate(state: GameState) {
    state = Object.assign(new GameState(), state);
    
    let selfIndex = 0;
    let pot = 0;
    state.players.forEach((p, i) => {
      if(state.isInProcess) {
        p.isCurrentPlayer = state.currentPlayerIndex == i;
        p.betInCurrentStage = p.bets[state.stage];
      } else {
        p.isCurrentPlayer = false;
        p.betInCurrentStage = 0;
      }

      p.isDealer = state.dealerIndex == i;
      p.isWinner = state.winnerIds.indexOf(p.id) >= 0;
      if(p.id == this.authService.getUser().name) {
        selfIndex = i;
      }
      pot += p.bets.reduce((prev, curr, stageIndex) => {
          if(stageIndex != state.stage) return prev + curr;
          else return prev;
        }, 0);
    });

    // Adjust the order of the players array
    if(selfIndex > 0) {
      let leftPlayers = state.players.splice(0, selfIndex);
      state.players = state.players.concat(leftPlayers);
    }
    state.pot = pot;
    state.gameEvent = Object.assign(new GameEvent(), state.gameEvent);
    this.gameEvents.push(state.gameEvent);

    return state;
  }

  ngOnInit() {
    this.signalr.connect();
    this.signalr.gameState$.pipe(
      map(state => {
        if(state) return this.processGameSate(state);
        return state;
      })
    ).subscribe(game => this.game = game);
  }

  ngAfterViewChecked() {
    if(!this.isEventContainerInitialized && this.eventContainer) {
      this.eventContainerEle = this.eventContainer.nativeElement;
      this.events.changes.subscribe(() => {
        this.eventContainerEle.scroll({
          top: this.eventContainerEle.scrollHeight,
          left: 0,
          behavior: 'smooth'
        });
      });
      this.isEventContainerInitialized = true;
    }
  }

  ngOnDestroy() {
    this.signalr.disconnect();
  }

  startGame() {
    this.gameEvents = [];
    this.signalr.startGame();
  }

  trackCard(index, item) {
    return index;
  }
}
