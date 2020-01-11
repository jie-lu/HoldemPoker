import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GameComponent } from './game/game.component';
import { LoginComponent } from './login/login.component';
import { AuthGuardService } from './services/auth-guard.service';


const routes: Routes = [
  { 
    path:'',
    pathMatch: 'full',
    redirectTo: 'game' 
  }, { 
    path: 'login',
    component: LoginComponent
  }, { 
    path: 'game',
    component: GameComponent,
    canActivate: [AuthGuardService]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
