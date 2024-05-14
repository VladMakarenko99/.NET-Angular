import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Service } from 'src/app/models/service.model';
import { GetServicesService } from 'src/app/services/get-services.service';
import { Purchase } from 'src/app/models/purchase.model';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { AuthService } from 'src/app/services/auth.service';
import { User } from 'src/app/models/user.model';
import { Router } from '@angular/router';


@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})
export class PurchaseComponent implements OnInit {
  purchase: Purchase = {
    service: undefined,
    steamId: ''
  };
  service?: Service;
  @Input() purchaseInput: Purchase = this.purchase;

  purchaseForm = this.formBuilder.group({
    selectedOption: ['', [Validators.required, Validators.min(1)]]
  })

  constructor(private getServicesService: GetServicesService,
    private formBuilder: FormBuilder,
    private http: HttpClient,
    private authService: AuthService,
    private router: Router) { }

  ngOnInit(): void {
    const id = window.location.href.split('/').at(-1);
    this.getServicesService.getServiceById(Number(id))
      .subscribe((result: Service) => {
        this.purchase.service = result;
        this.service = result;
      });

    this.authService.getCurrentUser()
      .subscribe((result: User | null) => {
        if(result === null){
          window.location.href = `${environment.apiUrl}/api/steam-signin`
        }
        this.purchase.steamId = result!.steamId;
      });
    
  }

  isSubmited = false;
  submitForm() {
    this.isSubmited = true;
    this.purchase.service!.selectedOption = this.purchaseForm.value.selectedOption!;
    if (!this.purchase.service!.selectedOption)
      this.purchase.service!.selectedOption = this.service?.optionsToSelect[0]!;
    this.http.post(`${environment.apiUrl}/services/buy`, this.purchase)
    .subscribe(response => {
      console.log(response);

      //window.location.href = '/success';
      this.router.navigate(['/success'], { state: { fromSource: true }})
    },
      error => {
        console.log(error);
      });
    
  }
}