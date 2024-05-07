import { Component, OnInit } from '@angular/core';
import { GetServicesService } from './services/get-services.service';
import { AuthService } from './services/auth.service';
import { User } from './models/user.model';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Fullstack.Client';
  isAuthenticated = false;
  currentUser?: User;
  
  constructor(private getServicesService: GetServicesService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.authService.authenticate();
    if (localStorage.getItem('token')) {
      this.isAuthenticated = true;
      
      this.authService.getCurrentUser()
      .subscribe((result: User) => this.currentUser = result);
    }

  }

  signOut() {
    this.authService.signOut();
  }
  
}

