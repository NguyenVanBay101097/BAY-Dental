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
import { RouterModule } from '@angular/router';
import { ChangePasswordDialogComponent } from './change-password-dialog/change-password-dialog.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LayoutSidebarComponent } from './layout-sidebar/layout-sidebar.component';
import { LayoutHeaderComponent } from './layout-header/layout-header.component';
import { NavSidebarService } from './nav-sidebar.service';
import { FieldBinaryImageSimpleComponent } from './field-binary-image-simple/field-binary-image-simple.component';
import { ImageViewerComponent } from './image-viewer/image-viewer.component';

@NgModule({
  declarations: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent,
    ChangePasswordDialogComponent,
    LayoutSidebarComponent,
    LayoutHeaderComponent,
    FieldBinaryImageSimpleComponent,
    ImageViewerComponent
  ],
  exports: [
    HeaderComponent,
    HeaderMenuComponent,
    HeaderBarComponent,
    TopMenuComponent,
    ConfirmDialogComponent,
    LayoutSidebarComponent,
    LayoutHeaderComponent,
    FieldBinaryImageSimpleComponent
  ],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    MyOwnCustomMaterialModule,
    FlexLayoutModule,
    RouterModule,
    ReactiveFormsModule
  ],
  providers: [NavSidebarService],
  entryComponents: [ConfirmDialogComponent, ChangePasswordDialogComponent, ImageViewerComponent],
})
export class SharedModule { }
