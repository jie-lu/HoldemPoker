import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Player, PlayerBet } from '../models/player';
import { GameStage, GameState } from '../models/game';
import { SignalRService } from '../services/signalr.service';

@Component({
  selector: 'app-player-action',
  templateUrl: './player-action.component.html',
  styleUrls: ['./player-action.component.less']
})
export class PlayerActionComponent implements OnInit, OnChanges {

  @Input() player: Player;
  @Input() game: GameState;
  playerBet: PlayerBet = {
    value: 0,
    min: 0,
    max: 0,
  };

  constructor(private signalrService: SignalRService) { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes.game && changes.game.currentValue) {
      this.updatePlayerBet();
    }
  }

  toggleRaisePopover(popover, game: GameState) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.updatePlayerBet();
      popover.open({ game });
    }
  }

  decreaseBet() {
    let change = Math.min(this.player.chips, this.game.blind);
    change = Math.min(change, this.playerBet.value)
    this.playerBet.value -= change;
    this.player.chips += change;
  }

  increaseBet() {
    let change = Math.min(this.player.chips, this.game.blind);
    change = Math.min(change, this.player.chips);
    this.playerBet.value += change;
    this.player.chips -= change;
  }

  clearBet() {
    this.player.chips += this.playerBet.value;
    this.playerBet.value = 0;
  }

  increaseBetToOneTimePot() {
    this.clearBet();

    let change = Math.min(this.player.chips, this.game.pot);
    this.playerBet.value += change;
    this.player.chips -= change;
  }

  increaseBetToOnePointFiveTimesPot() {
    this.clearBet();

    let change = Math.floor(Math.min(this.player.chips, this.game.pot * 1.5));
    change = change / this.game.blind * this.game.blind;
    this.playerBet.value += change;
    this.player.chips -= change;
  }

  increaseBetToTwoTimesPot() {
    this.clearBet();

    let change = Math.floor(Math.min(this.player.chips, this.game.pot * 2));
    this.playerBet.value += change;
    this.player.chips -= change;
  }

  increaseBetToAll() {
    // Clear prevsiou bet
    this.player.chips += this.playerBet.value;
    this.playerBet.value = 0;

    let change = this.player.chips;
    this.playerBet.value += change;
    this.player.chips -= change;
  }

  updatePlayerBet() {
    if(this.game && this.game.isInProcess) {
      this.playerBet = {
        value: Math.min(this.player.chips, this.game.maxBetAtThisStage),
        min: Math.min(this.player.chips, this.game.maxBetAtThisStage),
        max: Math.max(this.player.chips, this.game.maxBetAtThisStage)
      }
    }
  }

  fold() {
    if(this.player && this.player.isCurrentPlayer) {
      this.signalrService.fold();
    }
  }

  check() {
    if(this.player && this.player.isCurrentPlayer) {
      this.signalrService.check();
    }
  }

  call() {
    if(this.player && this.player.isCurrentPlayer) {
      this.signalrService.call();
    }
  }

  bet(p) {
    if(this.player && this.player.isCurrentPlayer) {
      this.signalrService.bet(this.playerBet.value);
      p.close();
    }
  }

}
