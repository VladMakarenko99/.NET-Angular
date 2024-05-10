import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, map } from 'rxjs';
import { User } from '../models/user.model';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  public authenticate() {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      if (token) {
        console.log('Token from query parameter:', token);
        localStorage.setItem('token', token);
        window.location.href = '/'
      }
    });
  }

  getCurrentUser(): Observable<User | null> {
    return this.http.get<User>(`${environment.apiUrl}/users/current`, { observe: 'response' })
      .pipe(
        map((response: HttpResponse<User>) => {
          if (response.status === 200) {
            return response.body;
          }
          else {
            console.log('Received status code:', response.status);
            return null;
          }
        })
      );
  }

  public signOut(): void {
    localStorage.removeItem('token');
    window.location.href = `${environment.apiUrl}/signout`;
  }
}

