import { DOCUMENT } from '@angular/common';
import { Component, HostBinding, Inject, Input, OnInit, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  template: '<ng-content></ng-content>',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  @Input() fixed: boolean;
  @Input() display: any;
  @HostBinding('class.sidebar') sidebarClass = true;
  constructor(
    @Inject(DOCUMENT) private document: any,
    private renderer: Renderer2
  ) { }

  ngOnInit() {
    this.isFixed(this.fixed);
    this.displayBreakpoint(this.display);
  }

  isFixed(fixed: boolean = this.fixed) {
    if (fixed) {
      this.renderer.addClass(this.document.body, 'sidebar-fixed');
    }
  }

  displayBreakpoint(display: any = this.display) {
    if (display !== false) {
      const cssClass = display ? `sidebar-${display}-show`: 'sidebar-show';
      this.renderer.addClass(this.document.body, cssClass);
    }
  }

}
