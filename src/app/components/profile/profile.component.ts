import { Component, OnInit } from '@angular/core';
import { Service } from 'src/app/models/service.model';
import { User } from 'src/app/models/user.model';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  currentUser?: User;
  boughtServicesData?: Service[];
  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.getCurrentUser()
      .subscribe((result: User) => {
        this.currentUser = result;
        if (this.currentUser?.boughtServicesJson) {
          const parsedServices = JSON.parse(this.currentUser.boughtServicesJson);
          this.boughtServicesData = parsedServices.map((service: any) => {
            return {
              id: service.Id,
              name: service.Name,
              description: service.Description,
              optionsToSelect: service.OptionsToSelect,
              selectedOption: service.SelectedOption,
              expireDate: service.ExpireDate
            };
          });
          console.log(this.boughtServicesData);
        }
      });
  }

}
