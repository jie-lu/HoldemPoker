import { Injectable, InjectionToken, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { shareReplay, tap, takeWhile } from 'rxjs/operators';
import { User } from '../models/user';
import { environment } from '../../environments/environment'
import { BehaviorSubject, Subject } from 'rxjs';

export const BROWSER_STORAGE = new InjectionToken('Browser Storage', {
  providedIn: 'root',
  factory: () => localStorage
});

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl: string;
  private userSource = new BehaviorSubject<User>(null);
  public userChanges = this.userSource.asObservable().pipe(takeWhile((user) => user != null));

  constructor(private http: HttpClient, @Inject(BROWSER_STORAGE) private storage: Storage) { 
    this.apiUrl = environment.apiUrl;
    this.userSource.next(this.getUser());
  }

  public login(userName: string) {
    return this.http.post<User>(`${this.apiUrl}/jwt-token`, {
      userName: userName
    }).pipe(
      tap(user => {
        this.rememberUser(user);
        this.userSource.next(user);
      })
    );
  }

  private rememberUser(user: User) {
    this.storage.setItem('user', JSON.stringify(user));
  }

  public getUser() : User {
    let userJson = this.storage.getItem('user');
    if(userJson) return JSON.parse(userJson);
    else return null;
  }

  public getUserToken() {
    let user = this.getUser();
    if(user) return user.token;
    else return null;
  }
}
