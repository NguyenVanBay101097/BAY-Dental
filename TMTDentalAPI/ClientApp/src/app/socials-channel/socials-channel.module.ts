import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SocialsChannelRoutingModule } from './socials-channel-routing.module';
import { FacebookComponent } from './facebook/facebook.component';
import { FacebookDialogComponent } from './facebook-dialog/facebook-dialog.component';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [FacebookComponent, FacebookDialogComponent],
  imports: [
    CommonModule,
    SocialsChannelRoutingModule,
    FormsModule,
    NgbModule
  ],
  exports: [
  ],
  entryComponents: [
    FacebookDialogComponent,
  ]
})
export class SocialsChannelModule { }
