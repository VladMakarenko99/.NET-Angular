import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './components/profile/profile.component';
import { HomeComponent } from './components/home/home.component';
import { TopUpComponent } from './components/top-up/top-up.component';
import { PurchaseComponent } from './components/purchase/purchase.component';
import { SuccessComponent } from './components/success/success.component';

const routes: Routes = [
  {
    path: 'profile',
    component: ProfileComponent
  },
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'top-up',
    component: TopUpComponent
  },
  {
    path: 'buy/:id',
    component: PurchaseComponent
  },
  {
    path: 'success',
    component: SuccessComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
