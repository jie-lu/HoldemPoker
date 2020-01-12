import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { Player } from '../models/player';
import { PlayingCardService } from '../services/playing-card.service';
import { GameEventType } from '../models/game';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { interval, empty, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.less'],
  animations: [
    trigger(
      'inOutAnimation', [
        state('in', style({ transform: 'rotate(0)', opacity: 1 })),
        transition('void => *', [
          style({ transform: 'rotate(-60deg)', opacity: 0 }),
          animate('0.5s ease-in')
        ]),
        transition('* => void', [
          animate('0.5s ease-out', style({ transform: 'rotate(60deg)', opacity: 0 }))
        ])
      ]
    ),
    trigger(
      'popoverAnimation', [
        state('in', style({ transform: 'scale(1)'})),
        transition('void => *', [
          style({ transform: 'scale(0)'}),
          animate('0.5s ease-in')
        ]),
        transition('* => void', [
          animate('0.5s ease-out', style({ transform: 'scale(0)'}))
        ])
      ]
    )
  ]
})
export class PlayerComponent implements OnInit, OnChanges {
  constructor(public cardService: PlayingCardService) { }

  @Input() player: Player;
  @ViewChild('actionPopover', {static: false}) actionPopover: NgbPopover;

  isActionVisible: boolean;
  actionName: string;
  isActionAmountVisible: boolean;
  actionAmount: number;

  countDown$ = interval(1000);
  countDownSub: Subscription;
  countDown = 0;

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.player.currentValue) {
      this.updateAction();
    }


    if((changes.player.currentValue != null && changes.player.previousValue == null) ||
       (changes.player.currentValue && changes.player.previousValue
        && changes.player.currentValue.isCurrentPlayer != changes.player.previousValue.isCurrentPlayer)
      ) {
      if(changes.player.currentValue.isCurrentPlayer) {
        this.countDown$ = interval(1000).pipe(
          take(changes.player.currentValue.countDown));
        this.countDownSub = this.countDown$.subscribe((i) => {
          this.countDown = i;
          console.log(i)
        });
        console.log('player ' + changes.player.currentValue.id + ' ' + changes.player.currentValue.isCurrentPlayer);
      } else {
        if(this.countDownSub) this.countDownSub.unsubscribe();
        this.countDown = 0;
        console.log('player ' + changes.player.currentValue.id + ' ' + changes.player.currentValue.isCurrentPlayer);
      }
    }
  }

  private updateAction() {
    this.isActionVisible = this.player.hasFolded || !!this.player.lastActionInCurrentStage
      || (!this.player.lastActionInCurrentStage && this.player.betInCurrentStage > 0);
    
    if(this.isActionVisible) {
      if(this.actionPopover) this.actionPopover.open();
      if(this.player.hasFolded) {
        this.actionName = 'Fold';
        this.isActionAmountVisible = false;
      }
      else if(this.player.lastActionInCurrentStage) {
       this.actionName = this.getActionNameByEventType(this.player.lastActionInCurrentStage.eventType);
       this.isActionAmountVisible = true;
       this.actionAmount = this.player.betInCurrentStage;
      } else {
        this.actionName = "Blind";
        this.isActionAmountVisible = true;
        this.actionAmount = this.player.betInCurrentStage;
      }
    } else {
      if(this.actionPopover) this.actionPopover.close();
    }
  }

  getActionNameByEventType(eventType: GameEventType) {
    switch(eventType) {
      case GameEventType.Check:
        return 'Check';
      case GameEventType.Call:
        return 'Call';
      case GameEventType.Raise:
        return 'Raise';
      case GameEventType.Fold:
        return 'Fold';
    }
    return '';
  }
}
