import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { Player } from '../models/player';
import { PlayingCardService } from '../services/playing-card.service';
import { Observable, interval } from 'rxjs';
import { map, takeWhile } from 'rxjs/operators';
import { GameEventType } from '../models/game';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.less']
})
export class PlayerComponent implements OnInit, OnChanges {
  constructor(public cardService: PlayingCardService) { }

  @Input() player: Player;
  @Input() isCurrentPlayer: boolean;
  @ViewChild('actionPopover', {static: false}) actionPopover: NgbPopover;

  isActionVisible: boolean;
  actionName: string;
  isActionAmountVisible: boolean;
  actionAmount: number;

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.player && changes.player.currentValue) {
      this.updateAction();
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
