import { Component, OnInit, Input } from '@angular/core';
import { interval, Observable } from 'rxjs';
import { map, takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-count-down',
  templateUrl: './count-down.component.html',
  styleUrls: ['./count-down.component.less']
})
export class CountDownComponent implements OnInit {
  public count$: Observable<number>;
  @Input() count: number;

  constructor() { }

  ngOnInit() {
    this.count$ = this.createCountDown(this.count);
  }

  private createCountDown(count: number) {
    return interval(1000).pipe(
      map(i => count - i),
      takeWhile(c => c >= 0)
    );
  }
}
