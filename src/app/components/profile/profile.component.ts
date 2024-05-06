import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit{
  currentUser?: User;
  constructor(private authService: AuthService){}

  ngOnInit(): void {
    this.authService.getCurrentUser()
    .subscribe((result: User) => this.currentUser = result);
  }

}
