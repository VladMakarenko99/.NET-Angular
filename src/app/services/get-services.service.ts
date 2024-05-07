import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Service } from '../models/service.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class GetServicesService {

  constructor(private http: HttpClient) { }
  url = "/services";
  
  public getServices(): Observable<Service[]> {
    return this.http.get<Service[]>(`${environment.apiUrl}${this.url}`);
  }
}
