import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GenerateTokenComponent } from './generate-token/generate-token.component';

const routes: Routes = [
    {
        path: '',
        component: GenerateTokenComponent
      }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SettingPublicApiRoutingModule { }
