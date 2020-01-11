import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent implements OnInit {
  public form: FormGroup;
  private returnUrl: string;

  constructor(private fb: FormBuilder, 
               private authService: AuthService, 
               private router: Router,
               private route: ActivatedRoute) {

      this.form = this.fb.group({
        email: ['', Validators.required]
      });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(
      params => this.returnUrl = params['return'] || '/game'
    );
  }

  login() {
    const val = this.form.value;

    if (val.email) {
      this.authService.login(val.email)
        .subscribe(() => {
          console.log("User is logged in");
          this.router.navigateByUrl(this.returnUrl);
        }
      );
    }
  }
}