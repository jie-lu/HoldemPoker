import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router';
import { HelperService } from '../services/helper.service';
import { Subject, interval, Observable } from 'rxjs';
import { take, map, tap } from 'rxjs/operators';
import { trigger, state, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less'],
  animations: [
    trigger(
      'inOutAnimation', [
        state('in', style({ transform: 'translateX(0) rotate(0)'})),
        transition('void => *', [
          style({ transform: 'translateX(100%) rotate(360deg)'}),
          animate('0.5s ease-in')
        ]),
        transition('* => void', [
          animate('0.5s ease-out', style({ transform: 'translateX(-100%)'}))
        ])
      ]
    )
  ]
})
export class LoginComponent implements OnInit {
  private returnUrl: string;
  form: FormGroup;
  titleStream: Observable<string[]>;
  titleWords: string[] = [];

  constructor(private fb: FormBuilder, 
               private authService: AuthService, 
               private router: Router,
               private route: ActivatedRoute,
               private helper: HelperService,) {

      this.form = this.fb.group({
        email: ['', Validators.required]
      });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(
      params => this.returnUrl = params['return'] || '/game'
    );

    interval(500).pipe(
      take(3),
      tap(i => {
        console.log(i);
        switch(i) {
          case 0:
            this.titleWords.push('Texas ');
            break;
          case 1:
            this.titleWords.push('H♦ld\'em ');
            break;
          case 2:
            this.titleWords.push('环贸内测版');
            break;
        }
      })
    ).subscribe();
  }

  login() {
    const val = this.form.value;

    if (val.email) {
      this.authService.login(val.email).toPromise()
        .then(() => this.router.navigateByUrl(this.returnUrl))
        .catch(error => this.helper.handleError(error));
    }
  }
}