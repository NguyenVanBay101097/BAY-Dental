import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MyOwnCustomMaterialModule } from './my-own-custom-material.module';
import { HeaderComponent } from './header/header.component';
import { HeaderMenuComponent } from './header-menu/header-menu.component';
import { HeaderBarComponent } from './header-bar/header-bar.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';

@NgModule({
  declarations: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent
  ],
  exports: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent
  ],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    MyOwnCustomMaterialModule,
    FlexLayoutModule
  ],
  entryComponents: [ConfirmDialogComponent],
})
export class SharedModule { }
