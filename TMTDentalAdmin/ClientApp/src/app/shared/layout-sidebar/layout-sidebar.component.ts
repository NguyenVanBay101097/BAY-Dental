import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-layout-sidebar',
  templateUrl: './layout-sidebar.component.html',
  styleUrls: ['./layout-sidebar.component.css']
})
export class LayoutSidebarComponent implements OnInit {
  activeIndex = -1;
  folded = false;
  menuItems: { name: string, icon?: string, link?: string, children?: { name: string, link?: string, params?: Object }[] }[] = [
    {
      name: 'Tổng quan', icon: 'fas fa-home', children: [], link: '/',
    },
    {
      name: 'Tenants',
      icon: 'fas fa-tag',
      link: '/tenants',
      children: []
    },
  ];
  constructor(public sidebarService: NavSidebarService, public authService: AuthService) { }

  ngOnInit() {
  }

  onMenuClick(index) {
    if (this.sidebarService.collapsed) {
      this.activeIndex = -1;
    } else {
      if (this.activeIndex == index) {
        this.activeIndex = -1;
      } else {
        this.activeIndex = index;
      }
    }
  }

}
