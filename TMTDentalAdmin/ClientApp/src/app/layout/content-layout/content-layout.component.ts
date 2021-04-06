import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-content-layout',
  templateUrl: './content-layout.component.html',
  styleUrls: ['./content-layout.component.css']
})
export class ContentLayoutComponent implements OnInit {

  public navItems: any[] = [
    { name: 'Danh sach dang ky', url: '/tenants'},
    { name: 'Nhan vien', url: '/tenants'}
  ]
  
  constructor() { }

  ngOnInit() {
  }

}
