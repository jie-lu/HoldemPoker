import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { GameComponent } from './game/game.component';
import { PlayerComponent } from './player/player.component';
import { PlayingCardComponent } from './playing-card/playing-card.component';
import { CountDownComponent } from './count-down/count-down.component';
import { NgbPopoverModule, NgbProgressbarModule, NgbDropdownModule, NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { PlayerActionComponent } from './player-action/player-action.component';
import { ToastsContainerComponent } from './toasts-container/toasts-container.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    GameComponent,
    PlayerComponent,
    PlayingCardComponent,
    CountDownComponent,
    PlayerActionComponent,
    ToastsContainerComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    NgbPopoverModule,
    NgbProgressbarModule,
    NgbDropdownModule,
    NgbToastModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
