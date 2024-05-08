import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TopUpPurchase } from 'src/app/models/top-up.model';
import { environment } from 'src/environments/environment.development';
import { AuthService } from 'src/app/services/auth.service';
import { User } from 'src/app/models/user.model';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-top-up',
  templateUrl: './top-up.component.html',
  styleUrls: ['./top-up.component.css'],
})
export class TopUpComponent implements OnInit {
  constructor(private http: HttpClient, private authService: AuthService, private formBulider: FormBuilder) { }

  topUpData: TopUpPurchase = { amount: 0, steamId: '' };
  topUpForm = this.formBulider.group({
    amount: [0, Validators.min(20)]
  })

  ngOnInit(): void {
    this.authService.getCurrentUser()
      .subscribe((result: User) => this.topUpData.steamId = result.steamId);
  }

  isSubmited = false;
  submitForm(): void {
    this.isSubmited = true;
    this.topUpData.amount = this.topUpForm.value.amount;

    this.http.post(`${environment.apiUrl}/payment/top-up`, this.topUpData, { responseType: 'text' }).subscribe(
      response => {
        window.location.href = response;
      },
      error => {
        console.log(error);
      }
    );
  }
}
