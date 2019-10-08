import { Component, OnInit } from '@angular/core';
import { NavSidebarService } from '../nav-sidebar.service';

@Component({
  selector: 'app-layout-sidebar',
  templateUrl: './layout-sidebar.component.html',
  styleUrls: ['./layout-sidebar.component.css']
})
export class LayoutSidebarComponent implements OnInit {
  activeIndex = -1;
  folded = false;
  menuItems: { name: string, children: { name: string }[] }[] = [
    {
      name: 'Tổng quan',
      children: [
        { name: 'Page 1' },
        { name: 'Page 2' },
        { name: 'Page 3' }
      ]
    },
    { name: 'Điều trị', children: [] },
    { name: 'Danh mục', children: [] },
    {
      name: 'Cấu hình',
      children: [
        { name: 'Page 1' },
        { name: 'Page 2' },
        { name: 'Page 3' }
      ]
    },
  ];
  constructor(public sidebarService: NavSidebarService) { }

  ngOnInit() {
  }

}
