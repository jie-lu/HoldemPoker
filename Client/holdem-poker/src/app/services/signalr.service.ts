import { Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, LogLevel } from '@aspnet/signalr'
import { AuthService } from './auth.service';
import { BehaviorSubject } from 'rxjs';
import { GameState, GameStage } from '../models/game';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { User } from '../models/user';
import { HelperService } from './helper.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private signalrUrl: string;
  public isConnected = false;
  private connection: HubConnection;
  private gameStateSource = new BehaviorSubject<GameState>(null);
  public gameState$ = this.gameStateSource.asObservable();

  constructor(private authService: AuthService,
    private router: Router,
    private helper: HelperService) {
    this.signalrUrl = environment.signalrUrl;
    this.authService.userChanges.subscribe((user) => this.setupConnection(user));
  }

  setupConnection(user: User) {
    this.connection = new HubConnectionBuilder()
      .withUrl(this.signalrUrl, { accessTokenFactory: () => user.token })
      .configureLogging(LogLevel.Information)
      .build();
    
    this.connection.onclose(error => {
      this.isConnected = false;
      this.helper.handleError(error);
    });

    this.connection.on('update', gameState => {
      gameState = Object.assign(new GameState(), gameState);
      console.log(gameState);
      this.gameStateSource.next(gameState);
    });
  }

  connect() {
    this.connection.start()
      .then(() => {
        this.isConnected = true;
      })
      .catch(error => {
        if(error && error.statusCode === 401) {
          this.router.navigate(['login']);
        } else {
          this.helper.handleError(error);
        }
      });
  }

  disconnect() {
    this.connection.stop();
  }

  startGame() {
    this.connection.invoke("StartGame")
    .then(() => {
      console.log('game started');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }

  resetGame() {
    this.connection.invoke("ResetGame")
    .then(() => {
      console.log('game has been reset');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }

  fold() {
    this.connection.invoke("Fold")
    .then(() => {
      console.log('Fold');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }

  check() {
    this.connection.invoke("Check")
    .then(() => {
      console.log('Check');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }

  call() {
    this.connection.invoke("Call")
    .then(() => {
      console.log('Call');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }

  bet(amount: number) {
    this.connection.invoke("Bet", amount)
    .then(() => {
      console.log('Bet');
    })
    .catch(err => {
      this.helper.handleError(err);
    });
  }
}
