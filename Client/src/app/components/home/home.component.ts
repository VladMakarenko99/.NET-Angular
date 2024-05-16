import { Component, OnInit } from '@angular/core';
import { Service } from 'src/app/models/service.model';
import { GetServicesService } from 'src/app/services/get-services.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  constructor(private getServicesService: GetServicesService) { }
  services: Service[] = [];

  ngOnInit(): void {
     this.getServicesService.getServices()
       .subscribe((result: Service[]) => this.services = result);
  }
  
  
}
