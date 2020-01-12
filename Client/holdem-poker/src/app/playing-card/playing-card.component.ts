import { Component, OnInit, Input } from '@angular/core';
import { PlayingCard, CardSuit } from '../models/playing-card';

@Component({
  selector: 'app-playing-card',
  templateUrl: './playing-card.component.html',
  styleUrls: ['./playing-card.component.less'],
})
export class PlayingCardComponent implements OnInit {

  cardSuitEnum = CardSuit;
  @Input() bigSymbolVisible: boolean = true;

  constructor() { }

  ngOnInit() {
  }

  @Input() card: PlayingCard;
}
