import { Component, HostListener, OnInit } from '@angular/core';
import { NavSidebarService } from '@shared/services/nav-sidebar.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
    if (window.innerWidth <= 768) {
      this.sidebarService.set(true);
    }
    else {
      this.sidebarService.set(false);
    }
  }

  constructor(public sidebarService: NavSidebarService) { }

  ngOnInit() {
    this.onResize();
  }

  clickNavLink() {
    this.onResize();
  }
}
