import { Component, OnInit } from '@angular/core';
import { Service } from 'src/app/models/service.model';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'app-services',
  templateUrl: './services.component.html',
  styleUrls: ['./services.component.css']
})
export class ServicesComponent implements OnInit {
  services: Service[] = [];
  

  constructor(private http: HttpClient) { }
  async ngOnInit(): Promise<void> {
    await this.http.get<Service[]>('https://asmodeus.bsite.net/api/services').subscribe({
      next: (Services) => {
        this.services = Services;
        console.log(this.services)
      },
      error: (response) => {
        console.log(response);
      }
    });

  }

}
