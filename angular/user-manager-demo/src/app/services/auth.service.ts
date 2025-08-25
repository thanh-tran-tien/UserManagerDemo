import { inject, Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { Observable, of, BehaviorSubject, catchError, map, tap } from 'rxjs';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { AuthResponse } from '../models/auth-response.model';
import { FluentResult } from '../models/fluent-result.model';
import { User } from '../models/user.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  public currentUser$ = new BehaviorSubject<User | null>(null);
  private router = inject(Router);
  constructor(http: HttpClient) {
    super(http, 'Auth');
  }

  getCurrentUser(): Observable<FluentResult<User>> {
    return this.get<User>('CurrentUser').pipe(
      tap({
        next: res => {
          if (res.isSuccess && res.value) {
            this.currentUser$.next(res.value);
          } else {
            this.handleUnauthorized();
          }
        },
        error: () => {
          this.handleUnauthorized();
        }
      })
    );
  }


  isAuthenticated() {
    return this.currentUser$.value !== null;
  }

  login(request: LoginRequest): Observable<FluentResult<AuthResponse>> {
    return this.post<AuthResponse>('Login', request).pipe(
      tap(res => {
        if (res.isSuccess && res.value) {
          this.getCurrentUser().subscribe();
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<FluentResult<any>> {
    return this.post<any>('Register', request);
  }

  refresh(refreshToken: string): Observable<FluentResult<AuthResponse>> {
    return this.post<AuthResponse>('Refresh', { refreshToken }).pipe(
    );
  }

  logout(): Observable<any> {
    return this.post('Logout', {}).pipe(
      map(() => {
        this.handleUnauthorized();
      })
    );
  }

  private handleUnauthorized() {
    this.currentUser$.next(null);
    // Angular Router redirect
    this.router.navigate(['/login']);
  }

}
