import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
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

  public getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/current`);
  }

  public signOut() : void{
    localStorage.removeItem('token');
    window.location.href = `${environment.apiUrl}/signout`;
  }
}

