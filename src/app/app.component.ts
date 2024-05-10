import { Component, OnInit } from '@angular/core';
import { GetServicesService } from './services/get-services.service';
import { AuthService } from './services/auth.service';
import { User } from './models/user.model';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Fullstack.Client';
  isAuthenticated = false;
  currentUser?: User;
  private routerSubscription: Subscription | undefined;

  constructor(private getServicesService: GetServicesService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.authenticate();
    if (localStorage.getItem('token')) {
      this.isAuthenticated = true;

      this.authService.getCurrentUser()
        .subscribe((result: User | null) => {
          if (result === null) {
            localStorage.removeItem('token');
            return;
          }
          this.currentUser = result
        });
    }
    this.routerSubscription = this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      if (event.url === '/success') {
        this.authService.getCurrentUser()
          .subscribe((result: User | null) => {
            this.currentUser = result!;
          });
      }
    });
  }

  signOut() {
    this.authService.signOut();
  }

}

