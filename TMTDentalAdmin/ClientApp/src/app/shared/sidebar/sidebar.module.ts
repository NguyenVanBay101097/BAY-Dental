import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SidebarNavComponent } from './sidebar-nav/sidebar-nav.component';
import { SidebarNavDropdownComponent } from './sidebar-nav-dropdown/sidebar-nav-dropdown.component';
import { SidebarNavItemsComponent } from './sidebar-nav-items/sidebar-nav-items.component';
import { SidebarNavLinkComponent } from './sidebar-nav-link/sidebar-nav-link.component';

@NgModule({
  declarations: [SidebarComponent, SidebarNavComponent, SidebarNavDropdownComponent, SidebarNavItemsComponent, SidebarNavLinkComponent],
  imports: [
    CommonModule
  ]
})
export class SidebarModule { }
