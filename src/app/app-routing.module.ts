import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ServicesComponent } from './components/services/services.component';

const routes: Routes = [
  {
    path: 'services',
    component: ServicesComponent
   },
  // {
  //   path: '',
  //   component: 
  // }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
